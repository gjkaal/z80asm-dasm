using System;
using System.Collections.Generic;
using System.IO;

namespace Z80Asm
{
    public class Parser
    {
        public Parser()
        {
        }

        private Tokenizer? tokenizer;

        public List<string> IncludePaths { get; } = new();

        public AstContainer Parse(string displayName, FileInfo file)
        {
            var stream = file.OpenText();
            var filetext = stream.ReadToEnd();
            var source = new StringSource(filetext + "\n", displayName, file.FullName);
            return Parse(source);
        }

        public AstContainer Parse(string displayName, string code)
        {
            var source = new StringSource(code + "\n", displayName, "literal");
            return Parse(source);
        }

        public ExprNode ParseExpression(string displayName, string expr)
        {
            tokenizer = new Tokenizer(new StringSource(expr, displayName, "expression"));
            var exprNode = ParseExpression();
            tokenizer.CheckToken(Token.EOF);
            return exprNode;
        }

        public ExprNode ParseExpression(string expr)
        {
            tokenizer = new Tokenizer(new StringSource(expr, string.Empty, string.Empty));
            var exprNode = ParseExpression();
            tokenizer.CheckToken(Token.EOF);
            return exprNode;
        }

        public AstContainer Parse(StringSource source)
        {
            try
            {
                // Create tokenizer
                tokenizer = new Tokenizer(source);

                // Create AST container
                var container = new AstContainer(
                    source.DisplayName,
                    tokenizer.Source.CreatePosition(0)
                );

                // Parse all elements
                ParseIntoContainer(container);

                // Should be at EOF?
                tokenizer.CheckToken(Token.EOF);

                // Return the container
                return container;
            }
            catch
            {
                tokenizer = null;
                throw;
            }
        }

        private Parser? OuterParser
        {
            get;
            set;
        }

        private bool IsParsing(string filename)
        {
            if (tokenizer == null) return false;
            if (tokenizer.Source.Location == filename)
                return true;
            if (OuterParser != null)
                return OuterParser.IsParsing(filename);
            return false;
        }

        private void ParseIntoContainer(AstContainer container)
        {
            if (tokenizer == null) return;
            while (tokenizer.Token != Token.EOF)
            {
                try
                {
                    var pos = tokenizer.TokenPosition;

                    // Check for container terminators
                    if (tokenizer.Token == Token.Identifier)
                    {
                        switch (tokenizer.TokenString.ToUpperInvariant())
                        {
                            case "ENDIF":
                            case "ELSE":
                            case "ELSEIF":
                            case "ENDP":
                            case "ENDM":
                                return;
                        }
                    }

                    // Parse an element
                    var elem = ParseAstElement(pos);

                    // Anything returned?
                    if (elem != null)
                    {
                        container.AddElement(elem);
                    }

                    // Unless it's a label, we should hit EOL after each element
                    if (!(elem is AstLabel))
                    {
                        tokenizer.SkipToken(Token.EOL);
                    }
                }
                catch (CodeException x)
                {
                    // Log error
                    Log.Error(x.Position, x.Message);

                    // Skip to next line
                    while (tokenizer.Token != Token.EOF && tokenizer.Token != Token.EOL)
                    {
                        try
                        {
                            tokenizer.Next();
                        }
                        catch (CodeException)
                        {
                            // Ignore other parse errors on this line
                        }
                    }
                }
            }
        }

