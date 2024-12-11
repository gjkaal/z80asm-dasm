using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Z80Asm.UnitTests;

[TestClass]
public sealed class UtilsTests
{
    [DataTestMethod]
    [DataRow(new byte[] { 0x01 }, 0x807E)]
    [DataRow(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }, 0xBB2A)]
    [DataRow(new byte[] {}, 0xFFFF)]
    public void Crc16_Can_Calculate(byte[] data, int expectedValue)
    {
        var crc = new CRC16();
        crc.Update(data);
        Console.WriteLine($"CRC16: {crc.Value:X4}");
        Assert.AreEqual((ushort)expectedValue, crc.Value);
    }

    [DataTestMethod]
    [DataRow(new byte[] { 0x01 }, 0x26)]
    [DataRow(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }, 0x38)]
    [DataRow(new byte[] { }, 0xFF)]
    public void Crc8_Can_Calculate(byte[] data, int expectedValue)
    {
        var crc = new CRC8();
        crc.Update(data);
        Console.WriteLine($"CRC8: {crc.Value:X2}");
        Assert.AreEqual((byte)expectedValue, crc.Value);
    }
}