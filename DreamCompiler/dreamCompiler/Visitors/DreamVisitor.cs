using System;
using System.Collections.Generic;
using System.Text;


namespace DreamCompiler.Visitors
{
    using DreamCompiler.Grammar;
    using DreamCompiler.token;

    class DreamVisitor : DreamGrammarBaseVisitor<Token>
    {
        public override Token VisitCompileUnit(DreamGrammarParser.CompileUnitContext context)
        {
            return base.VisitCompileUnit(context);
        }
    }
}
