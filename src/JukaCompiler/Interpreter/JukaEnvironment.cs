using JukaCompiler.Lexer;

namespace JukaCompiler.Interpreter
{
    internal class JukaEnvironment
    {
        private JukaEnvironment? enclosing;
        private Dictionary<string, Object> values = new Dictionary<string, Object>();
        
        internal JukaEnvironment()
        {
            enclosing = null;
        }

        internal JukaEnvironment Enclosing
        {
            set { enclosing = value; }
        }

        internal JukaEnvironment(JukaEnvironment enclosing)
        {
            this.enclosing = enclosing;
        }

        internal Object Get(Lexeme name)
        {
            if (values.ContainsKey(name.ToString()))
            {
                return values[name.ToString()];
            }

            if (enclosing != null)
            {
                return enclosing.Get(name);
            }

            throw new ArgumentException("JukaEnvironment.Get() has an undefined variable ('" + name.ToString() + "') undefined variable");
        }

        internal void Assign(Lexeme name, Object value)
        {
            if (values.ContainsKey(name.ToString()))
            {
                values[name.ToString()] = value;
                return;
            }

            if (enclosing !=null)
            {
                enclosing.Assign(name, value);
                return;
            }

            throw new ArgumentException("JukaEnvironment.Assign() has an undefined variable ('" + name.ToString() + "') undefined variable");
        }

        internal void Define(string name, Object value)
        {
            values.Add(name, value);
        }

        internal JukaEnvironment Ancestor(int distance)
        {
            JukaEnvironment environment = this;
            for(int i= 0; i < distance; i++)
            {
                environment = environment.enclosing;
            }

            return environment;
        }

        internal Object GetAt(int distance, string name)
        {
            return Ancestor(distance).values[name];
        }

        internal void AssignAt(int distance, Lexeme name, Object value)
        {
            Ancestor(distance).values[name.ToString()] = value;
        }

        override public string ToString()
        {
            string result = values.ToString();
            if (enclosing != null)
            {
                result += " -> " + enclosing.ToString();
            }

            return result;
        }
    }
}
