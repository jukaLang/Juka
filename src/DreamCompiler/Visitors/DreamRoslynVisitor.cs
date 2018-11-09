using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DReAMCompiler.Visitors
{
    using DReAMCompiler.Grammar;
    using DReAMCompiler.RoslynCompile;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;

    class DreamRoslynVisitor : DReAMGrammarBaseVisitor<CSharpSyntaxNode>
    {
        public override CSharpSyntaxNode VisitCompileUnit([NotNull] DReAMGrammarParser.CompileUnitContext context)
        {
            CSharpSyntaxNode tree = base.VisitCompileUnit(context);

            MethodDeclarationSyntax method = GenerateStaticMethod.CreateClass("Main");

            ClassDeclarationSyntax classDeclarationSyntax = SyntaxFactory.ClassDeclaration("tempClass")
                .AddMembers(items: new[] { method });      
            
            NamespaceDeclarationSyntax namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("tempoary"))
                .AddMembers(items: new[] { classDeclarationSyntax });

            return SyntaxFactory.CompilationUnit().AddMembers(items: new[] { namespaceDeclaration });
        }

        public override CSharpSyntaxNode VisitFunctionDeclaration([NotNull] DReAMGrammarParser.FunctionDeclarationContext context)
        {
            return base.VisitFunctionDeclaration(context);
        }

        public override CSharpSyntaxNode Visit(IParseTree tree)
        {
            return base.Visit(tree);
        }
    }
}
