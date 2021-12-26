using JukaCompiler.Scan;
using JukaCompiler.Parse;
using JukaCompiler.Interpreter;
using JukaCompiler.Statements;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace JukaCompiler
{
    public class Compiler
    {
        public string Go(String ouputFileName, String path)
        {
            try
            {
                Parser parser = new(new Scanner(path));
                List<Stmt> statements = parser.Parse();

                var interpreter = new Interpreter.Interpreter();
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

