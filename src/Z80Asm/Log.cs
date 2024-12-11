using System;
using System.IO;

namespace Z80Asm
{
    public static class Log
    {
        private static readonly TextWriter _output = Console.Out;
        private static int _warnings = 0;
        private static int _errors = 0;

        public static int ErrorCount => _errors;

        public static void Reset()
        {
            _warnings = 0;
            _errors = 0;
        }

        public static void Warning(SourcePosition position, string message)
        {
            _warnings++;
            _output.WriteLine($"{position.Describe()}: warning: {message}");
        }

        public static void Error(SourcePosition position, string message)
        {
            _errors++;
            _output.WriteLine($"{position.Describe()}: {message}");

            if (_errors > 100)
            {
                throw new InvalidDataException("Error limit exceeded, aborting");
            }
        }

        public static void Error(string message)
        {
            _output.WriteLine($"{message}");
        }

        public static void Error(CodeException exception)
        {
            Error(exception.Position, exception.Message);
        }

        public static void DumpSummary()
        {
            _output.WriteLine($"Errors: {_errors} Warnings: {_warnings}");
        }
    }
}