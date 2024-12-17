﻿using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class IN_r_C : InstructionsExecutionTestsBase
    {
        public static object[] IN_r_C_Source =
        [
            new Object[] { "A", (byte)0x78 },
            new Object[] { "B", (byte)0x40 },
            new Object[] { "C", (byte)0x48 },
            new Object[] { "D", (byte)0x50 },
            new Object[] { "E", (byte)0x58 },
            new Object[] { "H", (byte)0x60 },
            new Object[] { "L", (byte)0x68 }
        ];

        public static object[] IN_F_C_Source =
        [
            new Object[] { "F", (byte)0x70 },

        ];

        [Test]
        [TestCaseSource(nameof(IN_r_C_Source))]
        public void IN_r_C_reads_value_from_port(string reg, byte opcode)
        {
            var portNumber = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();
            var oldValue = Fixture.Create<byte>();

            if (reg != "C")
                SetReg(reg, value);
            Registers.A = oldValue;

            Execute(opcode, portNumber, value);

            Assert.That(value, Is.EqualTo(GetReg<byte>(reg)));
        }

        [Test]
        [TestCaseSource(nameof(IN_r_C_Source))]
        [TestCaseSource(nameof(IN_F_C_Source))]
        public void IN_r_C_sets_SF_appropriately(string reg, byte opcode)
        {
            var portNumber = Fixture.Create<byte>();

            Execute(opcode, portNumber, 0xFE);
            Assert.That(Registers.SF == 1);

            Execute(opcode, portNumber, 0xFF);
            Assert.That(Registers.SF == 1);

            Execute(opcode, portNumber, 0);
            Assert.That(Registers.SF == 0);

            Execute(opcode, portNumber, 1);
            Assert.That(Registers.SF == 0);
        }

        [Test]
        [TestCaseSource(nameof(IN_r_C_Source))]
        [TestCaseSource(nameof(IN_F_C_Source))]
        public void IN_r_C_sets_ZF_appropriately(string reg, byte opcode)
        {
            var portNumber = Fixture.Create<byte>();

            Execute(opcode, portNumber, 0xFF);
            Assert.That(Registers.ZF == 0);

            Execute(opcode, portNumber, 0);
            Assert.That(Registers.ZF == 1);

            Execute(opcode, portNumber, 1);
            Assert.That(Registers.ZF == 0);
        }

        [Test]
        [TestCaseSource(nameof(IN_r_C_Source))]
        [TestCaseSource(nameof(IN_F_C_Source))]
        public void IN_r_C_resets_HF_NF(string reg, byte opcode)
        {
            AssertResetsFlags(opcode, 0xED, "H", "N");
        }

        [Test]
        [TestCaseSource(nameof(IN_r_C_Source))]
        [TestCaseSource(nameof(IN_F_C_Source))]
        public void IN_r_C_does_not_change_CF(string reg, byte opcode)
        {
            var randomValues = Fixture.Create<byte[]>();
            var portNumber = Fixture.Create<byte>();

            foreach (var value in randomValues)
            {
                Registers.CF = 0;
                Execute(opcode, portNumber, value);
                Assert.That(Registers.CF == 0);

                Registers.CF = 1;
                Execute(opcode, portNumber, value);
                Assert.That(Registers.CF == 1);
            }
        }

        [Test]
        [TestCaseSource(nameof(IN_r_C_Source))]
        [TestCaseSource(nameof(IN_F_C_Source))]
        public void IN_r_C_sets_PF_as_parity(string reg, byte opcode)
        {
            var randomValues = Fixture.Create<byte[]>();
            var portNumber = Fixture.Create<byte>();

            foreach (var value in randomValues)
            {
                Execute(opcode, portNumber, value);
                Assert.That(Registers.PF == (Parity[value]));
            }
        }

        [Test]
        [TestCaseSource(nameof(IN_r_C_Source))]
        [TestCaseSource(nameof(IN_F_C_Source))]
        public void IN_r_C_sets_bits_3_and_5_from_result(string reg, byte opcode)
        {
            var portNumber = Fixture.Create<byte>();
            var value = ((byte)0).WithBit(3, 1).WithBit(5, 0);
            Execute(opcode, portNumber, value);
            Assert.That(Registers.Flag3 == 1);
            Assert.That(Registers.Flag5 == 0);

            value = ((byte)0).WithBit(3, 0).WithBit(5, 1);
            Execute(opcode, portNumber, value);
            Assert.That(Registers.Flag3 == 0);
            Assert.That(Registers.Flag5 == 1);
        }

        [Test]
        [TestCaseSource(nameof(IN_r_C_Source))]
        [TestCaseSource(nameof(IN_F_C_Source))]
        public void IN_r_C_returns_proper_T_states(string reg, byte opcode)
        {
            var portNumber = Fixture.Create<byte>();
            var value = Fixture.Create<byte>();
            var states = Execute(opcode, portNumber, value);
            Assert.That(states, Is.EqualTo(12));
        }

        private int Execute(byte opcode, byte portNumber, byte value)
        {
            Registers.C = portNumber;
            SetPortValue(portNumber, value);
            return Execute(opcode, 0xED);
        }
    }
}
