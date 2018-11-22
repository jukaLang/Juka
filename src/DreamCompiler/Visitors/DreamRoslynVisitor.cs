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

            var parameters = new List<ExpressionSyntax>();

            for (int i = 2; i < context.ChildCount -1; i++)
            {
                parameters.Add((ExpressionSyntax)context.children[i].Accept(this));
            }

            var builtinFunctions = new List<String>() { "print", "printLine", "read", "readLine" };

            String funcName = context.funcName().GetText();

            if (builtinFunctions.Contains(funcName))
            {
                CSharpSyntaxNode builtInFunctionNode = GetBuiltInFunction(
                    funcName,
                    parameters);

                if (builtInFunctionNode != null)
                {
                    return builtInFunctionNode;
                }
            }

            return SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.IdentifierName(funcName)));

        }


        private CSharpSyntaxNode GetBuiltInFunction(String funcName, List<ExpressionSyntax> parameters)
        {
            var methodMap = new Dictionary<String, String>() { { "print", "Write" }, { "printLine", "WriteLine" } };

            String mappedFunc = methodMap[funcName];

            if (mappedFunc.Equals(String.Empty))
            {
                return null;
            }

            var argument = parameters[0];

            return SyntaxFactory.ExpressionStatement(
              SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("System"),
                                SyntaxFactory.IdentifierName("Console")),
                                SyntaxFactory.IdentifierName(mappedFunc)))
                                .WithArgumentList(
                                    SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                    SyntaxFactory.Argument(argument)))));
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
            foreach (IParseTree tree in context.children)
            {
                var syntaxNode = tree.Accept(this);
                if (syntaxNode is StatementSyntax)
                {
                    return syntaxNode;
                }
            }

            return null;
        }

        public override CSharpSyntaxNode VisitStringValue([NotNull] DReAMGrammarParser.StringValueContext context)
        {
            return SyntaxFactory.LiteralExpression(
                                   SyntaxKind.StringLiteralExpression,
                                   SyntaxFactory.Literal(context.STRING().GetText()));
        }

        public override CSharpSyntaxNode VisitVariableDeclarationExpression([NotNull] DReAMGrammarParser.VariableDeclarationExpressionContext context)
        {
            String variableName = context.variable().GetText();
  
            var expressions = context.singleExpression();
            var nodeList = new List<CSharpSyntaxNode>();

            foreach (var expression in expressions)
            {
                nodeList.Add(expression.Accept(this));
            }

            var keywordToken = GetKeywordTokenType(context.keywords().GetText());
            var binaryExpression = nodeList[0] ?? nodeList[0] as BinaryExpressionSyntax;

            return SyntaxFactory.LocalDeclarationStatement(
            SyntaxFactory.VariableDeclaration(
                SyntaxFactory.PredefinedType( 
                    SyntaxFactory.Token(SyntaxKind.IntKeyword)))
            .WithVariables(
                SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                    SyntaxFactory.VariableDeclarator(
                        SyntaxFactory.Identifier(variableName))
                    .WithInitializer(
                        SyntaxFactory.EqualsValueClause(
                           (BinaryExpressionSyntax)binaryExpression)))));
        }

        private SyntaxToken GetKeywordTokenType(String keyword)
        {
            switch (keyword)
            {
                case "int":
                    return SyntaxFactory.Token(SyntaxKind.IntKeyword);
                case "string":
                    break;
                case "double":
                    break;
            }

            return SyntaxFactory.Token(SyntaxKind.ErrorKeyword);
        }
       
        public override CSharpSyntaxNode VisitBinaryExpression([NotNull] DReAMGrammarParser.BinaryExpressionContext context)
        {
            var nodeList = new List<CSharpSyntaxNode>();

            foreach(var expression in context.singleExpression())
            {
                nodeList.Add(expression.Accept(this));
            }

            String unaryOp = context.unaryOperator().GetText();

            var expressionArray = nodeList.ToArray();
            var left = (ExpressionSyntax)expressionArray[0];
            var right = (ExpressionSyntax)expressionArray[1];

            if (unaryOp.Equals("+") || unaryOp.Equals("*"))
            {
                SyntaxKind kind = unaryOp.Equals("+") ? SyntaxKind.AddExpression : SyntaxKind.MultiplyExpression;
                return SyntaxFactory.BinaryExpression(kind, left, right);
            }

            if (unaryOp.Equals("-") || unaryOp.Equals("/"))
            {
                SyntaxKind kind = unaryOp.Equals("-") ? SyntaxKind.SubtractExpression : SyntaxKind.DivideExpression;
                return SyntaxFactory.BinaryExpression(kind, left, right);
            }

            throw new Exception("Invalid expression");
        }

        public override CSharpSyntaxNode VisitDecimalValue([NotNull] DReAMGrammarParser.DecimalValueContext context)
        {
            try
            {
                return GetNumericAsNode(context.GetText(), NumberValueEnum.jinteger);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        enum NumberValueEnum
        {
            jinteger,
            jdouble,
            jlong,
            jfloat,
        }

        private CSharpSyntaxNode GetNumericAsNode([NotNull]String numberValue, NumberValueEnum type)
        {
            CSharpSyntaxNode node = null;
            switch (type)
            {
                case NumberValueEnum.jinteger:
                    int intergerValue = int.Parse(numberValue);

                    return SyntaxFactory.LiteralExpression(
                                    SyntaxKind.NumericLiteralExpression,
                                    SyntaxFactory.Literal(intergerValue));
                case NumberValueEnum.jlong:
                    long longValue = long.Parse(numberValue);

                    return SyntaxFactory.LiteralExpression(
                                    SyntaxKind.NumericLiteralExpression,
                                    SyntaxFactory.Literal(longValue));
                case NumberValueEnum.jdouble:
                    double doubleValue = double.Parse(numberValue);

                    return SyntaxFactory.LiteralExpression(
                                    SyntaxKind.NumericLiteralExpression,
                                    SyntaxFactory.Literal(doubleValue));
                case NumberValueEnum.jfloat:
                    float floatValue = float.Parse(numberValue);

                    return SyntaxFactory.LiteralExpression(
                                    SyntaxKind.NumericLiteralExpression,
                                    SyntaxFactory.Literal(floatValue));
            }

            throw new InvalidCastException("Can't parse types");
        }

        public override CSharpSyntaxNode VisitIntValue([NotNull] DReAMGrammarParser.IntValueContext context)
        {
            return SyntaxFactory.LiteralExpression(SyntaxKind.DecimalKeyword, SyntaxFactory.Literal(int.Parse(context.GetText())));
        }

        public override CSharpSyntaxNode VisitLiteral([NotNull] DReAMGrammarParser.LiteralContext context)
        {
            var literal =  SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(context.GetText()));
            return literal;
        }

        public override CSharpSyntaxNode VisitNumericTypes([NotNull] DReAMGrammarParser.NumericTypesContext context)
        {
            return base.VisitNumericTypes(context);
        }

        public override CSharpSyntaxNode Visit(IParseTree tree)
        {
            return base.Visit(tree);
        }
    }
}
