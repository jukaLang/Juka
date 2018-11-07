using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DReAMCompiler.Visitors
{
    using DReAMCompiler.Grammar;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;

    class DreamRoslynVisitor : DReAMGrammarBaseVisitor<SyntaxTree>
    {
        public override SyntaxTree VisitCompileUnit([NotNull] DReAMGrammarParser.CompileUnitContext context)
        {
            CompilationUnitSyntax syntax = SyntaxFactory.CompilationUnit();
            SyntaxTree tree =  base.VisitCompileUnit(context);

            syntax.add

            //syntax.AddMembers(items: new[] { tree });

            return syntax.SyntaxTree;
        }

        public override SyntaxTree Visit(IParseTree tree)
        {
            return base.Visit(tree);
        }
    }
}