        private AstElement? ParseAstElement(SourcePosition sourcePosition)
        {
            // Tokenizer not initialized?
            if (tokenizer == null)
                return null;
            // EOF?
            if (tokenizer.Token == Token.EOF)
                return null;

            // Blank line?
            if (tokenizer.Token == Token.EOL)
                return null;

            // ORG Element
            if (tokenizer.TrySkipIdentifier("ORG"))
            {
                return new AstOrgElement(ParseExpression(), sourcePosition);
            }

            // SEEK Element
            if (tokenizer.TrySkipIdentifier("SEEK"))
            {
                return new AstSeekElement(ParseExpression(), sourcePosition);
            }

            // END Element
            if (tokenizer.TrySkipIdentifier("END"))
            {
                while (tokenizer.Token != Token.EOF)
                    tokenizer.Next();
                return null;
            }

            // Include?
            if (tokenizer.TrySkipIdentifier("INCLUDE"))
            {
                // Load the include file
                string includeFile = ParseIncludePath();

                // Check for recursive inclusion of the same file
                if (IsParsing(includeFile))
                {
                    throw new CodeException($"error: recursive include file {tokenizer.TokenRaw}", tokenizer.TokenPosition);
                }

                string includeText;
                try
                {
                    includeText = System.IO.File.ReadAllText(includeFile);
                }
                catch (Exception x)
                {
                    throw new CodeException($"error: include file '{tokenizer.TokenRaw}' - {x.Message}", tokenizer.TokenPosition);
                }

                // Parse it
                var p = new Parser
                {
                    OuterParser = this
                };
                var content = p.Parse(new StringSource(includeText + "\n", System.IO.Path.GetFileName(includeFile), includeFile));

                // Skip the filename
                tokenizer.Next();

                // Return element
                return new AstInclude(includeFile, content, sourcePosition);
            }

            // IncBin?
            if (tokenizer.TrySkipIdentifier("INCBIN"))
            {
                // Load the include file
                string includeFile = ParseIncludePath();
                byte[] includeBytes;
                try
                {
                    includeBytes = File.ReadAllBytes(includeFile);
                }
                catch (Exception x)
                {
                    throw new CodeException($"error loading incbin file '{includeFile}' - {x.Message}", tokenizer.TokenPosition);
                }

                // Skip the filename
                tokenizer.Next();

                // Return element
                return new AstIncBin(includeFile, includeBytes, sourcePosition);
            }

            // DB?
            /*
            if (tokenizer.TrySkipIdentifier("DB") || tokenizer.TrySkipIdentifier("DEFB")
                || tokenizer.TrySkipIdentifier("DM") || tokenizer.TrySkipIdentifier("DEFM"))
            {
                var elem = new AstDbElement();
                ParseDxExpressions(elem);
                return elem;
            }

            // DW?
            if (tokenizer.TrySkipIdentifier("DW") || tokenizer.TrySkipIdentifier("DEFW"))
            {
                var elem = new AstDwElement();
                ParseDxExpressions(elem);
                return elem;
            }
            */

            // DS?
            if (tokenizer.TrySkipIdentifier("DS") || tokenizer.TrySkipIdentifier("DEFS"))
            {
                var elem = new AstDsElement(ParseExpression(), sourcePosition);
                if (tokenizer.TrySkipToken(Token.Comma))
                {
                    elem.ValueExpression = ParseExpression();
                }
                return elem;
            }

            // IF Block
            if (tokenizer.TrySkipIdentifier("IF"))
            {
                return ParseConditional(sourcePosition);
            }

            // PROC?
            if (tokenizer.TrySkipIdentifier("PROC"))
            {
                tokenizer.SkipToken(Token.EOL);

                var proc = new AstProc(sourcePosition);
                ParseIntoContainer(proc);

                tokenizer.SkipIdentifier("ENDP");

                return proc;
            }

            // RADIX
            if (tokenizer.IsIdentifier("RADIX"))
            {
                var saveRadix = tokenizer.DefaultRadix;
                try
                {
                    tokenizer.DefaultRadix = 10;
                    tokenizer.Next();
                    tokenizer.CheckToken(Token.Number);

                    switch (tokenizer.TokenNumber)
                    {
                        case 2:
                        case 8:
                        case 10:
                        case 16:
                            break;

                        default:
                            throw new CodeException("Invalid radix - must be 2, 8, 10, or 16", tokenizer.TokenPosition);
                    }

                    tokenizer.DefaultRadix = (int)tokenizer.TokenNumber;
                    tokenizer.Next();
                    return null;
                }
                catch
                {
                    tokenizer.DefaultRadix = saveRadix;
                    throw;
                }
            }

            // Error
            if (tokenizer.TrySkipIdentifier("ERROR"))
            {
                tokenizer.CheckToken(Token.String);
                var message = tokenizer.TokenString;
                tokenizer.Next();

                return new AstErrorWarning(message, false, sourcePosition);
            }

            // Warning
            if (tokenizer.TrySkipIdentifier("WARNING"))
            {
                tokenizer.CheckToken(Token.String);
                var message = tokenizer.TokenString;
                tokenizer.Next();

                return new AstErrorWarning(message, true, sourcePosition);
            }

            // DEFBITS?
            if (tokenizer.TrySkipIdentifier("DEFBITS"))
            {
                // Get the character
                tokenizer.CheckToken(Token.String);
                var character = tokenizer.TokenString;
                tokenizer.Next();

                // Skip the comma
                tokenizer.SkipToken(Token.Comma);

                if (tokenizer.Token == Token.String)
                {
                    // Get the bit pattern
                    tokenizer.CheckToken(Token.String);
                    var bitPattern = tokenizer.TokenString;
                    tokenizer.Next();
                    return new AstDefBits(character, bitPattern, sourcePosition);
                }
                else
                {
                    var bitWidth = ParseExpression();
                    tokenizer.SkipToken(Token.Comma);
                    var value = ParseExpression();
                    return new AstDefBits(character, value, bitWidth, sourcePosition);
                }
            }

            // BITMAP
            if (tokenizer.TrySkipIdentifier("BITMAP"))
            {
                // Parse width and height
                var width = ParseExpression();
                tokenizer.SkipToken(Token.Comma);
                var height = ParseExpression();

                // Bit order spec?
                bool msbFirst = true;
                if (tokenizer.TrySkipToken(Token.Comma))
                {
                    if (tokenizer.TrySkipIdentifier("msb"))
                        msbFirst = true;
                    else if (tokenizer.TrySkipIdentifier("lsb"))
                        msbFirst = false;
                    else
                        throw new CodeException("Expected 'MSB' or 'LSB'", tokenizer.TokenPosition);
                }

                // Create bitmap ast element
                var bitmap = new AstBitmap(width, height, msbFirst, sourcePosition);

                // Move to next line
                tokenizer.SkipToken(Token.EOL);

                // Consume all strings, one per line
                while (tokenizer.Token == Token.String)
                {
                    bitmap.AddString(tokenizer.TokenString);
                    tokenizer.Next();
                    tokenizer.SkipToken(Token.EOL);
                    continue;
                }

                // Skip the end dilimeter
                bitmap.EndPosition = tokenizer.TokenPosition;
                tokenizer.SkipIdentifier("ENDB");
                return bitmap;
            }

            if (tokenizer.Token == Token.Identifier)
            {
                // Remember the name
                var pos = tokenizer.TokenPosition;
                var name = tokenizer.TokenString;
                tokenizer.Next();
                string[] paramNames = [];

                // Parameterized EQU?
                if (tokenizer.Token == Token.OpenRound && !IsReservedWord(name))
                {
                    paramNames = ParseParameterNames();
                }

                // Followed by colon?
                bool haveColon = false;
                if (tokenizer.TrySkipToken(Token.Colon))
                {
                    haveColon = true;
                }

                // EQU?
                if (tokenizer.TrySkipIdentifier("EQU"))
                {
                    return new AstEquate(name, ParseOperandExpression(), pos)
                    {
                        ParameterNames = paramNames,
                    };
                }

                // MACRO?
                if (tokenizer.TrySkipIdentifier("MACRO"))
                {
                    tokenizer.SkipToken(Token.EOL);

                    var macro = new AstMacroDefinition(name, paramNames, sourcePosition);
                    ParseIntoContainer(macro);

                    tokenizer.SkipIdentifier("ENDM");

                    return macro;
                }

                // STRUCT?
                if (tokenizer.TrySkipIdentifier("STRUCT"))
                {
                    var structDef = new AstStructDefinition(name, sourcePosition);

                    // Process field definitions
                    while (tokenizer.Token != Token.EOF)
                    {
                        // Skip blank lines
                        if (tokenizer.TrySkipToken(Token.EOL))
                            continue;

                        // End of struct
                        if (tokenizer.TrySkipIdentifier("ENDS"))
                            break;

                        // Capture the field name (or could be type name)
                        var fieldDefPos = tokenizer.TokenPosition;
                        string? fieldName = null;

                        if (!tokenizer.TrySkipToken(Token.Colon))
                        {
                            tokenizer.CheckToken(Token.Identifier);
                            fieldName = tokenizer.TokenString;

                            // Next token
                            tokenizer.Next();
                        }

                        // Must be an identifier (for the type name)
                        tokenizer.CheckToken(Token.Identifier);
                        var fieldType = tokenizer.TokenString;
                        tokenizer.Next();

                        // Add the field definition
                        structDef.AddField(new AstFieldDefinition(fieldDefPos, fieldName, fieldType, ParseExpressionList()));

                        tokenizer.SkipToken(Token.EOL);
                    }

                    tokenizer.SkipToken(Token.EOL);
                    return structDef;
                }

                // Nothing from here on expected parameters
                if (paramNames.Length>0)
                    throw new CodeException("Unexpected parameter list in label", pos);

                // Was it a label?
                if (haveColon)
                {
                    if (IsReservedWord(name))
                        throw new CodeException($"Unexpected colon after reserved word '{name}'", pos);

                    return new AstLabel(name, pos, sourcePosition);
                }

                // Is it an instruction?
                if (InstructionSet.IsValidInstructionName(name))
                {
                    return ParseInstruction(pos, name, sourcePosition);
                }

                // Must be a macro invocation or a data declaration
                return ParseMacroInvocationOrDataDeclaration(pos, name);
            }

            // What?
            throw tokenizer.Unexpected();
        }

