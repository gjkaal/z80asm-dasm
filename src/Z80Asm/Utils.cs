using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Z80Asm
{
    /// <summary>
    /// Cyclic Redundancy Check 16 bits implementation
    /// </summary>
    public class CRC16
    {
        public const ushort Polynomial = 0xA001;
        public ushort Value { get; private set; } = 0xFFFF;

        public void Reset()
        {
            Value = 0xFFFF;
        }

        public void Update(byte data)
        {
            Value ^= data;
            for (int i = 0; i < 8; i++)
            {
                if ((Value & 1) == 1)
                {
                    Value = (ushort)((Value >> 1) ^ Polynomial);
                }
                else
                {
                    Value >>= 1;
                }
            }
        }
        public void Update(byte[] data)
        {
            foreach (var b in data)
            {
                Update(b);
            }
        }
    }

    public class CRC8
    {
        public const byte Polynomial = 0x31;
        public byte Value { get; private set; } = 0xFF;

        public void Reset()
        {
            Value = 0xFF;
        }

        public void Update(byte data)
        {
            Value ^= data;
            for (int i = 0; i < 8; i++)
            {
                if ((Value & 1) == 1)
                {
                    Value = (byte)((Value >> 1) ^ Polynomial);
                }
                else
                {
                    Value >>= 1;
                }
            }
        }
        public void Update(byte[] data)
        {
            foreach (var b in data)
            {
                Update(b);
            }
        }
    }

    public class HexDumpOptions
    {
        public bool ShowCrc { get; set; }
        public bool NoHeader { get; set; }
        public bool NoSpaces { get; set; }
        public bool ShowLineNumbers { get; set; }
    }

    public static class Utils
    {
        public static void HexDump(byte[] data, 
            int bytesPerLine = 8, 
            Action<HexDumpOptions>? options = null,
            Action<string>? output = null)
        {
            output ??= Console.WriteLine;
            var opt = new HexDumpOptions();
            if(options!=null) options(opt);

            if (!opt.NoHeader)
            {
                output.Invoke(string.Empty);
                output.Invoke("# HEX Dump");
            }

            var crc8 = new CRC8();
            var crc16 = new CRC16();
            var line = 0;
            var hex = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                crc8.Update(data[i]);
                crc16.Update(data[i]);
                hex.Append(data[i].ToString("X2"));
                if ((i % bytesPerLine) == bytesPerLine-1 || i == data.Length-1)
                {
                    if (opt.ShowLineNumbers)
                    {
                        hex.Insert(0, $"{line++:0000} ");
                    }
                    if(opt.ShowCrc)
                    {
                        hex.Append($" / {crc8.Value:X2}");
                    }

                    output.Invoke(hex.ToString());
                    hex.Clear();
                    crc8.Reset();
                }
                else
                {
                    if(!opt.NoSpaces)
                        hex.Append(' ');
                }
            }
            if(opt.ShowCrc)
            {
                output.Invoke($"CRC16: {crc16.Value:X4}");
            }
        }

        public static string NormalizeSymbol(this string symbol)
        {
            return symbol.Trim().ToLowerInvariant();
        }

        public static BinaryWriter OpenBinaryWriter(this FileInfo fileInfo, string filename, string defExt)
        {
            if (fileInfo == null)
            {
                throw new InvalidOperationException("No input file specified");
            }
            if (filename == ":default")
            {
                var filePath = fileInfo.FullName;
                return new BinaryWriter(File.OpenWrite(System.IO.Path.ChangeExtension(filePath, defExt)));
            }
            else
                return new BinaryWriter(File.OpenWrite(filename));
        }

        public static TextWriter OpenTextWriter(this FileInfo fileInfo, string filename, string defExt)
        {
            if (fileInfo == null)
            {
                throw new InvalidOperationException("No input file specified");
            }
            if (filename == ":default")
            {
                var filePath = fileInfo.FullName;
                return new StreamWriter(System.IO.Path.ChangeExtension(filePath, defExt));
            }
            else
                return new StreamWriter(filename);
        }

        public static string AstDesc(this SourcePosition? pos)
        {
            if (pos == null)
                return "(no file location)";
            else
                return $"({pos.Source.DisplayName} {pos.LineNumber + 1},{pos.CharacterPosition + 1})";
        }

        public static string Describe(this SourcePosition? pos)
        {
            if (pos == null)
                return "<unknown>";
            else
                return $"{pos.Source.DisplayName}({pos.LineNumber + 1},{pos.CharacterPosition + 1})";
        }

        public static string Indent(int indent)
        {
            return new string(' ', indent * 2);
        }

        public static string TypeName(object o)
        {
            if (o == null)
                return "null";
            var name = o.GetType().Name;
            if (name.StartsWith("ExprNode"))
                return name.Substring(8).ToLowerInvariant();
            return name;
        }

        public static byte PackByte(SourcePosition pos, object value)
        {
            if (value is long longValue)
                return PackByte(pos, longValue);
            Log.Error(pos, $"Can't convert {Utils.TypeName(value)} to byte");
            return 0xFF;
        }

        public static ushort PackWord(SourcePosition pos, object value)
        {
            if (value is long longValue)
                return PackWord(pos, longValue);
            Log.Error(pos, $"Can't convert {Utils.TypeName(value)} to word");
            return 0xFF;
        }

        public static byte PackByte(SourcePosition pos, long value)
        {
            // Check range (yes, sbyte and byte)
            if (value < sbyte.MinValue || value > byte.MaxValue)
            {
                Log.Error(pos, $"value out of range: {value} (0x{value:X}) doesn't fit in 8-bits");
                return 0xFF;
            }
            else
            {
                return (byte)(value & 0xFF);
            }
        }

        public static ushort PackWord(SourcePosition pos, long value)
        {
            // Check range (yes, short and ushort)
            if (value < short.MinValue || value > ushort.MaxValue)
            {
                Log.Error(pos, $"value out of range: {value} (0x{value:X}) doesn't fit in 16-bits");
                return 0xFFFF;
            }
            else
            {
                return (ushort)(value & 0xFFFF);
            }
        }

        public static ushort ParseUShort(string str)
		{
			try
			{
				if (str.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
				{
					return Convert.ToUInt16(str.Substring(2), 16);
				}
				else
				{
					return ushort.Parse(str);
				}
			}
			catch (Exception)
			{
				throw new InvalidOperationException(string.Format("Invalid number: '{0}'", str));
			}
		}

		public static int[] ParseIntegers(string str, int Count)
		{
			var values = new List<int>();
			if (str != null)
			{
				foreach (var n in str.Split(','))
				{
					values.Add(int.Parse(n));
				}
			}

			if (Count != 0 && Count != values.Count)
			{
				throw new InvalidOperationException(string.Format("Invalid value - expected {0} comma separated values", Count));
			}


			return [.. values];
		}


		public static List<string> ParseCommandLine(string args)
		{
			var newargs = new List<string>();

			var temp = new StringBuilder();

			int i = 0;
			while (i < args.Length)
			{
				if (char.IsWhiteSpace(args[i]))
				{
					i++;
					continue;
				}

				bool bInQuotes = false;
				temp.Length = 0;
				while (i < args.Length && (!char.IsWhiteSpace(args[i]) || bInQuotes))
				{
					if (args[i] == '\"')
					{
						if (args[i + 1] == '\"')
						{
							temp.Append('"');
							i++;
						}
						else
						{
							bInQuotes = !bInQuotes;
						}
					}
					else
					{
						temp.Append(args[i]);
					}

					i++;
				}

				if (temp.Length > 0)
				{
					newargs.Add(temp.ToString());
				}
			}

			return newargs;
		}

	}
}
