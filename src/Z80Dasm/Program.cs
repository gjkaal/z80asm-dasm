﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Z80Mnemonics;

namespace Z80DAsm
{
    internal class Program
    {
        private string _inputFile;
        private string _outputFile;
        private int _baseAddr = 0;
        private int _header = 0;
        private int _start = 0;
        private int _len = 0;
        private bool _xref = false;
        private bool _lst = false;
        private bool _lowerCase = false;
        private bool _markWordRefs = false;
        private bool _reloffs = false;
        private bool _htmlMode = false;
        private bool _autoOpen = false;
        private bool _coalescStrings = true;
        private readonly List<int> _entryPoints = [];
        private int _addrSpaceStart;
        private int _addrSpaceEnd;

        private void CheckAddress(int a)
        {
            if (a < 0 || a > 0xFFFF)
            {
                throw new InvalidOperationException(string.Format("Address 0x{0:X} is out of range", a));
            }

            if (a < _addrSpaceStart || a > _addrSpaceEnd)
            {
                throw new InvalidOperationException(string.Format("Address 0x{0:X4} is out of range 0x{1:X4}-0x{2:X4}", a, _addrSpaceStart, _addrSpaceEnd));
            }
        }

        public bool ProcessArg(string arg)
        {
            if (arg == null)
                return true;

            if (arg.StartsWith("#"))
                return true;

            // Response file
            if (arg.StartsWith("@"))
            {
                // Get the fully qualified response file name
                var strResponseFile = System.IO.Path.GetFullPath(arg.Substring(1));

                // Load and parse the response file
                var args = Utils.ParseCommandLine(System.IO.File.ReadAllText(strResponseFile));

                // Set the current directory
                var OldCurrentDir = System.IO.Directory.GetCurrentDirectory();
                System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(strResponseFile));

                // Load the file
                var bRetv = ProcessArgs(args);

                // Restore current directory
                System.IO.Directory.SetCurrentDirectory(OldCurrentDir);

                return bRetv;
            }

            // Args are in format [/-]<switchname>[:<value>];
            if (arg.StartsWith("/") || arg.StartsWith("-"))
            {
                var SwitchName = arg.Substring(arg.StartsWith("--") ? 2 : 1);
                string Value = null;

                var colonpos = SwitchName.IndexOf(':');
                if (colonpos >= 0)
                {
                    // Split it
                    Value = SwitchName.Substring(colonpos + 1);
                    SwitchName = SwitchName.Substring(0, colonpos);
                }

                switch (SwitchName)
                {
                    case "help":
                    case "h":
                    case "?":
                        ShowLogo();
                        ShowHelp();
                        return false;

                    case "v":
                    case "version":
                        ShowLogo();
                        return false;

                    case "addr":
                        _baseAddr = Utils.ParseUShort(Value);
                        if (_start == 0)
                            _start = _baseAddr;
                        break;

                    case "header":
                        _header = Utils.ParseUShort(Value);
                        break;

                    case "start":
                        _start = Utils.ParseUShort(Value);
                        break;

                    case "end":
                        _len = Utils.ParseUShort(Value) - _start;
                        break;

                    case "len":
                        _len = Utils.ParseUShort(Value);
                        break;

                    case "entry":
                        _entryPoints.Add(Utils.ParseUShort(Value));
                        break;

                    case "xref":
                        _xref = true;
                        break;

                    case "lst":
                    case "list":
                        _lst = true;
                        break;

                    case "lc":
                    case "lowercase":
                        _lowerCase = true;
                        break;

                    case "hex":
                        switch (Value)
                        {
                            case "$":
                            case "x":
                            case "h":
                            case "&":
                                Disassembler.HexFormat = Value[0];
                                break;

                            case "dollar":
                                Disassembler.HexFormat = '$';
                                break;

                            case "amper":
                                Disassembler.HexFormat = '&';
                                break;

                            default:
                                throw new InvalidOperationException("Invalid hex format specifier");
                        }
                        break;

                    case "reloffs":
                        _reloffs = true;
                        break;

                    case "mwr":
                    case "markwordrefs":
                        _markWordRefs = true;
                        break;

                    case "html":
                        _htmlMode = true;
                        break;

                    case "open":
                        _autoOpen = true;
                        break;

                    case "ncs":
                    case "nocoalescstrings":
                        _coalescStrings = false;
                        break;

                    default:
                        throw new InvalidOperationException(string.Format("Unknown switch '{0}'", arg));
                }
            }
            else
            {
                if (_inputFile == null)
                    _inputFile = arg;
                else if (_outputFile == null)
                    _outputFile = arg;
                else
                    throw new InvalidOperationException(string.Format("Too many command line arguments, don't know what to do with '{0}'", arg));
            }

