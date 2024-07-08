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
    public class Compiler
    {
        private ServiceProvider serviceProvider;
        private HostBuilder hostBuilder;


        public Compiler()
        {
            hostBuilder = new HostBuilder();
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


        public string Go(string data, bool isFile = true)
        {
            CheckServiceProvider();

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

            TextWriter currentOut = Console.Out;

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
            Stmt.Function mainFunction = statements.OfType<Stmt.Function>().FirstOrDefault(f => f.StmtLexemeName.Equals("main")) ?? throw new Exception("No main function is defined");
            Lexeme lexeme = new(LexemeType.Types.IDENTIFIER, 0, 0);
            lexeme.AddToken("main");
            Expr.Variable subroutineName = new(lexeme);
            Expr.Call call = new(subroutineName, false, []);
            Stmt.Expression expression = new(call);
            resolver.Resolve([expression]);
        }

        public bool HasErrors()
        {
            CheckServiceProvider();
            return serviceProvider.GetRequiredService<ICompilerError>().HasErrors();
        }

        public List<string> ListErrors()
        {
            CheckServiceProvider();
            return serviceProvider.GetRequiredService<ICompilerError>().ListErrors();
        }

        private void CheckServiceProvider()
        {
            if (serviceProvider == null)
            {
                throw new JRuntimeException("Service provider is not created");
            }
        }
    }
    public class CompilerException(string message, Exception innerException) : Exception(message, innerException)
    {
    }
}


