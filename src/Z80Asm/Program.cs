using System;
using System.Collections.Generic;
using System.IO;

namespace Z80Asm
{
    internal class Program
    {
        private readonly List<string> includePaths = [];

        private readonly Dictionary<string, string?> userDefines = [];

        /// <summary>
        /// Abstract Syntax Tree file
        /// </summary>
        private string astFile = string.Empty;

        private FileInfo? inputFile;

        private string listFile = string.Empty;

        private string outputFile = string.Empty;

        private string symbolsFile = string.Empty;

        public static void ShowHelp()
        {
            Console.WriteLine("usage: Z80Asm source.asm [options] [@responsefile]");
            Console.WriteLine();

            Console.WriteLine("Options:");
            Console.WriteLine("  --ast[:<filename>]         Dump the AST");
            Console.WriteLine("  --define:<symbol>[=<expr>] Define a symbol with optional value");
            Console.WriteLine("  --help                     Show these help instruction");
            Console.WriteLine("  --include:<directory>      Specifies an additional include/incbin directory");
            Console.WriteLine("  --instructionSet           Display a list of all support instructions");
            Console.WriteLine("  --list[:<filename>]        Generate a list file (or, --lst)");
            Console.WriteLine("  --output:<filename>        Output file");
            Console.WriteLine("  --sym[:<filename>]         Generate a symbols file");
            Console.WriteLine("  --v                        Show version information");

            Console.WriteLine();
            Console.WriteLine("Numeric arguments can be in decimal (no prefix) or hex if prefixed with '0x'.");

            Console.WriteLine();
            Console.WriteLine("Response file containing arguments can be specified using the @ prefix");

            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("    Z80Asm input.asm");
            Console.WriteLine();
        }

        public static void ShowLogo()
        {
            var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine("Z80Asm v{0} - Z80 Assembler", v);
            Console.WriteLine("Copyright (C) Kaalenco. All Rights Reserved.");

            Console.WriteLine("");
        }

