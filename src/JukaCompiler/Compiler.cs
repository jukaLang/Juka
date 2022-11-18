using JukaCompiler.Interpreter;
using JukaCompiler.Parse;
using JukaCompiler.Scan;
using JukaCompiler.Statements;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using JukaCompiler.Exceptions;
using JukaCompiler.Expressions;
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
        private ServiceProvider? serviceProvider = null;

        public Compiler()
        {
            Initialize();
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
                var provider = this.serviceProvider;
                if (provider != null)
                {
                    provider.GetRequiredService<ICompilerError>().SourceFileName(data);

                    Parser parser = new(new Scanner(data, provider, isFile), provider);
                    List<Stmt> statements = parser.Parse();

                    return Compile(statements);
                }
            }
            catch (Exception ex)
            {
                return $"Error compiling {ex.Message}";
            }

            throw new Exception("unhandled errors");
        }

        private string Compile(List<Stmt> statements)
        {
            if (serviceProvider != null)
            {
                var interpreter = new JukaInterpreter(serviceProvider);
                Resolver? resolver = new(interpreter);
                resolver.Resolve(statements);

                SetupMainMethodRuntimeHook(statements, resolver);

                var currentOut = Console.Out;

                try
                {
                    using StringWriter stringWriter = new();
                    Console.SetOut(stringWriter);
                    interpreter.Interpret(statements);

                    string consoleOutput = stringWriter.ToString();
                    Console.SetOut(currentOut);
                    return consoleOutput;
                }
                catch (Exception e)
                {
                    Console.SetOut(currentOut);
                    return e.ToString();
                }
            }

            throw new JRuntimeException("Service provider is not created");
        }

        private static void SetupMainMethodRuntimeHook(List<Stmt> statements, Resolver resolver)
        {
            var allFunctions = statements.Where(e => e is Stmt.Function == true).ToList();

            foreach (var m in allFunctions)
            {
                if (((Stmt.Function)m).StmtLexemeName.Equals("main"))
                {
                    break;
                }
                else
                {
                    continue;
                }

                throw new Exception("No main function is defined");
            }

            Lexeme? lexeme = new(LexemeType.IDENTIFIER, 0, 0);
            lexeme.AddToken("main");
            Expr.Variable functionName = new(lexeme);
            Expr.Call call = new(functionName, false, new List<Expr>());
            Stmt.Expression expression = new(call);
            resolver.Resolve(new List<Stmt>() { expression });
        }

        public bool HasErrors()
        {
            var provider = this.serviceProvider;
            return provider != null && provider.GetRequiredService<ICompilerError>().HasErrors();
        }

        public List<String> ListErrors()
        {
            var provider = this.serviceProvider;
            if (provider != null)
            {
                return provider.GetRequiredService<ICompilerError>().ListErrors();
            }

            throw new JRuntimeException("unable to initialize provider for errors");
        }

    }
}

