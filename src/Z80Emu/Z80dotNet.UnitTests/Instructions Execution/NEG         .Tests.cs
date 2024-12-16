using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class NEG_tests : InstructionsExecutionTestsBase
    {
        private const byte opcode = 0x44;
        private const byte prefix = 0xED;

        [Test]
        public void NEG_substracts_A_from_zero()
        {
            var oldValue = Fixture.Create<byte>();

            Registers.A = oldValue;
            Execute();

            var expected = (byte)((byte)0).Sub(oldValue);
            Assert.That(Registers.A, Is.EqualTo(expected));
        }

        [Test]
        public void NEG_sets_SF_appropriately()
        {
            Registers.A = 2;
            Execute();
            Assert.That(Registers.SF == 1);

            Registers.A = 1;
            Execute();
            Assert.That(Registers.SF == 1);

            Registers.A = 0;
            Execute();
            Assert.That(Registers.SF == 0);

            Registers.A = 0xFF;
            Execute();
            Assert.That(Registers.SF == 0);

            Registers.A = 0x80;
            Execute();
            Assert.That(Registers.SF == 1);
        }

        [Test]
        public void NEG_sets_ZF_appropriately()
        {
            Registers.A = 2;
            Execute();
            Assert.That(Registers.ZF == 0);

            Registers.A = 1;
            Execute();
            Assert.That(Registers.ZF == 0);

            Registers.A = 0;
            Execute();
            Assert.That(Registers.ZF == 1);

            Registers.A = 0xFF;
            Execute();
            Assert.That(Registers.ZF == 0);

            Registers.A = 0x80;
            Execute();
            Assert.That(Registers.ZF == 0);
        }

        [Test]
        public void NEG_sets_HF_appropriately()
        {
            for (var i = 0; i <= 255; i++)
            {
                var b = (byte)i;
                Registers.A = b;
                Execute();
                var expected = (Bit)((b ^ Registers.A) & 0x10);
                Assert.That(Registers.HF, Is.EqualTo(expected));
            }
        }

        [Test]
        public void NEG_sets_PF_appropriately()
        {
            for (var i = 0; i <= 255; i++)
            {
                var b = (byte)i;
                Registers.A = b;
                Execute();
                var expected = b == 0x80 ? 1 : 0;
                Assert.That(Registers.PF == expected);
            }
        }

        [Test]
        public void NEG_sets_NF()
        {
            AssertSetsFlags(opcode, prefix, "N");
        }

        [Test]
        public void NEG_sets_CF_appropriately()
        {
            for (var i = 0; i <= 255; i++)
            {
                var b = (byte)i;
                Registers.A = b;
                Execute();
                var expected = b == 0 ? 0 : 1;
                Assert.That(Registers.CF == expected);
            }
        }

        [Test]
        public void NEG_sets_bits_3_and_5_from_result()
        {
            Registers.A = 0x0F;
            Execute();
            Assert.That(Registers.Flag3 == 0);
            Assert.That(Registers.Flag5 == 1);

            Registers.A = 0xF1;
            Execute();
            Assert.That(Registers.Flag3 == 1);
            Assert.That(Registers.Flag5 == 0);
        }

        [Test]
        public void NEG_returns_proper_T_states()
        {
            var states = Execute();
            Assert.That(states, Is.EqualTo(8));
        }

        private int Execute()
        {
            return Execute(opcode, prefix);
        }
    }
}