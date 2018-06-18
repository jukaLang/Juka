using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Antlr4.Runtime.Tree;
using DreamCompiler.Tokens;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Runtime.Remoting.Metadata.W3cXsd2001;


namespace DreamCompiler.Visitors
{
    using DreamCompiler.Grammar;

    internal enum BinaryExpressionTypes
    {
        Int,
        Double,
        String
    }

    internal class DreamVisitor : DreamGrammarBaseVisitor<Expression>
    {
        private Stack<BinaryExpressionTypes> binaryExpressionStack = new Stack<BinaryExpressionTypes>();
        private Dictionary<string, LabelExpression> highLevelFunctions = new Dictionary<string, LabelExpression>();

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
            var expressions = new List<Expression>();
            foreach (var t in context.children)
            {
                expressions.Add( t.Accept(this));
            }

            BinaryExpression binaryExpression = null;
            BinaryExpressionTypes currentType = binaryExpressionStack.Peek();

            String binaryOperator = RemoveLeadingAndTrailingQuotes(expressions[1].ToString());

            switch (binaryOperator)
            {
                case "+":
                case "/":
                case "-":
                case "*":
                    if (currentType == BinaryExpressionTypes.Int)
                    {
                        Expression left;
                        Expression right;

                        // Assumptions: Constant Expressions may be wrapped in quotes. So strips quotes.
                        if (expressions[0] is ConstantExpression)
                        {
                            left = Expression.Constant(Convert.ToInt32(RemoveLeadingAndTrailingQuotes(expressions[0].ToString())));
                        }
                        else
                        {
                            left = expressions[0];
                        }

                        if (expressions[2] is ConstantExpression)
                        {
                            right = Expression.Constant(Convert.ToInt32(RemoveLeadingAndTrailingQuotes(expressions[2].ToString())));
                        }
                        else
                        {
                            right = expressions[2];
                        }

                        if (binaryOperator.Equals("+"))
                        {
                            binaryExpression = Expression.Add(left, right);
                        }
                        else if (binaryOperator.Equals("/"))
                        {
                            binaryExpression = Expression.Divide(left, right);
                        }
                        else if (binaryOperator.Equals("-"))
                        {
                            binaryExpression = Expression.Subtract(left, right);
                        }
                        else if (binaryOperator.Equals("*"))
                        {
                            binaryExpression = Expression.Multiply(left, right);
                        }
                    }

                    break;
                
                default:
                    break;
            }

            Debug.Assert( binaryExpression != null, "you probably need to add a support for a binary operator");

            return binaryExpression;
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
                    GenerateBinaryIntExpression(context, variableName, variableExpressions);
                    break;
                case "double":
                    binaryExpressionStack.Push(BinaryExpressionTypes.Double);
                    expression = Expression.Variable(typeof(double), variableName);
                    variableExpressions.Add(expression);
                    binaryExpressionStack.Pop();
                    break;
                case "string":
                    binaryExpressionStack.Push(BinaryExpressionTypes.String);
                    expression = Expression.Variable(typeof(string), variableName);

                    var constantExpression = Expression.Constant(RemoveLeadingAndTrailingQuotes(context.children[3].GetText()));

                    var binaryExpression = Expression.Assign(
                        expression, 
                        constantExpression);

                    variableExpressions.Add(expression);
                    variableExpressions.Add(binaryExpression);

#if DEBUG
                    try
                    {
                        // Push a WriteLine into the expression list
                        // to debug the variable assignement.
                        var variableType = variableExpressions.GetType();
                        var writeLineExpression = Expression.Call(null,
                            typeof(Trace).GetMethod("WriteLine", new Type[] {typeof(object)}) ??
                            throw new InvalidOperationException(),
                            expression);

                        variableExpressions.Add(writeLineExpression);
                    }
                    catch (Exception ex)
                    {
                        Debug.Assert( true, ex.Message);
                    }
                   
#endif                    

                    binaryExpressionStack.Pop();
                    break;
                default:
                    throw new Exception("variable type not found");
            }