        private ExprNode ParseExpressionList()
        {
            ArgumentNullException.ThrowIfNull(tokenizer);   
            var pos = tokenizer.TokenPosition;
            var node = ParseExpression();
            if (tokenizer.TrySkipToken(Token.Comma))
            {
                var concat = new ExprNodeConcat(pos);
                concat.AddElement(node);

                while (tokenizer.Token != Token.EOL && tokenizer.Token != Token.CloseRound)
                {
                    concat.AddElement(ParseExpression());

                    if (tokenizer.TrySkipToken(Token.Comma))
                        continue;
                    else
                        break;
                }

                node = concat;
            }

            return node;
        }

        private AstElement ParseConditional(SourcePosition sourcePosition)
        {
            ArgumentNullException.ThrowIfNull(tokenizer);
            // var ifBlock = new AstConditional(sourcePosition);

            ExprNode condition;
            AstElement trueBlock;
            AstElement falseBlock;
            try
            {
                condition = ParseExpression();
            }
            catch (CodeException x)
            {
                Log.Error(x);
                while (tokenizer.Token != Token.EOF && tokenizer.Token != Token.EOL)
                    tokenizer.Next();
                condition = new ExprNodeUninitialized(sourcePosition);
            }
            trueBlock = new AstContainer("true block", sourcePosition);
            ParseIntoContainer((AstContainer)trueBlock);

            if (tokenizer.TrySkipIdentifier("ELSE"))
            {
                falseBlock = new AstContainer("false block", sourcePosition);
                ParseIntoContainer((AstContainer)falseBlock);
            }
            else if (tokenizer.TrySkipIdentifier("ELSEIF"))
            {
                falseBlock = ParseConditional(sourcePosition);
                return new AstConditional(sourcePosition, condition, trueBlock, falseBlock); 
            }
            else
            {
                // No ELSE block
                falseBlock = new AstContainer("false block", sourcePosition);
            }

            tokenizer.SkipIdentifier("ENDIF");
            return new AstConditional(sourcePosition, condition, trueBlock, falseBlock);
        }

