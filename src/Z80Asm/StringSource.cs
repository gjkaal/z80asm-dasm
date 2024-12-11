using System;
using System.Text;

namespace Z80Asm
{
    // Represents a position within a StringSource
    // with helpers to map back to the source itself and the line
    // number and character offset
    public class SourcePosition
    {
        private int _charPosition = -1;

        private int _lineNumber = -1;

        public int Position { get; private set; }

        public StringSource Source { get; private set; }

        public SourcePosition(StringSource source, int pos)
        {
            Source = source;
            Position = pos;
        }

        public int CharacterPosition
        {
            get
            {
                if (_lineNumber < 0)
                {
                    Source.LineNumbers.FromFileOffset(Position, out _lineNumber, out _charPosition);
                }
                return _charPosition;
            }
        }

        public int LineNumber
        {
            get
            {
                if (_lineNumber < 0)
                {
                    Source.LineNumbers.FromFileOffset(Position, out _lineNumber, out _charPosition);
                }
                return _lineNumber;
            }
        }
    }

    public class StringSource
    {
        private readonly string displayName;

        private readonly string location;

        private readonly StringBuilder sb = new();

        private readonly int startPos;

        private readonly int stopPos;

        private readonly string source;

        private LineNumbers? lineNumbers;

        private int pos;

        // Construct a string source from a string
        public StringSource(string str, string displayName, string location)
        {
            // Remove BOM
            if (str.Length > 0 && (str[0] == 0xFFFE || str[0] == 0xFEFF))
                str = str[1..];

            source = str;
            pos = 0;
            startPos = 0;
            stopPos = str.Length;
            this.displayName = displayName;
            this.location = location;

            // Skip BOM if exists
            if (!Skip((char)0xFFFE))
                Skip((char)0xFEFF);
        }

        // Construct a string source from a string
        public StringSource(string str, int startPos, int length, string displayName = "", string location = "")
        {
            source = str;
            pos = startPos;
            this.startPos = startPos;
            stopPos = startPos + length;
            this.displayName = displayName;
            this.location = location;
        }

        // The current character
        public char Current => pos < stopPos ? source[pos] : '\0';

        public string DisplayName => displayName;

        // Have we reached the end of the file?
        public bool EOF => pos >= stopPos;

        public bool EOL
        {
            get
            {
                if (pos >= stopPos)
                    return true;

                return source[pos] == '\r' || source[pos] == '\n';
            }
        }

        public LineNumbers LineNumbers
        {
            get
            {
                lineNumbers ??= new LineNumbers(source);
                return lineNumbers;
            }
        }

        public string Location => location;

        // The current position
        public int Position
        {
            get => pos;
            set
            {
                System.Diagnostics.Debug.Assert(value >= startPos && value <= stopPos);
                pos = value;
            }
        }

        // The remaining text (handy for watching in debugger)
        public string Remaining => source.Substring(pos);

        public string SourceText => source.Substring(startPos, stopPos - startPos);

        public SourcePosition CapturePosition()
        {
            return new SourcePosition(this, pos);
        }

        // The character at offset from current
        public char CharAt(int offset)
        {
            var pos = this.pos + offset;
            return pos < stopPos ? source[pos] : '\0';
        }

        public StringSource CreateEmbeddedSource(int from, int length)
        {
            return new StringSource(source, from, length, displayName, location);
        }

        public SourcePosition CreateEndPosition()
        {
            return CreatePosition(stopPos);
        }

        public SourcePosition CreateLinePosition(int line)
        {
            return new SourcePosition(this, LineNumbers.ToFileOffset(line, 0));
        }

        public SourcePosition CreatePosition(int position)
        {
            return new SourcePosition(this, position);
        }

        // Check if the current position in the string matches a substring
        public bool DoesMatch(string str)
        {
            if (pos + str.Length > stopPos)
                return false;

            for (int i = 0; i < str.Length; i++)
            {
                if (source[pos + i] != str[i])
                    return false;
            }

            return true;
        }

        // Check if the current position in the string matches a substring (case insensitive)
        public bool DoesMatchI(string str)
        {
            if (pos + str.Length > stopPos)
                return false;

            for (int i = 0; i < str.Length; i++)
            {
                if (char.ToLowerInvariant(source[pos + i]) != char.ToLowerInvariant(str[i]))
                    return false;
            }

            return true;
        }

        // Extract text from the specified position to the current position
        public string Extract(int fromPosition)
        {
            return source[fromPosition..pos];
        }

