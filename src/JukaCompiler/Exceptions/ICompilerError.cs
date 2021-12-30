using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JukaCompiler.Exceptions
{
    internal interface ICompilerError
    {
        internal void AddError(string errorMessage);
        internal bool HasErrors();
    }
}
