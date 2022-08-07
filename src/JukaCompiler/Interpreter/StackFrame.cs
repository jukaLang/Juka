using JukaCompiler.Lexer;

namespace JukaCompiler.Interpreter
{
    internal class StackFrame
    {
        private Dictionary<string, Lexeme> frameVariables = new Dictionary<string, Lexeme>();
        private string frameName;
        private Dictionary<string, Object> variables = new Dictionary<string, Object>();


        internal string FrameName
        {
            get { return frameName; }
        }

        internal StackFrame(string name)
        {
            this.frameName = name;
        }

        internal void AddVariables(Dictionary<string,Object> variables)
        {
            this.variables = variables;
        }

        internal bool TryGetStackVariableByName(string name, out object variable)
        {
            if(variables.ContainsKey(name))
            {
                variable = this.variables[name];
                return true;
            }

            variable = "object not found";

            return false;
        }
    }
}
