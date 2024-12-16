﻿using AutoFixture;

namespace Konamiman.Z80dotNet.Tests.InstructionsExecution
{
    public class LD_SP_HL_IX_IY_tests : InstructionsExecutionTestsBase
    {
        public static object[] LD_Source =
        [
            new object[] { "HL", (byte)0xF9, (byte?)null },
            new object[] { "IX", (byte)0xF9, (byte?)0xDD },
            new object[] { "IY", (byte)0xF9, (byte?)0xFD }
        ];

        [Test]
        [TestCaseSource("LD_Source")]
        public void LD_SP_HL_IX_IY_loads_SP_correctly(string reg, byte opcode, byte? prefix)
        {
            var newSp = Fixture.Create<short>();
            var oldSP = Fixture.Create<short>();

            SetReg(reg, newSp);
            Registers.SP = oldSP;

            Execute(opcode, prefix);

            Assert.That(newSp, Is.EqualTo(GetReg<short>(reg)));
            Assert.That(newSp, Is.EqualTo(Registers.SP));
        }

        [Test]
        [TestCaseSource("LD_Source")]
        public void LD_SP_HL_IX_IY_fire_FetchFinished_with_isLdSp_true(string reg, byte opcode, byte? prefix)
        {
            var eventFired = false;

            Sut.InstructionFetchFinished += (sender, e) =>
            {
                eventFired = true;
                Assert.That(e.IsLdSpInstruction);
            };

            Execute(opcode, prefix);

            Assert.That(eventFired);
        }

        [Test]
        [TestCaseSource("LD_Source")]
        public void LD_SP_HL_IX_IY_do_not_change_flags(string reg, byte opcode, byte? prefix)
        {
            AssertNoFlagsAreModified(opcode, prefix);
        }

        [Test]
        [TestCaseSource("LD_Source")]
        public void LD_SP_HL_IX_IY_return_proper_T_states(string reg, byte opcode, byte? prefix)
        {
            var states = Execute(opcode, prefix);
            Assert.That(states, Is.EqualTo(reg == "HL" ? 6 : 10));
        }
    }
}