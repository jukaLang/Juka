using System;
using System.Collections.Generic;
using System.Linq;

namespace DreamCompiler.RoslynCompile
{
    using Antlr4.Runtime.Tree;
    using Antlr4.Runtime;
    using DreamCompiler.Grammar;
    using DreamCompiler.Visitors;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System.Diagnostics;

    class GenerateBinaryExpression
    {
        private LocalDeclarationStatementSyntax statementSyntax;
        private ParserRuleContext currentKeyWord;
        private Stack<ParserRuleContext> operators = new Stack<ParserRuleContext>();
        private List<ParserRuleContext> postfix = new List<ParserRuleContext>();
        private Stack<ContextExpressionUnion> unionStack = new Stack<ContextExpressionUnion>();
        private const string LeftParen = "(";
        private const string RightParen = ")";
        private const int DefaultNumberOfChildren = 1;
        private bool isDeclaration = false;

        internal static CSharpSyntaxNode CreateBinaryExpression(ParserRuleContext context, DreamRoslynVisitor visitor)
        {
            return null;
        }

        internal CSharpSyntaxNode GetLocalDeclarationStatementSyntax()
        {
            if (isDeclaration)
            {
                if (statementSyntax == null)
                {
                    throw new Exception("Statement not created");
                }
                return statementSyntax;
            }
            else if (unionStack.Count >= 1)
            {
                return unionStack.Pop().syntax as BinaryExpressionSyntax;
            }
            else
            {
                throw new Exception("No binary expression on stack");
            }
        }