        private string[] ParseParameterNames()
        {
            ArgumentNullException.ThrowIfNull(tokenizer);
            tokenizer.SkipToken(Token.OpenRound);

            var names = new List<string>();
            while (true)
            {
                // Get parameter name
                tokenizer.CheckToken(Token.Identifier);
                if (IsReservedWord(tokenizer.TokenString))
                {
                    throw new CodeException($"Illegal use of reserved word as a name: '{tokenizer.TokenString}'", tokenizer.TokenPosition);
                }

                // Add to list
                names.Add(tokenizer.TokenString);

                tokenizer.Next();

                // End of list
                if (tokenizer.TrySkipToken(Token.CloseRound))
                    return names.ToArray();

                // Comma
                tokenizer.SkipToken(Token.Comma);
            }
        }

        private string ParseIncludePath()
        {
            ArgumentNullException.ThrowIfNull(tokenizer);
            tokenizer.CheckToken(Token.String, "for filename");

            // Look up relative to this file first
            try
            {
                var thisFileDir = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(tokenizer.Source.Location));
                var includeFile = System.IO.Path.Combine(thisFileDir, tokenizer.TokenString);
                if (System.IO.File.Exists(includeFile))
                    return includeFile;
            }
            catch
            {
            }

            // Search include paths
            var parser = this;
            while (parser != null)
            {
                if (parser.IncludePaths != null)
                {
                    foreach (var p in parser.IncludePaths)
                    {
                        try
                        {
                            var includeFile = System.IO.Path.Combine(p, tokenizer.TokenString);
                            if (System.IO.File.Exists(includeFile))
                                return includeFile;
                        }
                        catch { }
                    }
                }

                // Try outer parser
                parser = parser.OuterParser;
            }

            // Assume current directory
            return tokenizer.TokenString;
        }

        /*
        void ParseDxExpressions(AstDxElement elem)
        {
            while (true)
            {
                if (tokenizer.Token == Token.String)
                {
                    if (elem is AstDbElement)
                    {
                        foreach (var b in Encoding.UTF8.GetBytes(tokenizer.TokenString))
                        {
                            elem.AddValue(new ExprNodeLiteral(tokenizer.TokenPosition, b));
                        }
                    }
                    else
                    {
                        foreach (var ch in tokenizer.TokenString)
                        {
                            elem.AddValue(new ExprNodeLiteral(tokenizer.TokenPosition, ch));
                        }
                    }
                    tokenizer.Next();
                }
                else
                {
                    elem.AddValue(ParseExpression());
                }
                if (!tokenizer.TrySkipToken(Token.Comma))
                    return;
            }
        }
        */

