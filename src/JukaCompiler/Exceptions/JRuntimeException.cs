using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JukaCompiler.Exceptions
{
    internal class JRuntimeException : Exception
    {
        internal JRuntimeException(string message)
            : base(message)
        {
        }
    }
}
