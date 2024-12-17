﻿using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class AND_r_tests : InstructionsExecutionTestsBase
    {
        static AND_r_tests()
        {
            var combinations = new List<object[]>();

            var registers = new[] { "B", "C", "D", "E", "H", "L", "(HL)", "n", "IXH", "IXL", "IYH", "IYL", "(IX+n)", "(IY+n)" };
            for (var src = 0; src < registers.Length; src++)
            {
                var reg = registers[src];
                var i = src;
                byte? prefix = null;

                ModifyTestCaseCreationForIndexRegs(reg, ref i, out prefix);

                var opcode = (byte)(i == 7 ? 0xE6 : (i | 0xA0));
                combinations.Add([reg, opcode, prefix]);
            }

            AND_r_Source = combinations.ToArray();
        }

        public static object[] AND_r_Source;

        public static object[] AND_A_Source =
        [
            new object[] {"A", (byte)0xA7, (byte?)null},
        ];

        [Test]
        [TestCaseSource(nameof(AND_r_Source))]
        public void AND_r_ands_both_registers(string src, byte opcode, byte? prefix)
        {
            var oldValue = Fixture.Create<byte>();
            var valueToAnd = Fixture.Create<byte>();

            Setup(src, oldValue, valueToAnd);
            Execute(opcode, prefix);

            Assert.That(oldValue & valueToAnd, Is.EqualTo(Registers.A));
        }

        [Test]
        [TestCaseSource(nameof(AND_A_Source))]
        public void AND_A_does_not_change_A(string src, byte opcode, byte? prefix)
        {
            var value = Fixture.Create<byte>();

            Registers.A = value;
            Execute(opcode, prefix);

            Assert.That(value, Is.EqualTo(Registers.A));
        }

        private void Setup(string src, byte oldValue, byte valueToAnd)
        {
            Registers.A = oldValue;

            if (src == "n")
            {
                SetMemoryContentsAt(1, valueToAnd);
            }
            else if (src == "(HL)")
            {
                var address = Fixture.Create<ushort>();
                ProcessorAgent.Memory[address] = valueToAnd;
                Registers.HL = address.ToShort();
            }
            else if (src.StartsWith("(I"))
            {
                var address = Fixture.Create<ushort>();
                var offset = Fixture.Create<byte>();
                var realAddress = address.Add(offset.ToSignedByte());
                ProcessorAgent.Memory[realAddress] = valueToAnd;
                SetMemoryContentsAt(2, offset);
                SetReg(src.Substring(1, 2), address.ToShort());
            }
            else if (src != "A")
            {
                SetReg(src, valueToAnd);
            }
        }

        [Test]
        [TestCaseSource(nameof(AND_r_Source))]
        public void AND_r_sets_SF_appropriately(string src, byte opcode, byte? prefix)
        {
            ExecuteCase(src, opcode, 0xFF, 0xFF, prefix);
            Assert.That(Registers.SF == 1);

            ExecuteCase(src, opcode, 0xFF, 0x80, prefix);
            Assert.That(Registers.SF == 1);

            ExecuteCase(src, opcode, 0xFF, 0, prefix);
            Assert.That(Registers.SF == 0);
        }

        private void ExecuteCase(string src, byte opcode, int oldValue, int valueToAnd, byte? prefix)
        {
            Setup(src, (byte)oldValue, (byte)valueToAnd);
            Execute(opcode, prefix);
        }

        [Test]
        [TestCaseSource(nameof(AND_r_Source))]
        public void AND_r_sets_ZF_appropriately(string src, byte opcode, byte? prefix)
        {
            ExecuteCase(src, opcode, 0xFF, 0xFF, prefix);
            Assert.That(Registers.ZF == 0);

            ExecuteCase(src, opcode, 0xFF, 0x80, prefix);
            Assert.That(Registers.ZF == 0);

            ExecuteCase(src, opcode, 0xFF, 0, prefix);
            Assert.That(Registers.ZF == 1);
        }

        [Test]
        [TestCaseSource(nameof(AND_r_Source))]
        public void AND_r_sets_HF(string src, byte opcode, byte? prefix)
        {
            AssertSetsFlags(opcode, null, "H");
        }

        [Test]
        [TestCaseSource(nameof(AND_r_Source))]
        public void AND_r_sets_PF_appropriately(string src, byte opcode, byte? prefix)
        {
            ExecuteCase(src, opcode, 0xFF, 0x7E, prefix);
            Assert.That(Registers.PF == 1);

            ExecuteCase(src, opcode, 0xFF, 0x7F, prefix);
            Assert.That(Registers.PF == 0);

            ExecuteCase(src, opcode, 0xFF, 0x80, prefix);
            Assert.That(Registers.PF == 0);

            ExecuteCase(src, opcode, 0xFF, 0x81, prefix);
            Assert.That(Registers.PF == 1);
        }

        [Test]
        [TestCaseSource(nameof(AND_r_Source))]
        [TestCaseSource(nameof(AND_A_Source))]
        public void AND_r_resets_NF_and_CF(string src, byte opcode, byte? prefix)
        {
            AssertResetsFlags(opcode, null, "N", "C");
        }

        [Test]
        [TestCaseSource(nameof(AND_r_Source))]
        public void AND_r_sets_bits_3_and_5_from_result(string src, byte opcode, byte? prefix)
        {
            var value = ((byte)0).WithBit(3, 1).WithBit(5, 0);
            Setup(src, value, value);
            Execute(opcode, prefix);
            Assert.That(Registers.Flag3 == 1);
            Assert.That(Registers.Flag5 == 0);

            value = ((byte)0).WithBit(3, 0).WithBit(5, 1);
            Setup(src, value, value);
            Execute(opcode, prefix);
            Assert.That(Registers.Flag3 == 0);
            Assert.That(Registers.Flag5 == 1);
        }

        [Test]
        [TestCaseSource(nameof(AND_r_Source))]
        [TestCaseSource(nameof(AND_A_Source))]
        public void AND_r_returns_proper_T_states(string src, byte opcode, byte? prefix)
        {
            var states = Execute(opcode, prefix);
            Assert.That(
states, Is.EqualTo((src == "(HL)" || src == "n") ? 7 :
                src.StartsWith("I") ? 8 :
                src.StartsWith("(I") ? 19 :
                4));
        }
    }
}