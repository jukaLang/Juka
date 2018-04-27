using System;
using System.Collections.Generic;
using System.Text;
using DreamCompiler.Tokens;


namespace DreamCompiler.Visitors
{
    using DreamCompiler.Grammar;

    class DreamVisitor : DreamGrammarBaseVisitor<Token>
    {
        public override Token VisitCompileUnit(DreamGrammarParser.CompileUnitContext context)
        {
            return base.VisitCompileUnit(context);
        }
    }
}
