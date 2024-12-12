using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Z80Mnemonics;

namespace Z80Asm
{
    public static class InstructionSet
    {
        // Static constructor - initialize the instruction set table
        static InstructionSet()
        {
            ProcessOpCodeTable([.. OpCodes.DasmBase], []);
            ProcessOpCodeTable([.. OpCodes.DasmED], [0xED]);
            ProcessOpCodeTable([.. OpCodes.DasmCB], [0xCB]);
            ProcessOpCodeTable([.. OpCodes.DasmDD], [0xDD]);
            ProcessOpCodeTable([.. OpCodes.DasmFD], [0xFD]);
            ProcessOpCodeTable([.. OpCodes.DasmDDCB], [0xDD, 0xCB], true);
            ProcessOpCodeTable([.. OpCodes.DasmFDCB], [0xFD, 0xCB], true);

            // Build a list of all instruction names
            _instructionNames = new HashSet<string>(_opMap.Values.Select(x => x.InstructionName), StringComparer.InvariantCultureIgnoreCase);
        }

        // Given a Mnemonic pattern, find the associated instruction
        public static Instruction? Find(string mnemonic)
        {
            if (!_opMap.TryGetValue(mnemonic, out var op))
            {
                return null;
            }
            return op;
        }

        // Dictionary of Mnemonic to instruction group or instruction definition
        private static readonly Dictionary<string, Instruction> _opMap = new(StringComparer.InvariantCultureIgnoreCase);

        // Convert a type qualified Mnemonic into a look up key
        private static string KeyOfMnemonic(string mnemonic, out int? immValue)
        {
            // Default to no immediate value
            immValue = null;

            // Start after the instruction name
            var findString = mnemonic.Trim();
            var spacePos = 0;
            while (spacePos < findString.Length && char.IsLetterOrDigit(findString[spacePos]))
                spacePos++;

            //var spacePos = mnemonic.IndexOf(' ');

            if (spacePos < 0 || spacePos == findString.Length)
                return mnemonic;

            // Build the new Mnemonic
            var sb = new StringBuilder();
            sb.Append(mnemonic.AsSpan(0, spacePos));

            for (var i = spacePos; i < mnemonic.Length; i++)
            {
                // Get the character
                var ch = mnemonic[i];

                // Typed placeholder?
                if (ch == '%' || ch == '@' || ch == '#' || ch == '$')
                {
                    sb.Append('?');
                    continue;
                }

                // Immediate value
                if (ch >= '0' && ch <= '9')
                {
                    sb.Append('?');

                    // Extract the immediate value
                    var pos = i;
                    while (i < mnemonic.Length && ((mnemonic[i] >= '0' && mnemonic[i] <= '9') || mnemonic[i] == 'x'))
                        i++;
                    var immString = mnemonic.Substring(pos, i - pos);

                    // Parse the immediate value
                    if (immString.StartsWith("0x"))
                    {
                        immValue = Convert.ToInt32(immString.Substring(2), 16);
                    }
                    else
                    {
                        immValue = Convert.ToInt32(immString);
                    }

                    i--;
                    continue;
                }

                sb.Append(ch);
            }

            return sb.ToString();
        }

        // Add an instruction to the set of available instructions
        private static void AddInstruction(InstructionDefinition instruction)
        {
            // Prepare the instruction
            instruction.Prepare();

            // Get it's key, replacing operand placeholders with '?'
            var key = KeyOfMnemonic(instruction.Mnemonic, out var immValue);

            // Get the existing instruction
            _opMap.TryGetValue(key, out var existing);

            // Does it need to go into an instruction group?
            if (immValue.HasValue)
            {
                if (existing != null)
                {
                    // Use existing group
                    var group = existing as InstructionGroup;
                    if (group == null)
                    {
                        throw new InvalidOperationException("Internal error: instruction overloaded with typed imm and bit imm");
                    }

                    group.AddInstructionDefinition(immValue.Value, instruction);
                }
                else
                {
                    // Create new instruction group
                    var group = new InstructionGroup(key);
                    group.AddInstructionDefinition(immValue.Value, instruction);
                    _opMap.Add(key, group);
                }
            }
            else
            {
                if (existing != null)
                {
                    if (existing is InstructionGroup)
                    {
                        throw new InvalidOperationException("Internal error: instruction overloaded with typed imm and bit imm");
                    }

                    /*
                    if (existing.opCode.Mnemonic != instruction.opCode.Mnemonic)
                    {
                        Console.WriteLine("Arg overload: {0}", instruction.opCode.Mnemonic);
                    }
                    else
                    {
                        Console.WriteLine("Duplicate Mnemonic: {0}", instruction.opCode.Mnemonic);
                    }
                    */
                }
                else
                {
                    _opMap.Add(key, instruction);
                }
            }
        }

