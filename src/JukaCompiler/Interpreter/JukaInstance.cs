using JukaCompiler.Lexer;

namespace JukaCompiler.Interpreter
{
    internal class JukaInstance
    {
        private JukaClass Class;
        private Dictionary<string,object> fields = new Dictionary<string,object>();

        internal JukaInstance(JukaClass Class)
        {
            this.Class = Class;
        }

        internal object Get(Lexeme name)
        {
            if (fields.ContainsKey(name.ToString()))
            {
                return fields[name.ToString()];
            }

            JukaFunction? method = this.Class.FindMethod(name.ToString());

            if (method != null)
            {
                return method.Bind(this);
            }

            throw new ArgumentException("undefined property" + name.ToString());
        }

        internal void Set(Lexeme name, object value)
        {
            this.fields.Add(name.ToString(), value);
        }
    }
}
