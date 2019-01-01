using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DReAMCompiler.RoslynCompile
{
    using Antlr4.Runtime.Tree;
    using DReAMCompiler.Grammar;
    using DReAMCompiler.Visitors;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using DReAMCompiler.Constants;

    class GenerateBinaryExpression
    {

        private DReAMCompiler.Grammar.DReAMGrammarParser.VariableContext currentVariable;
        private Antlr4.Runtime.ParserRuleContext currentKeyWord;
        private Stack<Antlr4.Runtime.ParserRuleContext> operators = new Stack<Antlr4.Runtime.ParserRuleContext>();
        private List<Antlr4.Runtime.ParserRuleContext> postfix = new List<Antlr4.Runtime.ParserRuleContext>();
        private static readonly string leftParen = "(";
        private static readonly string rightParen = ")";
        private static readonly int numberOfChildren = 1; 

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
            if (node != null)
            {
                WalkChildren(node.children);
                if (node is DReAMGrammarParser.VariableContext)
                {
                    currentVariable = node as DReAMGrammarParser.VariableContext;
                    return this;
                }
                else if (node is DReAMGrammarParser.KeywordsContext)
                {
                    currentKeyWord = node;
                    System.Diagnostics.Trace.WriteLine(node.GetText());
                    return this;
                }
                else if (node is DReAMGrammarParser.VariableDeclarationContext)
                {
                    System.Diagnostics.Trace.WriteLine(node.GetText());
                    return this;
                }
                else if (node is DReAMGrammarParser.AssignmentOperatorContext)
                {
                    return this;
                }
                else if (!(node is DReAMGrammarParser.BinaryOperatorContext))
                {
                    // other non binary operands will need to be tested here.
                    if (node.children.Count() == numberOfChildren)
                    {
                        if (node.children[0].GetText().Equals(leftParen))
                        {
                            operators.Push(node);
                        }
                        else if (node.children[0].GetText().Equals(rightParen))
                        {
                            while (operators.Count > 0 && !operators.Peek().children[0].GetText().Equals(leftParen))
                            {
                                postfix.Add(operators.Pop());
                            }

                            if (operators.Count > 0 && !operators.Peek().children[0].GetText().Equals(leftParen))
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
                            System.Diagnostics.Trace.WriteLine(node.GetText());
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
                    while (operators.Count > 0 && Precedence(node) <= Precedence(operators.Peek()))
                    {
                        postfix.Add(operators.Pop());
                    }

                    operators.Push(node);
                    return this;
                }
            }

            return this;
        }

        private int Precedence(IParseTree op)
        {
            if (op.GetChild(0).GetType().Name.Equals("AddSubtractOpContext"))
            {
                return 5;
            }

            if (op.GetChild(0).GetType().Name.Equals("MultiplyDivideOpContext"))
            {
                return 10;
            }

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


            return 0;
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

        public void PostWalk()
        {
            while (operators.Count > 0)
            {
                postfix.Add(operators.Pop());
            }
        }
        
        private enum SrEval
        {
            Shift,
            Reduce,
            Error,
            End,
        }

        private class operatorState
        {
            public SrEval[,] shiftReduceOperationTable = new SrEval[,]
            { /*             plus            minus          multiply       divide          function*/
              /* plus */  {  SrEval.Reduce,  SrEval.Reduce, SrEval.Shift,  SrEval.Shift,   SrEval.Shift},
              /* min  */  {  SrEval.Reduce,  SrEval.Reduce, SrEval.Shift,  SrEval.Shift,   SrEval.Shift},
              /* mult */  {  SrEval.Reduce,  SrEval.Reduce, SrEval.Reduce, SrEval.Reduce,  SrEval.Shift}
            };
        }
    }
}
