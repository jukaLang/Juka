using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Antlr4.Runtime.Tree;
using DreamCompiler.Tokens;
using System.Linq.Expressions;


namespace DreamCompiler.Visitors
{
    using DreamCompiler.Grammar;

    internal class foo : DreamGrammarBaseVisitor<Expression[]>
    {

    }

    internal class DreamVisitor : DreamGrammarBaseVisitor<Expression>
    {
        public override Expression VisitCompileUnit(DreamGrammarParser.CompileUnitContext context)
        {
            Expression compileUnit = base.VisitCompileUnit(context);
            return compileUnit;
        }

        public override Expression VisitStatement(DreamGrammarParser.StatementContext context)
        {
            return base.VisitStatement(context);
        }

        public override Expression VisitAdd(DreamGrammarParser.AddContext context)
        {
            return base.VisitAdd(context);
        }

        public override Expression VisitAssignmentExpression(DreamGrammarParser.AssignmentExpressionContext context)
        {
            return base.VisitAssignmentExpression(context);
        }

        public override Expression VisitAssignmentOperator(DreamGrammarParser.AssignmentOperatorContext context)
        {
            return base.VisitAssignmentOperator(context);
        }

        public override Expression VisitAssignmentOperatorExpression(DreamGrammarParser.AssignmentOperatorExpressionContext context)
        {
            return base.VisitAssignmentOperatorExpression(context);
        }

        public override Expression VisitBinaryExpression(DreamGrammarParser.BinaryExpressionContext context)
        {
            return base.VisitBinaryExpression(context);
        }

        public override Expression VisitBooleanExpression(DreamGrammarParser.BooleanExpressionContext context)
        {
            return base.VisitBooleanExpression(context);
        }

        public override Expression VisitClassifications(DreamGrammarParser.ClassificationsContext context)
        {
            return base.VisitClassifications(context);
        }

        public override Expression VisitLiteral(DreamGrammarParser.LiteralContext context)
        {
            return base.VisitLiteral(context);
        }

        public override Expression Visit(IParseTree tree)
        {
            return base.Visit(tree);
        }

        public override Expression VisitEqualsign(DreamGrammarParser.EqualsignContext context)
        {
            return base.VisitEqualsign(context);
        }

        public override Expression VisitEqualequal(DreamGrammarParser.EqualequalContext context)
        {
            return base.VisitEqualequal(context);
        }

        public override Expression VisitEqualityExpression(DreamGrammarParser.EqualityExpressionContext context)
        {
            return base.VisitEqualityExpression(context);
        }

        public override Expression VisitExpression(DreamGrammarParser.ExpressionContext context)
        {
            return base.VisitExpression(context);
        }

        public override Expression VisitEndLine(DreamGrammarParser.EndLineContext context)
        {
            var endline = Expression.Constant(context.GetText());
            return endline;
        }

        public override Expression VisitEvaluatable(DreamGrammarParser.EvaluatableContext context)
        {
            return base.VisitEvaluatable(context);
        }

        public override Expression VisitTerminal(ITerminalNode node)
        {
            return Expression.Constant(node.GetText());
        }

        public override Expression VisitVariableDeclaration(DreamGrammarParser.VariableDeclarationContext context)
        {
            return base.VisitVariableDeclaration(context);
        }

        public override Expression VisitVariableDeclarationExpression(
            DreamGrammarParser.VariableDeclarationExpressionContext context)
        {
            string type = context.children[0].GetText();
            string variableName = context.children[1].GetText();

            ParameterExpression expression = null;

            switch(type)
            {
                case "int":
                    expression = Expression.Parameter(typeof(int), variableName);
                    break;
                case "double":
                    expression = Expression.Parameter(typeof(double), variableName);
                    break;
                case "string":
                    expression = Expression.Parameter(typeof(string), variableName);
                    break;
                default:
                    throw new Exception("variable type not found");
            }

            //return base.VisitVariableDeclarationExpression(context);

            return expression;
        }

        public override Expression VisitVariable(DreamGrammarParser.VariableContext context)
        {
            return base.VisitVariable(context);
        }

        public override Expression VisitExpressionSequence(DreamGrammarParser.ExpressionSequenceContext context)
        {
            return base.VisitExpressionSequence(context);
        }

        public override Expression VisitFuncName(DreamGrammarParser.FuncNameContext context)
        {
            return Expression.Constant(context.GetText());
        }

        public override Expression VisitFunctionDeclaration(DreamGrammarParser.FunctionDeclarationContext context)
        {
            var children = context.children;
            int i = children.Count;
            int currentChild = 0;

            var functionKeyWord = children[currentChild].Accept(this);
            if (!functionKeyWord.ToString().Equals("\"function\""))
            {
                throw new Exception("invalid function declaration");
            }
            
            var functionName = children[++currentChild].Accept(this);

            var parameters = children[++currentChild].Accept(this);
            if (parameters.ToString().Equals("\"()\""))
            {
                // no input parameters read equal sign and brackets
                var equalSign = children[++currentChild].Accept(this);
                if (equalSign.ToString().Equals("\"=\"")) {
                    var leftBracket = children[++currentChild].Accept(this);
                    var firstStatement = children[++currentChild].Accept(this);

                    if (firstStatement.ToString().Equals("\"}\""))
                    {
                        //Empty body function
                        return Expression.Block(Expression.Constant(functionName.ToString()));
                    }
                    else
                    {
                        
                    }
                }

            }
            Expression expression =  base.VisitFunctionDeclaration(context);
            return Expression.Block(expression);
        }

        public override Expression VisitFunctionCall(DreamGrammarParser.FunctionCallContext context)
        {
            var functionCallName = context.children[0].GetText();//

            object[] t = new object[context.ChildCount - 1];
            for (int i = 1; i < context.ChildCount; i++)
            {
                t[i-1] = context.children[i].Accept(this);
            }

            return Expression.Constant(functionCallName);

            //var functionToCall = context.Accept(this);
            //return functionToCall;
            /*
            Expression expression = base.VisitFunctionCall(context);


            List<string> funcParams = new List<string>();
            int count = context.ChildCount - 1;
            for(int i = 1; i < count; i++)
            {
                funcParams.Add(context.children[i].GetText());
            }

            string[,] gradeArray =
                {{"chemistry", "history", "mathematics"}, {"78", "61", "82"}};

            Expression arrayExpression = Expression.Constant(gradeArray);
            MethodCallExpression methodCall = Expression.ArrayIndex(arrayExpression);

            string[,] gradeArray =
                {{"chemistry", "history", "mathematics"}, {"78", "61", "82"}};

            Expression arrayExpression = Expression.Constant(gradeArray);

            MethodCallExpression methodCall = Expression.ArrayIndex(arrayExpression, Expression.Constant(0), Expression.Constant(2));


            return methodCall;*/
        }

        public override Expression VisitFunctionCallExpression(DreamGrammarParser.FunctionCallExpressionContext context)
        {
            Expression[] t = new Expression[context.ChildCount];
            for (int i = 0; i < context.ChildCount; i++)
            {
                t[i] = context.children[i].Accept(this);
            }

            var afterall = base.VisitFunctionCallExpression(context);

            return t[0];
        }

        public override Expression VisitIdentifierName(DreamGrammarParser.IdentifierNameContext context)
        {
            Expression[] t = new Expression[context.ChildCount];
            for (int i = 0; i < context.ChildCount; i++)
            {
                t[i] = context.children[i].Accept(this);
            }

            var afterId = base.VisitIdentifierName(context);
            return t[0];
        }
    }
}
