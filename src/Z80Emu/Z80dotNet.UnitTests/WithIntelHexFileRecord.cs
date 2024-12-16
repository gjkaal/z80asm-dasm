namespace Konamiman.Z80dotNet.Tests
{
    public class WithIntelHexFileRecord
    {
        [Test]
        [TestCaseSource(nameof(RecordTypes))]
        public void Can_parse_record_type(string record, IntelHexFileRecordType expectedType, int expectedAddress, string expectedData)
        {
            var sut = IntelHexFileRecord.Parse(record);
            Assert.That(sut.RecordType, Is.EqualTo(expectedType));
            Assert.That(sut.Address, Is.EqualTo(expectedAddress));

            var checkData = Convert.ToHexString(sut.Data).ToUpperInvariant();
            Assert.That(checkData, Is.EqualTo(expectedData));
        }

        public static object[] RecordTypes =
        [
            new object[] { ":1001A00079746573004552523A005741524E3A00F5",  IntelHexFileRecordType.DataRecord, 0x01A0, "79746573004552523A005741524E3A00"},
            new object[] { ":00000001FF",  IntelHexFileRecordType.EndOfFile, 0x0000, ""},
        ];
    }
}