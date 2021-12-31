using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JukaCompiler.Exceptions
{
    internal class CompilerError : ICompilerError
    {
        internal List<string> Errors = new List<string> ();

        public CompilerError()
        {
        }
        void ICompilerError.AddError(string errorMessage)
        {
            Errors.Add(errorMessage);
            System.Diagnostics.Debugger.Break();
        }

        bool ICompilerError.HasErrors()
        {
            return Errors.Count > 0;
        }
    }
}
