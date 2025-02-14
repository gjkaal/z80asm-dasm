﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Z80Asm
{
    public class GenerateContext
    {
        public int Ip { get; private set; }

        private readonly Action<string, bool> ListFile;

        public int Op { get; private set; }

        public List<byte> OutputBytes { get; } = [];

        private const int ListColumnWidth = 32;

        public AstContainer? Container { get; set; }

        private readonly Stack<SourcePosition> _sourceFileStack = new();

        private readonly List<byte?> _unlistedBytes = [];

        private int _currentInstructionAddress;

        private AstInstruction? _instruction;

        private int _listColumnPos;

        private bool _listEnabled = true;

        private SourcePosition? _listPosition;

        private int _macroDepth;

        private bool _truncateErrorShown;

        private int _unlistedBytesAddress;

        public GenerateContext(
            Action<string, bool>? listFile)
        {
            OutputBytes = [];
            if (listFile == null)
            {
                ListFile = NullWriter;
            }
            else
            {
                ListFile = listFile;
            }
        }

        private void NullWriter(string text, bool newLine)
        {
            // Do nothing
        }

        public IReadOnlyCollection<Contentblock> ContentBlocks => _contentBlocks;
        private int BlockStartAddress = 0;
        private int BlockEndAddress = 0;
        private readonly List<Contentblock> _contentBlocks = [];

        private void NextContentblock(byte[] bytes)
        {
            if (bytes.Length == 0)
            {
                return;
            }

            var cb = new Contentblock
            {
                Name = $"ADDRESS {BlockStartAddress:X4}",
                Source = _listPosition?.Source.DisplayName ?? string.Empty,
                Address = BlockStartAddress,
                Length = BlockEndAddress - BlockStartAddress
            };
            cb.Data = new byte[cb.Length];

            Array.Copy(
                bytes,
                cb.Address,
                cb.Data,
                0,
                cb.Length);
            _contentBlocks.Add(cb);
        }

        public void Emit(byte? val)
        {
            if (_listEnabled)
            {
                if (_listColumnPos == 0)
                    WriteListingText($"{Ip:X4}: ");

                if (_listColumnPos + 3 < ListColumnWidth)
                {
                    WriteListingText($"{FormatByte(val)} ");
                }
                else
                {
                    if (_unlistedBytes.Count == 0)
                        _unlistedBytesAddress = Ip;
                    _unlistedBytes.Add(val);
                }
            }

            if (Ip < 0 || Ip > 0xFFFF)
            {
                if (!_truncateErrorShown)
                {
                    Log.Error($"error: output truncated, address 0x{Ip:X} out of range");
                    _truncateErrorShown = true;
                }
            }

            if (Ip > OutputBytes.Count)
            {
                // Add empty, start next code block?
                BlockEndAddress = Op;
                NextContentblock(OutputBytes.ToArray());
                OutputBytes.AddRange(Enumerable.Repeat<byte>(0xFF, Ip - OutputBytes.Count));
                BlockStartAddress = Op = Ip;
            }

            if (Ip < Op)
            {
                throw new InvalidDataException($"error: output bytes out of sync: {Ip} < {Op}");
            }

            if (Op < OutputBytes.Count)
            {
                if (val.HasValue)
                {
                    // Do not overwrite existing data
                    if (OutputBytes[Op] != 0xFF)
                    {
                        throw new InvalidDataException($"error: output overwrite at address 0x{Op:X}");
                    }
                    else
                    {
                        OutputBytes[Op] = val.Value;
                    }
                }
            }
            else if (Op == OutputBytes.Count)
            {
                OutputBytes.Add(val ?? 0xFF);
            }
            else
            {
                OutputBytes.AddRange(Enumerable.Repeat<byte>(0xFF, Op - OutputBytes.Count));
                Op += OutputBytes.Count;
                OutputBytes.Add(val ?? 0xFF);
            }

            Op++;
            Ip++;
        }

        public void Emit(ushort? val)
        {
            if (val.HasValue)
            {
                Emit((byte?)(val.Value & 0xFF));
                Emit((byte?)((val.Value >> 8) & 0xFF));
            }
            else
            {
                Emit((byte?)null);
                Emit((byte?)null);
            }
        }

        // Emit any 16-bit vaue (must be between -16384 and 65535)
        public void Emit16(SourcePosition pos, long value)
        {
            var mempos = pos.Position;
            Emit(Utils.PackWord(pos, value));
        }

        // Emit any 8-bit value (must be between -128 and 255)
        public void Emit8(SourcePosition pos, long value)
        {
            var mempos = pos.Position;
            Emit(Utils.PackByte(pos, value));
        }

        // Emit an array of bytes
        public void EmitBytes(byte[] bytes, bool list)
        {
            var wasListEnabled = _listEnabled;
            _listEnabled = list;

            for (var i = 0; i < bytes.Length; i++)
            {
                Emit(bytes[i]);
            }

            _listEnabled = wasListEnabled;
        }

        // Emit an array of bytes
        public void EmitBytes(byte?[] bytes, bool list)
        {
            var wasListEnabled = _listEnabled;
            _listEnabled = list;

            for (var i = 0; i < bytes.Length; i++)
            {
                Emit(bytes[i]);
            }

            _listEnabled = wasListEnabled;
        }

        // Emit a relative offset where addr is the address
        // And the current instruction address is current IP
        public void EmitRelOffset(SourcePosition pos, int addr)
        {
            // Calculate the offset
            var offset = addr - (_currentInstructionAddress + 2);

            // Check range (yes, sbyte and byte)
            if (offset < sbyte.MinValue || offset > sbyte.MaxValue)
            {
                Log.Error(pos, $"relative offset out of range: from 0x{_currentInstructionAddress:X4} to 0x{addr:X4} is {offset} and must be between {sbyte.MinValue} and {sbyte.MaxValue}");
                Emit(0xFF);
                return;
            }

            Emit((byte)(offset & 0xFF));
        }

        public void EnterInstruction(AstInstruction instruction)
        {
            _instruction = instruction;
            _currentInstructionAddress = Ip;
        }

        public void EnterMacro()
        {
            _macroDepth++;
        }

        // Save current source file position
        public void EnterSourceFile(SourcePosition pos)
        {
            if (ListFile != null && _listPosition != null)
            {
                ListFile.Invoke(string.Empty, true);
                ListFile.Invoke($"{new string(' ', ListColumnWidth)}---------------------------------", true);
                ListFile.Invoke(string.Empty, true);
            }

            if (_listPosition != null)
                _sourceFileStack.Push(_listPosition);
            _listPosition = pos.Source.CreatePosition(0);
        }

        // Get the generated bytes in a range
        public byte[] GetGeneratedBytes()
        {
            BlockEndAddress = Op;
            NextContentblock(OutputBytes.ToArray());
            return OutputBytes.ToArray();
        }

        public void LeaveInstruction(AstInstruction instruction)
        {
            if (_instruction != instruction)
                Log.Error($"Internal error: instruction mismatch");
            _currentInstructionAddress = 0;
        }

        public void LeaveMacro()
        {
            _macroDepth--;
        }

        // Pop current source file position
        public void LeaveSourceFile()
        {
            // List the rest of the file
            if (_listPosition != null)
                ListTo(_listPosition.Source.CreateEndPosition());

            if (_sourceFileStack.Count > 0)
                _listPosition = _sourceFileStack.Pop();
            else
                _listPosition = null;

            if (ListFile != null)
            {
                if (_listPosition != null)
                {
                    ListFile.Invoke(string.Empty, true);
                    ListFile.Invoke($"{new string(' ', ListColumnWidth)}---------------------------------", true);
                    ListFile.Invoke(string.Empty, true);
                }
                else
                {
                    ListFile.Invoke($"\n{Ip:X4}:", true);
                }
            }
        }

        public void ListTo(SourcePosition pos)
        {
            if (ListFile == null || _macroDepth > 0)
                return;

            // Make sure it's the same file
            System.Diagnostics.Debug.Assert(_listPosition == null || _listPosition.Source == pos.Source);

            // Work out the start and end lines
            var fromLine = _listPosition == null ? 0 : _listPosition.LineNumber;
            var toLine = pos.LineNumber;

            // Add all listed lines
            var indent = new string(' ', ListColumnWidth);
            for (var i = fromLine; i < toLine; i++)
            {
                if (i == fromLine)
                {
                    var spaceNeeded = indent.Length - _listColumnPos;
                    if (spaceNeeded > 0)
                        ListFile.Invoke(new string(' ', spaceNeeded), false);
                }
                else
                {
                    ListFile.Invoke(indent, false);
                }

                ListFile.Invoke(pos.Source.ExtractLine(i), true);

                if (i == fromLine && _unlistedBytes.Count > 0)
                {
                    DumpUnlistedBytes();
                }
            }

            if (_unlistedBytes.Count > 0)
            {
                ListFile.Invoke(string.Empty, true);
                DumpUnlistedBytes();
            }

            _listColumnPos = 0;
            _unlistedBytes.Clear();

            // Store where we've listed to
            _listPosition = pos;
        }

        public void ListToInclusive(SourcePosition pos)
        {
            ListTo(pos.Source.CreateLinePosition(pos.LineNumber + 1));
        }

        public void Seek(int address)
        {
            Op = address;
        }

        public void SetOrg(int address)
        {
            Ip = address;
        }

        public void WriteListingText(string str)
        {
            // Write the listing bytes
            ListFile.Invoke(str, false);

            // Update the current position
            _listColumnPos += str.Length;
        }

        private static string FormatByte(byte? val)
        {
            if (val.HasValue)
                return $"{val.Value:X2}";
            else
                return "??";
        }

        private void DumpUnlistedBytes()
        {
            for (var b = 0; b < _unlistedBytes.Count; b++)
            {
                if (b % 8 == 0)
                {
                    if (b > 0)
                        ListFile.Invoke(string.Empty, true);
                    ListFile.Invoke($"{_unlistedBytesAddress + b:X4}: ", false);
                }
                ListFile.Invoke($"{FormatByte(_unlistedBytes[b])} ", false);
            }
            ListFile.Invoke(string.Empty, true);
            _unlistedBytes.Clear();
        }
    }
}