        private static void ProcessOpCodeTable(OpCode[] table, byte[] prefixBytes, bool opIsSuffix = false)
        {
            for (var i = 0; i < table.Length; i++)
            {
                var opCode = table[i];
                if (opCode.mnemonic == null || opCode.mnemonic.StartsWith("shift") || opCode.mnemonic.StartsWith("ignore"))
                    continue;

                if ((opCode.flags & OpCodeFlags.NoAsm) != 0)
                    continue;

                // Create the bytes
                byte[] bytes;
                byte[] suffixBytes = null;
                if (opIsSuffix)
                {
                    bytes = prefixBytes;
                    suffixBytes = [(byte)i];
                }
                else
                {
                    bytes = new byte[prefixBytes.Length + 1];
                    Array.Copy(prefixBytes, bytes, prefixBytes.Length);
                    bytes[prefixBytes.Length] = (byte)i;
                }

                // Create the instruction
                AddInstruction(new InstructionDefinition(opCode.mnemonic)
                {
                    bytes = bytes,
                    suffixBytes = suffixBytes,
                    opCode = opCode,
                });

                if (opCode.altMnemonic != null)
                {
                    AddInstruction(new InstructionDefinition(opCode.altMnemonic)
                    {
                        bytes = bytes,
                        suffixBytes = suffixBytes,
                        opCode = opCode,
                    });
                }
            }
        }

        public static void DumpAll()
        {
            foreach (var kv in _opMap.OrderBy(x => x.Value.Mnemonic))
            {
                //Console.WriteLine(kv.Key);

                // Is it a group?
                var group = kv.Value as InstructionGroup;
                if (group != null)
                {
                    Console.WriteLine($"<group>          {group.Mnemonic}");
                    foreach (var d in group.Definitions)
                    {
                        d.Dump();
                    }
                }

                // Is it a definition?
                var def = kv.Value as InstructionDefinition;
                if (def != null)
                {
                    def.Dump();
                }
            }
        }

        #region Instruction Names

        private static readonly HashSet<string> _instructionNames;

        public static bool IsValidInstructionName(string name)
        {
            return _instructionNames.Contains(name);
        }

        private static readonly string[] _subOpNames =
        [
            "RES", "SET", "RL", "RLC", "RR", "RRC", "SLA", "SLL", "SRA", "SRL",
        ];

        private static readonly HashSet<string> _subOpNameMap = new(_subOpNames, StringComparer.InvariantCultureIgnoreCase);

        public static bool IsValidSubOpName(string name)
        {
            return _subOpNameMap.Contains(name);
        }

        #endregion Instruction Names

        #region Register Names

        private static readonly string[] _registerNames =
        [
            "AF", "AF'", "I", "R",
            "A", "B", "C", "D", "E", "H", "L",
            "BC", "DE", "HL", "SP",
            "IX", "IY", "IXH", "IXL", "IYH", "IYL",
        ];

        private static readonly HashSet<string> _registerNameMap = new(_registerNames, StringComparer.InvariantCultureIgnoreCase);

        public static bool IsValidRegister(string name)
        {
            return _registerNameMap.Contains(name);
        }

        #endregion Register Names

        #region Condition Flag Names

        private static readonly string[] _conditionFlags =
        [
            "Z", "NZ", "C", "NC", "PE", "P", "PO", "M",
        ];

        private static readonly HashSet<string> _conditionFlagMap = new(_conditionFlags, StringComparer.InvariantCultureIgnoreCase);

        public static bool IsConditionFlag(string name)
        {
            return _conditionFlagMap.Contains(name);
        }

        #endregion Condition Flag Names
    }
}