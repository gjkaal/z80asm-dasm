using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Z80Asm
{
    // Base class for all Abstract Syntax Tree elements
    public abstract class AstElement
    {
        public AstElement(SourcePosition sourcePosition)
        {
            SourcePosition = sourcePosition;
        }

        public Guid Id { get; } = Guid.NewGuid();

        // The container holding this element
        public AstContainer? Container { get; private set; }

        public SourcePosition SourcePosition { get; private set; }

        public void AddContainer(AstContainer astContainer)
        {
            if (Container != null)
            {
                throw new CodeException("Container can only be set once.");
            }
            Container = astContainer;
        }

        public void ResetContainer()
        {
            Container = null;
        }

        // Find the close containing scope
        public AstScope? ContainingScope
        {
            get
            {
                var temp = Container;
                while (temp != null)
                {
                    var scope = temp as AstScope;
                    if (scope != null)
                    {
                        return scope;
                    }
                    temp = temp.Container;
                }
                return null;
            }
        }

        public abstract void Dump(Action<string> w, int indent);

        public virtual int DefineSymbols(AstScope currentScope)
        {
            return 1;
        }

        /// <summary>
        /// Layout the element in memory.
        /// </summary>
        /// <param name="currentScope">The abstract syntax tree scope.</param>
        /// <param name="ctx">the layout context</param>
        public abstract void Layout(AstScope currentScope, LayoutContext ctx);

        /// <summary>
        /// Generate machine code for this element
        /// </summary>
        /// <param name="currentScope">The abstract syntax tree scope</param>
        /// <param name="ctx">The generation context</param>
        public abstract void Generate(AstScope currentScope, GenerateContext ctx);
    }

    // Container for other AST elements
    public class AstContainer : AstElement
    {
        public AstContainer(string name, SourcePosition sourcePosition) : base(sourcePosition)
        {
            this.name = name;
        }

        private readonly string name;

        public string Name => name;

        public virtual void AddElement(AstElement element)
        {
            element.AddContainer(this);
            AddElementInternal(element);
        }

        protected void AddElementInternal(AstElement element)
        {
            if (element.Container == null)
            {
                throw new CodeException("Element must have a container");
            }
            elementList.Add(element);
        }

        public ReadOnlyCollection<AstElement> Elements => elementList.AsReadOnly();

        private readonly List<AstElement> elementList = [];

        public bool IsEmpty => elementList.Count == 0;
        public bool ElementExist(Guid id) => elementList.Exists(m => m.Id == id);

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- CONTAINER {Name} {SourcePosition.AstDesc()}");
            foreach (var e in elementList)
            {
                e.Dump(w, indent + 1);
            }
        }

        public override int DefineSymbols(AstScope currentScope)
        {
            foreach (var e in elementList)
            {
                try
                {
                    e.DefineSymbols(currentScope);
                }
                catch (CodeException x)
                {
                    Log.Error(x);
                }
            }
            return elementList.Count;
        }

        public override void Layout(AstScope currentScope, LayoutContext ctx)
        {
            foreach (var e in elementList)
            {
                try
                {
                    e.Layout(currentScope, ctx);
                }
                catch (CodeException x)
                {
                    Log.Error(x);
                }
            }
        }

        public override void Generate(AstScope currentScope, GenerateContext ctx)
        {
            foreach (var e in elementList)
            {
                try
                {
                    e.Generate(currentScope, ctx);
                }
                catch (CodeException x)
                {
                    Log.Error(x);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                    throw;
                }
            }
        }
    }

    public class AstConditional : AstElement
    {
        public AstConditional(
            SourcePosition pos,
            ExprNode condition,
            AstElement trueBlock,
            AstElement falseBlock
            ) : base(pos)
        {
            Condition = condition;
            TrueBlock = trueBlock;
            FalseBlock = falseBlock;
        }

        public ExprNode Condition { get; private set; }
        public AstElement TrueBlock { get; private set; }
        public AstElement FalseBlock { get; private set; }
        private bool _isTrue;

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- CONDITIONAL {SourcePosition.AstDesc()}");
            Condition.Dump(w, indent + 1);

            w.Invoke($"{Utils.Indent(indent + 1)}- TRUE BLOCK");
            TrueBlock.Dump(w, indent + 2);

            if (FalseBlock != null)
            {
                w.Invoke($"{Utils.Indent(indent + 1)}- FALSE BLOCK");
                FalseBlock.Dump(w, indent + 2);
            }
        }

        public override int DefineSymbols(AstScope currentScope)
        {
            _isTrue = Condition.EvaluateNumber(currentScope) != 0;

            if (_isTrue)
                TrueBlock.DefineSymbols(currentScope);
            else if (FalseBlock != null)
                FalseBlock.DefineSymbols(currentScope);
            return 1;
        }

        public override void Layout(AstScope currentScope, LayoutContext ctx)
        {
            if (_isTrue)
                TrueBlock.Layout(currentScope, ctx);
            else if (FalseBlock != null)
                FalseBlock.Layout(currentScope, ctx);
        }

        public override void Generate(AstScope currentScope, GenerateContext ctx)
        {
            if (_isTrue)
                TrueBlock.Generate(currentScope, ctx);
            else if (FalseBlock != null)
                FalseBlock.Generate(currentScope, ctx);
        }
    }

    // Like a container but contains symbol definitions
    public class AstScope : AstContainer
    {
        public AstScope(string name, SourcePosition pos) : base(name, pos)
        {
        }

        public AstScope(string name, SourcePosition pos, AstContainer outerContainer) : base(name, pos)
        {
            AddContainer(outerContainer);
        }

        // Define a symbols value
        public void Define(string symbol, ISymbolValue value, bool canReplace = false)
        {
            var normalized = symbol.NormalizeSymbol();
            if (!canReplace)
            {
                // Check not already defined in an outer scope
                var outerScope = ContainingScope;
                if (outerScope != null && outerScope.IsSymbolDefined(normalized))
                {
                    throw new InvalidOperationException(string.Format("The symbol '{0}' is already defined in an outer scope", symbol));
                }

                // Check if already defined
                if (_symbols.TryGetValue(normalized, out var existing))
                {
                    throw new InvalidOperationException(string.Format("Duplicate symbol: '{0}'", symbol));
                }
            }

            // Store it
            if (_symbols.ContainsKey(normalized))
            {
                _symbols.Remove(normalized, out _);
            }
            _symbols.Add(normalized, value);
            _weakMatchTable.Clear();
        }

        // Check if a symbol is defined in this scope or any outer scope
        public bool IsSymbolDefined(string symbol, bool weakMatch = false)
        {
            var normalized = symbol.NormalizeSymbol();
            if (weakMatch)
            {
                if (_weakMatchTable.Count == 0)
                {
                    foreach (var s in _symbols.Keys.Select(ExprNodeParameterized.RemoveSuffix))
                    {
                        _weakMatchTable.Add(s);
                    }
                }

                if (_weakMatchTable.Contains(normalized))
                    return true;

                var outerScope = ContainingScope;
                if (outerScope == null)
                    return false;

                return outerScope.IsSymbolDefined(normalized, weakMatch);
            }
            else
            {
                return FindSymbol(normalized) != null;
            }
        }

        // Find the definition of a symbol
        public ISymbolValue? FindSymbol(string symbol)
        {
            var normalized = symbol.NormalizeSymbol();

            if (normalized.Contains('/'))
            {
                // find parametrized symbol
                _symbols.TryGetValue(normalized, out var value);
                if (value != null)
                {
                    return value;
                }
            }
            // retry without parameters
            normalized = normalized.Split('/')[0];
            foreach (var s in _symbols.Keys)
            {
                var s0 = s.Split('/')[0];
                if (normalized == s0)
                    return _symbols[s];
            }
            var outerScope = ContainingScope;
            if (outerScope == null)
                return null;

            return outerScope.FindSymbol(normalized);
        }

        // Dictionary of symbols in this scope
        private readonly Dictionary<string, ISymbolValue> _symbols = new(StringComparer.InvariantCultureIgnoreCase);

        private readonly HashSet<string> _weakMatchTable = new(StringComparer.InvariantCultureIgnoreCase);

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- SCOPE {Name} {SourcePosition.AstDesc()}");
            foreach (var e in Elements)
            {
                e.Dump(w, indent + 1);
            }
        }

        public void DumpSymbols(Action<string> w)
        {
            w.Invoke("Symbols:");
            foreach (var kv in _symbols)
            {
                var sym = kv.Value as ExprNode;
                if (sym != null && sym is not ExprNodeParameterized)
                    w.Invoke($"    {kv.Key,20}: 0x{sym.EvaluateNumber(this):X4}");
            }
        }

        public IEnumerable<(string, long)> GetAllSymbols()
        {
            var symbolList = new List<(string, long)>();
            foreach (var kv in _symbols)
            {
                var sym = kv.Value as ExprNode;
                if (sym != null && sym is not ExprNodeParameterized)
                    symbolList.Add(new(kv.Key, sym.EvaluateNumber(this)));
            }
            return symbolList;
        }

        public override int DefineSymbols(AstScope? currentScope = null)
        {
            if (this is AstMacroDefinition macro)
                base.DefineSymbols(macro);
            else
                base.DefineSymbols(this);
            return _symbols.Count;
        }

        public override void Layout(AstScope? currentScope, LayoutContext ctx)
        {
            if (this is AstMacroDefinition macro)
                base.Layout(macro, ctx);
            else
                base.Layout(this, ctx);
        }

        public override void Generate(AstScope? currentScope, GenerateContext ctx)
        {
            if (this is AstMacroDefinition macro)
                base.Generate(macro, ctx);
            else
                base.Generate(this, ctx);
        }

        public void AddUserDefines(Dictionary<string, string?> userDefines)
        {
            foreach (var kv in userDefines)
            {
                var defParser = new Parser();
                if (kv.Value != null)
                {
                    try
                    {
                        var exprNode = defParser.ParseExpression(kv.Value);
                        Define(kv.Key, exprNode);
                    }
                    catch (CodeException x)
                    {
                        throw new InvalidOperationException(x.Message + " in command line symbol definition");
                    }
                }
                else
                {
                    Define(kv.Key, new ExprNodeNumberLiteral(null, 1));
                }
            }
        }

        public SourcePosition ParseFile(FileInfo inputFile, IEnumerable<string> includePaths)
        {
            var p = new Parser();
            p.IncludePaths.AddRange(includePaths);
            var file = p.Parse(inputFile.Name, inputFile);
            AddElement(file);
            var position = file.SourcePosition;
            return position;
        }

        public SourcePosition ParseLiteral(string name, string code)
        {
            var p = new Parser();
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
            var literal = p.Parse(name, code);
            AddElement(literal);
            var position = literal.SourcePosition;
            return position;
        }

        // See ExprNodeEquWrapper
        public int? ipOverride;

        public int? opOverride;
    }

    // "ORG" directive
    public class AstOrgElement : AstElement
    {
        public AstOrgElement(ExprNode expr, SourcePosition sourcePosition) : base(sourcePosition)
        {
            _expr = expr;
        }

        private readonly ExprNode _expr;
        private int _address;

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- ORG {SourcePosition.AstDesc()}");
            _expr.Dump(w, indent + 1);
        }

        public override void Layout(AstScope currentScope, LayoutContext ctx)
        {
            ctx.SetOrg(_address = (int)_expr.EvaluateNumber(currentScope));
        }

        public override void Generate(AstScope currentScope, GenerateContext ctx)
        {
            ctx.ListTo(SourcePosition);
            ctx.SetOrg(_address);
        }
    }

    // "SEEK" directive
    public class AstSeekElement : AstElement
    {
        public AstSeekElement(ExprNode expr, SourcePosition pos) : base(pos)
        {
            _expr = expr;
        }

        private readonly ExprNode _expr;
        private int _address;

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- SEEK {SourcePosition.AstDesc()}");
            _expr.Dump(w, indent + 1);
        }

        public override void Layout(AstScope currentScope, LayoutContext ctx)
        {
            ctx.Seek(_address = (int)_expr.EvaluateNumber(currentScope));
        }

        public override void Generate(AstScope currentScope, GenerateContext ctx)
        {
            ctx.ListTo(SourcePosition);
            ctx.Seek(_address);
        }
    }

    // "DS" directive
    public class AstDsElement : AstElement
    {
        public AstDsElement(ExprNode bytesExpr, SourcePosition pos) : base(pos)
        {
            _bytesExpression = bytesExpr;
        }

        private readonly ExprNode _bytesExpression;
        private int _bytes;

        public ExprNode? ValueExpression { get; set; }

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- DW {SourcePosition.AstDesc()}");
            _bytesExpression.Dump(w, indent + 1);
        }

        public override void Layout(AstScope currentScope, LayoutContext ctx)
        {
            ctx.ReserveBytes(_bytes = (int)_bytesExpression.EvaluateNumber(currentScope));
        }

        public override void Generate(AstScope currentScope, GenerateContext ctx)
        {
            ctx.ListTo(SourcePosition);
            ctx.WriteListingText($"{ctx.Ip:X4}: [{_bytes} bytes]");
            ctx.ListToInclusive(SourcePosition);

            var bytes = new byte?[_bytes];

            if (ValueExpression != null)
            {
                var value = Utils.PackByte(ValueExpression.SourcePosition, ValueExpression.EvaluateNumber(currentScope));

                for (var i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = value;
                }
            }

            ctx.EmitBytes(bytes, false);
        }
    }

    // Label
    public class AstLabel : AstElement
    {
        public AstLabel(
            string name,
            SourcePosition target,
            SourcePosition sourcePosition) : base(sourcePosition)
        {
            _name = name;
            _position = target;
        }

        private readonly string _name;
        private readonly SourcePosition _position;
        private readonly Dictionary<AstScope, ExprNodeDeferredValue> _valueToScopeMap = new();

        public string Name => _name;

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- LABEL '{_name}' {SourcePosition.AstDesc()}");
        }

        public override int DefineSymbols(AstScope currentScope)
        {
            var valueInScope = new ExprNodeDeferredValue(_position, _name);
            _valueToScopeMap.Add(currentScope, valueInScope);
            currentScope.Define(_name, valueInScope);
            return _valueToScopeMap.Count;
        }

        public override void Layout(AstScope currentScope, LayoutContext ctx)
        {
            _valueToScopeMap[currentScope].Resolve(ctx.Ip);
        }

        public override void Generate(AstScope currentScope, GenerateContext ctx)
        {
            // Mot extra work needed for labels in generated code
            ctx.ListTo(SourcePosition);
        }
    }

    // "EQU" directive
    public class AstEquate : AstElement
    {
        public AstEquate(string name, ExprNode value, SourcePosition position) : base(position)
        {
            _name = name;
            _value = new ExprNodeEquWrapper(position, value, name);
        }

        private readonly string _name;
        private readonly ExprNode _value;

        public string Name => _name;
        public ExprNode Value => _value;

        public string[] ParameterNames;

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- EQU '{_name}' {SourcePosition.AstDesc()}");

            if (ParameterNames != null)
            {
                w.Invoke($"{Utils.Indent(indent + 1)}- PARAMETERS");
                foreach (var p in ParameterNames)
                {
                    w.Invoke($"{Utils.Indent(indent + 2)}- '{p}'");
                }
            }

            _value.Dump(w, indent + 1);
        }

        public override int DefineSymbols(AstScope currentScope)
        {
            if (ParameterNames == null)
            {
                // Wrap it in an IP override declaration
                currentScope.Define(_name, _value);
                return 1;
            }
            else
            {
                var value = new ExprNodeParameterized(SourcePosition, ParameterNames, _value);
                currentScope.Define(_name + ExprNodeParameterized.MakeSuffix(ParameterNames.Length), value);
                return ParameterNames.Length;
            }
        }

        public override void Layout(AstScope currentScope, LayoutContext ctx)
        {
            // Capture the current Ip address as the place where this EQU was defined
            ((ExprNodeEquWrapper)_value).SetOverrides(ctx.Ip, ctx.Op);
        }

        public override void Generate(AstScope currentScope, GenerateContext ctx)
        {
            // No code generation needed for EQU
            ctx.ListTo(SourcePosition);
        }
    }

    // Include directive
    public class AstInclude : AstElement
    {
        public AstInclude(string filename, AstContainer content, SourcePosition position) : base(position)
        {
            _filename = filename;
            _content = content;
        }

        private readonly string _filename;
        private readonly AstContainer _content;

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- INCLUDE '{_filename}' {SourcePosition.AstDesc()}");
            _content.Dump(w, indent + 1);
        }

        public override int DefineSymbols(AstScope currentScope)
        {
            return _content.DefineSymbols(currentScope);
        }

        public override void Layout(AstScope currentScope, LayoutContext ctx)
        {
            _content.Layout(currentScope, ctx);
        }

        public override void Generate(AstScope currentScope, GenerateContext ctx)
        {
            ctx.ListToInclusive(SourcePosition);
            ctx.EnterSourceFile(_content.SourcePosition);
            _content.Generate(currentScope, ctx);
            ctx.LeaveSourceFile();
        }
    }

    // IncBin directive
    public class AstIncBin : AstElement
    {
        public AstIncBin(string filename, byte[] data, SourcePosition position) : base(position)
        {
            _filename = filename;
            _data = data;
        }

        private readonly string _filename;
        private readonly byte[] _data;

        public override void Dump(Action<string> w, int indent)
        {
            if (_data.Length == 0)
            {
                w.Invoke($"{Utils.Indent(indent)}- INCBIN '{_filename}' {SourcePosition.AstDesc()}");
                w.Invoke($"{Utils.Indent(indent + 1)}- <empty>");
                return;
            }

            w.Invoke($"{Utils.Indent(indent)}- INCBIN '{_filename}' {SourcePosition.AstDesc()}");

            var sb = new StringBuilder();
            for (var i = 0; i < _data.Length; i++)
            {
                if ((i % 16) == 0 && i > 0)
                {
                    sb.AppendLine($"\n{Utils.Indent(indent + 1)}- ");
                }
                sb.Append($"{_data[i]:X2} ");
            }
            sb.AppendLine();

            if (_data.Length > 0)
                w.Invoke(sb.ToString());
        }

        public override void Layout(AstScope currentScope, LayoutContext ctx)
        {
            ctx.ReserveBytes(_data.Length);
        }

        public override void Generate(AstScope currentScope, GenerateContext ctx)
        {
            ctx.ListTo(SourcePosition);
            ctx.WriteListingText($"{ctx.Ip:X4}: [{_data.Length} bytes]");
            ctx.ListToInclusive(SourcePosition);
            ctx.EmitBytes(_data, false);
        }
    }

    public class AstInstruction : AstElement
    {
        public AstInstruction(
            SourcePosition position,
            string mnemonic,
            SourcePosition sourcePosition) : base(sourcePosition)
        {
            _position = position;
            _mnemonic = mnemonic;
        }

        public void AddOperand(ExprNode operand)
        {
            _operands.Add(operand);
        }

        private readonly SourcePosition _position;
        private readonly string _mnemonic;
        private readonly List<ExprNode> _operands = new();
        private Instruction? _instruction;

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- {_mnemonic.ToUpperInvariant()} {SourcePosition.AstDesc()}");
            foreach (var o in _operands)
            {
                o.Dump(w, indent + 1);
            }
        }

        private bool IsIndexRegister(string reg)
        {
            // Don't insert implicit +0 for (IX) and (IY) when JP instruction
            if (_mnemonic.ToUpper() == "JP")
                return false;

            reg = reg.ToUpperInvariant();
            return reg == "IX" || reg == "IY";
        }

        public override void Layout(AstScope currentScope, LayoutContext ctx)
        {
            var sb = new StringBuilder();

            sb.Append(_mnemonic);

            for (var i = 0; i < _operands.Count; i++)
            {
                if (i > 0)
                    sb.Append(',');
                else
                    sb.Append(' ');

                var o = _operands[i];

                var addressingMode = o.GetAddressingMode(currentScope);

                if ((addressingMode & AddressingMode.SubOp) != 0)
                {
                    sb.Append(o.GetSubOp());
                    sb.Append(' ');
                    addressingMode = addressingMode & ~AddressingMode.SubOp;
                }

                switch (addressingMode)
                {
                    case AddressingMode.Deref | AddressingMode.Immediate:
                        sb.Append($"(?)");
                        break;

                    case AddressingMode.Deref | AddressingMode.Register:
                        {
                            var reg = o.GetRegister(currentScope);
                            if (IsIndexRegister(reg))
                            {
                                sb.Append($"({reg}+?)");
                            }
                            else
                            {
                                sb.Append($"({reg})");
                            }
                        }
                        break;

                    case AddressingMode.Deref | AddressingMode.RegisterPlusImmediate:
                        sb.Append($"({o.GetRegister(currentScope)}+?)");
                        break;

                    case AddressingMode.Immediate:
                        sb.Append('?');
                        break;

                    case AddressingMode.Register:
                        sb.Append($"{o.GetRegister(currentScope)}");
                        break;

                    case AddressingMode.RegisterPlusImmediate:
                        sb.Append($"{o.GetRegister(currentScope)}+?");
                        break;

                    default:
                        sb.Append($"<illegal expression>");
                        break;
                }
            }

            _instruction = InstructionSet.Find(sb.ToString());

            if (_instruction == null)
            {
                Log.Warning(_position, $"invalid instruction: {sb.ToString()}");
            }
            else
            {
                ctx.ReserveBytes(_instruction.Length);
            }
        }

        public override void Generate(AstScope currentScope, GenerateContext ctx)
        {
            ctx.ListTo(SourcePosition);
            ctx.EnterInstruction(this);
            try
            {
                List<long> immediateValues = [];
                for (var i = 0; i < _operands.Count; i++)
                {
                    var o = _operands[i];

                    var addressingMode = o.GetAddressingMode(currentScope);

                    if ((addressingMode & AddressingMode.Register) != 0 &&
                        (addressingMode & AddressingMode.Deref) != 0 &&
                        (addressingMode & AddressingMode.Immediate) == 0 &&
                        IsIndexRegister(o.GetRegister(currentScope)))
                    {
                        immediateValues.Add(0);
                        continue;
                    }

                    if ((addressingMode & AddressingMode.Immediate) != 0)
                    {
                        immediateValues.Add(o.GetImmediateValue(currentScope));
                    }
                }

                // Generate the instruction
                if (_instruction != null)
                    _instruction.Generate(ctx, SourcePosition, immediateValues.ToArray());
            }
            finally
            {
                ctx.LeaveInstruction(this);
            }
        }
    }

    public class AstProc : AstScope
    {
        public AstProc(SourcePosition sourcePosition) : base("PROC", sourcePosition)
        {
        }
    }

    public class AstMacroDefinition : AstScope, ISymbolValue
    {
        public AstMacroDefinition(string name, string[] parameters, SourcePosition sourcePosition)
            : base("MACRO " + name, sourcePosition)
        {
            _name = name;
            _parameters = parameters;
        }

        private readonly string _name;
        private string[] _parameters;

        public override int DefineSymbols(AstScope currentScope)
        {
            if (_parameters == null)
            {
                _parameters = [];
            }

            // Define the symbol
            currentScope.Define(_name + ExprNodeParameterized.MakeSuffix(_parameters.Length), this);
            return _parameters.Length;
        }

        public override void Layout(AstScope currentScope, LayoutContext ctx)
        {
            // no-Op
        }

        public override void Generate(AstScope currentScope, GenerateContext ctx)
        {
            // no-Op
        }

        public AstScope Resolve(AstScope currentScope, ExprNode[] arguments)
        {
            // Create new scope for macro evaluation            
            var scope = new AstScope("MACRO INVOCATION", currentScope.SourcePosition, currentScope);

            // Define it
            for (var i = 0; i < _parameters.Length; i++)
            {
                scope.Define(_parameters[i], arguments[i]);
            }
            return scope;
        }

        private bool _recursionFlag;

        private void CheckForRecursion()
        {
            if (_recursionFlag)
            {
                throw new CodeException($"Recursive macro reference: {_name}", SourcePosition);
            }
        }

        public void DefineSymbolsResolved(AstScope resolvedScope)
        {
            CheckForRecursion();
            _recursionFlag = true;
            try
            {
                base.DefineSymbols(resolvedScope);
            }
            finally
            {
                _recursionFlag = false;
            }
        }

        public void LayoutResolved(AstScope resolvedScope, LayoutContext ctx)
        {
            CheckForRecursion();
            _recursionFlag = true;
            try
            {
                base.Layout(resolvedScope, ctx);
            }
            finally
            {
                _recursionFlag = false;
            }
        }

        public void GenerateResolved(AstScope resolvedScope, GenerateContext ctx)
        {
            CheckForRecursion();
            _recursionFlag = true;
            try
            {
                base.Generate(resolvedScope, ctx);
            }
            finally
            {
                _recursionFlag = false;
            }
        }
    }

    public class AstMacroInvocationOrDataDeclaration : AstElement
    {
        public AstMacroInvocationOrDataDeclaration(
            string macro,
            SourcePosition source) : base(source)
        {
            _macroOrDataTypeName = macro;
        }

        public void AddOperand(ExprNode operand)
        {
            _operands.Add(operand);
        }

        private readonly string _macroOrDataTypeName;
        private readonly List<ExprNode> _operands = new();

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- {_macroOrDataTypeName} {SourcePosition.AstDesc()}");
            foreach (var o in _operands)
            {
                o.Dump(w, indent + 1);
            }
        }

        public override int DefineSymbols(AstScope currentScope)
        {
            return base.DefineSymbols(currentScope);
        }

        private AstType? _dataType;
        private int _reservedBytes;
        private AstMacroDefinition? _macroDefinition;
        private AstScope? _resolvedScope;

        public override void Layout(AstScope currentScope, LayoutContext ctx)
        {
            // Is it a data declaration?
            _dataType = currentScope.FindSymbol(_macroOrDataTypeName) as AstType;
            if (_dataType != null)
            {
                // Work out how many elements in total
                var totalElements = 0;
                foreach (var n in _operands)
                {
                    totalElements += n.EnumData(currentScope).Count();
                }

                // Reserve space
                ctx.ReserveBytes(_reservedBytes = totalElements * _dataType.SizeOf);

                return;
            }

            // Is it a macro invocation?
            _macroDefinition = currentScope.FindSymbol(_macroOrDataTypeName + ExprNodeParameterized.MakeSuffix(_operands.Count)) as AstMacroDefinition;
            if (_macroDefinition != null)
            {
                // Create resolved scope
                _resolvedScope = _macroDefinition.Resolve(currentScope, _operands.ToArray());

                // Define macro symbols
                _macroDefinition.DefineSymbolsResolved(_resolvedScope);

                // Layout
                _macroDefinition.LayoutResolved(_resolvedScope, ctx);

                return;
            }

            throw new CodeException($"Unrecognized symbol: '{_macroOrDataTypeName}' is not a known data type or macro (with {_operands.Count} arguments)", SourcePosition);
        }

        public override void Generate(AstScope currentScope, GenerateContext ctx)
        {
            ctx.ListTo(SourcePosition);

            // Is it a data declaration?
            if (_dataType != null)
            {
                // Setup storage for data
                var data = new List<byte?>();

                // Pack data elements
                var anyPackErrors = false;
                foreach (var n in _operands.SelectMany(x => x.EnumData(currentScope)))
                {
                    try
                    {
                        PackData(currentScope, data, _dataType, 1, n);
                    }
                    catch (CodeException x)
                    {
                        Log.Error(x);
                        anyPackErrors = true;
                    }
                }

                // Sanity check
                if (!anyPackErrors && _reservedBytes != data.Count)
                    throw new CodeException($"Internal error packing data declaration (should have generated {_reservedBytes} but actually generated {data.Count}", SourcePosition);

                // Emit the data
                ctx.EmitBytes(data.ToArray(), true);
                return;
            }

            // Is it a macro invocation?
            if (_macroDefinition != null)
            {
                ctx.EnterMacro();
                try
                {
                    _macroDefinition.GenerateResolved(_resolvedScope, ctx);
                }
                finally
                {
                    ctx.LeaveMacro();
                }
                return;
            }
        }

        private void PackData(AstScope scope, List<byte?> buffer, AstType dataType, int arraySize, ExprNode expr)
        {
            // Packing into an array?
            if (arraySize != 1)
            {
                var packCount = 0;
                foreach (var d in expr.EnumData(scope))
                {
                    PackData(scope, buffer, dataType, 1, d);
                    packCount++;
                }

                // Too big?
                if (packCount > arraySize)
                {
                    throw new CodeException($"Data too big for field: room for {arraySize}, but found {packCount}", expr.SourcePosition);
                }

                // Fill the rest with zero
                if (packCount < arraySize)
                {
                    buffer.AddRange(Enumerable.Repeat<byte?>(0, dataType.SizeOf * (arraySize - packCount)));
                }
                return;
            }

            // Get the value
            var value = expr.Evaluate(scope);

            // Uninitialized data?
            if (value is ExprNodeUninitialized)
            {
                buffer.AddRange(Enumerable.Repeat<byte?>(null, dataType.SizeOf));
                return;
            }

            // Zero fill data?
            if ((value is long) && (long)value == 0)
            {
                buffer.AddRange(Enumerable.Repeat<byte?>(0, dataType.SizeOf));
                return;
            }

            // Byte?
            if (dataType is AstTypeByte)
            {
                while (value is ExprNode)
                    value = ((ExprNode)value).Evaluate(scope);

                buffer.Add(Utils.PackByte(expr.SourcePosition, value));
                return;
            }

            // Word?
            if (dataType is AstTypeWord)
            {
                while (value is ExprNode)
                    value = ((ExprNode)value).Evaluate(scope);

                var word = Utils.PackWord(expr.SourcePosition, value);
                buffer.Add((byte)(word & 0xFF));
                buffer.Add((byte)((word >> 8) & 0xFF));
                return;
            }

            // Pack into struct
            var structDef = dataType as AstStructDefinition;
            if (structDef != null)
            {
                // Initializing with an array?
                var array = value as ExprNode[];
                if (array != null)
                {
                    // Check length match
                    if (array.Length != structDef.Fields.Count)
                    {
                        throw new CodeException($"Data declaration error: type '{structDef.Name}' requires {structDef.Fields.Count} initializers, but {array.Length} specified", expr.SourcePosition);
                    }

                    // Pack all fields
                    for (var i = 0; i < array.Length; i++)
                    {
                        PackData(scope, buffer, structDef.Fields[i].Type, structDef.Fields[i].ArraySize, array[i]);
                    }

                    return;
                }

                var map = value as Dictionary<string, ExprNode>;
                if (map != null)
                {
                    // Create buffer to pack structure
                    var data = new byte?[dataType.SizeOf];
                    foreach (var kv in map)
                    {
                        // Find the field
                        var fd = dataType.FindField(kv.Key);
                        if (fd == null)
                            throw new CodeException($"Data declaration error: type '{structDef.Name}' doesn't have a field '{kv.Key}'", kv.Value.SourcePosition);

                        // Pack the field
                        var fieldPack = new List<byte?>();
                        PackData(scope, fieldPack, fd.Type, fd.ArraySize, kv.Value);

                        // Check packed correct number of bytes
                        if (fieldPack.Count != fd.Type.SizeOf)
                        {
                            throw new CodeException($"Internal error packing data declaration (should have generated {fd.Type.SizeOf} but actually generated {fieldPack.Count}", SourcePosition);
                        }

                        // Pack it
                        for (var i = 0; i < fieldPack.Count; i++)
                        {
                            data[fd.Offset + i] = fieldPack[i];
                        }
                    }

                    // Add to buffer
                    buffer.AddRange(data);
                    return;
                }

                throw new CodeException($"Data declaration error: can't pack <{Utils.TypeName(expr)}> as '{structDef.Name}'", expr.SourcePosition);
            }

            throw new CodeException($"Internal error: don't know how to pack {Utils.TypeName(expr)} into {Utils.TypeName(dataType)}", expr.SourcePosition);
        }
    }

    public class AstDefBits : AstElement, ISymbolValue
    {
        public AstDefBits(
            string character,
            string bitPattern,
            SourcePosition position) : base(position)
        {
            _character = character;
            _bitPattern = bitPattern;
        }

        public AstDefBits(
            string character,
            ExprNode value,
            ExprNode bitWidth,
            SourcePosition position) : base(position)
        {
            _character = character;
            _value = value;
            _bitWidth = bitWidth;
        }

        private readonly string _character;
        private string? _bitPattern;
        private readonly ExprNode? _value;
        private readonly ExprNode? _bitWidth;

        public string GetBitPattern(AstScope scope)
        {
            if (_bitPattern == null)
            {
                if (_value == null || _bitWidth == null)
                    throw new CodeException("Internal error: value and bit width must be set", SourcePosition);

                var value = (int)_value.EvaluateNumber(scope);
                var bitWidth = (int)_bitWidth.EvaluateNumber(scope);
                var str = Convert.ToString(value, 2);
                if (str.Length > bitWidth)
                {
                    var cutPart = str.Substring(0, str.Length - bitWidth);
                    if (cutPart.Distinct().Count() != 1 || cutPart[0] != str[str.Length - bitWidth])
                        throw new CodeException($"DEFBITS value 0b{str} doesn't fit in {bitWidth} bits", SourcePosition);
                }
                else
                {
                    str = new string('0', bitWidth - str.Length) + str;
                }

                _bitPattern = str;
            }

            return _bitPattern;
        }

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- DEFBITS '{_character}' = '{_bitPattern}' {SourcePosition.AstDesc()}");
        }

        public override int DefineSymbols(AstScope currentScope)
        {
            if (_character.Length != 1)
            {
                throw new CodeException("Bit pattern names must be a single character", SourcePosition);
            }

            if (_bitPattern != null)
            {
                for (var i = 0; i < _bitPattern.Length; i++)
                {
                    if (_bitPattern[i] != '0' && _bitPattern[i] != '1')
                    {
                        throw new CodeException("Bit patterns must only contain '1' and '0' characters", SourcePosition);
                    }
                }
            }

            currentScope.Define($"bitpattern'{_character}'", this, true);
            return 1;
        }

        public override void Layout(AstScope currentScope, LayoutContext ctx)
        {
            // No layout needed
        }

        public override void Generate(AstScope currentScope, GenerateContext ctx)
        {
            // No code generation needed
        }
    }

    public class AstBitmap : AstElement
    {
        public AstBitmap(
            ExprNode width,
            ExprNode height,
            bool msbFirst,
            SourcePosition position) : base(position)
        {
            _width = width;
            _height = height;
            _msbFirst = msbFirst;
        }

        private readonly ExprNode _width;
        private readonly ExprNode _height;
        private readonly bool _msbFirst;
        protected List<string> _strings = new();

        public void AddString(string str)
        {
            _strings.Add(str);
        }

        public SourcePosition EndPosition;

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- BITMAP {SourcePosition.AstDesc()}");
            _width.Dump(w, indent + 1);
            _height.Dump(w, indent + 1);
            foreach (var v in _strings)
            {
                w.Invoke($"{Utils.Indent(indent + 1)} '{v}'");
            }
        }

        private byte[] _bytes;

        public override void Layout(AstScope currentScope, LayoutContext ctx)
        {
            // Work out width and height
            var blockWidth = (int)_width.EvaluateNumber(currentScope);
            var blockHeight = (int)_height.EvaluateNumber(currentScope);
            if (blockWidth < 1)
                throw new CodeException("Invalid bitmap block width", SourcePosition);
            if (blockHeight < 1)
                throw new CodeException("Invalid bitmap block height", SourcePosition);

            // Build the bitmap
            var bits = new List<string>();
            for (var i = 0; i < _strings.Count; i++)
            {
                var row = new StringBuilder();

                foreach (var ch in _strings[i])
                {
                    // Find the bit definition
                    var bitdef = currentScope.FindSymbol($"bitpattern'{ch}'") as AstDefBits;
                    if (bitdef == null)
                        throw new CodeException($"No bit definition for character '{ch}'", SourcePosition);

                    row.Append(bitdef.GetBitPattern(currentScope));
                }

                bits.Add(row.ToString());
            }

            if (bits.Select(x => x.Length).Distinct().Count() != 1)
                throw new CodeException("All rows in a bitmap must the same length", SourcePosition);

            if ((bits[0].Length % blockWidth) != 0)
                throw new CodeException("Bitmap width must be a multiple of the block width", SourcePosition);

            if ((bits.Count % blockHeight) != 0)
                throw new CodeException("Bitmap height must be a multiple of the block height", SourcePosition);

            if ((blockWidth * bits.Count % 8) != 0)
                throw new CodeException("Bitmap block width multiplied by bitmap height must be a multiple of 8", SourcePosition);

            var blocksAcross = bits[0].Length / blockWidth;
            var blocksDown = bits.Count / blockHeight;

            var bitCounter = 0;
            byte assembledByte = 0;
            var bytes = new List<byte>();
            for (var blockY = 0; blockY < blocksDown; blockY++)
            {
                for (var blockX = 0; blockX < blocksAcross; blockX++)
                {
                    for (var bitY = 0; bitY < blockHeight; bitY++)
                    {
                        for (var bitX = 0; bitX < blockWidth; bitX++)
                        {
                            var bit = bits[(blockY * blockHeight) + bitY][(blockX * blockWidth) + bitX];
                            if (_msbFirst)
                                assembledByte = (byte)((assembledByte << 1) | (bit == '1' ? 1 : 0));
                            else
                                assembledByte = (byte)((assembledByte >> 1) | (bit == '1' ? 0x80 : 0));

                            bitCounter++;
                            if (bitCounter == 8)
                            {
                                bitCounter = 0;
                                bytes.Add(assembledByte);
                            }
                        }
                    }
                }
            }

            _bytes = bytes.ToArray();
            ctx.ReserveBytes(_bytes.Length);
        }

        public override void Generate(AstScope currentScope, GenerateContext ctx)
        {
            // List out the bitmap definition
            ctx.ListToInclusive(EndPosition);

            if (_bytes == null)
                return;

            // Now list the bytes
            ctx.EmitBytes(_bytes, true);
        }
    }

    public class AstErrorWarning : AstElement
    {
        public AstErrorWarning(
            string message,
            bool warning,
            SourcePosition position) : base(position)
        {
            _message = message;
            _warning = warning;
        }

        private readonly string _message;
        private readonly bool _warning;

        public override int DefineSymbols(AstScope currentScope)
        {
            return base.DefineSymbols(currentScope);
        }

        public override void Layout(AstScope currentScope, LayoutContext ctx)
        {
            // No layout needed
        }

        public override void Dump(Action<string> w, int indent)
        {
            if (_warning)
                w.Invoke($"{Utils.Indent(indent)}- WARNING: '{_message}' {SourcePosition.AstDesc()}");
            else
                w.Invoke($"{Utils.Indent(indent)}- ERROR: '{_message}' {SourcePosition.AstDesc()}");
        }

        public override void Generate(AstScope currentScope, GenerateContext ctx)
        {
            if (_warning)
            {
                Log.Warning(SourcePosition, _message);
            }
            else
            {
                Log.Error(SourcePosition, _message);
            }
        }
    }

    public class AstFieldDefinition : AstElement
    {
        public AstFieldDefinition(
            SourcePosition pos,
            string name,
            string typename,
            ExprNode initializer) : base(pos)
        {
            _name = name;
            _typename = typename;
            _initializer = initializer;
        }

        private readonly string _name;
        private readonly string _typename;
        private AstType? _type;
        private int _offset;
        private int _arraySize;
        private readonly ExprNode _initializer;

        public string Name => _name;
        public AstType? Type => _type;
        public int Offset => _offset;
        public int ArraySize => _arraySize;

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- FIELD: '{_name}' {_typename} {SourcePosition.AstDesc()}");
        }

        public override int DefineSymbols(AstScope currentScope)
        {
            if (Parser.IsReservedWord(_name))
                throw new CodeException($"Illegal field name: '{_name}' is a reserved word", SourcePosition);
            return base.DefineSymbols(currentScope);
        }

        public void BuildType(AstScope definingScope, ref int offset)
        {
            // Store offset
            _offset = offset;

            // Find the field's type
            var symbol = definingScope.FindSymbol(_typename);
            if (symbol == null)
                throw new CodeException($"Unknown type: '{_typename}'", SourcePosition);
            _type = symbol as AstType;
            if (_type == null)
                throw new CodeException($"Invalid type declaration: '{_typename}' is not a type");

            // Resolve the array size
            _arraySize = 0;
            foreach (var d in _initializer.EnumData(definingScope))
            {
                if (!(d is ExprNodeUninitialized))
                {
                    throw new CodeException($"Invalid struct definition: all fields must be declared with uninitialized data", SourcePosition);
                }
                _arraySize++;
            }

            // Update the size
            offset += _type.SizeOf * _arraySize;
        }

        public override void Layout(AstScope currentScope, LayoutContext ctx)
        {
            // No layout needed
        }

        public override void Generate(AstScope currentScope, GenerateContext ctx)
        {
            // No code generation needed
        }
    }

    public abstract class AstType : AstElement, ISymbolValue
    {
        public AstType(SourcePosition position) : base(position)
        {
        }

        public abstract string Name { get; }
        public abstract int SizeOf { get; }

        public virtual AstFieldDefinition? FindField(string name)
        {
            return null;
        }

        public override int DefineSymbols(AstScope currentScope)
        {
            currentScope.Define(Name, this);
            return base.DefineSymbols(currentScope);
        }
    }

    public class AstTypeByte : AstType
    {
        public AstTypeByte(SourcePosition sourcePosition) : base(sourcePosition)
        {
        }

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- TYPE: 'BYTE'");
        }

        public override string Name => "BYTE";
        public override int SizeOf => 1;

        public override int DefineSymbols(AstScope currentScope)
        {
            currentScope.Define("DB", this);
            currentScope.Define("DEFB", this);
            currentScope.Define("DM", this);
            currentScope.Define("DEFM", this);
            return base.DefineSymbols(currentScope);
        }

        public override void Layout(AstScope currentScope, LayoutContext ctx)
        {
            // No layout needed
        }

        public override void Generate(AstScope currentScope, GenerateContext ctx)
        {
            // No code generation needed
        }
    }

    public class AstTypeWord : AstType
    {
        public AstTypeWord(SourcePosition position) : base(position)
        {
        }

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- TYPE: 'WORD'");
        }

        public override string Name => "WORD";
        public override int SizeOf => 2;

        public override int DefineSymbols(AstScope currentScope)
        {
            currentScope.Define("DW", this);
            currentScope.Define("DEFW", this);
            return base.DefineSymbols(currentScope);
        }

        public override void Layout(AstScope currentScope, LayoutContext ctx)
        {
            // No layout needed
        }

        public override void Generate(AstScope currentScope, GenerateContext ctx)
        {
            // No code generation needed
        }
    }

    internal class AstStructDefinition : AstType
    {
        public AstStructDefinition(string name, SourcePosition source) : base(source)
        {
            _name = name;
        }

        public void AddField(AstFieldDefinition fieldDef)
        {
            _fields.Add(fieldDef);
        }

        public override string Name => _name;

        public override int SizeOf
        {
            get
            {
                BuildType();
                return _sizeof;
            }
        }

        public List<AstFieldDefinition> Fields => _fields;

        private readonly string _name;
        private int _sizeof = -1;
        private readonly List<AstFieldDefinition> _fields = [];
        private readonly Dictionary<string, AstFieldDefinition> _fieldsByName = new(StringComparer.InvariantCultureIgnoreCase);
        private AstScope? _definingScope;

        private void BuildType()
        {
            // Alredy built?
            if (_sizeof >= 0)
                return;

            _sizeof = 0;

            foreach (var fd in _fields)
            {
                fd.BuildType(_definingScope, ref _sizeof);
                if (fd.Name != null)
                {
                    if (_fieldsByName.ContainsKey(fd.Name))
                    {
                        Log.Error(fd.SourcePosition, $"Duplicate field name: '{fd.Name}'");
                    }
                    else
                    {
                        _fieldsByName.Add(fd.Name, fd);
                    }
                }
            }
        }

        public override AstFieldDefinition? FindField(string name)
        {
            BuildType();

            if (_fieldsByName.TryGetValue(name, out var def))
                return def;
            return null;
        }

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- STRUCT: '{_name}' {SourcePosition.AstDesc()}");
            foreach (var f in _fields)
            {
                f.Dump(w, indent + 1);
            }
        }

        public override int DefineSymbols(AstScope currentScope)
        {
            if (Parser.IsReservedWord(_name))
                throw new CodeException($"Illegal struct name: '{_name}' is a reserved word", SourcePosition);

            _definingScope = currentScope;

            foreach (var f in _fields)
                f.DefineSymbols(currentScope);

            return base.DefineSymbols(currentScope);
        }

        public override void Layout(AstScope currentScope, LayoutContext ctx)
        {
            BuildType();
        }

        public override void Generate(AstScope currentScope, GenerateContext ctx)
        {
            // No code generation needed
        }
    }
}