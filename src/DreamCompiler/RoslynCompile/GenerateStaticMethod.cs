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
        public MethodDeclarationSyntax CreateClass(String methodName)
        {
            IdentifierNameSyntax name = SyntaxFactory.IdentifierName(methodName);
            TypeDeclarationSyntax returnType = SyntaxFactory.TypeDeclaration(SyntaxKind.VoidKeyword, "void");
            //SyntaxFactory.MethodDeclaration(TypeSyntax)
            return null;
        }
    }
}
