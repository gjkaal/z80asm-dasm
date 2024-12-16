namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Represents a record in an Intel HEX file.
    /// </summary>
    public class IntelHexFileRecord
    {
        public IntelHexFileRecordType RecordType { get; set; }
        public byte Length { get; set; }
        public ushort Address { get; set; }
        public byte[] Data { get; set; }

        // :1001A00079746573004552523A005741524E3A00F5
        // :00000001FF
        public static IntelHexFileRecord Parse(string line)
        {
            // Parse the line and return a new IntelHexFileRecord object
            if (line.Length < 11) throw new InvalidOperationException("Line is too short");
            if (line[0] != ':') throw new InvalidOperationException("Line does not start with ':'");
            // last 2 characters are the checksum
            var checksum = Convert.ToByte(line.Substring(line.Length - 2), 16);

            var byteData = new List<byte>();
            for (var i = 1; i < line.Length - 2; i += 2)
            {
                var byteString = line.Substring(i, 2);
                byteData.Add(Convert.ToByte(byteString, 16));
            }
            // Calculate the checksum
            var data = byteData.ToArray();
            Checksum(data);
            if (checksum != Checksum(data))
                throw new InvalidOperationException("Checksum does not match");
            return new IntelHexFileRecord()
            {
                RecordType = (IntelHexFileRecordType)data[3],
                Length = data[0],
                Address = (ushort)((data[1] << 8) | data[2]),
                Data = data.Skip(4).Take(data[0]).ToArray()
            };
        }

        private static byte Checksum(byte[] data)
        {
            byte sum = 0;
            for (var i = 0; i < data.Length; i++)
            {
                sum += data[i];
            }
            return (byte)-sum;
        }

        public static bool TryParse(string line, out IntelHexFileRecord record)
        {
            record = new IntelHexFileRecord();
            try
            {
                record = Parse(line);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
