using System;
using System.Collections.Generic;
using System.Linq;
using Z80Mnemonics;

namespace Z80Asm
{
    public abstract class Instruction
    {
        protected Instruction(string mnemonic)
        {
            Mnemonic = mnemonic;
        }

        // The simplified version of the Mnemonic (ie: with immediate arguments replaced by '?')
        public string Mnemonic { get; protected set; } = string.Empty;

        // Final calculated length of the instruction in bytes
        public int Length { get; protected set; }

        // Get just the instruction name from the Mnemonic
        public string InstructionName
        {
            get
            {
                var space = Mnemonic.IndexOf(' ');
                if (space < 0)
                    return Mnemonic;
                else
                    return Mnemonic.Substring(0, space);
            }
        }

        public abstract void Generate(GenerateContext ctx, SourcePosition sourcePosition, long[] immArgs);
    }

    // Represents and instruction group (ie: a set of instructions with variations on an immediate value)
    //  eg: SET 0,(HL), SET 1,(HL), SET 2,(HL) etc.. all form a group
    // Typically instruction group are the same instruction with a limited set of values implemented
    // as a bit pattern within the opcodes.  Rather than get into sub-byte bit handling, we just
    // fake the immediate parameters on top of the underlying instruction definitions
    public class InstructionGroup : Instruction
    {
        public InstructionGroup(string mnemonic) : base(mnemonic)
        {
        }

        // Add an instruction definition
        public void AddInstructionDefinition(int imm, InstructionDefinition definition)
        {
            // All variations must have the same length
            if (immVariations.Count == 0)
            {
                Length = definition.Length;
            }
            else
            {
                System.Diagnostics.Debug.Assert(Length == definition.Length);
            }

            immVariations.TryAdd(imm, definition);
        }

        // Find the definition for a specified immediate value
        public InstructionDefinition? FindDefinition(int imm)
        {
            if (immVariations.TryGetValue(imm, out var def))
            {
                return def;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<InstructionDefinition> Definitions
        {
            get
            {
                foreach (var k in immVariations.OrderBy(x => x.Key))
                {
                    yield return k.Value;
                }
            }
        }

        public override void Generate(GenerateContext ctx, SourcePosition sourcePosition, long[] immArgs)
        {
            // Find the definition
            if (!immVariations.TryGetValue((int)immArgs[0], out var def))
            {
                throw new CodeException($"the immediate value 0x{immArgs[0]:X2} isn't a valid value for this instruction");
            }

            // Pass to the undelying definition (after removing the imm variation arg)
            def.Generate(ctx, sourcePosition, immArgs.Skip(1).ToArray());
        }

        // For instructions that contain hard coded immediate values eg: RST 0x68, SET 7,(HL) etc...
        // this is a dictionary of of the immediate values to a real final instruction
        private readonly Dictionary<int, InstructionDefinition> immVariations = new();
    }

    // Represents a concrete definition of an instruction
    public class InstructionDefinition : Instruction
    {
        public InstructionDefinition(string mnemonic) : base(mnemonic)
        {
        }

        // The Op code definition
        public OpCode? opCode;

        // Instruction bytes (before the immediate values)
        public byte[] bytes = [];

        // Instruction suffix bytes (null if none, after the immediate values)
        public byte[]? suffixBytes;

        // Prepare this instruction instance
        public void Prepare()
        {
            // Calculate the length of the instruction in bytes
            Length = bytes.Length;
            if (suffixBytes != null)
                Length += suffixBytes.Length;

            foreach (var ch in Mnemonic)
            {
                switch (ch)
                {
                    case '@':
                        Length += 2;
                        break;

                    case '$':
                    case '%':
                    case '#':
                        Length++;
                        break;
                }
            }
        }

        // Prepare this instruction instance
        public override void Generate(
            GenerateContext ctx,
            SourcePosition sourcePosition,
            long[] immArgs)
        {
            var oldIp = ctx.Ip;

            // Emit the bytes before the value
            ctx.EmitBytes(bytes, true);

            // Emit the immediate bytes
            var arg = 0;
            foreach (var ch in Mnemonic)
            {
                switch (ch)
                {
                    case '@':
                        // 16-bit immediate
                        ctx.Emit16(sourcePosition, immArgs[arg++]);
                        break;

                    case '$':
                        ctx.Emit8(sourcePosition, immArgs[arg++]);
                        break;

                    case '%':
                        ctx.EmitRelOffset(sourcePosition, (int)immArgs[arg++]);
                        break;

                    case '#':
                        // 8 bit immediate
                        ctx.Emit8(sourcePosition, immArgs[arg++]);
                        break;
                }
            }

            // Emit the suffix bytes
            if (suffixBytes != null)
                ctx.EmitBytes(suffixBytes, true);

            // Sanity checks
            System.Diagnostics.Debug.Assert(arg == (immArgs == null ? 0 : immArgs.Length));
            System.Diagnostics.Debug.Assert(ctx.Ip - oldIp == this.Length);
        }

        public void Dump()
        {
            for (var i = 0; i < bytes.Length; i++)
            {
                Console.Write("{0:X2} ", bytes[i]);
            }

            var length = bytes.Length;
            var operandCount = 0;
            if (Mnemonic.Contains('@'))
            {
                Console.Write("@@ @@ ");
                length += 2;
                operandCount++;
            }
            if (Mnemonic.Contains('$'))
            {
                Console.Write("$$ ");
                length++;
                operandCount++;
            }
            if (Mnemonic.Contains('#', StringComparison.CurrentCulture))
            {
                Console.Write("## ");
                length++;
                operandCount++;
            }
            if (Mnemonic.Contains('%'))
            {
                Console.Write("%% ");
                length++;
                operandCount++;
            }

            if (suffixBytes != null)
            {
                for (var i = 0; i < suffixBytes.Length; i++)
                {
                    Console.Write("{0:X2} ", suffixBytes[i]);
                    length++;
                }
            }

            while (length < 5)
            {
                Console.Write("   ");
                length++;
            }

            if (operandCount > 1)
            {
                Console.Write("? ");
            }
            else
            {
                Console.Write("  ");
            }

            Console.WriteLine(Mnemonic);
        }
    }
}