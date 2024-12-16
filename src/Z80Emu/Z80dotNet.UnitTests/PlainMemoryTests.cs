using AutoFixture;

namespace Konamiman.Z80dotNet.Tests
{
    public class PlainMemoryTests
    {
        private int MemorySize { get; set; }
        private PlainMemory Sut { get; set; } = new(1000);
        private Fixture Fixture { get; set; } = new();
        private readonly Random random = new();

        private int Random(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();
            MemorySize = Random(100, 1000000);
            Sut = new PlainMemory(MemorySize);
        }

        [Test]
        public void Can_create_instances()
        {
            Assert.That(Sut, Is.Not.Null);
        }

        [Test]
        public void Cannot_create_memory_with_negative_size()
        {
            Assert.Throws<InvalidOperationException>(() => new PlainMemory(-MemorySize));
        }

        [Test]
        public void Cannot_create_memory_with_size_zero()
        {
            Assert.Throws<InvalidOperationException>(() => new PlainMemory(0));
        }

        [Test]
        public void Can_write_value_and_read_it_back_in_valid_address()
        {
            var address = Random(0, MemorySize - 1);
            var value = Fixture.Create<byte>();

            Sut[address] = value;
            var actual = Sut[address];
            Assert.That(value, Is.EqualTo(actual));

            value ^= 255;

            Sut[address] = value;
            actual = Sut[address];
            Assert.That(value, Is.EqualTo(actual));
        }

        [Test]
        public void Cannot_access_value_on_address_equal_to_memory_size()
        {
            Assert.Throws<IndexOutOfRangeException>(() => Sut[MemorySize].ToString());
        }

        [Test]
        public void Cannot_access_value_on_address_larger_than_memory_size()
        {
            Assert.Throws<IndexOutOfRangeException>(() => Sut[MemorySize + 1].ToString());
        }

        [Test]
        public void Cannot_access_value_on_negative_address()
        {
            Assert.Throws<IndexOutOfRangeException>(() => Sut[-Fixture.Create<int>()].ToString());
        }

        [Test]
        public void Can_set_contents_and_read_them_back_within_valid_range()
        {
            var data = Fixture.CreateMany<byte>(MemorySize / 3).ToArray();
            var address = Random(0, MemorySize / 3);

            Sut.SetContents(address, data);

            var actual = Sut.GetContents(address, data.Length);
            Assert.That(data, Is.EqualTo(actual));
        }

        [Test]
        public void Can_set_contents_and_read_them_back_when_touching_upper_boundary()
        {
            var data = Fixture.CreateMany<byte>(MemorySize / 3).ToArray();
            var address = MemorySize - data.Length;

            Sut.SetContents(address, data);

            var actual = Sut.GetContents(address, data.Length);
            Assert.That(data, Is.EqualTo(actual));
        }

