//java org.antlr.v4.Tool -Dlanguage= CSharp DreamGrammar.g4 -no-listener

using DreamCompiler.RoslynCompile;
using DreamCompiler.Visitors;

namespace DreamCompiler
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq.Expressions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    interface ICompilerInterface
    {
        CSharpSyntaxNode Go(String ouputFileName, MemoryStream stream);
    }

    public class Compiler
    {
        public CSharpSyntaxNode Go(String ouputFileName, MemoryStream memoryStream)
        {
            try
            {
                Console.WriteLine("starting the compiler...");
                CompileRoslyn.CompileSyntaxTree(null, ouputFileName);
                return null;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}