            mulitipleExpressions.AddExpressions(variableExpressions.ToArray());
            return mulitipleExpressions;
        }

        private void GenerateBinaryIntExpression(DreamGrammarParser.VariableDeclarationExpressionContext context, string variableName,
            List<Expression> variableExpressions)
        {
            ParameterExpression expression;
            binaryExpressionStack.Push(BinaryExpressionTypes.Int);
            expression = Expression.Variable(typeof(int), variableName);
            variableExpressions.Add(expression);
            const int expressionCount = 2;
            if (context.ChildCount > expressionCount)
            {
                for (int iChildCount = expressionCount; iChildCount < context.ChildCount; iChildCount++)
                {
                    var childExpression = context.children[iChildCount].Accept(this);
                    if (childExpression is ConstantExpression)
                    {
                        // Need to do some validation to ensure the operator is really an equal sign.
                        // Should be the only allowable operator for a variable declaration
                        if (!RemoveLeadingAndTrailingQuotes(childExpression.ToString()).Equals("="))
                        {
                            throw new Exception("Illegal assignment operator");
                        }

                        continue;
                    }

                    variableExpressions.Add(childExpression);
                }
            }

            if (variableExpressions.Count == expressionCount)
            {
                //var assignExpression = Expression.Assign(variableExpressions[0], Expression.Constant(1));
                var assignExpression = Expression.Assign(variableExpressions[0], variableExpressions[1]);
#if DEBUG
                var tempExpression0 = new List<Expression>() {assignExpression};
                var tempExpression1 = new List<ParameterExpression>() {(ParameterExpression) variableExpressions[0]};
                tempExpression0.Add(Expression.Assign(variableExpressions[0], variableExpressions[1]));
                var block = Expression.Block(tempExpression1, tempExpression0);
                Expression.Lambda(block).Compile();
#endif
                variableExpressions.RemoveAt(expressionCount - 1);
                variableExpressions.Add(assignExpression);
            }

            binaryExpressionStack.Pop();
        }

        private string RemoveLeadingAndTrailingQuotes(String theString)
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

            var functionKeyWord = RemoveLeadingAndTrailingQuotes(children[currentChild].Accept(this).ToString());
            if (!functionKeyWord.Equals("function"))
            {
                throw new Exception("invalid function declaration");
            }

            var functionName = children[++currentChild].Accept(this);

            var parameters = RemoveLeadingAndTrailingQuotes(children[++currentChild].Accept(this).ToString());
            var expressionList = new List<Expression>();

            if (parameters.Equals("()"))
            {
                // no input parameters read equal sign and brackets
                var equalSign = RemoveLeadingAndTrailingQuotes(children[++currentChild].Accept(this).ToString());
                if (equalSign.Equals("="))
                {
                    var leftBracket = RemoveLeadingAndTrailingQuotes(children[++currentChild].Accept(this).ToString());
                    if (!leftBracket.Equals("{"))
                    {
                        throw new Exception("No left bracket");
                    }

                    while (currentChild <= i)
                    {
                        var currentStatementExpression = children[++currentChild].Accept(this);

                        if (RemoveLeadingAndTrailingQuotes(currentStatementExpression.ToString()).Equals("}"))
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

            var label = Expression.Label(Expression.Label());

            expressionsToAdd.Add(label);
            highLevelFunctions.Add( RemoveLeadingAndTrailingQuotes(functionName.ToString()) , label);

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
        private MethodCallExpression methodCall;

        public DreamMethodCall(ConstantExpression methodName, ParameterExpression[] inputParameters, ParameterExpression returnParameters)
        {
            this.methodName = methodName;
            this.inputParameters = inputParameters;
            this.returnParameters = returnParameters;
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => typeof(object);

        public override bool CanReduce => true;

        public override Expression Reduce()
        {
            return base.Reduce();
        }
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

        public override ExpressionType NodeType => ExpressionType.Extension;
    }
}
