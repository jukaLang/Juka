using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DReAMCompiler.RoslynCompile
{
    using DReAMCompiler.Grammar;
    using DReAMCompiler.RoslynCompile;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis;
    using DReAMCompiler.Visitors;
    using Antlr4.Runtime.Tree;

    class GenerateStaticMethod
    {
        static public MethodDeclarationSyntax CreateStaticMethod(DReAMGrammarParser.FunctionDeclarationContext context, DreamRoslynVisitor visitor)
        {
            String methodName = context.funcName().GetText();
            //Since C# main methods musbe be named Main this converts to C# style.
            if (methodName.Equals("main"))
            {
                methodName = "Main";
            }

            SyntaxToken name = SyntaxFactory.Identifier(methodName);
            SyntaxToken returnType = SyntaxFactory.Token(SyntaxKind.VoidKeyword);

            List<StatementSyntax> statementList = new List<StatementSyntax>();

            foreach (IParseTree child in context.children)
            {
                CSharpSyntaxNode node = child.Accept(visitor);
                if (node is StatementSyntax)
                {
                    statementList.Add(node as StatementSyntax);
                }
            }

            var block = SyntaxFactory.Block().AddStatements(statementList.ToArray());

            return SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(returnType), name)
                .WithBody(
                    block)
                     .WithModifiers(
            SyntaxFactory.TokenList(
                new[]{
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)}));
        }
    }
}
