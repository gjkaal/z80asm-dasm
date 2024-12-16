using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class ADD_rr_rr_tests : InstructionsExecutionTestsBase
    {
        public static object[] ADD_rr_rr_Source =
        [
            new object[] {"HL", "BC", (byte)0x09, (byte?)null},
            new object[] {"HL", "DE", (byte)0x19, (byte?)null},
            new object[] {"HL", "SP", (byte)0x39, (byte?)null},
            new object[] {"IX", "BC", (byte)0x09, (byte?)0xDD},
            new object[] {"IX", "DE", (byte)0x19, (byte?)0xDD},
            new object[] {"IX", "SP", (byte)0x39, (byte?)0xDD},
            new object[] {"IY", "BC", (byte)0x09, (byte?)0xFD},
            new object[] {"IY", "DE", (byte)0x19, (byte?)0xFD},
            new object[] {"IY", "SP", (byte)0x39, (byte?)0xFD},
        ];

        public static object[] ADD_rr_rr_Source_same_src_and_dest =
        [
            new object[] {"HL", "HL", (byte)0x29, (byte?)null},
            new object[] {"IX", "IX", (byte)0x29, (byte?)0xDD},
            new object[] {"IY", "IY", (byte)0x29, (byte?)0xFD},
        ];

        [Test]
        [TestCaseSource("ADD_rr_rr_Source")]
        [TestCaseSource("ADD_rr_rr_Source_same_src_and_dest")]
        public void ADD_rr_rr_adds_register_values(string dest, string src, byte opcode, byte? prefix)
        {
            var value1 = Fixture.Create<short>();
            var value2 = (src == dest) ? value1 : Fixture.Create<short>();

            SetReg(dest, value1);
            if (src != dest)
                SetReg(src, value2);

            Execute(opcode, prefix);

            Assert.That(value1.Add(value2), Is.EqualTo(GetReg<short>(dest)));
            if (src != dest)
                Assert.That(value2, Is.EqualTo(GetReg<short>(src)));
        }

        [Test]
        [TestCaseSource("ADD_rr_rr_Source")]
        public void ADD_rr_rr_sets_CF_properly(string dest, string src, byte opcode, byte? prefix)
        {
            Registers.CF = 1;
            SetReg(dest, 0xFFFE.ToShort());
            SetReg(src, 1);

            Execute(opcode, prefix);
            Assert.That(Registers.CF == 0);

            Execute(opcode, prefix);
            Assert.That(Registers.CF == 1);
        }

        [Test]
        [TestCaseSource("ADD_rr_rr_Source")]
        [TestCaseSource("ADD_rr_rr_Source_same_src_and_dest")]
        public void ADD_rr_rr_resets_N(string dest, string src, byte opcode, byte? prefix)
        {
            AssertResetsFlags(opcode, prefix, "N");
        }

        [Test]
        [TestCaseSource("ADD_rr_rr_Source")]
        public void ADD_rr_rr_sets_HF_appropriately(string dest, string src, byte opcode, byte? prefix)
        {
            SetReg(src, 0x10);
            foreach (var b in new byte[] { 0x0F, 0x7F, 0xFF })
            {
                SetReg(dest, NumberUtils.CreateShort(0xFF, b));

                Execute(opcode, prefix);
                Assert.That(Registers.HF == 1);

                Execute(opcode, prefix);
                Assert.That(Registers.HF == 0);
            }
        }

        [Test]
        [TestCaseSource("ADD_rr_rr_Source")]
        [TestCaseSource("ADD_rr_rr_Source_same_src_and_dest")]
        public void ADD_rr_rr_does_not_change_SF_ZF_PF(string dest, string src, byte opcode, byte? prefix)
        {
            var randomValues = Fixture.Create<byte[]>();
            var randomSF = Fixture.Create<Bit>();
            var randomZF = Fixture.Create<Bit>();
            var randomPF = Fixture.Create<Bit>();

            Registers.SF = randomSF;
            Registers.ZF = randomZF;
            Registers.PF = randomPF;

            SetReg(src, Fixture.Create<byte>());
            foreach (var value in randomValues)
            {
                SetReg(src, value);
                Execute(opcode, prefix);

                Assert.That(randomSF == Registers.SF);
                Assert.That(randomZF == Registers.ZF);
                Assert.That(randomPF == Registers.PF);
            }
        }

        [Test]
        [TestCaseSource("ADD_rr_rr_Source")]
        public void ADD_rr_rr_sets_bits_3_and_5_from_high_byte_of_result(string dest, string src, byte opcode, byte? prefix)
        {
            SetReg(dest, NumberUtils.CreateShort(0, ((byte)0).WithBit(3, 1).WithBit(5, 0)));
            SetReg(src, 0);
            Execute(opcode, prefix);
            Assert.That(Registers.Flag3 == 1);
            Assert.That(Registers.Flag5 == 0);

            SetReg(dest, NumberUtils.CreateShort(0, ((byte)0).WithBit(3, 0).WithBit(5, 1)));
            Execute(opcode, prefix);
            Assert.That(Registers.Flag3 == 0);
            Assert.That(Registers.Flag5 == 1);
        }

        [Test]
        [TestCaseSource("ADD_rr_rr_Source")]
        [TestCaseSource("ADD_rr_rr_Source_same_src_and_dest")]
        public void ADD_rr_rr_returns_proper_T_states(string dest, string src, byte opcode, byte? prefix)
        {
            var states = Execute(opcode, prefix);
            Assert.That(states, Is.EqualTo(IfIndexRegister(dest, 15, @else: 11)));
        }
    }
}