            return true;
        }

        public bool ProcessArgs(IEnumerable<string> args)
        {
            if (args == null)
                return true;

            // Parse args
            foreach (var a in args)
            {
                if (!ProcessArg(a))
                    return false;
            }

            return true;
        }

        public static void ShowLogo()
        {
            var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine("Z80DAsm v{0} - Z80 Disassembler", v);
            Console.WriteLine("Copyright (C) Kaalenco. All Rights Reserved.");
            Console.WriteLine("Disassembler engine based on http://z80ex.sourceforge.net/");

            Console.WriteLine("");
        }

        public static void ShowHelp()
        {
            Console.WriteLine("usage: Z80DAsm source.bin [destination.asm] [options] [@responsefile]");
            Console.WriteLine();

            Console.WriteLine("Options:");
            Console.WriteLine("  --addr:<N>             Z-80 base address of first byte after header, default=0x0000");
            Console.WriteLine("  --start:<N>            Z-80 address to disassemble from, default=addr");
            Console.WriteLine("  --end:<N>              Z-80 address to stop at, default=eof");
            Console.WriteLine("  --len:<N>              Number of bytes to disassemble (instead of --end)");
            Console.WriteLine("  --entry:<N>            Specifies an entry point (see below)");
            Console.WriteLine("  --xref                 Include referenced locations of labels");
            Console.WriteLine("  --list                 Generate a listing file (more detail, can't be assembled) (or, --lst)");
            Console.WriteLine("  --hex:<format>         Specify hex literal formatting (options: $, dollar, &, amper, x or h)");
            Console.WriteLine("  --html                 Generates a HTML file, with hyperlinked references");
            Console.WriteLine("  --open                 Automatically opens the generated file with default associated app");
            Console.WriteLine("  --lowercase|lc         Render in lowercase");
            Console.WriteLine("  --markwordrefs|mwr     Highlight with a comment literal word values (as they may be addresses)");
            Console.WriteLine("  --noCoalescStrings|ncs Don't coalesc strings");
            Console.WriteLine("  --reloffs              Show the offset of relative address mode instructions");
            Console.WriteLine("  --header:N             Skip N header bytes at start of file");
            Console.WriteLine("  --help                 Show these help instruction");
            Console.WriteLine("  --v                    Show version information");

            Console.WriteLine();
            Console.WriteLine("Numeric arguments can be in decimal (no prefix) or hex if prefixed with '0x'.");

            Console.WriteLine();
            Console.WriteLine("If one or more --entry arguments are specified (recommended), the file is disassembled by ");
            Console.WriteLine("following the code paths from those entry points.  All unvisited regions will be rendered ");
            Console.WriteLine("as 'DB' directives.");
            Console.WriteLine();
            Console.WriteLine("If the --entry argument is not specified, the file is disassembled from top to bottom.");

            Console.WriteLine();
            Console.WriteLine("Output is sent to stdout if no destination file specified.");

            Console.WriteLine();
            Console.WriteLine("Response file containing arguments can be specified using the @ prefix");

            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("    Z80DAsm --addr:0x0400 --entry:0x1983 -lst robotf.bin robotf.lst");
            Console.WriteLine();
        }

