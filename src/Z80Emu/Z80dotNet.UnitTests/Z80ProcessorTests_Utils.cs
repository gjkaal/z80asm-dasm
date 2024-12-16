using AutoFixture;

namespace Konamiman.Z80dotNet.Tests
{
    public class Z80ProcessorTests_Utils
    {
        public Fixture Fixture { get; set; }

        public Z80Processor Sut { get; set; }

        [SetUp]
        public void SetUp()
        {
            Fixture = new Fixture();
            Sut = new Z80Processor();
        }

        [Test]
        public void ExecuteCall_pushes_PC_and_jumps()
        {
            var oldPC = Fixture.Create<ushort>();
            var callAddress = Fixture.Create<ushort>();
            var oldSP = Fixture.Create<short>();
            Sut.Registers.SP = oldSP;
            Sut.Registers.PC = oldPC;

            Sut.ExecuteCall(callAddress);

            Assert.That(callAddress, Is.EqualTo(Sut.Registers.PC));
            Assert.That(oldSP.Sub(2), Is.EqualTo(Sut.Registers.SP));
            Assert.That(ReadShortFromMemory(Sut.Registers.SP.ToUShort()), Is.EqualTo(oldPC.ToShort()));
        }

        [Test]
        public void ExecuteRet_pops_PC()
        {
            var pushedAddress = Fixture.Create<ushort>();
            var sp = Fixture.Create<short>();

            Sut.Registers.SP = sp;
            Sut.Memory[sp.ToUShort()] = pushedAddress.GetLowByte();
            Sut.Memory[sp.ToUShort().Inc()] = pushedAddress.GetHighByte();

            Sut.ExecuteRet();

            Assert.That(pushedAddress, Is.EqualTo(Sut.Registers.PC));
            Assert.That(sp.Add(2), Is.EqualTo(Sut.Registers.SP));
        }

        private short ReadShortFromMemory(ushort address)
        {
            return NumberUtils.CreateShort(Sut.Memory[address], Sut.Memory[address.Inc()]);
        }
    }
}