        public string Extract(int fromPosition, int toPosition)
        {
            return source.Substring(fromPosition, toPosition - fromPosition);
        }

        public string ExtractLine(int line)
        {
            int linePos = LineNumbers.ToFileOffset(line, 0);
            int nextLinePos = LineNumbers.ToFileOffset(line + 1, 0);
            if (nextLinePos > 1 && source[nextLinePos - 1] == '\n')
                nextLinePos--;
            if (nextLinePos > 1 && source[nextLinePos - 1] == '\r')
                nextLinePos--;
            return source.Substring(linePos, nextLinePos - linePos);
        }

        // Move by n places
        public void Move(int delta)
        {
            pos += delta;
            if (pos < 0)
                pos = 0;
            if (pos > stopPos)
                pos = stopPos;
        }

        // Move to the next character (if available)
        public void Next()
        {
            if (pos < stopPos)
                pos++;
        }

        // Move to the previous character
        public void Previous()
        {
            if (pos > 0)
                pos--;
        }

        public string Process(Func<char, StringBuilder, bool> callback)
        {
            sb.Length = 0;
            while (pos < stopPos && callback(source[pos], sb))
            {
                pos++;
            }
            return sb.ToString();
        }

        // Skip characters matching predicate
        public bool Skip(Func<char, bool> predicate)
        {
            if (pos >= stopPos || !predicate(source[pos]))
                return false;

            pos++;
            while (pos < stopPos && predicate(source[pos]))
                pos++;
            return true;
        }

        // Skip the specified character
        public bool Skip(char ch)
        {
            if (pos >= stopPos)
                return false;

            if (source[pos] != ch)
                return false;

            pos++;
            return true;
        }

        // Skip a string (case sensitive)
        public bool Skip(string str)
        {
            if (DoesMatch(str))
            {
                pos += str.Length;
                return true;
            }
            return false;
        }

        // Skip characters matching predicate and return the matched characters
        public string SkipAndExtract(Func<char, bool> predicate)
        {
            int pos = this.pos;
            if (!Skip(predicate))
                return string.Empty;
            return Extract(pos);
        }

        public bool SkipEOL()
        {
            int oldPos = pos;
            if (pos < stopPos && source[pos] == '\r')
                pos++;
            if (pos < stopPos && source[pos] == '\n')
                pos++;
            return pos > oldPos;
        }

        // Skip the specified character
        public bool SkipI(char ch)
        {
            if (pos >= stopPos)
                return false;

            if (char.ToUpperInvariant(source[pos]) != char.ToUpperInvariant(ch))
                return false;

            pos++;
            return true;
        }

        // Skip a string (case insensitive)
        public bool SkipI(string str)
        {
            if (DoesMatchI(str))
            {
                pos += str.Length;
                return true;
            }
            return false;
        }

        // Skip whitespace
        public bool SkipLinespace()
        {
            if (pos >= stopPos)
                return false;
            if (!IsLineSpace(source[pos]))
                return false;

            pos++;
            while (pos < stopPos && IsLineSpace(source[pos]))
                pos++;

            return true;
        }

        public string SkipRemaining()
        {
            var str = Remaining;
            pos = stopPos;
            return str;
        }

        public bool SkipToEOL()
        {
            // Skip to end of line
            int start = pos;
            while (!EOF && source[pos] != '\r' && source[pos] != '\n')
                pos++;
            return pos > start;
        }

        public bool SkipToNextLine()
        {
            int start = pos;
            SkipToEOL();
            SkipEOL();
            return pos > start;
        }

        // Skip forward until a particular string is matched
        public bool SkipUntil(string str)
        {
            while (pos < stopPos)
            {
                if (DoesMatch(str))
                    return true;
                pos++;
            }
            return false;
        }

        // Skip forward until a particular string is matched (case insensitive)
        public bool SkipUntilI(string str)
        {
            while (pos < stopPos)
            {
                if (DoesMatchI(str))
                    return true;
                pos++;
            }
            return false;
        }

        // Skip whitespace
        public bool SkipWhitespace()
        {
            if (pos >= stopPos)
                return false;
            if (!char.IsWhiteSpace(source[pos]))
                return false;

            pos++;
            while (pos < stopPos && char.IsWhiteSpace(source[pos]))
                pos++;

            return true;
        }

        private static bool IsLineSpace(char ch) => ch == ' ' || ch == '\t';
    }
}