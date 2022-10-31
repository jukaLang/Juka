using JukaCompiler.Exceptions;
using JukaCompiler.Statements;
using System.Reflection;

namespace JukaCompiler.Interpreter
{
    internal class JukaFunction : IJukaCallable
    {
        private Stmt.Function? declaration;
        private JukaEnvironment? closure;
        private bool isInitializer;

        internal JukaFunction(Stmt.Function declaration, JukaEnvironment closure, bool isInitializer)
        {
            this.isInitializer = isInitializer;
            this.declaration = declaration;
            this.closure = closure;
        }

        internal JukaFunction Bind(JukaInstance? instance)
        {
            if (closure != null)
            {
                JukaEnvironment env = new(closure);
                env.Define("this", instance);

                if (declaration != null)
                {
                    return new JukaFunction(declaration, env, isInitializer);
                }
            }

            throw new JRuntimeException("Unable to bind");
        }

        internal Stmt.Function? Declaration => declaration;

        internal JukaEnvironment? Closure => closure;


        public int Arity()
        {
            if(declaration == null)
            {
                return 0;
            }

            return declaration.Params.Count;
        }

        public object? Call(string methodName, JukaInterpreter interpreter, List<object?> arguments)
        {
            if (closure != null)
            {
                JukaEnvironment environment = new(closure);

                if (declaration != null)
                {
                    for (int i = 0; i < declaration.Params.Count; i++)
                    {
                        var parameterNameExpressionLexeme = declaration.Params[i].parameterName.ExpressionLexeme;
                        if (parameterNameExpressionLexeme != null)
                        {
                            string? name = parameterNameExpressionLexeme.ToString();
                            if (string.IsNullOrEmpty(name))
                            {
                                throw new ArgumentException("Unable to call function");
                            }

                            object? value = arguments[i];
                            environment.Define(name, value);
                        }
                    }
                }

                try
                {
                    if (declaration?.body != null)
                    {
                        var body = declaration?.body!;
                        interpreter.ExecuteBlock(body, environment);
                    }
                }
                catch(Return ex)
                {
                    if (isInitializer)
                    {
                        return closure.GetAt(0, "this");
                    }

                    return ex.value;
                }
            }

            if (isInitializer)
            {
                return closure?.GetAt(0, "this");
            }

            return null;
        }
    }
}