        [Test]
        public void Can_set_contents_from_partial_contents_of_array()
        {
            var data = Fixture.CreateMany<byte>(MemorySize / 3).ToArray();
            var dataStartIndex = Random(1, data.Length - 1);
            var dataLength = Random(1, data.Length - dataStartIndex);
            var address = Random(0, MemorySize / 3);

            Sut.SetContents(address, data, dataStartIndex, dataLength);

            var expected = data.Skip(dataStartIndex).Take(dataLength).ToArray();
            var actual = Sut.GetContents(address, dataLength);
            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public void Throws_if_setting_contents_with_wrong_startIndex_and_length_combination()
        {
            var data = Fixture.CreateMany<byte>(MemorySize / 3).ToArray();
            var dataStartIndex = Random(1, data.Length - 1);
            var dataLength = data.Length - dataStartIndex + 1;
            var address = Random(0, MemorySize / 3);

            Assert.Throws<IndexOutOfRangeException>(() => Sut.SetContents(address, data, dataStartIndex, dataLength));
        }

        [Test]
        public void Cannot_set_contents_from_null_array()
        {
            var address = Random(0, MemorySize / 3);

            Assert.Throws<ArgumentNullException>(() => Sut.SetContents(address, null));
        }

        [Test]
        public void Cannot_set_contents_specifying_negative_startIndex()
        {
            var data = Fixture.CreateMany<byte>(MemorySize / 3).ToArray();
            var dataStartIndex = Random(1, data.Length - 1);
            var dataLength = Random(1, data.Length - dataStartIndex);
            var address = Random(0, MemorySize / 3);

            Assert.Throws<IndexOutOfRangeException>(() => Sut.SetContents(address, data, -dataStartIndex, dataLength));
        }

        [Test]
        public void Can_set_contents_with_zero_length_and_nothing_changes()
        {
            var data = Fixture.CreateMany<byte>(MemorySize / 3).ToArray();
            var dataStartIndex = Random(1, data.Length - 1);
            var address = Random(0, MemorySize / 3);

            var before = Sut.GetContents(0, MemorySize);
            Sut.SetContents(address, data, dataStartIndex, length: 0);
            var after = Sut.GetContents(0, MemorySize);

            Assert.That(after, Is.EqualTo(before));
        }

        [Test]
        public void Cannot_set_contents_beyond_memory_length()
        {
            var address = Random(0, MemorySize - 1);
            var data = Fixture.CreateMany<byte>(MemorySize - address + 1).ToArray();

            Assert.Throws<IndexOutOfRangeException>(() => Sut.SetContents(address, data));
        }

        [Test]
        public void Cannot_set_contents_specifying_address_beyond_memory_length()
        {
            var address = MemorySize + 1;
            var data = Fixture.CreateMany<byte>(1).ToArray();

            Assert.Throws<IndexOutOfRangeException>(() => Sut.SetContents(address, data));
        }

        [Test]
        public void Cannot_get_contents_beyond_memory_length()
        {
            var address = Random(0, MemorySize - 1);
            var length = MemorySize - address + 1;

            Assert.Throws<IndexOutOfRangeException>(() => Sut.GetContents(address, length));
        }

        [Test]
        public void Cannot_get_contents_specifying_address_beyond_memory_length()
        {
            var address = MemorySize + 1;

            Assert.Throws<IndexOutOfRangeException>(() => Sut.GetContents(address, 1));
        }

        [Test]
        public void Cannot_get_contents_specifying_negative_address()
        {
            var address = Random(0, MemorySize - 1);

            Assert.Throws<IndexOutOfRangeException>(() => Sut.GetContents(-address, 1));
        }

        [Test]
        public void Can_get_contents_with_lenth_zero_and_empty_array_is_returned()
        {
            var address = Random(0, MemorySize - 1);
            var actual = Sut.GetContents(address, 0);

            Assert.That(actual, Is.EqualTo(new Byte[0]));
        }

        [Test]
        public void CanLoadFromIntelHex()
        {
            var memStream = new MemoryStream(Encoding.UTF8.GetBytes(IntelHexData));
            var reader = new StreamReader(memStream);
            Sut.LoadFromIntelHexFile(0, reader);

            var expected = new byte[] { 0x31, 0xFF, 0x2F, 0x21, 0x00, 0x00, 0xE5, 0xF1, 0xD1, 0xC9, 0x00 };
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.That(Sut[i], Is.EqualTo(expected[i]));
            }
        }

        [Test]
        public void CanLoadFromListFile()
        {
            var memStream = new MemoryStream(Encoding.UTF8.GetBytes(ListFile));
            var reader = new StreamReader(memStream);
            Sut.LoadFromListFile(0, reader);

            var expected = new byte[] { 0x31, 0xFF, 0x2F, 0x21, 0x00, 0x00, 0xE5, 0xF1, 0xD1, 0xC9, 0x00 };
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.That(Sut[i], Is.EqualTo(expected[i]));
            }
        }

        private const string IntelHexData =
            ":0A00000031FF2F210000E5F1D1C906\n" +
            ":00000001FF\n";

        private const string ListFile = @"
                                ; This code causes a stack underflow by removing the return address
                                ; from the stack before returning from the interrupt handler.
                                RAM_END: equ 0x2FFF
                                
0000: 31 FF 2F                      ld sp, RAM_END
0003: 21 00 00                      ld hl, 0x0000
0006: E5                            push hl
0007: F1                            pop af
0008: D1                            pop de
0009: C9                            ret
                                
                                

000A:
";
    }
}
