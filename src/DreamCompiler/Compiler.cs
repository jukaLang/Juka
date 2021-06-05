//java org.antlr.v4.Tool -Dlanguage= CSharp DreamGrammar.g4 -no-listener

using DreamCompiler.RoslynCompile;

namespace DreamCompiler
{
    using System;
    using Microsoft.CodeAnalysis.CSharp;

    interface ICompilerInterface
    {
        String Go(String ouputFileName, String raw_string);
    }

    public class Compiler
    {
        public String Go(String ouputFileName, String raw_string)
        {
            try
            {

                Console.WriteLine("Starting the compiler...");

                var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(raw_string, null, "");
                return CompileRoslyn.CompileSyntaxTree(parsedSyntaxTree, ouputFileName);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}

