using JukaCompiler.Exceptions;
using JukaCompiler.Lexer;

namespace JukaCompiler.Interpreter
{
    internal class JukaEnvironment
    {
        private JukaEnvironment? enclosing;
        private Dictionary<string, object?> values = new Dictionary<string, object?>();
        
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

        internal object? Get(Lexeme name)
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

        internal void Assign(Lexeme? name, object? value)
        {
            var nameAsString = name?.ToString() ?? throw new JRuntimeException("unable to get variable name");

            if (values.ContainsKey(nameAsString))
            {
                values[name.ToString()] = value;
                return;
            }

            if (enclosing !=null)
            {
                enclosing.Assign(name, value);
                return;
            }

            throw new ArgumentException("JukaEnvironment.Assign() has an undefined variable ('" + name + "') undefined variable");
        }

        internal void Define(string name, object? value)
        {
            values.Add(name, value);
        }

        internal JukaEnvironment? Ancestor(int distance)
        {
            JukaEnvironment? environment = this;
            for(int i= 0; i < distance; i++)
            {
                environment = environment?.enclosing;
            }

            return environment;
        }

        internal object? GetAt(int distance, string name)
        {
            return Ancestor(distance)?.values[name];
        }

        internal void AssignAt(int distance, Lexeme name, object? value)
        {
            Ancestor(distance)!.values[name.ToString()] = value;
        }

        public override string ToString()
        {
            string result = values.ToString()!;
            if (enclosing != null)
            {
                result += " -> " + enclosing;
            }

            return result;
        }
    }
}
