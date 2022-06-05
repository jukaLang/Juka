using JukaCompiler.Interpreter;
using JukaCompiler.Parse;
using JukaCompiler.Scan;
using JukaCompiler.Statements;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using JukaCompiler.Exceptions;
using JukaCompiler.SystemCalls;
//using JukaCompiler.RoslynEmiter;

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

