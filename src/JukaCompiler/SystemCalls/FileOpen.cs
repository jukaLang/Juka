using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            throw new NotImplementedException();
        }
    }
}
