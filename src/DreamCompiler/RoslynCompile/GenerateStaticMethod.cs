using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DReAMCompiler.RoslynCompile
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis;

    class GenerateStaticMethod
    {
        static public MethodDeclarationSyntax CreateDefaultMainMethod(String methodName)
        {
            if (methodName.Equals("main"))
            {
                methodName = "Main";
            }
            SyntaxToken name = SyntaxFactory.Identifier(methodName);
            SyntaxToken returnType = SyntaxFactory.Token(SyntaxKind.VoidKeyword);

            return SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(returnType), name)
                .WithBody(
                    SyntaxFactory.Block())
                     .WithModifiers(
            SyntaxFactory.TokenList(
                new[]{
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)}));
        }
    }
}
