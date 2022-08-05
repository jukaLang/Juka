using JukaCompiler.Lexer;

namespace JukaCompiler.Interpreter
{
    internal class StackFrame
    {
        private Dictionary<string, Lexeme> frameVariables = new Dictionary<string, Lexeme>();
        private string frameName;


        internal string FrameName
        {
            get { return frameName; }
        }

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
