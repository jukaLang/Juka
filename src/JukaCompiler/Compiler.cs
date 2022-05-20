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
    public class Compiler
    {
        private ServiceProvider serviceProvider;
        // Creates Compiler Instance (Step: 1)
        public Compiler()
        {
            Initialize();
            if (serviceProvider == null)
            {
                throw new ArgumentException("unable to init host builder");
            }
        }

        // Create DI/Host Builder (Step: 2)
        public void Initialize()
        {
            var hostBuilder = new HostBuilder();
            hostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<ICompilerError,CompilerError>();
                services.AddSingleton<IFileOpen, FileOpen>();
                services.AddSingleton<ISystemClock, SystemClock>();
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
                    return "ParserError";
                }

                return Compile(statements);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private string Compile(List<Stmt> statements)
        {
            //RoslynGenerate roslynGenerate = new();
            
            // Don't turn on until ready to emit Roslyn.
            // roslynGenerate.Generate(statements);

            var interpreter = new Interpreter.JukaInterpreter(serviceProvider);
            Resolver? resolver = new(interpreter);
            resolver.Resolve(statements);

            var currentOut = Console.Out;


            // Action<Interpreter.Interpreter, List<Stmt>> wrap;

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

        //private void WrapCompilerOutputInMemoryStream(Action<Interpreter.Interpreter, List<Stmt>> wrap)
        //{
        //    wrap();

        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        StreamWriter writer = new StreamWriter(stream);
        //        Console.SetOut(writer);

        //        interpreter.Interpert(statements);

        //        // Console.WriteLine("this is a test");    

        //        writer.Flush();
        //        writer.Close();
        //        var byteArray = stream.GetBuffer();
        //        Console.SetOut(currentOut);
        //        return Encoding.UTF8.GetString(byteArray);
        //    }
        //}
    }
}