        private AstInstruction ParseInstruction(SourcePosition pos, string name, SourcePosition position)
        {
            var instruction = new AstInstruction(pos, name, position);

            if (tokenizer.Token == Token.EOL || tokenizer.Token == Token.EOF)
                return instruction;

            while (true)
            {
                instruction.AddOperand(ParseOperandExpression());
                if (!tokenizer.TrySkipToken(Token.Comma))
                    return instruction;
            }
        }

        private AstMacroInvocationOrDataDeclaration ParseMacroInvocationOrDataDeclaration(SourcePosition pos, string name)
        {
            var macroInvocation = new AstMacroInvocationOrDataDeclaration(name, pos);

            if (tokenizer.Token == Token.EOL || tokenizer.Token == Token.EOF)
                return macroInvocation;

            while (true)
            {
                macroInvocation.AddOperand(ParseOperandExpression());
                if (!tokenizer.TrySkipToken(Token.Comma))
                    return macroInvocation;
            }
        }

        private ExprNode[] ParseArgumentList()
        {
            tokenizer.SkipToken(Token.OpenRound);

            var args = new List<ExprNode>();
            while (true)
            {
                args.Add(ParseExpression());
                if (tokenizer.TrySkipToken(Token.CloseRound))
                    return args.ToArray();
                tokenizer.SkipToken(Token.Comma);
            }
        }

        private ExprNode ParseOrderedStructData()
        {
            var array = new ExprNodeOrderedStructData(tokenizer.TokenPosition);
            tokenizer.SkipToken(Token.OpenSquare);

            tokenizer.SkipWhitespace();

            while (tokenizer.Token != Token.EOF && tokenizer.Token != Token.CloseSquare)
            {
                array.AddElement(ParseExpression());
                if (tokenizer.TrySkipToken(Token.Comma))
                {
                    tokenizer.SkipWhitespace();
                    continue;
                }

                break;
            }

            tokenizer.SkipWhitespace();
            tokenizer.SkipToken(Token.CloseSquare);

            return array;
        }

        private ExprNode ParseNamedStructData()
        {
            var map = new ExprNodeNamedStructData(tokenizer.TokenPosition);
            tokenizer.SkipToken(Token.OpenBrace);

            tokenizer.SkipWhitespace();

            while (tokenizer.Token != Token.EOF && tokenizer.Token != Token.CloseBrace)
            {
                tokenizer.SkipWhitespace();

                // Get the token name
                tokenizer.CheckToken(Token.Identifier);
                var name = tokenizer.TokenString;
                if (IsReservedWord(name))
                    throw new CodeException($"Illegal use of reserved word '{name}'", tokenizer.TokenPosition);
                if (map.ContainsEntry(name))
                    throw new CodeException($"Duplicate key: '{name}'", tokenizer.TokenPosition);
                tokenizer.Next();

                // Skip the colon
                tokenizer.SkipToken(Token.Colon);

                // Parse the value
                var value = ParseExpression();

                // Add it
                map.AddEntry(name, value);

                // Another value?
                if (tokenizer.TrySkipToken(Token.Comma))
                {
                    tokenizer.SkipWhitespace();
                    continue;
                }

                break;
            }

            tokenizer.SkipWhitespace();
            tokenizer.SkipToken(Token.CloseBrace);

            return map;
        }

