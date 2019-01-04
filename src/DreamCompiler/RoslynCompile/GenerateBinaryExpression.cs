using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DReAMCompiler.RoslynCompile
{
    using Antlr4.Runtime.Tree;
    using Antlr4.Runtime;
    using DReAMCompiler.Grammar;
    using DReAMCompiler.Visitors;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System.Diagnostics;


    class GenerateBinaryExpression
    {

        private DReAMGrammarParser.VariableContext currentVariable;
        private ParserRuleContext currentKeyWord;
        private Stack<ParserRuleContext> operators = new Stack<ParserRuleContext>();
        private List<ParserRuleContext> postfix = new List<ParserRuleContext>();
        private Stack<ParserRuleContext> stackOne = new Stack<ParserRuleContext>();
        private Stack<ExpressionSyntax> stackTwo = new Stack<ExpressionSyntax>();
        private const string LeftParen = "(";
        private const string RightParen = ")";
        private const int DefaultNumberOfChildren = 1;

        static public CSharpSyntaxNode CreateBinaryExpression(DReAMGrammarParser.BinaryExpressionContext context, DreamRoslynVisitor visitor)
        {
            var nodeList = new List<CSharpSyntaxNode>();

            foreach (var expression in context.children)
            {
                var item = expression.Accept(visitor);
                nodeList.Add(item);
            }

            String unaryOp = context.binaryOperator().GetText();

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

        public GenerateBinaryExpression Walk(Antlr4.Runtime.ParserRuleContext node)
        {
            try
            {
                if (node != null)
                {
                    WalkChildren(node.children);
                    if (node is DReAMGrammarParser.VariableContext)
                    {
                        //currentVariable = node as DReAMGrammarParser.VariableContext;
                        postfix.Add(node);
                        return this;
                    }
                    else if (node is DReAMGrammarParser.KeywordsContext)
                    {
                        currentKeyWord = node;
                        Trace.WriteLine(node.GetText());
                        return this;
                    }
                    else if (node is DReAMGrammarParser.VariableDeclarationContext)
                    {
                        System.Diagnostics.Trace.WriteLine(node.GetText());
                        return this;
                    }
                    else if (node is DReAMGrammarParser.AssignmentOperatorContext)
                    {
                        if (operators.Count == 0)
                        {
                            operators.Push(node);
                            return this;
                        }

                        throw new ArgumentException("Invalid expressions");
                    }
                    else if (!(node is DReAMGrammarParser.BinaryOperatorContext))
                    {
                        // other non binary operands will need to be tested here.
                        if (node.children.Count() == DefaultNumberOfChildren)
                        {
                            if (node.children[0].GetText().Equals(LeftParen))
                            {
                                operators.Push(node);
                            }
                            else if (node.children[0].GetText().Equals(RightParen))
                            {
                                while (operators.Count > 0 && !operators.Peek().children[0].GetText().Equals(LeftParen))
                                {
                                    postfix.Add(operators.Pop());
                                }

                                if (operators.Count > 0 && !operators.Peek().children[0].GetText().Equals(LeftParen))
                                {
                                    throw new Exception("invalid expression");
                                }
                                else
                                {
                                    operators.Pop();
                                }
                            }
                            else if (node is DReAMGrammarParser.DecimalValueContext)
                            {
                                postfix.Add(node);
                                Trace.WriteLine(node.GetText());
                            }
                            else if (node is DReAMGrammarParser.FunctionCallExpressionContext)
                            {
                                postfix.Add(node);
                            }
                        }
                        return this;
                    }
                    else if (node is DReAMGrammarParser.BinaryOperatorContext)
                    {
                        if (Precedence(operators.Peek()) > Precedence(node) ||
                            Precedence(operators.Peek()) == Precedence(node))
                        {
                            postfix.Add(operators.Pop());
                            Trace.WriteLine(postfix.Last().children[0].GetText());
                            operators.Push(node);
                            return this;
                        }

                        if (Precedence(operators.Peek()) < Precedence(node))
                        {
                            operators.Push(node);
                            return this;
                        }

                        /*
                        while (operators.Count > 0 && Precedence(node) <= Precedence(operators.Peek()))
                        {
                            postfix.Add(operators.Pop());
                        }

                        operators.Push(node);
                        return this;
                        */
                    }
                }
            }
            catch(Exception ex)
            {
                throw;
            }

            return this;
        }

        private int Precedence(IParseTree op)
        {
            int precedence =  operaterLookup[op.GetChild(0).GetText()];
            return precedence;
            
            /*
             * 
            //LiteralExpressionSyntax opAndOne;
            //LiteralExpressionSyntax opAndTwo;
            //BinaryExpressionSyntax binaryExpression;
            opAndOne = SyntaxFactory.LiteralExpression(
                            SyntaxKind.NumericLiteralExpression,
                            SyntaxFactory.Literal(operators.Pop().children[0].GetText()));

            opAndTwo = SyntaxFactory.LiteralExpression(
                      SyntaxKind.NumericLiteralExpression,
                      SyntaxFactory.Literal(operators.Pop().children[0].GetText()));

            binaryExpression = SyntaxFactory.BinaryExpression(SyntaxKind.MultiplyExpression,
                opAndOne,
                opAndTwo);
                */
            /*
            opAndOne = SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(operators.Pop().children[0].GetText()));

            opAndTwo = SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(operators.Pop().children[0].GetText()));


            binaryExpression = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                opAndOne,
                opAndTwo);

            */
        }

        private void WalkChildren(IList<IParseTree> children)
        {
            foreach (var child in children)
            {
                if (children.Count > 1)
                {
                    Walk(child as Antlr4.Runtime.ParserRuleContext);
                }
                else
                {
                    return;
                }
            }
        }

        internal GenerateBinaryExpression PostWalk()
        {
            while (operators.Count > 0)
            {
                postfix.Add(operators.Pop());
            }

            return this;
        }

        internal void Eval()
        {
            foreach (var token in postfix)
            {
                if (token is DReAMGrammarParser.BinaryOperatorContext)
                {
                    if (stackTwo.Count >= 2)
                    {
                        var right = stackTwo.Pop();
                        var left = stackTwo.Pop();
                    }
                    else if (stackOne.Count >= 2)
                    {
                        var right = stackOne.Pop();
                        var left = stackOne.Pop();

                        LiteralExpressionSyntax rightLiteralExpressionSyntax = null;
                        LiteralExpressionSyntax leftLiteralExpressionSyntax = null;
                    
                        if (right is DReAMGrammarParser.DecimalValueContext)
                        {
                            rightLiteralExpressionSyntax = CreateNumericLiteralExpression(right);
                        }
                        if (right is DReAMGrammarParser.StringValueContext)
                        {
                            rightLiteralExpressionSyntax = CreateStringLiteralExpression(right);
                        }


                        if (left is DReAMGrammarParser.DecimalValueContext)
                        {
                            leftLiteralExpressionSyntax = CreateNumericLiteralExpression(left);
                        }
                        if (right is DReAMGrammarParser.StringValueContext)
                        {
                            leftLiteralExpressionSyntax = CreateStringLiteralExpression(left);
                        }

                        if (rightLiteralExpressionSyntax == null || leftLiteralExpressionSyntax == null)
                        {
                            throw new Exception("invalid ");
                        }


                        stackTwo.Push(SyntaxFactory.BinaryExpression(SyntaxKind.DivideExpression,
                            leftLiteralExpressionSyntax,
                            rightLiteralExpressionSyntax));
                    }
                    else if (stackOne.Count == 1 && stackTwo.Count == 1)
                    {
                        var left = CreateNumericLiteralExpression(stackOne.Pop());
                        var right = stackTwo.Pop();

                        var finalExpression = SyntaxFactory.BinaryExpression(SyntaxKind.DivideExpression,
                            left,
                            right);
                    }
                }
                else
                {
                    stackOne.Push(token);
                }
            }
        }

        private LiteralExpressionSyntax CreateNumericLiteralExpression(ParserRuleContext context)
        {
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                CreateNumericLiteral(context));
        }

        private LiteralExpressionSyntax CreateStringLiteralExpression(ParserRuleContext context)
        {
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                CreateStringLiteral(context));
        }

        private SyntaxToken CreateNumericLiteral(ParserRuleContext context)
        {
            return SyntaxFactory.Literal(context.GetChild(0).GetText());
        }

        private SyntaxToken CreateStringLiteral(ParserRuleContext context)
        {
            return SyntaxFactory.Literal(context.GetChild(0).GetText());
        }

        private enum SrEval
        {
            Sht,
            Red,
            Err,
            End,
        }

        private readonly Dictionary<string, int> operaterLookup = new Dictionary<string, int>()
        {
            { "=",  0 },
            { "||", 1 },
            { "&&", 2 },
            { "==", 3 },
            { "!=", 4 },
            { ">=", 5 },
            { "<=", 6 },
            { ">",  7 },
            { "<",  8 },
            { "+",  9 },
            { "-",  10 },
            { "/",  11 },
            { "%",  12 },
            { "*",  13 },
            { "()", 14 },
            { ".",  15 }
        };

        internal Dictionary<string, int> OperaterLookup => operaterLookup;

        private class operatorState
        {
            public SrEval[,] shiftReduceOperationTable = new SrEval[,]
            { /*             assign      OR          AND         EQEQ        NOTEQ       GTEQ        LTEQ        GT          LT          PLUS        MINUS       DIV         MOD         MULT        FUNC        DOT        /*
              /* assn*/   {  SrEval.Red, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht},
              /* OR   */  {  SrEval.Red, SrEval.Red, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht},
              /* AND  */  {  SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht},
              /* EQEQ */  {  SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht},
              /* NEQ  */  {  SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht},
              /* GTEQ */  {  SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht},
              /* LTEQ */  {  SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht},
              /* GT   */  {  SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht},
              /* LT   */  {  SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht},
              /* PLUS */  {  SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht},
              /* MINUS*/  {  SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht},
              /* DIV  */  {  SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Sht, SrEval.Sht},
              /* MULT */  {  SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Sht, SrEval.Sht},
              /* MOD  */  {  SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Sht, SrEval.Sht},
              /* FUNC */  {  SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Sht},
              /* DOT  */  {  SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red, SrEval.Red},
            };
        }
    }
}
