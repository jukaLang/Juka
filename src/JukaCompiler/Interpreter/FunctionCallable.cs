using JukaCompiler.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JukaCompiler.Interpreter
{
    internal class FunctionCallable
    {
        private Stmt.Function declaration;
        private DreamEnvironment closure;
        private bool isInitializer;

        internal FunctionCallable(Stmt.Function declaration, DreamEnvironment closure, bool isInitializer)
        {
            this.isInitializer = isInitializer;
            this.declaration = declaration;
            this.closure = closure;
        }
    }
}
