using JukaCompiler.Lexer;

namespace JukaCompiler.Parse
{
    internal abstract class Expression
    {
        internal interface IVisitor<R>
        {
            R VisitAssignExpr(Assign expr);
            R VisitBinaryExpr(Binary expr);
            R VisitCallExpr(Call expr);
            R VisitGetExpr(Get expr);
            R VisitGroupingExpr(Grouping expr);
            R VisitLiteralExpr(Literal expr);
            R VisitLogicalExpr(Logical expr);
            R VisitSetExpr(Set expr);
            R VisitSuperExpr(Super expr);
            R VisitThisExpr(This expr);
            R VisitUnaryExpr(Unary expr);
            R VisitVariableExpr(Variable expr);
            R VisitLexemeTypeLiteral(LexemeTypeLiteral expr);
            R VisitArrayExpr(ArrayDeclarationExpression expr);
            R VisitArrayAccessExpr(ArrayAccessExpression expr);
        }

        internal abstract R Accept<R>(Expression.IVisitor<R> visitor);

        private Lexeme? expressionLexeme;
        internal Lexeme? initializerContextVariableName;

        internal Lexeme? ExpressionLexeme
        {
            get => expressionLexeme;
            set { expressionLexeme = value; }
        }

        internal string ExpressionLexemeName
        {
            get => this.expressionLexeme.ToString();
        }

        internal class Assign : Expression
        {
            internal readonly Expression value;

            internal Assign(Lexeme ExpressionLexeme, Expression value)
            {
                this.expressionLexeme = ExpressionLexeme;
                this.value = value;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitAssignExpr(this);
            }
        }
        
        internal class Variable : Expression
        {
            internal Int64 lexemeType;

            internal Variable(Lexeme ExpressionLexeme)
            {
                this.ExpressionLexeme = ExpressionLexeme;
                this.lexemeType = ExpressionLexeme.LexemeType;
            }
            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }
        }
        
        internal class Binary : Expression
        {
            internal Expression? left;
            internal Lexeme? op;
            internal Expression? right;

            internal Binary(Expression expr, Lexeme op, Expression right)
            {
                this.left = expr;
                this.op = op;
                this.right = right;
            }

            internal Binary()
            {
                this.left=null;
                this.right=null;
                this.op=null;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }

        internal class ArrayDeclarationExpression : Expression
        {
            internal int ArraySize { get; }
            internal Lexeme ArrayDeclarationName { get; }

            internal ArrayDeclarationExpression(int size)
            {
                this.ArraySize = size;
            }

            internal ArrayDeclarationExpression(Lexeme arrayDeclarationName, Lexeme arraySize)
            {
                ArraySize = int.Parse(arraySize.ToString());
                ExpressionLexeme = arrayDeclarationName = arrayDeclarationName;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitArrayExpr(this);
            }
        }

        internal class ArrayAccessExpression : Expression
        {
            internal int ArraySize { get; }
            internal Lexeme ArrayVariableName { get; }

            internal ArrayAccessExpression(Lexeme arrayVariableName, Lexeme arraySize)
            {
                ArraySize = int.Parse(arraySize.ToString());
                ExpressionLexeme = ArrayVariableName = arrayVariableName;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitArrayAccessExpr(this);
            }
        }
        
        internal class Call : Expression
        {
            internal Expression callee;
            internal List<Expression> arguments;
            internal bool isJukaCallable = false;

            internal Call(Expression callee, bool isCallable, List<Expression> arguments)
            {
                this.callee = callee;
                this.arguments = arguments;
                this.isJukaCallable = isCallable;
            }
            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitCallExpr(this);
            }
        }

        internal class Get : Expression
        {
            internal Expression expr;
            internal Get(Expression expr, Lexeme lex)
            {
                this.expr = expr;
                this.ExpressionLexeme = lex;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitGetExpr(this);
            }
        }

        internal class Unary : Expression
        {
            internal Unary(Expression expr, Lexeme lex, Expression right)
            {
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                throw new NotImplementedException();
            }
        }

        internal class Grouping : Expression
        {
            internal Expression? expression;

            internal Grouping(Expression expr)
            {
                this.expression = expr;
            }

            internal Grouping()
            {
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
        }

        internal class Literal : Expression
        {
            private readonly object? value;
            private readonly long type;

            internal Literal(Lexeme literal, long type)
            {
                this.ExpressionLexeme = literal;
                this.value = literal;
                this.type = type;
            }
            internal Literal()
            {
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }

            internal object? LiteralValue()
            {
                if (value == null)
                {
                    throw new ArgumentNullException("literal");
                }

                LexemeTypeLiteral? v = new()
                {
                    lexemeType = this.Type,
                    literal = this.value.ToString()
                };

                return v;
                //return literal;
            }

            internal long Type
            {
                get { return type; }
            }
        }

        internal class Logical : Expression
        {
            internal Logical(Expression expr, Lexeme lex, Expression right)
            {
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                throw new NotImplementedException();
            }
        }

        internal class Set : Expression
        {
            internal Set(Expression expr, Lexeme lex, Expression right)
            {
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                throw new NotImplementedException();
            }
        }

        internal class Super : Expression
        {
            internal Super(Expression expr, Lexeme lex, Expression right)
            {

            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                throw new NotImplementedException();
            }
        }

        internal class This : Expression
        {
            internal This(Expression expr, Lexeme lex, Expression right)
            {
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                throw new NotImplementedException();
            }
        }

        internal class DefaultExpression : Expression
        {
            internal DefaultExpression()
            { }
            internal override R Accept<R>(IVisitor<R> visitor)
            {
                throw new NotImplementedException();
            }
        }

        internal class LexemeTypeLiteral : Expression
        {
            internal long lexemeType;
            internal object? literal;

            internal Object Literal
            { get { return this.literal; } } 

            internal long LexemeType
            { get { return this.lexemeType;} }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitLexemeTypeLiteral(this);
            }
        }
    }
}
