using JukaCompiler.Interpreter;
using System.Diagnostics;
using JukaCompiler.Expressions;
using System.Runtime.InteropServices;

namespace JukaCompiler.SystemCalls
{
    // Class to get the system clock
    internal class SystemClock : ISystemClock, IJukaCallable
    {
        public int Arity() => 0;

        public object? Call(string methodName, JukaInterpreter interpreter, List<object?> arguments)
        {
            Expr.LexemeTypeLiteral lexemeTypeLiteral = new Expr.LexemeTypeLiteral
            {
                literal = (double)DateTime.Now.Millisecond / 1000.0
            };
            return lexemeTypeLiteral;
        }

        public override string ToString() => "[Native Fn]";
    }

    // Class to get the available memory of the process
    internal class GetAvailableMemory : IGetAvailableMemory, IJukaCallable
    {
        public int Arity() => 0;

        public object? Call(string methodName, JukaInterpreter interpreter, List<object?> arguments)
        {
            decimal memory;

            using (Process proc = Process.GetCurrentProcess())
            {
                memory = Math.Round((decimal)proc.PrivateMemorySize64 / (1024 * 1024), 2);

                if (memory == 0 && RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                {
                    memory = Math.Round((decimal)proc.VirtualMemorySize64 / (1024 * 1024), 2);
                }
            }

            Expr.LexemeTypeLiteral lexemeTypeLiteral = new Expr.LexemeTypeLiteral
            {
                literal = memory
            };
            return lexemeTypeLiteral;
        }
    }
}
