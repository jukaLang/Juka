using JukaCompiler.Exceptions;
using JukaCompiler.Lexer;

namespace JukaCompiler.Interpreter
{
    /// <summary>
    /// This code defines a JukaEnvironment class that represents an environment in which variables can be defined, assigned, and accessed. It allows for nested environments where variables can be inherited from enclosing scopes.
    /// The class maintains a dictionary of variable names and values
    /// It has methods to get, assign, and define variables.
    /// It supports nested environments by having a reference to an enclosing environment.
    /// The Get method retrieves the value of a variable by name.
    /// The Assign method sets the value of a variable.
    /// The Define method adds a new variable to the environment.
    /// The Ancestor method finds an ancestor environment at a specified distance.
    /// The GetAt method retrieves the value of a variable at a specified distance.
    /// The AssignAt method sets the value of a variable at a specified distance.
    /// The ToString method provides a string representation of the environment, including variable values and its enclosing environment.
    /// </summary>
    internal class JukaEnvironment
    {
        private JukaEnvironment? enclosing;
        private Dictionary<string, object?> values = [];
        
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

            throw new ArgumentException("'" + name.ToString() + "' variable is undefined. (Error at JukaEnvironment.Get(): undefined variable)");
        }

        internal void Assign(Lexeme? name, object? value)
        {
            //?? throw new JRuntimeException("unable to get variable name: " + name)

            if (name != null)
            {
                if (enclosing != null)
                {
                    enclosing.Assign(name, value);
                    return;
                }

                if (values.ContainsKey(name.ToString()))
                {
                    values[name.ToString()] = value;
                    return;
                }
                else
                {
                    //DEFINE IF VARIABLE DOES NOT EXIST
                    values[name.ToString()] = value;
                    return;
                }
            }
            else
            {
                throw new JRuntimeException("Name of variable cannot be blank. Something went wrong!");
            }
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
