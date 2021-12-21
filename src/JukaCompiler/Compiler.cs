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
                Parser parser = new(new Scanner(path));
                List<Stmt> statements = parser.Parse();

                Resolver? resolver = new(new Interpreter.Interpreter());
                resolver.Resolve(statements);

                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

