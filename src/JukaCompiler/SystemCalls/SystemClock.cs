using JukaCompiler.Parse;
using JukaCompiler.Interpreter;
using System.Diagnostics;
using JukaCompiler.Expressions;

namespace JukaCompiler.SystemCalls
{
    internal class SystemClock : ISystemClock, IJukaCallable
    {
        public int Arity()
        {
            return 0;
        }
        public object? Call(string methodName, JukaInterpreter interpreter, List<object?> arguments)
        {
            Expr.LexemeTypeLiteral? lexemeTypeLiteral = new();
            lexemeTypeLiteral.literal = (double)DateTime.Now.Millisecond / 1000.0;
            return lexemeTypeLiteral;
        }
        override public string ToString()
        {
            return "[Native Fn]";
        }
    }


    /*
     * Gets the available memory of the process. Currently including the Juka Runtime.
     */
    internal class GetAvailableMemory : IGetAvailableMemory, IJukaCallable
    {
        public int Arity()
        {
            return 0;
        }

        public object? Call(string methodName, JukaInterpreter interpreter, List<object?> arguments)
        {
            decimal memory;
            
            Process proc = Process.GetCurrentProcess();
            memory = Math.Round((decimal)proc.PrivateMemorySize64 / (1024 * 1024), 2);
            proc.Dispose();

            Expr.LexemeTypeLiteral? lexemeTypeLiteral = new();
            lexemeTypeLiteral.literal = memory;
            return lexemeTypeLiteral;
        }
    }
}