        public bool ProcessArg(string arg)
        {
            if (arg == null)
                return true;

            if (arg.StartsWith('#'))
                return true;

            // Response file
            if (arg.StartsWith('@'))
            {
                // Get the fully qualified response file name
                var strResponseFile = Path.GetFullPath(arg.Substring(1));

                // Load and parse the response file
                var args = Utils.ParseCommandLine(File.ReadAllText(strResponseFile));

                // Set the current directory
                var OldCurrentDir = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(Path.GetDirectoryName(strResponseFile));

                // Load the file
                var bRetv = ProcessArgs(args);

                // Restore current directory
                Directory.SetCurrentDirectory(OldCurrentDir);

                return bRetv;
            }

            // Args are in format [/-]<switchname>[:<value>];
            if (arg.StartsWith("/") || arg.StartsWith("-"))
            {
                var SwitchName = arg[(arg.StartsWith("--") ? 2 : 1)..];
                var Value = string.Empty;

                var colonpos = SwitchName.IndexOf(':');
                if (colonpos >= 0)
                {
                    // Split it
                    Value = SwitchName.Substring(colonpos + 1);
                    SwitchName = SwitchName.Substring(0, colonpos);
                }

                switch (SwitchName.ToLowerInvariant())
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

                    case "instructionSet":
                        InstructionSet.DumpAll();
                        return false;

                    case "ast":
                        if (Value != null)
                        {
                            astFile = Value;
                        }
                        else
                        {
                            astFile = ":default";
                        }
                        break;

                    case "sym":
                        if (Value != null)
                        {
                            symbolsFile = Value;
                        }
                        else
                        {
                            symbolsFile = ":default";
                        }
                        break;

                    case "lst":
                    case "list":
                        if (Value != null)
                        {
                            listFile = Value;
                        }
                        else
                        {
                            listFile = ":default";
                        }
                        break;

                    case "define":
                        if (Value == null)
                        {
                            throw new InvalidOperationException("--define: requires argument value");
                        }
                        else
                        {
                            var eqPos = Value.IndexOf('=');
                            if (eqPos < 0)
                                userDefines.Add(Value, null);
                            else
                                userDefines.Add(Value.Substring(0, eqPos), Value.Substring(eqPos + 1));
                        }
                        break;

                    case "output":
                        if (Value == null)
                        {
                            throw new InvalidOperationException("--output: requires argument value");
                        }
                        else
                        {
                            outputFile = Value;
                        }
                        break;

                    case "include":
                        if (Value == null)
                        {
                            throw new InvalidOperationException("--include: requires argument value");
                        }
                        else
                        {
                            try
                            {
                                includePaths.Add(Path.GetFullPath(Value));
                            }
                            catch (Exception x)
                            {
                                throw new InvalidOperationException($"Invalid include path: {Value} - {x.Message}");
                            }
                        }
                        break;

                    default:
                        throw new InvalidOperationException(string.Format("Unknown switch '{0}'", arg));
                }
            }
            else
            {
                if (inputFile == null)
                {
                    var fileName = Path.GetFullPath(arg);
                    inputFile = new FileInfo(fileName);
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Too many command line arguments, don't know what to do with '{0}'", arg));
                }
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

        public int Run(string[] args)
        {
            // Process command line
            if (!ProcessArgs(args))
                return 0;

            // Was there an input file specified?
            if (inputFile == null)
            {
                ShowLogo();
                ShowHelp();
                return 7;
            }

            // Create the root scope
            var preString = new StringSource(string.Empty, "Pre-sets", "init");
            var preSource = new SourcePosition(preString, 0);
            var root = new AstScope("global", preSource);

            var exprNodeIP = new ExprNodeIP(true);
            root.Define("$", exprNodeIP);
            var exprNodeIP2 = new ExprNodeIP(false);
            root.Define("$$", exprNodeIP2);
            var exprNodeOP = new ExprNodeOFS(true);
            root.Define("$ofs", exprNodeOP);
            var exprNodeOP2 = new ExprNodeOFS(false);
            root.Define("$$ofs", exprNodeOP2);
            root.AddElement(new AstTypeByte(preSource));
            root.AddElement(new AstTypeWord(preSource));

            root.AddUserDefines(userDefines);

            // Step 1 - Parse the input file

            //var p = new Parser();
            //p.IncludePaths.AddRange(includePaths);
            //var file = p.Parse(inputFile.Name, inputFile);
            //root.AddElement(file);
            //var position = file.SourcePosition;
            var sourcePosition = root.ParseFile(inputFile, includePaths);

            // Run the "Define Symbols" pass
            root.DefineSymbols(null);

            if (astFile != null)
            {
                using (var w = inputFile.OpenTextWriter(astFile, ".ast.txt"))
                {
                    root.Dump(w.WriteLine, 0);
                }
            }

            // Step 2 - Layout
            var layoutContext = new LayoutContext();
            exprNodeIP.SetContext(layoutContext);
            exprNodeIP2.SetContext(layoutContext);
            exprNodeOP.SetContext(layoutContext);
            exprNodeOP2.SetContext(layoutContext);
            root.Layout(null, layoutContext);

            if (Log.ErrorCount == 0)
            {
                TextWriter? listWriter = null;
                if (inputFile != null && listFile != null)
                    listWriter = inputFile.OpenTextWriter(listFile, "lst");

                // Step 3 - Generate
                var generateContext = new GenerateContext(
                    (s, newLine) =>
                    {
                        if (listWriter == null) return;
                        if (newLine)
                        {
                            listWriter.WriteLine(s);
                        }
                        else
                        {
                            listWriter.Write(s);
                        }
                    }
                    );

                exprNodeIP.SetContext(generateContext);
                exprNodeIP2.SetContext(generateContext);
                exprNodeOP.SetContext(generateContext);
                exprNodeOP2.SetContext(generateContext);

                generateContext.EnterSourceFile(sourcePosition);
                root.Generate(null, generateContext);
                generateContext.LeaveSourceFile();

                if (Log.ErrorCount == 0)
                {
                    var code = generateContext.GetGeneratedBytes();
                    var outputFile = this.outputFile == null
                        ? Path.ChangeExtension(inputFile.FullName, ".bin")
                        : this.outputFile;
                    File.WriteAllBytes(outputFile, code);
                }
            }

            // Write symbols file
            if (symbolsFile != null && Log.ErrorCount == 0)
            {
                using (var w = inputFile.OpenTextWriter(symbolsFile, "sym"))
                    root.DumpSymbols(w.WriteLine);
            }

            Log.DumpSummary();

            if (Log.ErrorCount != 0)
                return 7;

            return 0;
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