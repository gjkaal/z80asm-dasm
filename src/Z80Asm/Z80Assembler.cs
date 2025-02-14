﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Z80Asm;

public sealed class Z80Assembler
{
    private readonly AstScope root;
    private readonly ExprNodeIP exprNodeIP;
    private readonly ExprNodeIP exprNodeIP2;
    private readonly ExprNodeOFS exprNodeOP;
    private readonly ExprNodeOFS exprNodeOP2;

    public Z80Assembler()
    {
        var preString = new StringSource(string.Empty, "Pre-sets", "init");
        var preSource = new SourcePosition(preString, 0);
        root = new AstScope("global", preSource);

        exprNodeIP = new ExprNodeIP(true);
        root.Define("$", exprNodeIP);
        exprNodeIP2 = new ExprNodeIP(false);
        root.Define("$$", exprNodeIP2);
        exprNodeOP = new ExprNodeOFS(true);
        root.Define("$ofs", exprNodeOP);
        exprNodeOP2 = new ExprNodeOFS(false);
        root.Define("$$ofs", exprNodeOP2);
        root.AddElement(new AstTypeByte(preSource));
        root.AddElement(new AstTypeWord(preSource));
    }

    public SourcePosition ParseLiteral(string code)
    {
        var result = root.ParseLiteral("TryParse", code);
        root.DefineSymbols(null);
        return result;
    }

    public (int errorCount, int warningCount) GenerateCode(
       SourcePosition position,
       Action<string, bool> listFile,
       Action<byte[], IEnumerable<Contentblock>> codeFile)
    {
        // Calculate the layout
        var layoutContext = new LayoutContext();
        exprNodeIP.SetContext(layoutContext);
        exprNodeIP2.SetContext(layoutContext);
        exprNodeOP.SetContext(layoutContext);
        exprNodeOP2.SetContext(layoutContext);
        root.Layout(null, layoutContext);
        if (Log.ErrorCount > 0)
        {
            return (Log.ErrorCount, Log.WarningCount);
        }

        // Generate the code
        var generateContext = new GenerateContext(listFile);
        exprNodeIP.SetContext(generateContext);
        exprNodeIP2.SetContext(generateContext);
        exprNodeOP.SetContext(generateContext);
        exprNodeOP2.SetContext(generateContext);

        generateContext.EnterSourceFile(position);
        root.Generate(null, generateContext);
        generateContext.LeaveSourceFile();
        if (Log.ErrorCount > 0)
        {
            return (Log.ErrorCount, Log.WarningCount);
        }

        var code = generateContext.GetGeneratedBytes();
        ContentBlocks.AddRange(generateContext.ContentBlocks);

        codeFile(code, ContentBlocks);
        return (Log.ErrorCount, Log.WarningCount);
    }

    public List<Contentblock> ContentBlocks { get; private set; } = [];

    public int CalculateLayout()
    {
        var layoutContext = new LayoutContext();
        exprNodeIP.SetContext(layoutContext);
        exprNodeIP2.SetContext(layoutContext);
        exprNodeOP.SetContext(layoutContext);
        exprNodeOP2.SetContext(layoutContext);
        root.Layout(null, layoutContext);
        return Log.ErrorCount;
    }

    public List<string> IncludePaths { get; } = new List<string>();

    public bool CreateSyntaxTreeFile { get; set; }
    public bool CreateListFile { get; set; }
    public bool CreateBinaryFile { get; set; }
    public bool CreateIntelHexFile { get; set; }

    public (int errorCount, int warningCount) Compile(FileInfo source)
    {
        var sourcePosition = root.ParseFile(source, IncludePaths);
        root.DefineSymbols(null);

        if (CreateSyntaxTreeFile)
        {
            using (var w = source.OpenTextWriter(":default", ".ast.txt"))
            {
                root.Dump(w.WriteLine, 0);
            }
        }

        StringBuilder? listWriter = null;
        if (CreateListFile)
        {
            listWriter = new StringBuilder();
        }

        var compileResult = GenerateCode(
            sourcePosition,
            (s, b) => ListWriter(listWriter, s, b),
            (bytes, blocks) => CodeWriter(source, bytes, blocks, root.GetAllSymbols));

        if (listWriter != null)
        {
            using (var w = source.OpenTextWriter(":default", ".lst"))
            {
                w.Write(listWriter.ToString());
            }
        }
        return compileResult;
    }

