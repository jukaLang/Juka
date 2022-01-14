namespace JukaCompiler.Interpreter
{
    internal class JukaClass : IJukaCallable
    {
        private string name;
        private JukaClass superClass;
        private Dictionary<string, JukaFunction> methods;

        internal JukaClass(string name, JukaClass superClass, Dictionary<string, JukaFunction> methods)
        {
            this.superClass = superClass;
            this.name = name;
            this.methods = methods;
        }

        public int Arity()
        {
            JukaFunction? initializer = FindMethod("main");
            if (initializer == null)
            {
                return 0;
            }

            return initializer.Arity();
        }

        public object Call(JukaInterpreter interpreter, List<object> arguments)
        {
            JukaInstance instance = new JukaInstance(this);
            JukaFunction? initializer = FindMethod("main");
            if (initializer != null)
            {
                initializer.Bind(instance).Call(interpreter, arguments);
            }

            return instance;
        }

        internal JukaFunction? FindMethod(string name)
        {
            if (methods.ContainsKey(name))
            {
                return methods[name];
            }

            if (superClass != null)
            {
                return superClass.FindMethod(name);
            }

            return new JukaFunction();
        }
    }
}
