using System;
using System.Collections.Generic;
using System.Linq;

namespace DReAMCompiler.Visitors
{
    using DReAMCompiler.Grammar;
    using DReAMCompiler.RoslynCompile;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using System.Diagnostics.Tracing;

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

            var parameters = new List<ArgumentSyntax>();

            for (int i = 2; i < context.ChildCount -1; i++)
            {
                var param = context.children[i].Accept(this);
                if (param != null)
                {
                    if (param is LiteralExpressionSyntax)
                    {
                        parameters.Add(SyntaxFactory.Argument(param as LiteralExpressionSyntax));
                        continue;
                    }

                    if (param is IdentifierNameSyntax)
                    {
                        parameters.Add(SyntaxFactory.Argument(param as IdentifierNameSyntax));
                        continue;
                    }

                    if (param is ArgumentSyntax)
                    {
                        parameters.Add(param as ArgumentSyntax);
                        continue;
                    }
                }
            }

            var builtinFunctions = new List<String>() { "print", "printLine", "read", "readLine" };

            String funcName = context.funcName().GetText();

            if (builtinFunctions.Contains(funcName))
            {
                return GenrateBuiltInFunction(funcName, parameters);
            }

            if (parameters.Count() > 0)
            {
                var seperatedList = BuildArgumentList(parameters);
                return SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.IdentifierName(funcName)).WithArgumentList(
                                SyntaxFactory.ArgumentList(BuildArgumentList(parameters))));
            }

            return SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.IdentifierName(funcName)));

        }

        private SeparatedSyntaxList<ArgumentSyntax> BuildArgumentList(List<ArgumentSyntax> parameters)
        {
            var list = SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                new SyntaxNodeOrToken[]{ parameters[0] });

            for (int i = 1; i < parameters.Count; i++)
            {
                list.Add(parameters[i]);
            }

            return list;
        }

        private CSharpSyntaxNode GenrateBuiltInFunction(String funcName, List<ArgumentSyntax> parameters)
        {
            CSharpSyntaxNode builtInFunctionNode = GetBuiltInFunction(
                funcName,
                parameters);

            if (builtInFunctionNode != null)
            {
                return builtInFunctionNode;
            }

            throw new ArgumentException("no built in function defined");
        }


        private CSharpSyntaxNode GetBuiltInFunction(String funcName, List<ArgumentSyntax> parameters)
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
                                    SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(argument))));
        }

        public override CSharpSyntaxNode VisitVariable([NotNull] DReAMGrammarParser.VariableContext context)
        {
            return SyntaxFactory.Argument(SyntaxFactory.IdentifierName(context.ID().GetText()));
            //return base.VisitVariable(context);
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
            String value = context.GetText();
            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value.Substring(1, value.Length - 2);
            }
            return SyntaxFactory.LiteralExpression(
                                   SyntaxKind.StringLiteralExpression,
                                   SyntaxFactory.Literal(value));
        }

        public override CSharpSyntaxNode VisitParameterVariableDeclaration([NotNull] DReAMGrammarParser.ParameterVariableDeclarationContext context)
        {
            var paramType = context.children[0].Accept(this) as PredefinedTypeSyntax;
            var paramName = SyntaxFactory.Identifier(context.children[1].GetText());

            return SyntaxFactory.Parameter(paramName).WithType(paramType);
        }
        
        public override CSharpSyntaxNode VisitKeywords([NotNull] DReAMGrammarParser.KeywordsContext context)
        {
            String keyword = context.GetText();
            switch (keyword)
            {
                case "int":
                    return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));
                case "string":
                    return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword));
            }

            throw new Exception("no valid keyword");
        }


        private List<IParseTree> WalkVariableDeclarationExpression([NotNull] IList<IParseTree> context)
        {
            foreach(var e in context)
            {
                var list = new List<IParseTree>();
                if (e.ChildCount > 1)
                {
                    return WalkVariableDeclarationExpression(GetChildren(e));
                }
            }

            return null;
        }

        private List<IParseTree> GetChildren(IParseTree tree)
        {
            List<IParseTree> children = new List<IParseTree>();

            for (int i = 0; i < tree.ChildCount; i++)
            {
                children.Add(tree.GetChild(i));
            }

            HashSet<int> t = new HashSet<int>();
            t.Add(2);

            return children;
        }


        public override CSharpSyntaxNode VisitVariableDeclarationExpression([NotNull] DReAMGrammarParser.VariableDeclarationExpressionContext context)
        {
            string variableName = context.variable().GetText();

            new GenerateBinaryExpression()
                .Walk(context)
                .PostWalk();
           
            var expressions = context.singleExpression();
            var nodeList = new List<CSharpSyntaxNode>();
            
            WalkVariableDeclarationExpression( new List<IParseTree>() { context.singleExpression() } );

            SyntaxToken keywordToken = GetKeywordTokenType(context.keywords().GetText());
            var binaryExpression = nodeList[0] ?? nodeList[0] as BinaryExpressionSyntax;

            return SyntaxFactory.LocalDeclarationStatement(
            SyntaxFactory.VariableDeclaration(
                SyntaxFactory.PredefinedType(keywordToken))
            .WithVariables(
                SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                    SyntaxFactory.VariableDeclarator(
                        SyntaxFactory.Identifier(variableName)) )));
            /*
                    .WithInitializer(
                        SyntaxFactory.EqualsValueClause(
                           (BinaryExpressionSyntax)binaryExpression)))));
            */
        }

        public override CSharpSyntaxNode VisitAssignmentOperator([NotNull] DReAMGrammarParser.AssignmentOperatorContext context)
        {
            return base.VisitAssignmentOperator(context);
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
            return GenerateBinaryExpression.CreateBinaryExpression(context, this);
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