    public static void ListWriter(StringBuilder? source, string text, bool newLine)
    {
        if (source != null)
        {
            if (newLine)
            {
                source.AppendLine(text);
            }
            else
            {
                source.Append(text);
            }
        }
    }

    public void CodeWriter(
        FileInfo source,
        byte[] code,
        IEnumerable<Contentblock> contentblocks,
        Func<IEnumerable<(string, long)>>? symbolNames)
    {
        if (CreateBinaryFile)
        {
            using (var w = source.OpenBinaryWriter(":default", ".bin"))
            {
                w.Write(code);
            }
        }
        if (CreateIntelHexFile)
        {
            var dictionary = new Dictionary<string, long>();
            if (symbolNames != null)
            {
                foreach (var (name, value) in symbolNames())
                {
                    dictionary.TryAdd(name, value);
                }
            }
            using (var w = source.OpenTextWriter(":default", ".hex"))
            {
                if (contentblocks.Count() == 0)
                {
                    IntelHex.Write(w, code);
                }
                else
                {
                    foreach (var block in contentblocks)
                    {
                        foreach (var symbol in dictionary)
                        {
                            if (block.Address == symbol.Value)
                            {
                                block.Name = symbol.Key;
                            }
                        }
                        IntelHex.Write(w, block);
                    }
                }
                IntelHex.EndOfFile(w);
            }
        }
    }
}

public class Contentblock
{
    public string Source { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Address { get; set; }
    public int Length { get; set; }
    public byte[] Data { get; set; } = [];
}

public static class IntelHex
{
    // see https://developer.arm.com/documentation/ka003292/latest/

    private const byte DataRecord = 00;
    private const string EOF = ":00000001FF";

    /// <summary>
    /// Write the Intel Hex format to the specified writer
    /// </summary>
    /// <param name="writer">Ascii output</param>
    /// <param name="code">The code</param>
    public static void Write(TextWriter writer, byte[] code)
    {
        // get every 16 bytes of data
        for (var i = 0; i < code.Length; i += 16)
        {
            var data = code.Skip(i).Take(16).ToArray();
            WriteRecord(writer, i, data);
        }
    }

    public static void Write(TextWriter writer, Contentblock block)
    {
        // Add comments (comments should not have a colon in the text)
        writer.WriteLine("// ------------------------------------------------------");
        writer.WriteLine($"// Name           {block.Name.Replace(':', '_')}");
        writer.WriteLine($"// Address space  {block.Address:X4}-{block.Address + block.Length:X4}");
        writer.WriteLine($"// Length         {block.Length} bytes");
        writer.WriteLine($"// Source         {block.Source.Replace(':', '_')}");
        writer.WriteLine("// ------------------------------------------------------");
        // get every 16 bytes of data
        for (var i = 0; i < block.Data.Length; i += 16)
        {
            var data = block.Data.Skip(i).Take(16).ToArray();
            WriteRecord(writer, block.Address + i, data);
        }
    }

    public static void EndOfFile(TextWriter writer)
    {
        // write the end of file record
        writer.WriteLine(EOF);
    }

    private static void WriteRecord(TextWriter writer, int address, byte[] record)
    {
        if (record.Length == 0)
        {
            return;
        }
        if (record.Length > 16)
        {
            throw new ArgumentException("record length must be less than 16 bytes");
        }

        // No need to write a record if it is all 0xFF
        if (record.All(b => b == 0xFF)) return;

        // calculate the length of the data
        var length = Math.Min(16, record.Length);

        // extra space for the length, address, and record type
        var testRecord = new byte[record.Length + 4];
        // set the length of the data
        testRecord[0] = (byte)length;
        // set the address of the data
        testRecord[1] = (byte)((address >> 8) & 0xff);
        testRecord[2] = (byte)(address & 0xff);
        // set the record type
        testRecord[3] = DataRecord;
        Array.Copy(record, 0, testRecord, 4, record.Length);

        // write the data
        writer.Write(':');
        for (var j = 0; j < testRecord.Length; j++)
        {
            writer.Write(testRecord[j].ToString("X2"));
        }
        // write the checksum
        writer.Write(Checksum(testRecord).ToString("X2"));
        writer.WriteLine();
    }

    public static byte Checksum(byte[] data)
    {
        byte sum = 0;
        for (var i = 0; i < data.Length; i++)
        {
            sum += data[i];
        }
        return (byte)-sum;
    }
}