using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Z80Asm
{
    [Flags]
    public enum AddressingMode
    {
        Deref = 0x80,
        SubOp = 0x40,

        Invalid = 0,

        Immediate = 0x01,
        Register = 0x02,
        RegisterPlusImmediate = Register | Immediate,

        Mask = 0x07,
    }

    public interface ISymbolValue
    {
        void Dump(Action<string> w, int indent);
    }

    public abstract class ExprNode : ISymbolValue
    {
        public ExprNode(SourcePosition? pos)
        {
            SourcePosition = pos;
        }

        public ExprNodeBinary? Parent { get; set; }

        public SourcePosition? SourcePosition { get; private set; }

        public abstract void Dump(Action<string> w, int indent);

        public virtual long EvaluateNumber(AstScope scope)
        {
            // Evaluate
            var val = Evaluate(scope);

            // Try to convert to long
            try
            {
                return Convert.ToInt64(val);
            }
            catch
            {
                throw new CodeException($"Can't convert {Utils.TypeName(val)} to number.");
            }
        }

        public virtual object Evaluate(AstScope scope)
        {
            return EvaluateNumber(scope);
        }

        public virtual AddressingMode GetAddressingMode(AstScope scope)
        {
            return AddressingMode.Immediate;
        }

        public virtual string GetRegister(AstScope scope)
        { 
            return null; 
        }

        public virtual string GetSubOp()
        { throw new InvalidOperationException(); }

        public virtual long GetImmediateValue(AstScope scope)
        { 
            return EvaluateNumber(scope); 
        }

        public virtual IEnumerable<ExprNode> EnumData(AstScope scope)
        { yield return this; }
    }

    public class ExprNodeParameterized : ExprNode
    {
        public ExprNodeParameterized(SourcePosition pos, string[] parameterNames, ExprNode body)
            : base(pos)
        {
            this.parameterNames = parameterNames;
            this.body = body;
        }

        public static string MakeSuffix(int parameterCount)
        {
            return $"/{parameterCount}";
        }

        public static string RemoveSuffix(string name)
        {
            var slash = name.IndexOf('/');
            if (slash < 0)
                return name;
            return name.Substring(0, slash);
        }

        private readonly string[] parameterNames;
        private readonly ExprNode body;

        public ExprNode Resolve(SourcePosition pos, ImmutableArray<ExprNode> arguments)
        {
            return new ExprNodeParameterizedInstance(pos, parameterNames, arguments, body);
        }

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- parameterized({string.Join(",", parameterNames)})");
            body.Dump(w, indent + 1);
        }

        public override long EvaluateNumber(AstScope scope)
        {
            throw new NotImplementedException("Parameterized expression can't be evaluated at compile time");
        }

        public override AddressingMode GetAddressingMode(AstScope scope) 
        {
            return base.GetAddressingMode(scope);
        }

        public override long GetImmediateValue(AstScope scope)
        {
            return base.GetImmediateValue(scope);
        }

        public override string GetRegister(AstScope scope)
        {
            return base.GetRegister(scope);
        }
    }

    public class ExprNodeParameterizedInstance : ExprNode
    {
        public ExprNodeParameterizedInstance(
            SourcePosition pos, 
            string[] parameterNames,
            ImmutableArray<ExprNode> arguments, 
            ExprNode body)
            : base(pos)
        {
            scope = new AstScope("parameterized equate", pos);
            for (int i = 0; i < parameterNames.Length; i++)
            {
                scope.Define(parameterNames[i], arguments[i]);
            }
            this.body = body;
        }

        private readonly AstScope scope;
        private readonly ExprNode body;

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- parameterized [resolved]");
            body.Dump(w, indent + 1);
        }

        public override long EvaluateNumber(AstScope scope)
        {
            try
            {
                this.scope.AddContainer(scope);
                return body.EvaluateNumber(this.scope);
            }
            finally
            {
                this.scope.ResetContainer();
            }
        }

        public override AddressingMode GetAddressingMode(AstScope scope)
        {
            try
            {
                this.scope.AddContainer(scope);
                return body.GetAddressingMode(this.scope);
            }
            finally
            {
                this.scope.ResetContainer();
            }
        }

        public override long GetImmediateValue(AstScope scope)
        {
            try
            {
                this.scope.AddContainer(scope);
                return body.GetImmediateValue(this.scope);
            }
            finally
            {
                this.scope.ResetContainer();
            }
        }

        public override string GetRegister(AstScope scope)
        {
            try
            {
                this.scope.AddContainer(scope);
                return body.GetRegister(this.scope);
            }
            finally
            {
                this.scope.AddContainer(scope);
            }
        }
    }

    public class ExprNodeNumberLiteral : ExprNode
    {
        public ExprNodeNumberLiteral(SourcePosition pos, long value)
            : base(pos)
        {
            this.value = value;
        }

        private readonly long value;

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- number literal {value}");
        }

        public override long EvaluateNumber(AstScope scope)
        {
            return value;
        }

        public override AddressingMode GetAddressingMode(AstScope scope)
        {
            return AddressingMode.Immediate;
        }
    }

    public class ExprNodeStringLiteral : ExprNode
    {
        public ExprNodeStringLiteral(SourcePosition pos, string value)
            : base(pos)
        {
            this.value = value;
        }

        private readonly string value;

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- string literal `{value}`");
        }

        public override object Evaluate(AstScope scope)
        {
            return value;
        }

        public override long EvaluateNumber(AstScope scope)
        {
            if (value.Length == 1)
                return value[0];

            throw new CodeException($"Only single character strings can be used as numeric literals", SourcePosition);
        }

        public override AddressingMode GetAddressingMode(AstScope scope)
        {
            if (value.Length == 1)
                return AddressingMode.Immediate;
            else
                return AddressingMode.Invalid;
        }

        public override IEnumerable<ExprNode> EnumData(AstScope scope)
        {
            foreach (var ch in value)
                yield return new ExprNodeNumberLiteral(SourcePosition, ch);
        }
    }

    public class ExprNodeConcat : ExprNode
    {
        public ExprNodeConcat(SourcePosition pos)
            : base(pos)
        {
        }

        private readonly List<ExprNode> elements = new List<ExprNode>();

        public void AddElement(ExprNode element)
        {
            elements.Add(element);
        }

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- concat");
            foreach (var e in elements)
            {
                e.Dump(w, indent + 1);
            }
        }

        public override object Evaluate(AstScope scope)
        {
            throw new CodeException("Invalid use of concatenated data");
        }

        public override IEnumerable<ExprNode> EnumData(AstScope scope)
        {
            return elements.SelectMany(x => x.EnumData(scope));
        }
    }

    public class ExprNodeDup : ExprNode
    {
        public ExprNodeDup(SourcePosition pos, ExprNode count, ExprNode value)
            : base(pos)
        {
            this.count = count;
            this.value = value;
        }

        private readonly ExprNode count;
        private readonly ExprNode value;

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- dup");

            w.Invoke($"{Utils.Indent(indent + 1)}- count:");
            count.Dump(w, indent + 2);

            w.Invoke($"{Utils.Indent(indent + 1)}- value:");
            value.Dump(w, indent + 2);
        }

        public override IEnumerable<ExprNode> EnumData(AstScope scope)
        {
            int count = (int)this.count.EvaluateNumber(scope);
            for (int i = 0; i < count; i++)
            {
                foreach (var e in value.EnumData(scope))
                    yield return e;
            }
        }

        public override object Evaluate(AstScope scope)
        {
            throw new CodeException("Invalid use of 'DUP'");
        }
    }

    public class ExprNodeDeferredValue : ExprNode
    {
        public ExprNodeDeferredValue(SourcePosition pos, string name)
            : base(pos)
        {
            this.name = name;
        }

        private long? value;
        private readonly string name;

        public void Resolve(long value)
        {
            this.value = value;
        }

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- deferred value: {value}");
        }

        public override long EvaluateNumber(AstScope scope)
        {
            if (value.HasValue)
                return value.Value;

            throw new CodeException($"The value of symbol '{name}' can't been resolved", SourcePosition);
        }

        public override AddressingMode GetAddressingMode(AstScope scope)
        {
            return AddressingMode.Immediate;
        }
    }

    public class ExprNodeUnary : ExprNode
    {
        public ExprNodeUnary(SourcePosition pos)
            : base(pos)
        {
        }

        public string OpName;
        public Func<long, long> Operator;
        public ExprNode RHS;

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- operator {OpName}");
            RHS.Dump(w, indent + 1);
        }

        public override long EvaluateNumber(AstScope scope)
        {
            return Operator(RHS.EvaluateNumber(scope));
        }

        public override AddressingMode GetAddressingMode(AstScope scope)
        {
            if (RHS.GetAddressingMode(scope) == AddressingMode.Immediate)
                return AddressingMode.Immediate;
            else
                return AddressingMode.Invalid;
        }

        public static long OpLogicalNot(long val)
        {
            return val == 0 ? 1 : 0;
        }

        public static long OpBitwiseComplement(long val)
        {
            return ~val;
        }

        public static long OpNegate(long val)
        {
            return -val;
        }
    }

    public class ExprNodeBinary : ExprNode
    {
        public ExprNodeBinary(
            SourcePosition pos,
            string opName,
            Func<long, long, long> operatorFunc
            )
            : base(pos)
        {
            OpName = opName;
            Operator = operatorFunc;
        }

        public string OpName {  get; private set; }
        public Func<long, long, long> Operator { get; private set; }

        private ExprNode? _LHS;
        public ExprNode? LValue { get => _LHS; 
            set { 
                _LHS = value;
                if (value != null)
                    value.Parent = this;
            } 
        }

        private ExprNode? _RHS;
        public ExprNode? RValue
        {
            get => _RHS;
            set
            {
                _RHS = value;
                if (value != null)
                    value.Parent = this;
            }
        }

        protected void VerifyExpression()
        {
            if (LValue == null && RValue == null)
            {
                throw new CodeException("Invalid binary expression", SourcePosition);
            }
        }

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- operator {OpName}");
            if (LValue == null)
            {

            }
            else
            {
                LValue.Dump(w, indent + 1);
            }
            if (RValue == null)
            {

            }
            else
            {
                RValue.Dump(w, indent + 1);
            }
        }

        public override long EvaluateNumber(AstScope scope)
        {
            VerifyExpression();
            return Operator(
                LValue.EvaluateNumber(scope),
                RValue.EvaluateNumber(scope));
        }

        public override AddressingMode GetAddressingMode(AstScope scope)
        {
            VerifyExpression();
            if (RValue.GetAddressingMode(scope) == AddressingMode.Immediate && 
                LValue.GetAddressingMode(scope) == AddressingMode.Immediate)
                return AddressingMode.Immediate;
            else
                return AddressingMode.Invalid;
        }

        public static long OpMul(long a, long b)
        {
            return a * b;
        }

        public static long OpDiv(long a, long b)
        {
            return a / b;
        }

        public static long OpMod(long a, long b)
        {
            return a % b;
        }

        public static long OpAdd(long a, long b)
        {
            return a + b;
        }

        public static long OpLogicalAnd(long a, long b)
        {
            return (a != 0 && b != 0) ? 1 : 0;
        }

        public static long OpLogicalOr(long a, long b)
        {
            return (a != 0 || b != 0) ? 1 : 0;
        }

        public static long OpBitwiseAnd(long a, long b)
        {
            return a & b;
        }

        public static long OpBitwiseOr(long a, long b)
        {
            return a | b;
        }

        public static long OpBitwiseXor(long a, long b)
        {
            return a ^ b;
        }

        public static long OpShl(long a, long b)
        {
            return a << (int)b;
        }

        public static long OpShr(long a, long b)
        {
            return a >> (int)b;
        }

        public static long OpEQ(long a, long b)
        {
            return a == b ? 1 : 0;
        }

        public static long OpNE(long a, long b)
        {
            return a != b ? 1 : 0;
        }

        public static long OpGT(long a, long b)
        {
            return a > b ? 1 : 0;
        }

        public static long OpLT(long a, long b)
        {
            return a < b ? 1 : 0;
        }

        public static long OpGE(long a, long b)
        {
            return a >= b ? 1 : 0;
        }

        public static long OpLE(long a, long b)
        {
            return a <= b ? 1 : 0;
        }
    }

    public class ExprNodeAdd : ExprNodeBinary
    {
        public ExprNodeAdd(SourcePosition pos)
            : base(pos, "+", ExprNodeBinary.OpAdd)
        {
        }

        public override AddressingMode GetAddressingMode(AstScope scope)
        {
            VerifyExpression();
            var lhsMode = LValue.GetAddressingMode(scope);
            var rhsMode = RValue.GetAddressingMode(scope);

            if (lhsMode == AddressingMode.Immediate && rhsMode == AddressingMode.Immediate)
                return AddressingMode.Immediate;

            if (lhsMode == AddressingMode.Immediate && rhsMode == AddressingMode.Register)
                return AddressingMode.RegisterPlusImmediate;

            if (lhsMode == AddressingMode.Register && rhsMode == AddressingMode.Immediate)
                return AddressingMode.RegisterPlusImmediate;

            if (lhsMode == AddressingMode.Immediate && rhsMode == AddressingMode.RegisterPlusImmediate)
                return AddressingMode.RegisterPlusImmediate;

            if (lhsMode == AddressingMode.RegisterPlusImmediate && rhsMode == AddressingMode.Immediate)
                return AddressingMode.RegisterPlusImmediate;

            return AddressingMode.Invalid;
        }

        public override long GetImmediateValue(AstScope scope)
        {
            VerifyExpression();
            var lhsMode = LValue.GetAddressingMode(scope);
            var rhsMode = RValue.GetAddressingMode(scope);

            long val = 0;
            if ((lhsMode & AddressingMode.Immediate) != 0)
                val += LValue.GetImmediateValue(scope);
            if ((rhsMode & AddressingMode.Immediate) != 0)
                val += RValue.GetImmediateValue(scope);

            return val;
        }

        public override string GetRegister(AstScope scope)
        {
            VerifyExpression();
            return LValue.GetRegister(scope) 
                ?? RValue.GetRegister(scope);
        }
    }

    public class ExprNodeTernery : ExprNode
    {
        public ExprNodeTernery(SourcePosition pos,
            ExprNode condition,
            ExprNode trueValue,
            ExprNode falseValue)
            : base(pos)
        {
            Condition = condition;
            TrueValue = trueValue;
            FalseValue = falseValue;
        }

        public ExprNode Condition { get; private set; }
        public ExprNode TrueValue { get; private set; }
        public ExprNode FalseValue { get; private set; }

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- ternery");
            w.Invoke($"{Utils.Indent(indent + 1)}- condition");
            Condition.Dump(w, indent + 2);
            w.Invoke($"{Utils.Indent(indent + 1)}- trueValue");
            TrueValue.Dump(w, indent + 2);
            w.Invoke($"{Utils.Indent(indent + 1)}- falseValue");
            FalseValue.Dump(w, indent + 2);
        }

        public override long EvaluateNumber(AstScope scope)
        {
            if (Condition.EvaluateNumber(scope) != 0)
            {
                return TrueValue.EvaluateNumber(scope);
            }
            else
            {
                return FalseValue.EvaluateNumber(scope);
            }
        }

        public override AddressingMode GetAddressingMode(AstScope scope)
        {
            if (TrueValue.GetAddressingMode(scope) == AddressingMode.Immediate && FalseValue.GetAddressingMode(scope) == AddressingMode.Immediate)
                return AddressingMode.Immediate;
            else
                return AddressingMode.Invalid;
        }

        public override IEnumerable<ExprNode> EnumData(AstScope scope)
        {
            if (Condition.EvaluateNumber(scope) != 0)
                return TrueValue.EnumData(scope);
            else
                return FalseValue.EnumData(scope);
        }
    }

    public class ExprNodeIdentifier : ExprNode
    {
        public ExprNodeIdentifier(SourcePosition pos, string name)
            : base(pos)
        {
            _name = name;
        }

        private readonly string _name;
        private readonly List<ExprNode> _arguments = [];
        public ReadOnlyCollection<ExprNode> Arguments => _arguments.AsReadOnly();

        public void SetArguments(IEnumerable<ExprNode> arguments)
        {
            _arguments.Clear();
            _arguments.AddRange(arguments);
        }

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- identifier '{_name}'");
            if (Arguments != null)
            {
                w.Invoke($"{Utils.Indent(indent + 1)}- args");
                foreach (var a in Arguments)
                {
                    a.Dump(w, indent + 2);
                }
            }
        }

        private ExprNode FindSymbol(AstScope scope)
        {
            if (Arguments.Count==0)
            {
                // Simple symbol
                var symbol = scope.FindSymbol(_name);
                if (symbol == null)
                {
                    throw new CodeException($"Unrecognized symbol: '{_name}'", SourcePosition);
                }
                if (symbol is not ExprNode expr)
                {
                    throw new CodeException($"Invalid expression: '{_name}' is not a value", SourcePosition);
                }

                if (symbol is ExprNodeParameterized param)
                {
                    // Resolve it
                    return param.Resolve(SourcePosition, Arguments.ToImmutableArray());
                }
                else
                {
                    return expr;
                }
            }
            else
            {
                // Parameterized symbol
                var symbol = scope.FindSymbol(_name + ExprNodeParameterized.MakeSuffix(Arguments.Count)) as ExprNodeParameterized;
                if (symbol == null)
                {
                    throw new CodeException($"Unrecognized symbol: '{_name}' (with {Arguments.Count} arguments)", SourcePosition);
                }

                // Resolve it
                return symbol.Resolve(SourcePosition, Arguments.ToImmutableArray());
            }
        }

        public override object Evaluate(AstScope scope)
        {
            if (Arguments == null)
            {
                // Simple symbol
                var symbol = scope.FindSymbol(_name);
                if (symbol == null)
                {
                    throw new CodeException($"Unrecognized symbol: '{_name}'", SourcePosition);
                }
                return symbol;
            }

            return base.Evaluate(scope);
        }

        public override long EvaluateNumber(AstScope scope)
        {
            return FindSymbol(scope).EvaluateNumber(scope);
        }

        public override AddressingMode GetAddressingMode(AstScope scope)
        {
            return FindSymbol(scope).GetAddressingMode(scope);
        }

        public override long GetImmediateValue(AstScope scope)
        {
            return FindSymbol(scope).GetImmediateValue(scope);
        }

        public override string GetRegister(AstScope scope)
        {
            return FindSymbol(scope).GetRegister(scope);
        }
    }

    public class ExprNodeMember : ExprNode
    {
        public ExprNodeMember(SourcePosition pos, string name, ExprNode lhs)
            : base(pos)
        {
            _lhs = lhs;
            _name = name;
        }

        private readonly ExprNode _lhs;
        private readonly string _name;

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- member '.{_name}' of:");
            _lhs.Dump(w, indent + 1);
        }

        public override object Evaluate(AstScope scope)
        {
            // Get the LValue (which must be a type)
            var lhsVal = _lhs.Evaluate(scope);
            var type = lhsVal as AstType;
            if (type == null)
            {
                if (lhsVal is AstFieldDefinition fd)
                {
                    type = fd.Type;
                }
                else
                {
                    throw new CodeException($"LHS of member operator '.{_name}' is not a type or field ", SourcePosition);
                }
            }
            if (type == null)
            {
                throw new CodeException($"LHS of member operator '.{_name}' is not a type or field ", SourcePosition);
            }

            // Get the field definition
            var fieldDefinition = type.FindField(_name);
            if (fieldDefinition == null)
                throw new CodeException($"The type '{type.Name}' does not contain a field named '{_name}'", SourcePosition);

            // Return the field definition
            return fieldDefinition;
        }

        public override long EvaluateNumber(AstScope scope)
        {
            // Get the LValue (which must be a type or another field definition)
            var lhsVal = _lhs.Evaluate(scope);

            // Direct member of a type?
            var lhsType = lhsVal as AstType;
            if (lhsType != null)
            {
                // Get the field
                var fieldDefinition = lhsType.FindField(_name);
                if (fieldDefinition == null)
                    throw new CodeException($"The type '{lhsType.Name}' does not contain a field named '{_name}'", SourcePosition);

                // Return the field's offset
                return fieldDefinition.Offset;
            }

            // Member of member?
            var lhsMember = lhsVal as AstFieldDefinition;
            if (lhsMember != null)
            {
                var fieldDefinition = lhsMember.Type.FindField(_name);
                if (fieldDefinition == null)
                    throw new CodeException($"The type '{lhsMember.Type.Name}' does not contain a field named '{_name}'", SourcePosition);

                return _lhs.EvaluateNumber(scope) + fieldDefinition.Offset;
            }

            throw new CodeException($"LHS of member operator '.{_name}' is not a type or field ", SourcePosition);
        }

        public override AddressingMode GetAddressingMode(AstScope scope)
        {
            return AddressingMode.Immediate;
        }
    }

    public class ExprNodeSizeOf : ExprNode
    {
        public ExprNodeSizeOf(SourcePosition pos, ExprNode target)
            : base(pos)
        {
            this.target = target;
        }

        private readonly ExprNode target;

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- sizeof");
            target.Dump(w, indent + 1);
        }

        public override long EvaluateNumber(AstScope scope)
        {
            // Get the LValue (which must be a type or another field definition)
            var targetVal = target.Evaluate(scope);

            // Is the target a type declaration
            var targetType = targetVal as AstType;
            if (targetType != null)
                return targetType.SizeOf;

            // Is the target a member
            var targetField = targetVal as AstFieldDefinition;
            if (targetField != null)
                return targetField.Type.SizeOf;

            throw new CodeException($"Invalid use of sizeof operator: must be a type or member", SourcePosition);
        }

        public override AddressingMode GetAddressingMode(AstScope scope)
        {
            return AddressingMode.Immediate;
        }
    }

    public class ExprNodeRegisterOrFlag : ExprNode
    {
        public ExprNodeRegisterOrFlag(SourcePosition pos, string name)
            : base(pos)
        {
            this.name = name;
        }

        private readonly string name;

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- reg/cond '{name}'");
        }

        public override long EvaluateNumber(AstScope scope)
        {
            throw new CodeException("'{_name}' can't be evaluated at compile time", SourcePosition);
        }

        public override AddressingMode GetAddressingMode(AstScope scope)
        {
            return AddressingMode.Register;
        }

        public override string GetRegister(AstScope scope)
        {
            return name;
        }
    }

    public class ExprNodeDeref : ExprNode
    {
        public ExprNodeDeref(SourcePosition pos, ExprNode pointer)
            : base(pos)
        {
            Pointer = pointer;
        }

        public ExprNode Pointer { get; private set; }

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- deref pointer");
            Pointer.Dump(w, indent + 1);
        }

        public override long EvaluateNumber(AstScope scope)
        {
            throw new CodeException("pointer dereference operator can't be evaluated at compile time", SourcePosition);
        }

        public override AddressingMode GetAddressingMode(AstScope scope)
        {
            var mode = Pointer.GetAddressingMode(scope);

            if ((mode & (AddressingMode.Deref | AddressingMode.SubOp)) == 0)
                return mode | AddressingMode.Deref;

            return AddressingMode.Invalid;
        }

        public override string GetRegister(AstScope scope)
        {
            return Pointer.GetRegister(scope);
        }

        public override long GetImmediateValue(AstScope scope)
        {
            return Pointer.GetImmediateValue(scope);
        }
    }

    public class ExprNodeIP : ExprNode
    {
        public ExprNodeIP(bool allowOverride)
            : base(null)
        {
            this.allowOverride = allowOverride;
        }

        private readonly bool allowOverride;
        private GenerateContext? generateContext;
        private LayoutContext? layoutContext;

        public void SetContext(GenerateContext ctx)
        {
            generateContext = ctx;
        }

        public void SetContext(LayoutContext ctx)
        {
            layoutContext = ctx;
        }

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- ip '$' pointer");
        }

        public override long EvaluateNumber(AstScope scope)
        {
            // Temporarily overridden? (See ExprNodeEquWrapper)
            if (allowOverride && scope.ipOverride.HasValue)
                return scope.ipOverride.Value;

            // Generating
            if (generateContext != null)
                return generateContext.Ip;

            // Layouting?
            if (layoutContext != null)
                return layoutContext.Ip;

            // No IP for you!
            throw new CodeException("Symbol $ can't be resolved at this time");
        }

        public override AddressingMode GetAddressingMode(AstScope scope)
        {
            return AddressingMode.Immediate;
        }

        public override long GetImmediateValue(AstScope scope)
        {
            // Temporarily overridden? (See ExprNodeEquWrapper)
            if (allowOverride && scope.ipOverride.HasValue)
                return scope.ipOverride.Value;

            return generateContext.Ip;
        }
    }

    public class ExprNodeOFS : ExprNode
    {
        public ExprNodeOFS(bool allowOverride)
            : base(null)
        {
            this.allowOverride = allowOverride;
        }

        private readonly bool allowOverride;
        private GenerateContext generateContext;
        private LayoutContext layoutContext;

        public void SetContext(GenerateContext ctx)
        {
            generateContext = ctx;
            //layoutContext = null;
        }

        public void SetContext(LayoutContext ctx)
        {
            layoutContext = ctx;
            //generateContext = null;
        }

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- ofs '$ofs' pointer");
        }

        public override long EvaluateNumber(AstScope scope)
        {
            // Temporarily overridden? (See ExprNodeEquWrapper)
            if (allowOverride && scope.opOverride.HasValue)
                return scope.opOverride.Value;

            // Generating
            if (generateContext != null)
                return generateContext.Op;

            // Layouting?
            if (layoutContext != null)
                return layoutContext.Op;

            // No OFS for you!
            throw new CodeException("Symbol $ofs can't be resolved at this time");
        }

        public override AddressingMode GetAddressingMode(AstScope scope)
        {
            return AddressingMode.Immediate;
        }

        public override long GetImmediateValue(AstScope scope)
        {
            // Temporarily overridden? (See ExprNodeEquWrapper)
            if (allowOverride && scope.opOverride.HasValue)
                return scope.opOverride.Value;

            return generateContext.Op;
        }
    }

    // Represents a "sub Op" operand.  eg: the "RES " part of "LD A,RES 0,(IX+1)"
    public class ExprNodeSubOp : ExprNode
    {
        public ExprNodeSubOp(SourcePosition pos, string subop)
            : base(pos)
        {
            this.subop = subop;
        }

        private readonly string subop;

        public ExprNode RHS;

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- sub-op '{subop}'");
            RHS.Dump(w, indent + 1);
        }

        public override long EvaluateNumber(AstScope scope)
        {
            throw new CodeException("sub-op operator can't be evaluated at compile time", SourcePosition);
        }

        public override AddressingMode GetAddressingMode(AstScope scope)
        {
            return AddressingMode.SubOp | RHS.GetAddressingMode(scope);
        }

        public override string GetRegister(AstScope scope)
        {
            return RHS.GetRegister(scope);
        }

        public override long GetImmediateValue(AstScope scope)
        {
            return RHS.GetImmediateValue(scope);
        }

        public override string GetSubOp()
        {
            return subop;
        }
    }

    // This expression node temporarily overrides the value of $ while
    // the RValue expression is evaluated.  This is used by EQU definitions
    // to resolve $ to the loation the EQU was defined - not the location
    // it was invoked from.  See also ExprNodeIP
    public class ExprNodeEquWrapper : ExprNode
    {
        public ExprNodeEquWrapper(SourcePosition pos, ExprNode rhs, string name)
            : base(pos)
        {
            this.rhs = rhs;
            ipOverride = 0;
            this.name = name;
        }

        public void SetOverrides(int ipOverride, int opOverride)
        {
            this.ipOverride = ipOverride;
            this.opOverride = opOverride;
        }

        private readonly ExprNode rhs;
        private int ipOverride;
        private int opOverride;
        private readonly string name;
        private bool recursionCheck;

        private void CheckForRecursion()
        {
            if (recursionCheck)
            {
                throw new CodeException($"Recursive symbol reference: {name}", SourcePosition);
            }
        }

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- EQU wrapper $ => 0x{ipOverride:X4} $ofs => 0x{opOverride:X4}");
            rhs.Dump(w, indent + 1);
        }

        public override long EvaluateNumber(AstScope scope)
        {
            CheckForRecursion();

            var ipSave = scope.ipOverride;
            var opSave = scope.opOverride;
            recursionCheck = true;
            try
            {
                scope.ipOverride = ipOverride;
                scope.opOverride = opOverride;
                return rhs.EvaluateNumber(scope);
            }
            finally
            {
                scope.ipOverride = ipSave;
                scope.opOverride = opSave;
                recursionCheck = false;
            }
        }

        public override AddressingMode GetAddressingMode(AstScope scope)
        {
            CheckForRecursion();

            recursionCheck = true;
            try
            {
                return rhs.GetAddressingMode(scope);
            }
            finally
            {
                recursionCheck = false;
            }
        }

        public override string GetRegister(AstScope scope)
        {
            return rhs.GetRegister(scope);
        }

        public override string GetSubOp()
        {
            return rhs.GetSubOp();
        }

        public override long GetImmediateValue(AstScope scope)
        {
            CheckForRecursion();

            var ipSave = scope.ipOverride;
            var opSave = scope.opOverride;
            recursionCheck = true;
            try
            {
                scope.ipOverride = ipOverride;
                scope.opOverride = opOverride;
                return rhs.GetImmediateValue(scope);
            }
            finally
            {
                scope.ipOverride = ipSave;
                scope.opOverride = opSave;
                recursionCheck = false;
            }
        }
    }

    // This expression node temporarily overrides the value of $ while
    // the RValue expression is evaluated.  This is used by EQU definitions
    // to resolve $ to the loation the EQU was defined - not the location
    // it was invoked from.  See also ExprNodeIP
    public class ExprNodeIsDefined : ExprNode
    {
        public ExprNodeIsDefined(SourcePosition pos, string symbolName)
            : base(pos)
        {
            this.symbolName = symbolName;
        }

        private readonly string symbolName;

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- defined({symbolName})");
        }

        public override long EvaluateNumber(AstScope scope)
        {
            if (scope.IsSymbolDefined(symbolName, true))
                return 1;
            else
                return 0;
        }

        public override AddressingMode GetAddressingMode(AstScope scope)
        {
            return AddressingMode.Immediate;
        }

        public override string GetRegister(AstScope scope)
        {
            return null;
        }

        public override string GetSubOp()
        {
            return null;
        }

        public override long GetImmediateValue(AstScope scope)
        {
            return EvaluateNumber(scope);
        }
    }

    // Represents an array of values [a,b,c]
    public class ExprNodeOrderedStructData : ExprNode
    {
        public ExprNodeOrderedStructData(SourcePosition pos)
            : base(pos)
        {
        }

        public void AddElement(ExprNode elem)
        {
            elements.Add(elem);
        }

        private readonly List<ExprNode> elements = new List<ExprNode>();

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- ordered struct data");
            foreach (var e in elements)
            {
                e.Dump(w, indent + 1);
            }
        }

        public override object Evaluate(AstScope scope)
        {
            return elements.ToArray();
        }
    }

    // Represents a map of values { x: a, y: b }
    public class ExprNodeNamedStructData : ExprNode
    {
        public ExprNodeNamedStructData(SourcePosition pos)
            : base(pos)
        {
        }

        public void AddEntry(string name, ExprNode value)
        {
            entries.Add(name, value);
        }

        public bool ContainsEntry(string name)
        {
            return entries.ContainsKey(name);
        }

        private readonly Dictionary<string, ExprNode> entries = new Dictionary<string, ExprNode>(StringComparer.InvariantCultureIgnoreCase);

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- named struct data");
            foreach (var e in entries)
            {
                w.Invoke($"{Utils.Indent(indent + 1)}- \"{e.Key}\"");
                e.Value.Dump(w, indent + 2);
            }
        }

        public override object Evaluate(AstScope scope)
        {
            return entries;
        }
    }

    // Uninitialized data node ie: '?'
    public class ExprNodeUninitialized : ExprNode
    {
        public ExprNodeUninitialized(SourcePosition pos)
            : base(pos)
        {
        }

        public override void Dump(Action<string> w, int indent)
        {
            w.Invoke($"{Utils.Indent(indent)}- uninitialized data '?'");
        }

        public override object Evaluate(AstScope scope)
        {
            return this;        // as good as anything, just need a marker
        }
    }
}