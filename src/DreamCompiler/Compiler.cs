//java org.antlr.v4.Tool -Dlanguage= CSharp DreamGrammar.g4 -no-listener

using DreamCompiler.RoslynCompile;
using DreamCompiler.Visitors;

namespace DreamCompiler
{
    using DreamCompiler.Grammar;
    using Antlr4.Runtime;
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
        private DreamGrammarParser parser;

        public CSharpSyntaxNode Go(String ouputFileName, MemoryStream memoryStream)
        {
            if (SetupAntlr(memoryStream))
            {
                var code = BeginVisitation();

                if (code != null)
                {
                    CompileRoslyn.CompileSyntaxTree(code.SyntaxTree, ouputFileName);
                    return code;
                }
            }

            throw new Exception("Unable to compile");
        }

        private bool SetupAntlr(MemoryStream stream)
        {
            var input = new AntlrInputStream(stream);
            var lexer = new DreamGrammarLexer(input);
            var tokenStream = new CommonTokenStream(lexer);
            this.parser = new DreamGrammarParser(tokenStream);

            if (this.parser == null)
            {
                throw new ArgumentException( "The parser was not created" );
            }

            return true;
        }

        private CSharpSyntaxNode BeginVisitation()
        {
            DreamGrammarParser.CompileUnitContext compileUnit = parser.compileUnit();
            Trace.WriteLine(compileUnit.ToStringTree(parser));

            //var visitor = new DreamVisitor();
            var visitor = new DreamRoslynVisitor();
            return visitor.Visit(compileUnit);
        }
    }
}

