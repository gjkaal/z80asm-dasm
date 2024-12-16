using Microsoft.Extensions.FileProviders;
using System;
using System.Globalization;
using System.Linq;

namespace Konamiman.Z80dotNet
{
    public enum BankType
    {
        None, // no data stored or retrieved
        Ram,  // data stored and retrieved
        Rom,  // read only memory  (except when SetContents is used or LoadFromIntelHexFile)
    }

    /// <summary>
    /// Represents a trivial memory implementation in which all the addresses are RAM 
    /// and the values written are simply read back. This is the default implementation
    /// of <see cref="IMemory"/>.
    /// </summary>
    public class PlainMemory : IMemory
    {
        private readonly byte[] memory;

        public PlainMemory(int size)
        {
            if (size < 1)
                throw new InvalidOperationException("Memory size must be greater than zero");

            memory = new byte[size];
            Size = size;
        }

        private readonly Dictionary<byte, BankType> memoryBanks = new();
        public void SetMemoryBankType(byte bank, BankType bankType)
        {
            // upper 4 bits are the bank number
            var bankNumber = (byte)(bank & 0xF0);
            if (memoryBanks.ContainsKey(bankNumber)) memoryBanks.Remove(bankNumber);
            memoryBanks.Add(bankNumber, bankType);
        }

        public BankType GetBanktype(int address)
        {
            var bankNumber = (byte)((address >> 8) & 0xF0);
            if (memoryBanks.ContainsKey(bankNumber))
                return memoryBanks[bankNumber];
            // default to RAM
            return BankType.Ram;
        }

        public int Size { get; private set; }

        public byte this[int address]
        {
            get
            {
                switch (GetBanktype(address))
                {
                    case BankType.Ram:
                        return memory[address];
                    case BankType.Rom:
                        return memory[address];
                    default:
                        return 0xFF;
                }
            }
            set
            {
                if (GetBanktype(address) == BankType.Ram)
                    memory[address] = value;
            }
        }

        /// <summary>
        /// Loads the contents of a memory from  an assembly listing file.
        /// </summary>
        /// <param name="startAddress">The address where the data will be loaded.</param>
        /// <param name="file">A file containing the data</param>
        public void LoadFromListFile(int startAddress, IFileInfo file)
        {
            ArgumentNullException.ThrowIfNull(file);
            if (!file.Exists)
                throw new InvalidOperationException("File does not exist");
            using var stream = file.CreateReadStream();
            using var reader = new StreamReader(stream);
            LoadFromListFile(startAddress, reader);
        }

        /// <summary>
        /// Loads the contents of a memory from an assembly listing file.
        /// </summary>
        /// <param name="startAddress">The address offset where the data will be loaded.</param>
        /// <param name="reader">The stream containing the data</param>
        public void LoadFromListFile(int startAddress, TextReader reader)
        {
            // Read each line of the file and parse it
            while (true)
            {
                var line = reader.ReadLine()?.Trim();
                if (line == null)
                    break;
                // A line starts with an address followed by a colon and a space
                var colonIndex = line.IndexOf(':');
                if (colonIndex != 4) continue; // Not an address line, line contains a symbol or comment
                if (line.Length < 10) continue; // Not enough data, only the address is present

                // After that, the data bytes are separated by spaces, max 8 bytes
                var data = line.Substring(colonIndex + 2).Split(' ');
                // Parse the address
                var address = int.Parse(line.Substring(0, colonIndex), NumberStyles.HexNumber);
                // Copy the data to the memory, if anything is there
                // There are maximum 8 bytes in a line
                var dataBytes = new List<byte>();
                for (var i = 0; i < 8; i++)
                {
                    // If there is no data, break, the line is done
                    // other data is mnemonics or comments
                    if (data[i].Trim() == string.Empty) break;
                    dataBytes.Add(byte.Parse(data[i], NumberStyles.HexNumber));
                }
                // If there is no data, continue to the next line
                if (dataBytes.Count == 0) continue;
                SetContents(
                    startAddress + address,
                    dataBytes.ToArray(),
                    0,
                    dataBytes.Count
                    );
            }
        }


        /// <summary>
        /// Loads the contents of a memory from an Intel HEX file.
        /// </summary>
        /// <param name="startAddress">The address where the data will be loaded.</param>
        /// <param name="file">A file containing the data</param>
        public void LoadFromIntelHexFile(int startAddress, IFileInfo file)
        {
            ArgumentNullException.ThrowIfNull(file);
            if (!file.Exists)
                throw new InvalidOperationException("File does not exist");
            using var stream = file.CreateReadStream();
            using var reader = new StreamReader(stream);
            LoadFromIntelHexFile(startAddress, reader);
        }

        /// <summary>
        /// Loads the contents of a memory from an Intel HEX file.
        /// </summary>
        /// <param name="startAddress">The address where the data will be loaded.</param>
        /// <param name="reader">The stream containing the data</param>
        public void LoadFromIntelHexFile(int startAddress, TextReader reader)
        {
            // Read each line of the file and parse it
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                    break;
                var record = IntelHexFileRecord.Parse(line);
                if (record == null)
                    continue;
                // If the record is a data record, copy the data to the memory
                if (record.RecordType == IntelHexFileRecordType.DataRecord)
                {
                    SetContents(
                        startAddress + record.Address,
                        record.Data,
                        0,
                        record.Data.Length
                        );
                }
            }
        }

        public void SetContents(int startAddress, byte[] contents, int startIndex = 0, int? length = null)
        {
            ArgumentNullException.ThrowIfNull(contents);

            if (length == null)
                length = contents.Length;

            if ((startIndex + length) > contents.Length)
                throw new IndexOutOfRangeException("startIndex + length cannot be greater than contents.length");

            if (startIndex < 0)
                throw new IndexOutOfRangeException("startIndex cannot be negative");

            if (startAddress + length > Size)
                throw new IndexOutOfRangeException("startAddress + length cannot go beyond the memory size");

            Array.Copy(
                sourceArray: contents,
                sourceIndex: startIndex,
                destinationArray: memory,
                destinationIndex: startAddress,
                length: length.Value
                );
        }

        public byte[] GetContents(int startAddress, int length)
        {
            if (startAddress >= memory.Length)
                throw new IndexOutOfRangeException("startAddress cannot go beyond memory size");

            if (startAddress + length > memory.Length)
                throw new IndexOutOfRangeException("startAddress + length cannot go beyond memory size");

            if (startAddress < 0)
                throw new IndexOutOfRangeException("startAddress cannot be negative");

            return memory.Skip(startAddress).Take(length).ToArray();
        }
    }
}
