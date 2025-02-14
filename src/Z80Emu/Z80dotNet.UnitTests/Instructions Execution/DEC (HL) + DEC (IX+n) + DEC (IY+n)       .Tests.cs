﻿using AutoFixture;
using NUnit.Framework;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class DEC_aHL_tests : InstructionsExecutionTestsBase
    {
        public static object[] DEC_Source =
        [
            new object[] { "HL", (byte)0x35, (byte?)null },
            new object[] { "IX", (byte)0x35, (byte?)0xDD },
            new object[] { "IY", (byte)0x35, (byte?)0xFD }
        ];

        [Test]
        [TestCaseSource(nameof(DEC_Source))]
        public void DEC_aHL_IX_IY_plus_n_increases_value_appropriately(string reg, byte opcode, byte? prefix)
        {
            var oldValue = Fixture.Create<byte>();
            var offset = reg == "HL" ? (byte)0 : Fixture.Create<byte>();
            var address = Setup(reg, oldValue, offset);

            if (reg == "HL")
                Execute(opcode, prefix);
            else
                Execute(opcode, prefix, offset);

            AssertMemoryContents(address, oldValue.Dec());
        }

        private ushort Setup(string reg, byte value, byte offset = 0)
        {
            var address = Fixture.Create<ushort>();
            var actualAddress = address.Add(offset.ToSignedByte());
            ProcessorAgent.Memory[actualAddress] = value;
            SetReg(reg, address.ToShort());
            return actualAddress;
        }

        private void AssertMemoryContents(ushort address, byte expected)
        {
            Assert.That(ProcessorAgent.Memory[address], Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(nameof(DEC_Source))]
        public void DEC_aHL_IX_IY_plus_n_sets_SF_appropriately(string reg, byte opcode, byte? prefix)
        {
            Setup(reg, 0x02);

            Execute(opcode, prefix);
            Assert.That(Registers.SF == 0);

            Execute(opcode, prefix);
            Assert.That(Registers.SF == 0);

            Execute(opcode, prefix);
            Assert.That(Registers.SF == 1);

            Execute(opcode, prefix);
            Assert.That(Registers.SF == 1);
        }

        [Test]
        [TestCaseSource(nameof(DEC_Source))]
        public void DEC_aHL_IX_IY_plus_n_sets_ZF_appropriately(string reg, byte opcode, byte? prefix)
        {
            Setup(reg, 0x03);

            Execute(opcode, prefix);
            Assert.That(Registers.ZF == 0);

            Execute(opcode, prefix);
            Assert.That(Registers.ZF == 0);

            Execute(opcode, prefix);
            Assert.That(Registers.ZF == 1);

            Execute(opcode, prefix);
            Assert.That(Registers.ZF == 0);
        }

        [Test]
        [TestCaseSource(nameof(DEC_Source))]
        public void DEC_aHL_IX_IY_plus_n_sets_HF_appropriately(string reg, byte opcode, byte? prefix)
        {
            foreach (var b in new byte[] { 0x11, 0x81, 0xF1 })
            {
                Setup(reg, b);

                Execute(opcode, prefix);
                Assert.That(Registers.HF == 0);

                Execute(opcode, prefix);
                Assert.That(Registers.HF == 1);

                Execute(opcode, prefix);
                Assert.That(Registers.HF == 0);
            }
        }

        [Test]
        [TestCaseSource(nameof(DEC_Source))]
        public void DEC_aHL_IX_IY_plus_n_sets_PF_appropriately(string reg, byte opcode, byte? prefix)
        {
            Setup(reg, 0x81);

            Execute(opcode, prefix);
            Assert.That(Registers.PF == 0);

            Execute(opcode, prefix);
            Assert.That(Registers.PF == 1);

            Execute(opcode, prefix);
            Assert.That(Registers.PF == 0);
        }

        [Test]
        [TestCaseSource(nameof(DEC_Source))]
        public void DEC_aHL_IX_IY_plus_n_sets_NF(string reg, byte opcode, byte? prefix)
        {
            AssertSetsFlags(opcode, prefix, "N");
        }

        [Test]
        [TestCaseSource(nameof(DEC_Source))]
        public void DEC_aHL_IX_IY_plus_n_does_not_change_CF(string reg, byte opcode, byte? prefix)
        {
            var randomValues = Fixture.Create<byte[]>();

            foreach (var value in randomValues)
            {
                Setup(reg, value);

                Registers.CF = 0;
                Execute(opcode, prefix);
                Assert.That(Registers.CF == 0);

                Registers.CF = 1;
                Execute(opcode, prefix);
                Assert.That(Registers.CF == 1);
            }
        }

        [Test]
        [TestCaseSource(nameof(DEC_Source))]
        public void DEC_aHL_IX_IY_plus_n_sets_bits_3_and_5_from_result(string reg, byte opcode, byte? prefix)
        {
            Setup(reg, ((byte)1).WithBit(3, 1).WithBit(5, 0));
            Execute(opcode, prefix);
            Assert.That(Registers.Flag3 == 1);
            Assert.That(Registers.Flag5 == 0, message: $"DEC {reg} {prefix} failed to set bit 5");

            Setup(reg, ((byte)1).WithBit(3, 0).WithBit(5, 1));
            Execute(opcode, prefix);
            Assert.That(Registers.Flag3 == 0);
            Assert.That(Registers.Flag5 == 1);
        }

        [Test]
        [TestCaseSource(nameof(DEC_Source))]
        public void DEC_aHL_IX_IY_plus_n_returns_proper_T_states(string reg, byte opcode, byte? prefix)
        {
            var states = Execute(opcode, prefix);
            Assert.That(states, Is.EqualTo(reg == "HL" ? 11 : 23));
        }
    }
}
