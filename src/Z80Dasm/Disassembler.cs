﻿using System;
using System.Text;
using Z80Mnemonics;

namespace Z80DAsm
{
    internal class Disassembler
    {
        public static char HexFormat = 'h';

        public static string FormatWord(ushort w)
        {
            switch (HexFormat)
            {
                case 'x':
                    return $"0x{w:X4}";

                case '$':
                    return $"${w:X4}";

                case '&':
                    return $"&h{w:X4}";

                case 'h':
                default:
                    var r = string.Format("{0:X4}h", w);
                    if (!char.IsDigit(r[0]))
                        r = "0" + r;
                    return r;
            }
        }

        public static string FormatByte(byte b)
        {
            switch (HexFormat)
            {
                case 'x':
                    return $"0x{b:X2}";

                case '$':
                    return $"${b:X2}";

                case '&':
                    return $"&h{b:X2}";

                case 'h':
                default:
                    var r = string.Format("{0:X2}h", b);
                    if (!char.IsDigit(r[0]))
                        r = "0" + r;
                    return r;
            }
        }

        public static string FormatSignedByte(sbyte b)
        {
            if (b < 0)
                return "-" + FormatByte((byte)-b);
            else
                return FormatByte((byte)b);
        }

        public static ushort LabelledRangeLow = 0;
        public static ushort LabelledRangeHigh = 0;
        public static bool LowerCase = false;
        public static bool HtmlMode = false;
        public static bool ShowRelativeOffsets = false;

        public static string FormatAddr(ushort addr, bool link = true, bool prefix = true)
        {
            if (addr >= LabelledRangeLow && addr < LabelledRangeHigh)
            {
                if (prefix)
                {
                    if (HtmlMode && link)
                        return string.Format("<a href=\"#L{0:X4}\">L{0:X4}</a>", addr);
                    else
                        return string.Format("L{0:X4}", addr);
                }
                else
                {
                    if (HtmlMode && link)
                        return string.Format("<a href=\"#L{0:X4}\">{0:X4}</a>", addr);
                    else
                        return string.Format("{0:X4}", addr);
                }
            }
            else
            {
                return FormatWord(addr);
            }
        }

        public static Instruction Disassemble(byte[] buffer, ushort offsetInBuffer, ushort addr)
        {
            Func<byte> readByte = () =>
            {
                addr++;
                return (byte)(offsetInBuffer < buffer.Length ? buffer[offsetInBuffer++] : 0);
            };

            var i = new Instruction
            {
                addr = addr
            };

            var start_addr = addr;
            byte opc = readByte();
            byte disp_u = 0;

            ushort jump_addr = 0;
            bool have_jump_addr = false;

            OpCode dasm = null;
            bool have_disp = false;

            switch (opc)
            {
                case 0xDD:
                case 0xFD:
                    byte next = readByte();
                    if ((next | 0x20) == 0xFD || next == 0xED)
                    {
                        i.Asm = "NOP*";
                        i.t_states = 4;
                        dasm = null;
                    }
                    else if (next == 0xCB)
                    {
                        disp_u = readByte();
                        next = readByte();

                        dasm = (opc == 0xDD) ? OpCodes.DasmDDCB[next] : OpCodes.DasmFDCB[next];
                        have_disp = true;
                    }
                    else
                    {
                        dasm = (opc == 0xDD) ? OpCodes.DasmDD[next] : OpCodes.DasmFD[next];
                        if (dasm.mnemonic == null) //mirrored instructions
                        {
                            dasm = OpCodes.DasmBase[next];
                            i.t_states = 4;
                            i.t_states2 = 4;
                        }
                    }
                    break;

                case 0xED:
                    next = readByte();
                    dasm = OpCodes.DasmED[next];
                    if (dasm.mnemonic == null)
                    {
                        i.Asm = "NOP*";
                        i.t_states = 8;
                        dasm = null;
                    }
                    break;

                case 0xCB:
                    next = readByte();
                    dasm = OpCodes.DasmCB[next];
                    break;

                default:
                    dasm = OpCodes.DasmBase[opc];
                    break;
            }

            if (dasm != null)
            {
                i.opCode = dasm;

                var sb = new StringBuilder();

                foreach (var ch in LowerCase ? dasm.mnemonic.ToLower() : dasm.mnemonic)
                {
                    switch (ch)
                    {
                        case '@':
                            {
                                var lo = readByte();
                                var hi = readByte();
                                ushort val = (ushort)(lo + (hi * 0x100));

                                if ((dasm.flags & (OpCodeFlags.RefAddr | OpCodeFlags.Jumps)) != 0)
                                    sb.Append(FormatAddr(val));
                                else
                                    sb.Append(FormatWord(val));

                                i.word_val = val;

                                jump_addr = val;
                                have_jump_addr = true;
                                break;
                            }

                        case '$':
                        case '%':
                            {
                                if (!have_disp)
                                    disp_u = readByte();
                                var disp = (disp_u & 0x80) != 0 ? -(((~disp_u) & 0x7f) + 1) : disp_u;

                                if (ShowRelativeOffsets)
                                {
                                    if (disp > 0)
                                    {
                                        i.Comment = string.Format("+{0}", disp);
                                    }
                                    else
                                    {
                                        i.Comment = string.Format("{0}", disp);
                                    }
                                }

                                if (ch == '$')
                                {
                                    if ((sbyte)disp < 0 && sb[sb.Length - 1] == '+')
                                    {
                                        sb.Length--;
                                    }
                                    sb.Append(FormatSignedByte((sbyte)disp));
                                }
                                else
                                {
                                    jump_addr = (ushort)(addr + disp);
                                    have_jump_addr = true;
                                    sb.Append(FormatAddr(jump_addr));
                                }

                                break;
                            }

                        case '#':
                            {
                                var lo = readByte();
                                sb.Append(FormatByte(lo));
                                if (lo >= 0x20 && lo <= 0x7f)
                                    i.Comment = string.Format("'{0}'", (char)lo);
                                i.byte_val = lo;
                                break;
                            }

                        default:
                            sb.Append(ch);
                            break;
                    }
                }

                i.Asm = sb.ToString();

                i.t_states += dasm.t_states;
                i.t_states2 += dasm.t_states2;

                // Return continue address
                if ((dasm.flags & OpCodeFlags.Continues) != 0)
                    i.next_addr_1 = addr;

                // Return jump target address (if have it)
                if ((dasm.flags & OpCodeFlags.Jumps) != 0 && have_jump_addr)
                {
                    i.next_addr_2 = jump_addr;
                }
            }
            else
            {
                i.next_addr_1 = addr;
            }

            if (i.t_states == i.t_states2)
                i.t_states2 = 0;

            i.bytes = (ushort)(addr - start_addr);
            return i;
        }
    }
}