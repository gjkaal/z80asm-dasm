using System;

namespace Z80Asm
{
    public class CodeException : Exception
    {
        public CodeException(string message, SourcePosition? position = null)
        {
            Position = position;
            this.message = message;

            if (position != null)
            {
                var s = position.Source.ExtractLine(position.LineNumber);
                if (s != null)
                {
                    this.message += Environment.NewLine + s;
                }
                var pos = position.CharacterPosition;
                if (pos > 0)
                {
                    this.message += Environment.NewLine + new string(' ', pos) + "^";
                }
            }
        }

        private readonly string message;

        public SourcePosition? Position { get; set; }
        public override string Message => message;
    }
}