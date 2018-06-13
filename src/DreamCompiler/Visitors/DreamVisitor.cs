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
            int size = context.ChildCount - 1;
            Expression[] t = new Expression[size];
            for (int i = 0; i < size; i++)
            {
                var exp = context.children[i].Accept(this);

                if (size == 1)
                {
                    return exp;
                }

                if (i < size)
                {
                    t[i] = exp;
                }
            }

            return null;
            //return new DreamMulitipleVariableExpressions(t);
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

            var variableExpressions = new List<Expression>();

            ParameterExpression expression = null;
            var mulitipleExpressions = new DreamMulitipleVariableExpressions();

            switch (type)
            {
                case "int":
                    expression = Expression.Variable(typeof(int), variableName);
                    variableExpressions.Add( expression );
                    break;
                case "double":
                    expression = Expression.Variable(typeof(double), variableName);
                    variableExpressions.Add(expression);
                    break;
                case "string":
                    expression = Expression.Variable(typeof(string), variableName);
                    var binaryExpression = Expression.Assign(
                        expression, 
                        Expression.Constant( 
                            StripValue(context.children[3].GetText())));
                    variableExpressions.Add(expression);
                    variableExpressions.Add(binaryExpression);
                    break;
                default:
                    throw new Exception("variable type not found");
            }

            mulitipleExpressions.AddExpressions(variableExpressions.ToArray());
            return mulitipleExpressions;
        }

        private string StripValue(String theString)
        {
            Trace.WriteLine(theString);
            if (theString.StartsWith(@"""") && theString.EndsWith(@""""))
            {
                return theString.Substring(1, theString.Length - 2);
            }

            return theString;
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
            var i = children.Count;
            var currentChild = 0;

            var functionKeyWord = StripValue(children[currentChild].Accept(this).ToString());
            if (!functionKeyWord.Equals("function"))
            {
                throw new Exception("invalid function declaration");
            }

            var functionName = children[++currentChild].Accept(this);

            var parameters = StripValue(children[++currentChild].Accept(this).ToString());
            var expressionList = new List<Expression>();

            if (parameters.Equals("()"))
            {
                // no input parameters read equal sign and brackets
                var equalSign = StripValue(children[++currentChild].Accept(this).ToString());
                if (equalSign.Equals("="))
                {
                    var leftBracket = StripValue(children[++currentChild].Accept(this).ToString());
                    if (!leftBracket.Equals("{"))
                    {
                        throw new Exception("No left bracket");
                    }

                    while (currentChild <= i)
                    {
                        var currentStatementExpression = children[++currentChild].Accept(this);

                        if (StripValue(currentStatementExpression.ToString()).Equals("}"))
                        {
                            break;
                        }
                        else
                        {
                            expressionList.Add(currentStatementExpression);
                        }
                    }
                }

            }

            // Test expression to add to the bottom of the list of expressions.
            var localParameters = new List<ParameterExpression>();
            var expressionsToAdd = new List<Expression>();

            foreach (var parm in expressionList)
            {
                if (parm is ParameterExpression item)
                {
                    localParameters.Add(item);
                    continue;
                }

                if (parm is DreamMulitipleVariableExpressions itemExpression)
                {
                    foreach (var exp in itemExpression.GetExpressions())
                    {
                        if (exp is ParameterExpression parameterExpression)
                        {
                            localParameters.Add(parameterExpression);
                        }
                        else
                        {
                            expressionsToAdd.Add(exp);
                        }
                    }
                }
            }

            expressionList.RemoveRange(0, localParameters.Count);
            expressionList.AddRange(expressionsToAdd);
            /*
            expressionList.Add(Expression.Call(null,
                typeof(Trace).GetMethod("WriteLine", new Type[] {typeof(String)}) ??
                throw new InvalidOperationException(),
                Expression.Constant("0s")));
                */

            return Expression.Block(localParameters, expressionList);
        }

        public override Expression VisitFunctionCall(DreamGrammarParser.FunctionCallContext context)
        {
            var functionCallName = context.children[0].GetText();//

            object[] t = new object[context.ChildCount - 1];
            for (int i = 1; i < context.ChildCount; i++)
            {
                t[i-1] = context.children[i].Accept(this);
            }

            DreamMethodCall dreamMethodCall = new DreamMethodCall(Expression.Constant(functionCallName), null, null);
            return dreamMethodCall;
        }

        public override Expression VisitFunctionCallExpression(DreamGrammarParser.FunctionCallExpressionContext context)
        {
            Expression[] t = new Expression[context.ChildCount];
            for (int i = 0; i < context.ChildCount; i++)
            {
                t[i] = context.children[i].Accept(this);
            }

            //var afterall = base.VisitFunctionCallExpression(context);

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

    internal class DreamMethodCall : Expression
    {
        private ConstantExpression methodName;
        private ParameterExpression[] inputParameters;
        private ParameterExpression returnParameters;

        public DreamMethodCall(ConstantExpression methodName, ParameterExpression[] inputParameters, ParameterExpression returnParameters)
        {
            this.methodName = methodName;
            this.inputParameters = inputParameters;
            this.returnParameters = returnParameters;
        }

        public override ExpressionType NodeType => ExpressionType.Call;

        public override Type Type => typeof(object);
    }


    internal class DreamMulitipleVariableExpressions : Expression
    {
        private Expression[] expressions;
        public void AddExpressions(Expression[] expressionArray)
        {
            expressions = expressionArray;
        }

        internal Expression[] GetExpressions()
        {
            return expressions;
        }

        public override ExpressionType NodeType => ExpressionType.Parameter;
    }
}
