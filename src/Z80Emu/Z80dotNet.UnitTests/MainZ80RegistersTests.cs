using AutoFixture;

namespace Konamiman.Z80dotNet.Tests
{
    public class MainZ80RegistersTests
    {
        private Fixture Fixture { get; set; }
        private MainZ80Registers Sut { get; set; }

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();
            Sut = new MainZ80Registers();
        }

        [Test]
        public void Gets_A_and_F_correctly_from_AF()
        {
            var A = Fixture.Create<byte>();
            var F = Fixture.Create<byte>();
            var AF = NumberUtils.CreateShort(F, A);

            Sut.AF = AF;

            Assert.That(A, Is.EqualTo(Sut.A));
            Assert.That(F, Is.EqualTo(Sut.F));
        }

        [Test]
        public void Sets_AF_correctly_from_A_and_F()
        {
            var A = Fixture.Create<byte>();
            var F = Fixture.Create<byte>();
            var expected = NumberUtils.CreateShort(F, A);

            Sut.A = A;
            Sut.ChangeFlags(F);

            Assert.That(Sut.AF, Is.EqualTo(expected));
        }

        [Test]
        public void Gets_B_and_C_correctly_from_BC()
        {
            var B = Fixture.Create<byte>();
            var C = Fixture.Create<byte>();
            var BC = NumberUtils.CreateShort(C, B);

            Sut.BC = BC;

            Assert.That(B, Is.EqualTo(Sut.B));
            Assert.That(C, Is.EqualTo(Sut.C));
        }

        [Test]
        public void Sets_BC_correctly_from_B_and_C()
        {
            var B = Fixture.Create<byte>();
            var C = Fixture.Create<byte>();
            var expected = NumberUtils.CreateShort(C, B);

            Sut.B = B;
            Sut.C = C;

            Assert.That(Sut.BC, Is.EqualTo(expected));
        }

        [Test]
        public void Gets_D_and_E_correctly_from_DE()
        {
            var D = Fixture.Create<byte>();
            var E = Fixture.Create<byte>();
            var DE = NumberUtils.CreateShort(E, D);

            Sut.DE = DE;

            Assert.That(D, Is.EqualTo(Sut.D));
            Assert.That(E, Is.EqualTo(Sut.E));
        }

        [Test]
        public void Sets_DE_correctly_from_D_and_E()
        {
            var D = Fixture.Create<byte>();
            var E = Fixture.Create<byte>();
            var expected = NumberUtils.CreateShort(E, D);

            Sut.D = D;
            Sut.E = E;

            Assert.That(Sut.DE, Is.EqualTo(expected));
        }

        [Test]
        public void Gets_H_and_L_correctly_from_HL()
        {
            var H = Fixture.Create<byte>();
            var L = Fixture.Create<byte>();
            var HL = NumberUtils.CreateShort(L, H);

            Sut.HL = HL;

            Assert.That(H, Is.EqualTo(Sut.H));
            Assert.That(L, Is.EqualTo(Sut.L));
        }

        [Test]
        public void Sets_HL_correctly_from_H_and_L()
        {
            var H = Fixture.Create<byte>();
            var L = Fixture.Create<byte>();
            var expected = NumberUtils.CreateShort(L, H);

            Sut.H = H;
            Sut.L = L;

            Assert.That(Sut.HL, Is.EqualTo(expected));
        }

        [Test]
        public void Gets_CF_correctly_from_F()
        {
            Sut.ChangeFlags(0xFE);
            Assert.That(Sut.CF == 0);

            Sut.ChangeFlags(0x01);
            Assert.That(Sut.CF == 1);
        }

        [Test]
        public void Sets_F_correctly_from_CF()
        {
            Sut.ChangeFlags(0xFF);
            Sut.CF = 0;
            Assert.That(Sut.F, Is.EqualTo(0xFE));

            Sut.ChangeFlags(0x00);
            Sut.CF = 1;
            Assert.That(Sut.F, Is.EqualTo(0x01));
        }

        [Test]
        public void Gets_NF_correctly_from_F()
        {
            Sut.ChangeFlags(0xFD);
            Assert.That(Sut.NF == 0);

            Sut.ChangeFlags(0x02);
            Assert.That(Sut.NF == 1);
        }

