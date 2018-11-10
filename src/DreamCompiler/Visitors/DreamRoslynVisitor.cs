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
            List<MethodDeclarationSyntax> methods = new List<MethodDeclarationSyntax>();

            for(int i=0; i < context.ChildCount; i++)
            {
                MethodDeclarationSyntax m = context.children[i].Accept(this) as MethodDeclarationSyntax;
                if (m != null)
                {
                    methods.Add(m);
                }
            }

            ClassDeclarationSyntax classDeclarationSyntax = SyntaxFactory.ClassDeclaration("tempClass")
                .AddMembers(items: methods.ToArray());      
            
            NamespaceDeclarationSyntax namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("tempoary"))
                .AddMembers(items: new[] { classDeclarationSyntax });

            return SyntaxFactory.CompilationUnit().AddMembers(items: new[] { namespaceDeclaration });
        }

        public override CSharpSyntaxNode VisitFunctionDeclaration([NotNull] DReAMGrammarParser.FunctionDeclarationContext context)
        {
            MethodDeclarationSyntax method = GenerateStaticMethod.CreateDefaultMainMethod( context.funcName().GetText());
            base.VisitFunctionDeclaration(context);

            return method;
        }

        public override CSharpSyntaxNode Visit(IParseTree tree)
        {
            return base.Visit(tree);
        }
    }
}
