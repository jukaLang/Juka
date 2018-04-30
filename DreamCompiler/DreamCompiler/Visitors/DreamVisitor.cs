using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime.Tree;
using DreamCompiler.Tokens;


namespace DreamCompiler.Visitors
{
    using DreamCompiler.Grammar;

    internal class DreamVisitor : DreamGrammarBaseVisitor<Token>
    {
        public override Token VisitCompileUnit(DreamGrammarParser.CompileUnitContext context)
        {
            Token compileUnit = base.VisitCompileUnit(context);
            return compileUnit;
        }

        public override Token VisitStatement(DreamGrammarParser.StatementContext context)
        {
            return base.VisitStatement(context);
        }

        public override Token VisitAdd(DreamGrammarParser.AddContext context)
        {
            return base.VisitAdd(context);
        }

        public override Token VisitAssignmentExpression(DreamGrammarParser.AssignmentExpressionContext context)
        {
            return base.VisitAssignmentExpression(context);
        }

        public override Token VisitAssignmentOperator(DreamGrammarParser.AssignmentOperatorContext context)
        {
            return base.VisitAssignmentOperator(context);
        }

        public override Token VisitAssignmentOperatorExpression(DreamGrammarParser.AssignmentOperatorExpressionContext context)
        {
            return base.VisitAssignmentOperatorExpression(context);
        }

        public override Token VisitBinaryExpression(DreamGrammarParser.BinaryExpressionContext context)
        {
            return base.VisitBinaryExpression(context);
        }

        public override Token VisitBooleanExpression(DreamGrammarParser.BooleanExpressionContext context)
        {
            return base.VisitBooleanExpression(context);
        }

        public override Token VisitClassifications(DreamGrammarParser.ClassificationsContext context)
        {
            return base.VisitClassifications(context);
        }

        public override Token VisitLiteral(DreamGrammarParser.LiteralContext context)
        {
            return base.VisitLiteral(context);
        }

        public override Token Visit(IParseTree tree)
        {
            return base.Visit(tree);
        }

        public override Token VisitEqualsign(DreamGrammarParser.EqualsignContext context)
        {
            return base.VisitEqualsign(context);
        }

        public override Token VisitEqualequal(DreamGrammarParser.EqualequalContext context)
        {
            return base.VisitEqualequal(context);
        }

        public override Token VisitEqualityExpression(DreamGrammarParser.EqualityExpressionContext context)
        {
            return base.VisitEqualityExpression(context);
        }

        public override Token VisitExpression(DreamGrammarParser.ExpressionContext context)
        {
            return base.VisitExpression(context);
        }

        public override Token VisitEndLine(DreamGrammarParser.EndLineContext context)
        {
            return base.VisitEndLine(context);
        }

        public override Token VisitEvaluatable(DreamGrammarParser.EvaluatableContext context)
        {
            return base.VisitEvaluatable(context);
        }

        public override Token VisitTerminal(ITerminalNode node)
        {
            base.VisitTerminal(node);
            NameToken token = new NameToken( node.GetText() );
            return token;
        }

        public override Token VisitExpressionSequence(DreamGrammarParser.ExpressionSequenceContext context)
        {
            return base.VisitExpressionSequence(context);
        }

        public override Token VisitFuncName(DreamGrammarParser.FuncNameContext context)
        {
            SymbolToken functionName = new SymbolToken(TokenKind.Name, context.GetText());
            //base.VisitFuncName(context);
            return functionName;
        }

        public override Token VisitFunctionDeclaration(DreamGrammarParser.FunctionDeclarationContext context)
        {
            return base.VisitFunctionDeclaration(context);
        }

    }
}
