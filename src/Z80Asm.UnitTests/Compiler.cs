
using Mono.Cecil.Cil;
using System.Text;

namespace Z80Asm.UnitTests;

public sealed class Compiler
{
    readonly AstScope root;
    readonly ExprNodeIP exprNodeIP;
    readonly ExprNodeIP exprNodeIP2;
    readonly ExprNodeOFS exprNodeOP;
    readonly ExprNodeOFS exprNodeOP2;

    public Compiler()
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

    public int GenerateCode(
       SourcePosition position,
       Action<string, bool> listFile,
       Action<byte[]> codeFile)
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
            return Log.ErrorCount;
        }

        // Generate the code
        var generateContext = new GenerateContext(layoutContext, listFile);
        exprNodeIP.SetContext(generateContext);
        exprNodeIP2.SetContext(generateContext);
        exprNodeOP.SetContext(generateContext);
        exprNodeOP2.SetContext(generateContext);

        generateContext.EnterSourceFile(position);
        root.Generate(null, generateContext);
        generateContext.LeaveSourceFile();
        if (Log.ErrorCount > 0)
        {
            return Log.ErrorCount;
        }

        var code = generateContext.GetGeneratedBytes();
        codeFile(code);
        return Log.ErrorCount;
    }

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

    public int Compile(FileInfo source)
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

        var compileResult = GenerateCode(sourcePosition, 
            (s, b) => ListWriter(listWriter, s, b), 
            (bytes) => CodeWriter(source, bytes));

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
        if(source != null)
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

    public void CodeWriter(FileInfo source, byte[] code)
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
            using (var w = source.OpenTextWriter(":default", ".hex"))
            {
                IntelHex.Write(w, code);
            }
        }
    }
}

public static class IntelHex
{
    // see https://developer.arm.com/documentation/ka003292/latest/

    private const string DataRecord = "00";
    private const string EOF = ":00000001FF";

    /// <summary>
    /// Write the Intel Hex format to the specified writer
    /// </summary>
    /// <param name="writer">Ascii output</param>
    /// <param name="code">The code</param>
    public static void Write(TextWriter writer, byte[] code)
    {
        // get every 16 bytes of data
        for (int i = 0; i < code.Length; i += 16)
        {
            var data = code.Skip(i).Take(16).ToArray();
            WriteRecord(writer, i, data);
        }
        // write the end of file record
        writer.WriteLine(EOF);

    }

    private static void WriteRecord(TextWriter writer, int address, byte[] record)
    {
        if(record.Length == 0)
        {
            return;
        }
        if(record.Length > 16)
        {
            throw new ArgumentException("record length must be less than 16 bytes");
        }
        // calculate the length of the data
        int length = Math.Min(16, record.Length);
        // write the length of the data
        writer.Write(':');
        writer.Write(length.ToString("X2"));
        // write the address of the data
        writer.Write(((address >> 8) & 0xff).ToString("X2"));
        writer.Write((address & 0xff).ToString("X2"));
        // write the record type
        writer.Write(DataRecord);
        // write the data
        for (int j = 0; j < length; j++)
        {
            writer.Write(record[j].ToString("X2"));
        }
        // write the checksum
        writer.Write(Checksum(record).ToString("X2"));
        writer.WriteLine();        
    }

    private static byte Checksum(byte[] data)
    {
        byte sum = 0;
        for (int i = 0; i < data.Length; i++)
        {
            sum += data[i];
        }
        return (byte)(-sum);
    }
}