        private ExprNode ParseLeaf()
        {
            var pos = tokenizer.TokenPosition;

            // Number literal?
            if (tokenizer.Token == Token.Number)
            {
                var node = new ExprNodeNumberLiteral(pos, tokenizer.TokenNumber);
                tokenizer.Next();
                return node;
            }

            // String literal?
            if (tokenizer.Token == Token.String)
            {
                var str = tokenizer.TokenString;
                tokenizer.Next();
                return new ExprNodeStringLiteral(pos, str);
            }

            // Defined operator?
            if (tokenizer.TrySkipIdentifier("defined"))
            {
                tokenizer.SkipToken(Token.OpenRound);
                tokenizer.CheckToken(Token.Identifier);
                var node = new ExprNodeIsDefined(pos, tokenizer.TokenString);
                tokenizer.Next();
                tokenizer.SkipToken(Token.CloseRound);
                return node;
            }

            // Sizeof operator?
            if (tokenizer.TrySkipIdentifier("sizeof"))
            {
                tokenizer.SkipToken(Token.OpenRound);
                var node = new ExprNodeSizeOf(pos, ParseExpression());
                tokenizer.SkipToken(Token.CloseRound);
                return node;
            }

            // Identifier
            if (tokenizer.Token == Token.Identifier)
            {
                // Special identifier
                if (InstructionSet.IsConditionFlag(tokenizer.TokenString) ||
                    InstructionSet.IsValidRegister(tokenizer.TokenString))
                {
                    var node = new ExprNodeRegisterOrFlag(pos, tokenizer.TokenString);
                    tokenizer.Next();
                    return node;
                }
                else
                {
                    var node = new ExprNodeIdentifier(pos, tokenizer.TokenString);
                    tokenizer.Next();

                    if (tokenizer.Token == Token.Period)
                    {
                        ExprNode retNode = node;
                        while (tokenizer.TrySkipToken(Token.Period))
                        {
                            tokenizer.CheckToken(Token.Identifier);
                            retNode = new ExprNodeMember(tokenizer.TokenPosition, tokenizer.TokenString, retNode);
                            tokenizer.Next();
                        }
                        return retNode;
                    }

                    if (tokenizer.Token == Token.OpenRound)
                    {
                        node.SetArguments(ParseArgumentList());
                    }

                    return node;
                }
            }

            // Parens?
            if (tokenizer.TrySkipToken(Token.OpenRound))
            {
                var node = ParseExpressionList();
                tokenizer.SkipToken(Token.CloseRound);
                return node;
            }

            // Array?
            if (tokenizer.Token == Token.OpenSquare)
                return ParseOrderedStructData();

            // Map?
            if (tokenizer.Token == Token.OpenBrace)
                return ParseNamedStructData();

            throw new CodeException($"syntax error in expression: {Tokenizer.DescribeToken(tokenizer.Token, tokenizer.TokenRaw)}", tokenizer.TokenPosition);
        }

        private ExprNode ParseUnary()
        {
            var pos = tokenizer.TokenPosition;
            if (tokenizer.TrySkipToken(Token.BitwiseComplement))
            {
                return new ExprNodeUnary(pos)
                {
                    OpName = "~",
                    Operator = ExprNodeUnary.OpBitwiseComplement,
                    RHS = ParseUnary(),
                };
            }

            if (tokenizer.TrySkipToken(Token.LogicalNot))
            {
                return new ExprNodeUnary(pos)
                {
                    OpName = "!",
                    Operator = ExprNodeUnary.OpLogicalNot,
                    RHS = ParseUnary(),
                };
            }

            if (tokenizer.TrySkipToken(Token.Minus))
            {
                return new ExprNodeUnary(pos)
                {
                    OpName = "-",
                    Operator = ExprNodeUnary.OpNegate,
                    RHS = ParseUnary(),
                };
            }

            if (tokenizer.TrySkipToken(Token.Plus))
            {
                return ParseUnary();
            }

            // Parse leaf node
            return ParseLeaf();
        }

        private ExprNode ParseMultiply()
        {
            var LHS = ParseUnary();

            while (true)
            {
                var pos = tokenizer.TokenPosition;
                if (tokenizer.TrySkipToken(Token.Multiply))
                {
                    LHS = new ExprNodeBinary(pos, "*", ExprNodeBinary.OpMul)
                    {
                        LValue = LHS,
                        RValue = ParseUnary(),
                    };
                }
                else if (tokenizer.TrySkipToken(Token.Divide))
                {
                    LHS = new ExprNodeBinary(pos, "/", ExprNodeBinary.OpDiv)
                    {
                        LValue = LHS,
                        RValue = ParseUnary(),
                    };
                }
                else if (tokenizer.TrySkipToken(Token.Modulus))
                {
                    LHS = new ExprNodeBinary(pos, "%", ExprNodeBinary.OpMod)
                    {
                        LValue = LHS,
                        RValue = ParseUnary(),
                    };
                }
                else
                {
                    return LHS;
                }
            }
        }

