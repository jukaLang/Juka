using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JukaCompiler.Exceptions;
using JukaCompiler.Interpreter;

namespace JukaCompiler.SystemCalls
{
    internal class FileOpen : IFileOpen, IJukaCallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(JukaInterpreter interpreter, List<object> arguments)
        {
            foreach(var argument in arguments)
            {
                if (argument is Parse.Expression.LexemeTypeLiteral)
                {
                    Console.Out.WriteLine(((Parse.Expression.LexemeTypeLiteral)argument));
                }
            }

            return Encoding.ASCII.GetBytes("don reamey");
        }
    }
}
