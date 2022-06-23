using JukaCompiler.Interpreter;
using JukaCompiler.Parse;
using JukaCompiler.Scan;
using JukaCompiler.Statements;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using JukaCompiler.Exceptions;
using JukaCompiler.SystemCalls;
using JukaCompiler.Lexer;


namespace JukaCompiler
{
    /*
     * Maing entry point into the compiler responsible for setting up DI container 
     * Calling parser and compiler (currently interpreter.
     */
    public class Compiler
    {
        private ServiceProvider serviceProvider;

        public Compiler()
        {
            Initialize();
            if (serviceProvider == null)
            {
                throw new ArgumentException("unable to init host builder");
            }
        }

        internal void Initialize()
        {
            var hostBuilder = new HostBuilder();
            hostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<ICompilerError,CompilerError>();
                services.AddSingleton<IJukaCallable, JukaSystemCalls>();
                services.AddSingleton<IFileOpen, FileOpen>();
                services.AddSingleton<ISystemClock, SystemClock>();
                services.AddSingleton<IGetAvailableMemory, GetAvailableMemory>();
                this.serviceProvider = services.BuildServiceProvider();
            });
            hostBuilder.Build();
        }

        // Run the Compiler (Step: 3)
        public string Go(String data, bool isFile = true)
        {
            try
            {
                this.serviceProvider.GetRequiredService<ICompilerError>().SourceFileName(data);

                Parser parser = new(new Scanner(data, this.serviceProvider, isFile), this.serviceProvider);
                List<Stmt> statements = parser.Parse();

                if (HasErrors())
                {
                    return String.Empty;
                }

                return Compile(statements);
            }
            catch (Exception ex)
            {
                return $"Error compiling {ex.Message}";
            }

            throw new Exception("unhandled errors");
        }

        private string Compile(List<Stmt> statements)
        {
            var interpreter = new Interpreter.JukaInterpreter(serviceProvider);
            Resolver? resolver = new(interpreter);
            resolver.Resolve(statements);

            SetupMainMethodRuntimeHook(statements, resolver);

            var currentOut = Console.Out;


            using (StringWriter stringWriter = new StringWriter())
            {
                Console.SetOut(stringWriter);
                interpreter.Interpert(statements);

                String ConsoleOutput = stringWriter.ToString();
                Console.SetOut(currentOut);

                return ConsoleOutput;
            }
        }

        private static void SetupMainMethodRuntimeHook(List<Stmt> statements, Resolver resolver)
        {
            var allFunctions = statements.Where(e => e is Stmt.Function == true).ToList();

            foreach (var m in allFunctions)
            {
                if (((Stmt.Function)m).name.ToString().Equals("main"))
                {
                    break;
                }
                else
                {
                    continue;
                }

                throw new Exception("No main function defined");
            }

            Lexeme? lexeme = new(LexemeType.IDENTIFIER, 0, 0);
            lexeme.AddToken("main");
            Expression.Variable functionName = new(lexeme);
            Expression.Call call = new(functionName, false, new List<Expression>());
            Stmt.Expression expression = new(call);
            resolver.Resolve(new List<Stmt>() { expression });
        }

        public bool HasErrors()
        {
            return this.serviceProvider.GetRequiredService<ICompilerError>().HasErrors();
        }

        public List<String> ListErrors()
        {
            return this.serviceProvider.GetRequiredService<ICompilerError>().ListErrors();
        }

    }
}

