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


        // Constructor for Compiler class that initializes service providers for error handling, system calls, file opening, C# operations, system clock, and available memory.
        // It creates a new HostBuilder instance, configures services by adding singletons for various interfaces like ICompilerError, IJukaCallable, IFileOpener, ICSharp, ISystemClock, and others,
        // and then builds a service provider to store these singletons. Finally, it builds the HostBuilder.
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


        // Compiles Juka code provided in 'data' and returns the console output after interpreting the statements or an error message if compilation fails.
        // CompileJukaCode that compiles Juka code provided in the data string. It uses a Parser to parse the code and then interprets the statements.
        // If successful, it returns the console output.
        // If there's an error during compilation, it returns an error message with details about the exception.
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


        // Compiles a list of statements using the JukaInterpreter and Resolver, then sets up a main method runtime hook.
        // Returns the console output after interpreting the statements or an exception message if an error occurs.
        //This code defines a method named Compile that compiles a list of statements using a JukaInterpreter and a Resolver.
        //It sets up a main method runtime hook, redirects console output to capture it, interprets the statements, and returns the console output as a string.
        //If an exception occurs during interpretation, it returns the exception message.

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


        /// <summary>
        /// This code snippet sets up a runtime hook for the main method by identifying the main function from a list of statements. 
        /// It then creates a call to the main function and resolves it using a resolver object. 
        /// If the main function is not found, it throws an exception indicating that no main function is defined.
        /// </summary>
        /// <param name="statements"></param>
        /// <param name="resolver"></param>
        /// <exception cref="Exception"></exception>
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