        [Test]
        public void Sets_F_correctly_from_NF()
        {
            Sut.ChangeFlags(0xFF);
            Sut.NF = 0;
            Assert.That(Sut.F, Is.EqualTo(0xFD));

            Sut.ChangeFlags(0x00);
            Sut.NF = 1;
            Assert.That(Sut.F, Is.EqualTo(0x02));
        }

        [Test]
        public void Gets_PF_correctly_from_F()
        {
            Sut.ChangeFlags(0xFB);
            Assert.That(Sut.PF == 0);

            Sut.ChangeFlags(0x04);
            Assert.That(Sut.PF == 1);
        }

        [Test]
        public void Sets_F_correctly_from_PF()
        {
            Sut.ChangeFlags(0xFF);
            Sut.PF = 0;
            Assert.That(Sut.F, Is.EqualTo(0xFB));

            Sut.ChangeFlags(0x00);
            Sut.PF = 1;
            Assert.That(Sut.F, Is.EqualTo(0x04));
        }

        [Test]
        public void Gets_Flag3_correctly_from_F()
        {
            Sut.ChangeFlags(0xF7);
            Assert.That(Sut.Flag3 == 0);

            Sut.ChangeFlags(0x08);
            Assert.That(Sut.Flag3 == 1);
        }

        [Test]
        public void Sets_F_correctly_from_Flag3()
        {
            Sut.ChangeFlags(0xFF);
            Sut.Flag3 = 0;
            Assert.That(Sut.F, Is.EqualTo(0xF7));

            Sut.ChangeFlags(0x00);
            Sut.Flag3 = 1;
            Assert.That(Sut.F, Is.EqualTo(0x08));
        }

        [Test]
        public void Gets_HF_correctly_from_F()
        {
            Sut.ChangeFlags(0xEF);
            Assert.That(Sut.HF == 0);

            Sut.ChangeFlags(0x10);
            Assert.That(Sut.HF == 1);
        }

        [Test]
        public void Sets_F_correctly_from_HF()
        {
            Sut.ChangeFlags(0xFF);
            Sut.HF = 0;
            Assert.That(Sut.F, Is.EqualTo(0xEF));

            Sut.ChangeFlags(0x00);
            Sut.HF = 1;
            Assert.That(Sut.F, Is.EqualTo(0x10));
        }

        [Test]
        public void Gets_Flag5_correctly_from_F()
        {
            Sut.ChangeFlags(0xDF);
            Assert.That(Sut.Flag5 == 0);

            Sut.ChangeFlags(0x20);
            Assert.That(Sut.Flag5 == 1);
        }

        [Test]
        public void Sets_F_correctly_from_Flag5()
        {
            Sut.ChangeFlags(0xFF);
            Sut.Flag5 = 0;
            Assert.That(Sut.F, Is.EqualTo(0xDF));

            Sut.ChangeFlags(0x00);
            Sut.Flag5 = 1;
            Assert.That(Sut.F, Is.EqualTo(0x20));
        }

        [Test]
        public void Gets_ZF_correctly_from_F()
        {
            Sut.ChangeFlags(0xBF);
            Assert.That(Sut.ZF == 0);

            Sut.ChangeFlags(0x40);
            Assert.That(Sut.ZF == 1);
        }

        [Test]
        public void Sets_F_correctly_from_ZF()
        {
            Sut.ChangeFlags(0xFF);
            Sut.ZF = 0;
            Assert.That(Sut.F, Is.EqualTo(0xBF));

            Sut.ChangeFlags(0x00);
            Sut.ZF = 1;
            Assert.That(Sut.F, Is.EqualTo(0x40));
        }

        [Test]
        public void Gets_SF_correctly_from_F()
        {
            Sut.ChangeFlags(0x7F);
            Assert.That(Sut.SF == 0);

            Sut.ChangeFlags(0x80);
            Assert.That(Sut.SF == 1);
        }

        [Test]
        public void Sets_F_correctly_from_SF()
        {
            Sut.ChangeFlags(0xFF);
            Sut.SF = 0;
            Assert.That(Sut.F, Is.EqualTo(0x7F));

            Sut.ChangeFlags(0x00);
            Sut.SF = 1;
            Assert.That(Sut.F, Is.EqualTo(0x80));
        }
    }
}
