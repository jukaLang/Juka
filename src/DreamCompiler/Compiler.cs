//java org.antlr.v4.Tool -Dlanguage= CSharp DreamGrammar.g4 -no-listener

using DReAMCompiler.Visitors;

namespace DReAMCompiler
{
    using DReAMCompiler.Grammar;
    using Antlr4.Runtime;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq.Expressions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    interface ICompilerInterface
    {
        void Go(MemoryStream stream);
    }

    public class Compiler : ICompilerInterface
    {
        private DReAMGrammarParser parser;

        public void Go(MemoryStream memoryStream)
        {
            if (SetupAntlr(memoryStream))
            {
                var code = BeginVisitation();
                //Expression.Lambda<Action>(code).Compile()();
            }
        }

        private bool SetupAntlr(MemoryStream stream)
        {
            var input = new AntlrInputStream(stream);
            var lexer = new DReAMGrammarLexer(input);
            var tokenStream = new CommonTokenStream(lexer);
            this.parser = new DReAMGrammarParser(tokenStream);

            if (this.parser == null)
            {
                throw new ArgumentException( "The parser was not created" );
            }

            return true;
        }

        private SyntaxTree BeginVisitation()
        {
            DReAMGrammarParser.CompileUnitContext compileUnit = parser.compileUnit();
            Trace.WriteLine(compileUnit.ToStringTree(parser));

            //var visitor = new DReAMVisitor();
            var visitor = new DreamRoslynVisitor();
            SyntaxTree expressionTree = CSharpSyntaxTree.ParseText(
@"using System;
using System.Collections;
using System.Linq;
using System.Text;
 
namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(""Hello, World!"");
        }
    }
}");

            //visitor.Visit(compileUnit);

            return expressionTree;
        }
    }
}

