﻿using JukaCompiler.Interpreter;
using JukaCompiler.Parse;
using JukaCompiler.Scan;
using JukaCompiler.Statements;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using JukaCompiler.Exceptions;
using System.Net;

namespace JukaCompiler
{
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

        public void Initialize()
        {
            var hostBuilder = new HostBuilder();
            hostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<ICompilerError,CompilerError>();
                this.serviceProvider = services.BuildServiceProvider();
            });
            hostBuilder.Build();//.Run();
        }

        public string Go(String ouputFileName, String path)
        {
            try
            {
                Parser parser = new(new Scanner(path), this.serviceProvider);
                List<Stmt> statements = parser.Parse();

                if(HasErrors())
                {
                    return "Errors during compiling";
                }

                var interpreter = new Interpreter.Interpreter(serviceProvider);
                Resolver? resolver = new(interpreter);
                resolver.Resolve(statements);

                var currentOut = Console.Out;


                // Action<Interpreter.Interpreter, List<Stmt>> wrap;

                using (MemoryStream stream = new MemoryStream())
                { 
                    StreamWriter writer = new StreamWriter(stream);
                    Console.SetOut(writer);

                    interpreter.Interpert(statements);

                    // Console.WriteLine("this is a test");    
                    
                    writer.Flush();
                    writer.Close();
                    var byteArray = stream.GetBuffer();
                    Console.SetOut(currentOut);
                    return Encoding.UTF8.GetString(byteArray);
                }

                throw new Exception("Unhandled error");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public bool HasErrors()
        {
            return this.serviceProvider.GetRequiredService<ICompilerError>().HasErrors();
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