        internal GenerateBinaryExpression Walk(ParserRuleContext node)
        {
            try
            {
                if (node != null)
                {
                    WalkChildren(node.children);
                    if (node is DreamGrammarParser.CombinedExpressionsContext ||
                        node is DreamGrammarParser.VariableDeclarationAssignmentContext ||
                        node is DreamGrammarParser.BinaryOpAndDoubleExpressionContext ||
                        node is DreamGrammarParser.BinaryOpAndSingleExpressionContext ||
                        node is DreamGrammarParser.BinaryOperatorContext)

                    {
                        return this;
                    }
                    else if (node is DreamGrammarParser.LeftParenContext)
                    {
                        operators.Push(node);
                        return this;
                    }
                    else if (node is DreamGrammarParser.RightParenContext)
                    {
                        while (!(operators.Peek() is DreamGrammarParser.LeftParenContext))
                        {
                            postfix.Add(operators.Pop());
                        }

                        operators.Pop();
                    }
                    else if (node is DreamGrammarParser.VariableContext)
                    {
                        postfix.Add(node);
                        return this;
                    }
                    else if (node is DreamGrammarParser.KeywordsContext)
                    {
                        currentKeyWord = node;
                        return this;
                    }
                    else if (node is DreamGrammarParser.VariableDeclarationContext)
                    {
                        return this;
                    }
                    else if (node is DreamGrammarParser.AssignmentOperatorContext)
                    {
                        if (operators.Count == 0)
                        {
                            operators.Push(node);
                            return this;
                        }

                        throw new ArgumentException("Invalid expressions");
                    }
                    else if (node is DreamGrammarParser.IntValueContext)
                    {
                        postfix.Add(node);
                        return this;
                    }
                    else if (
                        node is DreamGrammarParser.AddSubtractOpContext ||
                        node is DreamGrammarParser.MultiplyDivideOpContext)
                    {
                        if (operators.Count == 0)
                        {
                            operators.Push(node);
                            return this;
                        }

                        if (Precedence(operators.Peek()) > Precedence(node) ||
                            Precedence(operators.Peek()) == Precedence(node))
                        {
                            postfix.Add(operators.Pop());
                            operators.Push(node);
                            return this;
                        }

                        if (Precedence(operators.Peek()) < Precedence(node))
                        {
                            operators.Push(node);
                            return this;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return this;
        }

        private int Precedence(IParseTree op)
        {
            string opText = op.GetChild(0).GetText();
            int value = -1;
            if (operaterLookup.TryGetValue(opText, out value))
            {
                return value;
            }
            else
            {
                return value;
            }
        }

        private void WalkChildren(IList<IParseTree> children)
        {
            foreach (var child in children)
            {
                if (child is ParserRuleContext)
                {
                    Walk(child as ParserRuleContext);
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

        internal GenerateBinaryExpression PrintPostFix()
        {
            foreach (var p in postfix)
            {
                Trace.WriteLine(p.GetText());
            }

            return this;
        }

        internal void Eval()
        {
            foreach (var token in postfix)
            {
                if (token is DreamGrammarParser.AssignmentOperatorContext)
                {
                    if (postfix.Count > 0 && postfix[0] is DreamGrammarParser.VariableContext)
                    {
                        CreateVariableDeclarator(postfix[0], (BinaryExpressionSyntax)unionStack.Pop().syntax);
                        if (statementSyntax != null)
                        {
                            isDeclaration = true;
                        }
                        break;
                    }
                    else
                    {
                        throw new Exception("No variable to assign expression");
                    }
                }
                else if (token is DreamGrammarParser.AddSubtractOpContext ||
                        token is DreamGrammarParser.MultiplyDivideOpContext)
                {
                    string binaryOp = token.GetChild(0).GetText();

                    SyntaxKind op = syntaxKindLookup[operaterLookup[binaryOp]];

                    var right = unionStack.Pop().syntax;
                    var left = unionStack.Pop().syntax;

                    unionStack.Push(new ContextExpressionUnion()
                    {
                        context = null,
                        syntax = SyntaxFactory.BinaryExpression(op,
                        left,
                        right)
                    });
                }
                else if (token is DreamGrammarParser.VariableContext)
                {
                    if (unionStack.Count == 0)
                    {
                        continue;
                    }
                }
                else
                {
                    CheckContextType(token, unionStack);
                    continue;
                }
            }

            if (postfix.Count >= 2)
            {
                //Assume that there is a binary expression but not an assignment.
                isDeclaration = false;
            }
        }

        private void CheckContextType(ParserRuleContext context, Stack<ContextExpressionUnion> stack)
        {
            if (context != null)
            {
                if (context is DreamGrammarParser.IntValueContext)
                {
                    stack.Push(new ContextExpressionUnion()
                    {
                        context = null,
                        syntax = CreateNumericLiteralExpression(context)
                    });
                }
                else if (context is DreamGrammarParser.StringValueContext)
                {
                    stack.Push(new ContextExpressionUnion()
                    {
                        context = null,
                        syntax = CreateStringLiteralExpression(context)
                    });
                }
                else if (context is DreamGrammarParser.VariableContext)
                {
                    // CreateVariableDeclarator(context, stack.Pop().syntax as BinaryExpressionSyntax);
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

        private void CreateVariableDeclarator(ParserRuleContext context, BinaryExpressionSyntax binaryExpression)
        {
            if (!string.IsNullOrEmpty(currentKeyWord.GetText()))
            {
                statementSyntax = SyntaxFactory.LocalDeclarationStatement(
                  declaration: SyntaxFactory.VariableDeclaration(
                     SyntaxFactory.PredefinedType(GetKeywordTokenType(currentKeyWord.GetText())))
                 .WithVariables(
                     SyntaxFactory.SingletonSeparatedList(
                         SyntaxFactory.VariableDeclarator(
                             SyntaxFactory.Identifier(context.GetText())).WithInitializer(
                         SyntaxFactory.EqualsValueClause(
                           binaryExpression)))));
            }
            
            if (statementSyntax == null)
            {
                throw new Exception("Invalid variable declaration");
            }
        }

        internal LocalDeclarationStatementSyntax GetLocalDeclarationStatement()
        {
            return statementSyntax;
        }

        private SyntaxToken CreateNumericLiteral(ParserRuleContext context)
        {
            int value = Int16.Parse(context.GetChild(0).GetText());
            return SyntaxFactory.Literal(value);
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
            { "*",  12 },
            { "(",  -1 },
            { ")",  -1 },
            { "%",  13 },
            { "()", 14 },
            { ".",  15 }
        };

        private readonly Dictionary<int, SyntaxKind> syntaxKindLookup = new Dictionary<int, SyntaxKind>()
        {
            { 0 , SyntaxKind.SimpleAssignmentExpression},
            { 1 , SyntaxKind.LogicalOrExpression},
            { 2 , SyntaxKind.LogicalAndExpression},
            { 3 , SyntaxKind.EqualsEqualsToken},
            { 4 , SyntaxKind.NotEqualsExpression},
            { 5 , SyntaxKind.GreaterThanOrEqualExpression},
            { 6 , SyntaxKind.LessThanOrEqualExpression},
            { 7 , SyntaxKind.GreaterThanExpression},
            { 8 , SyntaxKind.LessThanExpression },
            { 9 , SyntaxKind.AddExpression},
            { 10 , SyntaxKind.SubtractExpression},
            { 11 , SyntaxKind.DivideExpression},
            { 12 , SyntaxKind.MultiplyExpression},
            { 13 , SyntaxKind.ModuloExpression},
            { 14 , SyntaxKind.LocalFunctionStatement},
            { 15 , SyntaxKind.DotToken}
        };

        private readonly Dictionary<int, string> terminalKindLookUP = new Dictionary<int, string>()
        {
            { 12, "(" },
            { 13, ")" },
        };

        private class OperatorState
        {
            public SrEval[,] shiftReduceOperationTable = new SrEval[,]
            { /*             assign      OR          AND         EQEQ        NOTEQ       GTEQ        LTEQ        GT          LT          PLUS        MINUS       DIV         MOD         MULT        FUNC        DOT        /*
              /* assn */  {  SrEval.Red, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht, SrEval.Sht},
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

        private class ContextExpressionUnion
        {
            public ParserRuleContext context;
            public ExpressionSyntax syntax;
        }
    }
}
