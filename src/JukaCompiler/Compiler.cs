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
using System.Linq;

namespace JukaCompiler
{
    /*
     * Main entry point into the compiler responsible for setting up DI container 
     * and calling parser and compiler (currently interpreter).
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
                services.AddSingleton<ICompilerError, CompilerError>();
                services.AddSingleton<IJukaCallable, JukaSystemCalls>();
                services.AddSingleton<IFileOpen, FileOpen>();
                services.AddSingleton<ICSharp, CSharp>();
                services.AddSingleton<ISystemClock, SystemClock>();
                services.AddSingleton<IGetAvailableMemory, GetAvailableMemory>();
                this.serviceProvider = services.BuildServiceProvider();
            });
            hostBuilder.Build();
        }

        // Run the Compiler (Step: 3)
        public string Go(string data, bool isFile = true)
        {
            if (serviceProvider == null)
            {
                throw new JRuntimeException("Service provider is not created");
            }

            try
            {
                serviceProvider.GetRequiredService<ICompilerError>().SourceFileName(data);

                Parser parser = new(new Scanner(data, serviceProvider, isFile), serviceProvider);
                List<Stmt> statements = parser.Parse();

                return Compile(statements);
            }
            catch (Exception ex)
            {
                return $"Error compiling {ex.Message}";
            }
        }

        private string Compile(List<Stmt> statements)
        {
            JukaInterpreter interpreter = new(services: serviceProvider);
            Resolver resolver = new(interpreter);
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

        private static void SetupMainMethodRuntimeHook(List<Stmt> statements, Resolver resolver)
        {
            var mainFunction = statements.OfType<Stmt.Function>().FirstOrDefault(f => f.StmtLexemeName.Equals("main")) ?? throw new Exception("No main function is defined");
            Lexeme lexeme = new(LexemeType.Types.IDENTIFIER, 0, 0);
            lexeme.AddToken("main");
            Expr.Variable functionName = new(lexeme);
            Expr.Call call = new(functionName, false, []);
            Stmt.Expression expression = new(call);
            resolver.Resolve([expression]);
        }

        public bool HasErrors()
        {
            return serviceProvider != null && serviceProvider.GetRequiredService<ICompilerError>().HasErrors();
        }

        public List<string> ListErrors()
        {
            if (serviceProvider == null)
            {
                throw new JRuntimeException("Unable to initialize provider for errors");
            }

            return serviceProvider.GetRequiredService<ICompilerError>().ListErrors();
        }
    }
}
