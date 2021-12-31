using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JukaCompiler.Interpreter
{
    internal interface Callable
    {
        int Arity();
        Object Call(Interpreter interpreter, List<Object> arguments);
    }
}
