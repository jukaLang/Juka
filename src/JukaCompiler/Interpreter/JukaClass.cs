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

        public object? Call(string methodName, JukaInterpreter interpreter, List<object?> arguments)
        {
            // FIND METHOD is broken
            // Declaration is never set correctly.
            JukaInstance? instance = new(this);
            JukaFunction? initializer = FindMethod("main");
            initializer?.Bind(instance).Call(methodName, interpreter, arguments);

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

            return null;
        }
    }
}