        public int Run(string[] args)
        {
            // Process command line
            if (!ProcessArgs(args))
                return 0;

            if (_inputFile == null)
            {
                ShowLogo();
                ShowHelp();
                return 7;
            }

            // Open input file
            var code = System.IO.File.ReadAllBytes(_inputFile);

            // Work out the available address space
            _addrSpaceStart = _baseAddr;
            _addrSpaceEnd = _baseAddr + (code.Length - _header);

            // Work out auto length
            if (_len == 0)
                _len = (ushort)(code.Length - _header);

            // Check specified address range
            CheckAddress(_start);
            CheckAddress(_start + _len - 1);
            foreach (var addr in _entryPoints)
                CheckAddress(addr);

            // Setup disassembler parameters
            Disassembler.LabelledRangeLow = (ushort)_start;
            Disassembler.LabelledRangeHigh = (ushort)(_start + _len);
            Disassembler.LowerCase = _lowerCase;
            Disassembler.HtmlMode = _htmlMode;
            Disassembler.ShowRelativeOffsets = _reloffs;

            // Disassemble
            var instructions = new Dictionary<int, Instruction>();
            if (_entryPoints.Count > 0)
            {
                var pendingCodePaths = new List<int>();
                pendingCodePaths.AddRange(_entryPoints);

                while (pendingCodePaths.Count > 0)
                {
                    // Get a new address that needs disassembling
                    var addr = pendingCodePaths[0];
                    pendingCodePaths.RemoveAt(0);

                    // Disassemble
                    while (!instructions.ContainsKey(addr) && addr >= _start && addr < _start + _len)
                    {
                        // Disassemble this instruction
                        var i = Disassembler.Disassemble(code, (ushort)(addr - _baseAddr + _header), (ushort)addr);

                        // Possible address reference?
                        if (_markWordRefs && i.word_val.HasValue && (i.opCode.flags & (OpCodeFlags.Jumps | OpCodeFlags.RefAddr)) == 0)
                        {
                            i.Comment = "address or value?";
                        }

                        // Add it
                        instructions.Add(addr, i);

                        // If have a jump address, dump it
                        if (i.next_addr_2.HasValue)
                        {
                            pendingCodePaths.Add(i.next_addr_2.Value);
                        }

                        // Continue
                        if (i.next_addr_1.HasValue)
                            addr = i.next_addr_1.Value;
                        else
                            break;
                    }
                }
            }
            else
            {
                // Linear disassembly
                for (var addr = _start; addr < _start + _len;)
                {
                    // Disassemble this instruction
                    var i = Disassembler.Disassemble(code, (ushort)(addr - _baseAddr + _header), (ushort)addr);

                    // Add it
                    instructions.Add(addr, i);

                    // Update address
                    addr += i.bytes;
                }
            }

            // Sort all instructions
            var sorted = instructions.Values.OrderBy(x => x.addr).ToList();

            // Helper for generating DB directives
            Func<int, int, int, int> FillData = delegate (int from, int to, int index)
            {
                for (var j = from; j < to; j++)
                {
                    var data = code[j - _baseAddr + _header];

                    // Get the byte
                    var instruction = new Instruction();
                    if (data >= 0x20 && data < 0x7f)
                    {
                        instruction.Comment = string.Format("'{0}'", (char)data);

                        if ((char)data != '\"')
                            instruction.AsciiChar = (char)data;
                    }
                    instruction.addr = (ushort)j;
                    instruction.Asm = string.Format("DB {0}", Disassembler.FormatByte(data));
                    instruction.next_addr_1 = (ushort)(j + 1);
                    instruction.bytes = 1;

                    // Add to instruction map
                    instructions.Add(instruction.addr, instruction);

                    // Add to sorted list
                    sorted.Insert(index, instruction);
                    index++;
                }
                return index;
            };

            // Fill in all unpopulated areas with DBs
            var expectedNextAddress = _start;
            for (var i = 0; i < sorted.Count; i++)
            {
                var inst = sorted[i];
                if (inst.addr != expectedNextAddress)
                {
                    i = FillData(expectedNextAddress, inst.addr, i);
                }

                expectedNextAddress = sorted[i].addr + sorted[i].bytes;
            }
            FillData(expectedNextAddress, _start + _len, instructions.Count);

            // Mark all entry points
            foreach (var e in _entryPoints)
            {
                instructions[e].entryPoint = true;
            }

            // Resolve references
            foreach (var i in instructions)
            {
                var ref_addr = i.Value.next_addr_2;
                if (!ref_addr.HasValue)
                {
                    if (i.Value.word_val.HasValue && (i.Value.opCode.flags & OpCodeFlags.RefAddr) != 0)
                        ref_addr = i.Value.word_val;
                }

                if (ref_addr.HasValue)
                {
                    for (var stepback = 0; stepback < 6; stepback++)
                    {
                        Instruction target;
                        if (instructions.TryGetValue(ref_addr.Value - stepback, out target))
                        {
                            if (target.referencedFrom == null)
                                target.referencedFrom = new List<Instruction>();
                            target.referencedFrom.Add(i.Value);

                            if (stepback != 0)
                            {
                                // patch the original instruction
                                i.Value.Asm = i.Value.Asm.Replace(Disassembler.FormatAddr(ref_addr.Value), Disassembler.FormatAddr(target.addr) + "+" + stepback.ToString());
                                i.Value.Comment = "reference not aligned to instruction";
                            }

                            break;
                        }
                    }
                }
            }

            // Coalesc Ascii strings
            if (_coalescStrings)
            {
                var coalescFrom = 0;
                for (var i = 0; i < sorted.Count; i++)
                {
                    var inst = sorted[i];

                    // Can we continue coalescing?
                    if (inst.AsciiChar == 0 || i - coalescFrom > 128 || inst.entryPoint || inst.referencedFrom != null) // || (i > 0 && sorted[i-1].next_addr_1.HasValue))
                    {
                        // Any thing to coalesc?
                        var bytesToCoalesc = i - coalescFrom;
                        if (bytesToCoalesc > 4)
                        {
                            // Get the first instruction
                            var fromInstruction = sorted[coalescFrom];

                            // Build the DB "string" string
                            var sb = new StringBuilder();
                            sb.Append("DB\t\"");
                            for (var j = 0; j < bytesToCoalesc; j++)
                            {
                                sb.Append((char)code[fromInstruction.addr - _baseAddr + _header + j]);
                            }
                            sb.Append('"');

                            // Create the new instruction
                            var instruction = new Instruction();
                            instruction.addr = fromInstruction.addr;
                            instruction.next_addr_1 = inst.addr;
                            instruction.bytes = (ushort)bytesToCoalesc;
                            instruction.Asm = sb.ToString();
                            instruction.referencedFrom = fromInstruction.referencedFrom;

                            // Remove the old instructions
                            for (var j = 0; j < bytesToCoalesc; j++)
                            {
                                instructions.Remove(fromInstruction.addr + j);
                                sorted.RemoveAt(coalescFrom);
                            }

                            // Insert the new instruction
                            instructions.Add(fromInstruction.addr, instruction);
                            sorted.Insert(coalescFrom, instruction);

                            // Rewind index
                            i = coalescFrom;
                        }

                        // Start next coalesc from the next instruction
                        coalescFrom = i + 1;
                    }
                }
            }

            var targetWriter = Console.Out;
            if (_outputFile != null)
            {
                targetWriter = new StreamWriter(_outputFile);
            }

            TextWriter w1;

            if (_htmlMode)
            {
                w1 = targetWriter;
            }
            else
            {
                w1 = new TabbedTextWriter(targetWriter);
                if (_lst)
                {
                    ((TabbedTextWriter)w1).TabStops = new int[] { 32, 40, 48, 56, 64 };
                }
                else
                {
                    ((TabbedTextWriter)w1).TabStops = new int[] { 8, 16, 32 };
                }
            }

            Action<string, bool> w = (s, b) =>
            {
                if (b)
                    w1.WriteLine(s);
                else
                    w1.Write(s);
            };

            if (_htmlMode)
            {
                w.Invoke("<html>", true);
                w.Invoke("<head>", true);
                w.Invoke("</head>", true);
                w.Invoke("<body>", true);
                w.Invoke("<pre><code>", true);
            }

            // Analyse call graph
            var cg = new CallGraphAnalyzer(instructions, sorted);
            cg.Analyse();
            var definedProcs = cg.Procs;

            // Write out the "ORG" directive
            if (sorted.Count > 0 && !_lst)
            {
                w.Invoke($"\n\tORG\t{Disassembler.FormatWord(sorted[0].addr)}\n", true);
            }

            // List it
            Instruction prev = null;
            foreach (var i in sorted)
            {
                // Blank line after data blocks
                if (prev != null && prev.opCode == null && i.opCode != null)
                {
                    w.Invoke(string.Empty, true);
                }

                if (_htmlMode)
                    w.Invoke($"<a name=\"L{i.addr:X4}\"></a>", false);

                // Include cross references?
                if (_xref && i.referencedFrom != null)
                {
                    // Ensure a blank line before reference comments
                    if (prev != null && prev.next_addr_1.HasValue)
                    {
                        w.Invoke(string.Empty, true);
                    }

                    if (_lst)
                        w.Invoke($"{new string(' ', 23)}\t", false);
                    w.Invoke($"\t; Referenced from {string.Join(", ", i.referencedFrom.Select(x => Disassembler.FormatAddr(x.addr, true, false)).ToList())}", true);
                }

                if (i.entryPoint)
                {
                    if (_lst)
                        w.Invoke($"{new string(' ', 23)}\t", false);
                    w.Invoke("\t; Entry Point", true);
                }

                if (definedProcs.ContainsKey(i.addr))
                {
                    // TODO : Check if address is a known address
                    // if (references.ContainsKey(i.addr))
                    // label = references.GetLabel(i.addr);

                    if (_lst)
                        w.Invoke($"{new string(' ', 23)}\t", false);
                    w.Invoke($"\t; --- START PROC {Disassembler.FormatAddr(i.addr, false, true)} ---", true);
                }

                if (_lst)
                {
                    w.Invoke($"{i.addr:X4}:", false);
                    for (var j = 0; j < Math.Min((int)i.bytes, 8); j++)
                    {
                        var data = code[i.addr + j - _baseAddr + _header];
                        w.Invoke($" {data:X2}", false);
                    }

                    var spaces = 3 * (6 - i.bytes);

                    if (spaces > 0)
                        w.Invoke(new string(' ', spaces), false);
                    w.Invoke("\t ", false);
                }

                // Work out label
                var label = "";
                if (i.entryPoint || i.referencedFrom != null || (prev != null && !prev.next_addr_1.HasValue))
                {
                    label = Disassembler.FormatAddr(i.addr, false);
                    label += ":";
                }

                // Write the disassembled instruction
                var asm = i.Asm;
                if (!asm.StartsWith("DB\t\""))
                    asm = asm.Replace(" ", "\t");

                w.Invoke($"{label}\t{asm}", false);

                // Write out an optional comment
                if (i.Comment != null)
                    w.Invoke($"\t; {i.Comment}", false);

                if (_lst)
                {
                    if (i.bytes > 8)
                    {
                        for (var j = 8; j < i.bytes; j++)
                        {
                            if ((j % 8) == 0)
                                w.Invoke($"\n{i.addr + j:X4}:", false);

                            var data = code[i.addr + j - _baseAddr + _header];
                            w.Invoke($" {data:X2}", false);
                        }
                    }
                }

                if (_htmlMode)
                    w.Invoke("</a>", true);
                else
                    w.Invoke(string.Empty, true);

                // If this instruction doesn't continue on, insert a blank line
                if (!i.next_addr_1.HasValue)
                {
                    w.Invoke(string.Empty, true);
                }

                // Remember the previous instruction
                prev = i;
            }

            if (_lst)
            {
                // Build a list of all possible address references
                var addressInfos = new Dictionary<int, AddressInfo>();
                var portInfos = new Dictionary<int, PortInfo>();
                foreach (var i in sorted)
                {
                    // Does this instruction reference a word value?
                    if (i.word_val.HasValue)
                    {
                        AddressInfo ai;
                        if (!addressInfos.TryGetValue(i.word_val.Value, out ai))
                        {
                            ai = new AddressInfo(i.word_val.Value);
                            addressInfos.Add(ai.addr, ai);
                        }

                        if ((i.opCode.flags & OpCodeFlags.RefAddr) != 0)
                        {
                            // Known referenced data address
                            ai.DataReferences.Add(i);
                        }

                        if ((i.opCode.flags & OpCodeFlags.Jumps) != 0)
                        {
                            // Known referenced code address
                            ai.CodeReferences.Add(i);
                        }

                        if ((i.opCode.flags & (OpCodeFlags.Jumps | OpCodeFlags.RefAddr)) == 0)
                        {
                            // Potential address
                            ai.PotentialReferences.Add(i);
                        }
                    }

                    // Is it a port reference?
                    if (i.opCode != null && (i.opCode.flags & OpCodeFlags.PortRef) != 0)
                    {
                        // Which port (-1, referenced through a register)
                        var port = -1;
                        if (i.byte_val.HasValue)
                            port = i.byte_val.Value;

                        // Get the port info
                        PortInfo pi;
                        if (!portInfos.TryGetValue(port, out pi))
                        {
                            pi = new PortInfo(port);
                            portInfos.Add(port, pi);
                        }

                        pi.References.Add(i);
                    }
                }

                if (w1 is TabbedTextWriter)
                    ((TabbedTextWriter)w1).TabStops = new int[] { 8, 16, 24, 32 };

                // Build a list of all external references
                var extRefs = addressInfos.Values.Where(x => x.addr < _start || x.addr >= _start + _len).OrderBy(x => x.addr);
                foreach (var r in extRefs)
                {
                    if (r.DataReferences.Count > 0 || r.CodeReferences.Count > 0)
                    {
                        w.Invoke($"\nreferences to external address {Disassembler.FormatAddr((ushort)r.addr, true, false)}:", true);

                        foreach (var i in r.DataReferences.Concat(r.CodeReferences).Concat(r.PotentialReferences).OrderBy(x => x.addr))
                        {
                            w.Invoke($"\t{Disassembler.FormatAddr(i.addr, true, false)} {i.Asm}", true);
                        }
                    }
                }

                foreach (var r in addressInfos.Values.Where(x => x.addr >= _start && x.addr < _start + _len && x.PotentialReferences.Count > 0).OrderBy(x => x.addr))
                {
                    w.Invoke($"\nreferences to external address {Disassembler.FormatAddr((ushort)r.addr, true, false)}:", true);
                    ListPotentialAddresses(w, r);
                }

                foreach (var r in addressInfos.Values.Where(x => (x.addr < _start || x.addr >= _start + _len) && x.PotentialReferences.Count > 0).OrderBy(x => x.addr))
                {
                    w.Invoke($"\nreferences to external address {Disassembler.FormatAddr((ushort)r.addr, true, false)}:", true);
                    ListPotentialAddresses(w, r);
                }

                foreach (var r in portInfos.Values.OrderBy(x => x.port))
                {
                    if (r.port == -1)
                    {
                        w.Invoke("\nport references through a register:", true);
                    }
                    else
                    {
                        w.Invoke($"\nreferences to port {Disassembler.FormatByte((byte)r.port)}", true);
                    }

                    foreach (var i in r.References.OrderBy(x => x.opCode.mnemonic[0]).ThenBy(x => x.addr))
                    {
                        w.Invoke($"\t{Disassembler.FormatAddr(i.addr, true, false)} {i.Asm}", true);
                    }
                }

                // Dump all procs
                w.Invoke($"\nProcedures ({definedProcs.Count}):", true);
                w.Invoke("  Proc  Length  References Dependants", true);
                foreach (var p in definedProcs.Values.OrderBy(x => x.firstInstruction.addr))
                {
                    w.Invoke(
                        $"  {Disassembler.FormatAddr(p.firstInstruction.addr, true, true)}" +
                        $"  {p.lengthInBytes:X4}" +
                        $"   {(p.firstInstruction.referencedFrom == null ? 0 : p.firstInstruction.referencedFrom.Count, 10)}" +
                        $" {p.dependants.Count,10}", true);
                }

                // Dump call graph
                w.Invoke("\nCall Graph:", true);

                var CallStack = new List<int>();
                Action<int> DumpCallGraph = null;
                DumpCallGraph = (int addr) =>
                {
                    // TODO : Check if address is a known address
                    // if (references.ContainsKey(addr))

                    // Display the proc address, indented
                    w.Invoke($"{new string(' ', CallStack.Count * 2)}{Disassembler.FormatAddr((ushort)addr, true, true)}", true);

                    if (CallStack.Count == 0)
                    {
                        w.Invoke(" - Entry Point", false);
                    }

                    // Is it a recursive call?
                    if (CallStack.Contains(addr))
                    {
                        w.Invoke(" - Recursive", true);
                        return;
                    }

                    // Is it external?
                    if (!definedProcs.TryGetValue(addr, out var p))
                    {
                        w.Invoke(" - External", true);
                        return;
                    }

                    // Dump dependants
                    w.Invoke(string.Empty, true);
                    CallStack.Add(addr);
                    foreach (var d in p.dependants)
                    {
                        DumpCallGraph(d);
                    }
                    CallStack.RemoveAt(CallStack.Count - 1);
                };

                foreach (var ep in _entryPoints.OrderBy(x => x))
                {
                    // Get the proc's entry point
                    DumpCallGraph(ep);
                }
            }

            if (_htmlMode)
            {
                w.Invoke("</code></pre>", true);
                w.Invoke("</body>", true);
                w.Invoke("</head>", true);
            }

            // Close file
            w1.Close();
            if (_outputFile != null)
            {
                targetWriter.Close();
            }

            // Open the file for browsing?
            if (_autoOpen && _outputFile != null)
            {
                Process.Start(_outputFile);
            }

            return 0;
        }