        private ExprNode ParseAdd()
        {
            var LHS = ParseMultiply();

            while (true)
            {
                var pos = tokenizer.TokenPosition;
                if (tokenizer.TrySkipToken(Token.Plus))
                {
                    LHS = new ExprNodeAdd(pos)
                    {
                        LValue = LHS,
                        RValue = ParseMultiply(),
                    };
                }
                else if (tokenizer.TrySkipToken(Token.Minus))
                {
                    LHS = new ExprNodeAdd(pos)
                    {
                        LValue = LHS,
                        RValue = new ExprNodeUnary(pos)
                        {
                            RHS = ParseMultiply(),
                            OpName = "-",
                            Operator = ExprNodeUnary.OpNegate
                        }
                    };
                }
                else
                {
                    return LHS;
                }
            }
        }

        // Shift
        private ExprNode ParseShift()
        {
            var LHS = ParseAdd();

            while (true)
            {
                var pos = tokenizer.TokenPosition;
                if (tokenizer.TrySkipToken(Token.Shl))
                {
                    LHS = new ExprNodeBinary(pos, "<<", ExprNodeBinary.OpShl)
                    {
                        LValue = LHS,
                        RValue = ParseAdd(),
                    };
                }
                else if (tokenizer.TrySkipToken(Token.Shr))
                {
                    LHS = new ExprNodeBinary(pos, ">>", ExprNodeBinary.OpShr)
                    {
                        LValue = LHS,
                        RValue = ParseAdd(),
                    };
                }
                else
                {
                    return LHS;
                }
            }
        }

        private ExprNode ParseDup()
        {
            var expr = ParseShift();

            var pos = tokenizer.TokenPosition;
            if (tokenizer.TrySkipIdentifier("DUP"))
            {
                return new ExprNodeDup(pos, expr, ParseTernery());
            }

            return expr;
        }

        // Relational
        private ExprNode ParseRelational()
        {
            var LHS = ParseDup();

            while (true)
            {
                var pos = tokenizer.TokenPosition;
                if (tokenizer.TrySkipToken(Token.LE))
                {
                    LHS = new ExprNodeBinary(pos, "<=", ExprNodeBinary.OpLE)
                    {
                        LValue = LHS,
                        RValue = ParseDup(),
                    };
                }
                else if (tokenizer.TrySkipToken(Token.GE))
                {
                    LHS = new ExprNodeBinary(pos, ">=", ExprNodeBinary.OpGE)
                    {
                        LValue = LHS,
                        RValue = ParseDup(),
                    };
                }
                else if (tokenizer.TrySkipToken(Token.GT))
                {
                    LHS = new ExprNodeBinary(pos, ">", ExprNodeBinary.OpGT)
                    {
                        LValue = LHS,
                        RValue = ParseDup(),
                    };
                }
                else if (tokenizer.TrySkipToken(Token.LT))
                {
                    LHS = new ExprNodeBinary(pos, "<", ExprNodeBinary.OpLT)
                    {
                        LValue = LHS,
                        RValue = ParseDup(),
                    };
                }
                else
                {
                    return LHS;
                }
            }
        }

        // Equality
        private ExprNode ParseEquality()
        {
            var LHS = ParseRelational();

            while (true)
            {
                var pos = tokenizer.TokenPosition;
                if (tokenizer.TrySkipToken(Token.EQ))
                {
                    LHS = new ExprNodeBinary(pos, "==", ExprNodeBinary.OpEQ)
                    {
                        LValue = LHS,
                        RValue = ParseRelational(),
                    };
                }
                else if (tokenizer.TrySkipToken(Token.NE))
                {
                    LHS = new ExprNodeBinary(pos, "!=", ExprNodeBinary.OpNE)
                    {
                        LValue = LHS,
                        RValue = ParseRelational(),
                    };
                }
                else
                {
                    return LHS;
                }
            }
        }

        // Bitwise AND
        private ExprNode ParseBitwiseAnd()
        {
            var LHS = ParseEquality();

            while (true)
            {
                var pos = tokenizer.TokenPosition;
                if (tokenizer.TrySkipToken(Token.BitwiseAnd))
                {
                    LHS = new ExprNodeBinary(pos, "&", ExprNodeBinary.OpBitwiseAnd)
                    {
                        LValue = LHS,
                        RValue = ParseEquality(),
                    };
                }
                else
                {
                    return LHS;
                }
            }
        }

        // Bitwise XOR
        private ExprNode ParseBitwiseXor()
        {
            var LHS = ParseBitwiseAnd();

            while (true)
            {
                var pos = tokenizer.TokenPosition;
                if (tokenizer.TrySkipToken(Token.BitwiseXor))
                {
                    LHS = new ExprNodeBinary(pos, "^", ExprNodeBinary.OpBitwiseXor)
                    {
                        LValue = LHS,
                        RValue = ParseBitwiseAnd(),
                    };
                }
                else
                {
                    return LHS;
                }
            }
        }

