//java org.antlr.v4.Tool -Dlanguage= CSharp DreamGrammar.g4 -no-listener

using System.Runtime.Remoting.Metadata.W3cXsd2001;
using DreamCompiler.Visitors;

namespace DreamCompiler
{
    using DreamCompiler.Grammar;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using Antlr4.Runtime.Misc;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq.Expressions;

    interface ICompilerInterface
    {
        void Go(MemoryStream stream);
    }

    public class Compiler : ICompilerInterface
    {
        private DreamGrammarParser parser;

        public void Go(MemoryStream memoryStream)
        {
            if (SetupAntlr(memoryStream))
            {
                BeginVisitation();
            }
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

        private void BeginVisitation()
        {
            DreamGrammarParser.CompileUnitContext compileUnit = parser.compileUnit();
            Trace.WriteLine(compileUnit.ToStringTree(parser));

            var visitor = new DreamVisitor();
            Expression expressionTree = visitor.Visit(compileUnit);

            if (expressionTree == null)
            {
                throw new Exception("tree is null");
            }

            if (expressionTree is BlockExpression)
            {
                Expression.Lambda<Action>(expressionTree).Compile()();
            }
        }
    }
}