        private static void ListPotentialAddresses(Action<string, bool> w, AddressInfo r)
        {
            foreach (var i in r.PotentialReferences.OrderBy(x => x.addr))
            {
                w.Invoke($"\t{Disassembler.FormatAddr(i.addr, true, false)} {i.Asm}", true);
            }

            var bFirstOtherRef = true;
            foreach (var i in r.CodeReferences.Concat(r.DataReferences).OrderBy(x => x.addr))
            {
                if (bFirstOtherRef)
                    w.Invoke("\t----------", true);
                bFirstOtherRef = false;

                w.Invoke($"\t{Disassembler.FormatAddr(i.addr, true, false)} {i.Asm}", true);
            }
        }

        private class PortInfo
        {
            public PortInfo(int port)
            {
                this.port = port;
            }

            public int port;
            public List<Instruction> References = new();
        }

        private class AddressInfo
        {
            public AddressInfo(int addr)
            {
                this.addr = addr;
            }

            public int addr;
            public List<Instruction> CodeReferences = new();
            public List<Instruction> DataReferences = new();
            public List<Instruction> PotentialReferences = new();
        }

        private static int Main(string[] args)
        {
            try
            {
                return new Program().Run(args);
            }
            catch (InvalidOperationException x)
            {
                Console.WriteLine("{0}", x.Message);
                return 7;
            }
            catch (IOException x)
            {
                Console.WriteLine("File Error - {0}", x.Message);
                return 7;
            }
        }
    }
}