using JukaCompiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JukaCompiler.Interpreter
{
    internal class StackFrame
    {
        private Dictionary<string, Lexeme> frameVariables = new Dictionary<string, Lexeme>();
        private string frameName;

        internal StackFrame(string name)
        {
            this.frameName = name;
        }

        internal void AddVariable(Lexeme variable)
        {
            this.frameVariables.Add(variable.ToString(), variable);
        }
    }
}
