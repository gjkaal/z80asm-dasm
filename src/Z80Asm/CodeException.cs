using System;

namespace Z80Asm
{
    public class CodeException : Exception
    {
        public CodeException(string message, SourcePosition? position = null)
        {
            Position = position;
            this.message = message;
        }

        private readonly string message;

        public SourcePosition? Position { get; set; }
        public override string Message => message;
    }
}