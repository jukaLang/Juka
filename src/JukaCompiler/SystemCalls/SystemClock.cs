using JukaCompiler.Parse;
using JukaCompiler.Interpreter;

namespace JukaCompiler.SystemCalls
{
    internal class SystemClock : ISystemClock, IJukaCallable
    {
        public int Arity()
        {
            return 0;
        }
        public object Call(JukaInterpreter interpreter, List<object> arguments)
        {
            Expression.LexemeTypeLiteral lexemeTypeLiteral = new();
            lexemeTypeLiteral.literal = (double)DateTime.Now.Millisecond / 1000.0;
            return lexemeTypeLiteral;
        }
        override public string ToString()
        {
            return "[Native Fn]";
        }
    }
}
