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

            /**
             * A name space and a class needs to be defined to hold all of the methods. 
             */         
            NamespaceDeclarationSyntax namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("tempoary"))
                .AddMembers(items: new[] { SyntaxFactory.ClassDeclaration("tempClass")
                .AddMembers(items: methods.ToArray()) });

            return SyntaxFactory.CompilationUnit().AddMembers(items: new[] { namespaceDeclaration });
        }

        public override CSharpSyntaxNode VisitFunctionDeclaration([NotNull] DReAMGrammarParser.FunctionDeclarationContext context)
        {
            MethodDeclarationSyntax method = GenerateStaticMethod.CreateStaticMethod( context , this);
            return method;
        }

        /*
        public override CSharpSyntaxNode VisitAssignmentExpression([NotNull] DReAMGrammarParser.AssignmentExpressionContext context)
        {
            return base.VisitAssignmentExpression(context);
        }
        */

        public override CSharpSyntaxNode VisitFunctionCall([NotNull] DReAMGrammarParser.FunctionCallContext context)
        {
            SyntaxToken name = SyntaxFactory.Identifier(context.funcName().GetText());
            SyntaxToken returnType = SyntaxFactory.Token(SyntaxKind.VoidKeyword);

            return SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.IdentifierName("foo")));

        }

        public override CSharpSyntaxNode VisitFunctionCallExpression([NotNull] DReAMGrammarParser.FunctionCallExpressionContext context)
        {
            var node = base.VisitFunctionCallExpression(context);
            return node;
        }

        public override CSharpSyntaxNode VisitStatement([NotNull] DReAMGrammarParser.StatementContext context)
        {
            var node = base.VisitStatement(context);
            return node;
        }

        public override CSharpSyntaxNode VisitExpression([NotNull] DReAMGrammarParser.ExpressionContext context)
        {
            List<CSharpSyntaxNode> list = new List<CSharpSyntaxNode>();
            foreach(IParseTree tree in context.children)
            {
                var syntaxNode = tree.Accept(this);
                if (syntaxNode is ExpressionStatementSyntax)
                {
                    return syntaxNode;
                }
            }

            return null;
        }

        public override CSharpSyntaxNode Visit(IParseTree tree)
        {
            return base.Visit(tree);
        }
    }
}
