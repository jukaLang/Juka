//java org.antlr.v4.Tool -Dlanguage= CSharp DreamGrammar.g4 -no-listener

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


    public class Compiler
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
            visitor.Visit(compileUnit);
        }
    }
}

