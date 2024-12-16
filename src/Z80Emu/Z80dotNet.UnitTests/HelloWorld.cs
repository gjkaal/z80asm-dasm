using Konamiman.Z80dotNet.Enums;
using NUnit.Framework;

namespace Konamiman.Z80dotNet.Tests
{
    public class HelloWorld
    {
        [Test]
        public void HelloWorldTest()
        {
            IZ80Processor Sut = new Z80Processor();
            Sut.AutoStopOnRetWithStackEmpty = true;
            Sut.InstructionExecutor = new Z80InstructionExecutor();

            var agent = Sut as IZ80ProcessorAgent;
            Assert.That(agent, Is.Not.Null);
            Sut.InstructionExecutor.InitProcessorAgent(agent!);

            var program = new byte[]
            {
                0x3E, 0x07, //LD A,7
                0xC6, 0x04, //ADD A,4
                0x3C,       //INC A
                0xC9        //RET
            };

            Sut.Memory.SetContents(0, program);
            Sut.Start();

            Assert.That(Sut.StopReason, Is.EqualTo(StopReason.RetWithStackEmpty));
            Assert.That(Sut.Registers.A, Is.EqualTo(12));
            Assert.That(Sut.TStatesElapsedSinceStart, Is.EqualTo(28));
        }

        [Test]
        public void TestStopOnRet()
        {
            IZ80Processor Sut = new Z80Processor();
            Sut.AutoStopOnRetInstruction = true;
            Sut.InstructionExecutor = new Z80InstructionExecutor();

            var agent = Sut as IZ80ProcessorAgent;
            Assert.That(agent, Is.Not.Null);
            Sut.InstructionExecutor.InitProcessorAgent(agent!);

            var program = new byte[]
            {
                0x3E, 0x07, //LD A,7
                0xC6, 0x04, //ADD A,4
                0x3C,       //INC A
                0xC9        //RET
            };

            Sut.Memory.SetContents(0, program);
            Sut.Start();

            Assert.That(Sut.StopReason, Is.EqualTo(StopReason.RetInstruction));
            Assert.That(Sut.Registers.A, Is.EqualTo(12));
            Assert.That(Sut.TStatesElapsedSinceStart, Is.EqualTo(28));
        }

        [Test]
        public void TestStopOnStackUnderflow()
        {
            // 210000E5F1D1C9
            IZ80Processor Sut = new Z80Processor();
            Sut.Registers.InitializeSP(0x2FFF);
            Sut.AutoStopOnRetInstruction = true;
            Sut.AutoStopOnStackLimits = true;
            Sut.InstructionExecutor = new Z80InstructionExecutor();

            var agent = Sut as IZ80ProcessorAgent;
            Assert.That(agent, Is.Not.Null);
            Sut.InstructionExecutor.InitProcessorAgent(agent!);

            // RAM_END: equ 0x2FFF

            var program = new byte[]
            {
                0x31, 0xFF, 0x2F,   // ld sp, RAM_END
                0x21, 0x00, 0x00,   // ld hl, 0x1000
                0xE5,               // push hl
                0xF1,               // pop af
                0xD1,               // pop de
                0xC9,               // ret
            };

            Sut.Memory.SetContents(0, program);
            Sut.Start();

            Assert.That(Sut.StopReason, Is.EqualTo(StopReason.StackUnderflow));
            Assert.That(Sut.Registers.A, Is.EqualTo(0));
        }

        [Test]
        public void TestStopOnStackOverflow()
        {
            // 210000E5F1D1C9
            IZ80Processor Sut = new Z80Processor();
            Sut.Registers.InitializeSP(0x2FFF);
            Sut.Registers.SPLowerLimit = 0x2FFF - 6;
            Sut.AutoStopOnRetInstruction = true;
            Sut.AutoStopOnStackLimits = true;
            Sut.InstructionExecutor = new Z80InstructionExecutor();

            var agent = Sut as IZ80ProcessorAgent;
            Assert.That(agent, Is.Not.Null);
            Sut.InstructionExecutor.InitProcessorAgent(agent!);

            var program = new byte[]
            {
                 0x31, 0xFF, 0x2F,   // ld sp, RAM_END
                 0xE5,               // push hl
                 0xE5,               // push hl
                 0xE5,               // push hl
                 0xE5,               // push hl
                 0xE5,               // push hl
                 0xE5,               // push hl
                 0xC9,               // ret
            };

            Sut.Memory.SetContents(0, program);
            Sut.Start();

            Assert.That(Sut.StopReason, Is.EqualTo(StopReason.StackOverflow));
            Assert.That(Sut.Registers.SP, Is.EqualTo(0x2FFF - 8));
        }
    }
}