        // Bitwise OR
        private ExprNode ParseBitwiseOr()
        {
            var LHS = ParseBitwiseXor();

            while (true)
            {
                var pos = tokenizer.TokenPosition;
                if (tokenizer.TrySkipToken(Token.BitwiseOr))
                {
                    LHS = new ExprNodeBinary(pos, "|", ExprNodeBinary.OpBitwiseOr)
                    {
                        LValue = LHS,
                        RValue = ParseBitwiseXor(),
                    };
                }
                else
                {
                    return LHS;
                }
            }
        }

        // Logical AND
        private ExprNode ParseLogicalAnd()
        {
            var LHS = ParseBitwiseOr();

            while (true)
            {
                var pos = tokenizer.TokenPosition;
                if (tokenizer.TrySkipToken(Token.LogicalAnd))
                {
                    LHS = new ExprNodeBinary(pos, "&&", ExprNodeBinary.OpLogicalAnd)
                    {
                        LValue = LHS,
                        RValue = ParseBitwiseOr(),
                    };
                }
                else
                {
                    return LHS;
                }
            }
        }

        // Logical OR
        private ExprNode ParseLogicalOr()
        {
            var LHS = ParseLogicalAnd();

            while (true)
            {
                var pos = tokenizer.TokenPosition;
                if (tokenizer.TrySkipToken(Token.LogicalOr))
                {
                    LHS = new ExprNodeBinary(pos, "||", ExprNodeBinary.OpLogicalOr)
                    {
                        LValue = LHS,
                        RValue = ParseLogicalAnd(),
                    };
                }
                else
                {
                    return LHS;
                }
            }
        }

        // Top level expression (except deref and z80 undocumented subops)
        private ExprNode ParseTernery()
        {
            // Uninitialized data initializer?
            var pos = tokenizer.TokenPosition;
            if (tokenizer.TrySkipToken(Token.Question))
                return new ExprNodeUninitialized(pos);

            // Parse the condition part
            var condition = ParseLogicalOr();

            // Is it a conditional
            pos = tokenizer.TokenPosition;
            if (tokenizer.TrySkipToken(Token.Question))
            {
                var trueNode = ParseExpression();

                tokenizer.SkipToken(Token.Colon);

                var falseNode = ParseExpression();

                return new ExprNodeTernery(pos, condition, trueNode, falseNode);
            }

            return condition;
        }

        private ExprNode ParseExpression()
        {
            var expr = ParseTernery();

            var pos = tokenizer.TokenPosition;
            if (tokenizer.TrySkipIdentifier("DUP"))
            {
                return new ExprNodeDup(pos, expr, ParseTernery());
            }

            return expr;
        }

        private ExprNode ParseOperandExpression()
        {
            // Save position
            var pos = tokenizer.TokenPosition;

            // Deref
            if (tokenizer.TrySkipToken(Token.OpenRound))
            {
                var node = new ExprNodeDeref(pos, ParseExpression());

                tokenizer.SkipToken(Token.CloseRound);

                return node;
            }

            // Is it a sub Op?
            if (tokenizer.Token == Token.Identifier && InstructionSet.IsValidSubOpName(tokenizer.TokenString))
            {
                var subOp = new ExprNodeSubOp(tokenizer.TokenPosition, tokenizer.TokenString);
                tokenizer.Next();
                subOp.RHS = ParseOperandExpression();
                return subOp;
            }

            // Normal expression
            return ParseExpression();
        }

        public static bool IsReservedWord(string str)
        {
            if (str == null)
                return false;

            if (InstructionSet.IsConditionFlag(str) ||
                InstructionSet.IsValidInstructionName(str) ||
                InstructionSet.IsValidRegister(str))
            {
                return true;
            }

            switch (str.ToUpperInvariant())
            {
                case "ORG":
                case "SEEK":
                case "END":
                case "INCLUDE":
                case "INCBIN":
                case "EQU":
                case "DB":
                case "DEFB":
                case "DEFM":
                case "DW":
                case "DEFW":
                case "DS":
                case "DEFS":
                case "IF":
                case "ENDIF":
                case "ELSE":
                case "ELSEIF":
                case "PROC":
                case "ENDP":
                case "MACRO":
                case "ENDM":
                case "DEFBITS":
                case "BITMAP":
                case "ENDB":
                case "ERROR":
                case "WARNING":
                case "RADIX":
                case "STRUCT":
                case "ENDS":
                case "BYTE":
                case "WORD":
                    return true;
            }

            return false;
        }
    }
}