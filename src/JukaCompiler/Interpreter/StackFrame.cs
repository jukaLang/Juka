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

        internal void AddVariable(string name, object obj)
        {
            if (variables.ContainsKey(name))
            { 
                variables[name] = obj;
            }
            else
            {
                variables.Add(name, obj);
            }
        }

        internal void UpdateVariable(string name, object value)
        {
            if(variables.ContainsKey(name))
            {
                variables[name] = value;
            }
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
