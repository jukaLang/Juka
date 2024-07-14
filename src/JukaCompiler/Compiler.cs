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
    /// <summary>
    /// Represents the Compiler class which is responsible for compiling Juka code.
    /// </summary>
    public class Compiler
    {
        private ServiceProvider _serviceProvider;
        private HostBuilder _hostBuilder;

        public Compiler()
        {
            _hostBuilder = new HostBuilder();
            _hostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<ICompilerError, CompilerError>();
                services.AddSingleton<IJukaCallable, JukaSystemCalls>();
                services.AddSingleton<IFileOpener, FileOpen>();
                services.AddSingleton<ICSharp, CSharp>();
                services.AddSingleton<ISystemClock, SystemClock>();
                services.AddSingleton<IGetAvailableMemory, GetAvailableMemory>();
                _serviceProvider = services.BuildServiceProvider();
            });
            _hostBuilder.Build();
        }

        public string CompileJukaCode(string data, bool isFile = true)
        {
            CheckServiceProvider();

            try
            {
                _serviceProvider.GetRequiredService<ICompilerError>().SourceFileName(data);

                Parser parser = new(new Scanner(data, _serviceProvider, isFile), _serviceProvider);
                List<Statement> statements = parser.Parse();

                return Compile(statements);
            }
            catch (Exception ex)
            {
                return $"Error compiling {ex.Message}";
            }
        }

        private string Compile(List<Statement> statements)
        {
            JukaInterpreter interpreter = new(services: _serviceProvider);
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

        private static void SetupMainMethodRuntimeHook(List<Statement> statements, Resolver resolver)
        {
            Statement.Function mainFunction = statements.OfType<Statement.Function>().FirstOrDefault(f => f.StmtLexemeName.Equals("main")) ?? throw new Exception("No main function is defined");
            Lexeme lexeme = new(LexemeType.Types.IDENTIFIER, 0, 0);
            lexeme.AddToken("main");
            Expr.Variable subroutineName = new(lexeme);
            Expr.Call call = new(subroutineName, false, []);
            Statement.Expression expression = new(call);
            resolver.Resolve([expression]);
        }

        public bool CheckForErrors()
        {
            CheckServiceProvider();
            return _serviceProvider.GetRequiredService<ICompilerError>().HasErrors();
        }

        public List<string> GetErrorList()
        {
            CheckServiceProvider();
            return _serviceProvider.GetRequiredService<ICompilerError>().ListErrors();
        }

        private void CheckServiceProvider()
        {
            if (_serviceProvider == null)
            {
                throw new JRuntimeException("Service provider is not created");
            }
        }
    }
    public class CompilerException(string message, Exception innerException) : Exception(message, innerException)
    {
    }
}


