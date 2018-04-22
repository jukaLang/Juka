//java org.antlr.v4.Tool -Dlanguage= CSharp DreamGrammar.g4 -no-listener

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
        public void Go(MemoryStream memoryStream)
        {
            var input = new AntlrInputStream(memoryStream);
            var lexer = new DreamGrammarLexer(input);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new DreamGrammarParser(tokenStream);

            DreamGrammarParser.CompileUnitContext compileUnit = parser.compileUnit();
            Trace.WriteLine(compileUnit.ToStringTree( parser ));

            Visitor visitor = new Visitor();
            visitor.Visit(compileUnit);

        }
    }


    public class Visitor : DreamGrammarBaseVisitor<String>
    {
        public override string VisitCompileUnit(DreamGrammarParser.CompileUnitContext context)
        {
            return base.VisitCompileUnit(context);
        }
    }
}

