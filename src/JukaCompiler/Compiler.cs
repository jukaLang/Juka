using JukaCompiler.Scan;
using JukaCompiler.Parse;
using JukaCompiler.Interpreter;
using JukaCompiler.Statements;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace JukaCompiler
{
    public class Compiler
    {
        public string Go(String ouputFileName, String path)
        {
            try
            {
                Parser parser = new Parser(new Scanner(path));
                List<Stmt> statements = parser.Parse();

                var interpreter = new Intepreter();
                interpreter.Interpert(statements);

                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

