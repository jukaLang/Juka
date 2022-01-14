using JukaCompiler.Lexer;

namespace JukaCompiler.Parse
{
    internal abstract class Expression
    {
        internal interface Visitor<R>
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
        }

        internal abstract R Accept<R>(Expression.Visitor<R> vistor);

        internal Lexeme? name;

        internal Lexeme? Name
        {
            get { return name; }
            set { name = value; }
        }

        internal class Assign : Expression
        {
            private Expression value;

            internal Assign(Lexeme name, Expression value)
            {
                this.name = name;
                this.value = value;
            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                return vistor.VisitAssignExpr(this);
            }
        }
        
        internal class Variable : Expression
        {
            internal Variable(Lexeme name)
            {
                this.name = name;
            }
            internal override R Accept<R>(Visitor<R> visitor)
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

            internal override R Accept<R>(Visitor<R> vistor)
            {
                return vistor.VisitBinaryExpr(this);
            }
        }
        
        internal class Call : Expression
        {
            internal Expression callee;
            internal Lexeme paren;
            internal List<Expression> arguments;

            internal Call(Expression callee, Lexeme paren, List<Expression> arguments)
            {
                this.callee = callee;
                this.paren = paren;
                this.arguments = arguments;
            }
            internal override R Accept<R>(Visitor<R> vistor)
            {
                return vistor.VisitCallExpr(this);
            }
        }

        internal class Get : Expression
        {
            internal Get(Expression expr, Lexeme lex, Expression right)
            {

            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
            }
        }

        internal class Unary : Expression
        {
            internal Unary(Expression expr, Lexeme lex, Expression right)
            {
            }

            internal override R Accept<R>(Visitor<R> vistor)
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

            internal override R Accept<R>(Visitor<R> vistor)
            {
                return vistor.VisitGroupingExpr(this);
            }
        }

        internal class Literal : Expression
        {
            private object? value;
            private long type;

            internal Literal(Lexeme literal, long type)
            {
                this.Name = literal;
                this.value = literal;
                this.type = type;
            }
            internal Literal()
            {
            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                return vistor.VisitLiteralExpr(this);
            }

            internal object LiteralValue()
            {
                if (value == null)
                {
                    throw new ArgumentNullException("literal");
                }

                var v = new LexemeTypeLiteral();
                v.lexemeType = this.Type;
                v.literal = this.value.ToString();

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

            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
            }
        }

        internal class Set : Expression
        {
            internal Set(Expression expr, Lexeme lex, Expression right)
            {
            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
            }
        }

        internal class Super : Expression
        {
            internal Super(Expression expr, Lexeme lex, Expression right)
            {

            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
            }
        }

        internal class This : Expression
        {
            internal This(Expression expr, Lexeme lex, Expression right)
            {
            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
            }
        }

        internal class DefaultExpression : Expression
        {
            internal DefaultExpression()
            { }
            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
            }
        }

        internal class LexemeTypeLiteral
        {
            internal long lexemeType;
            internal object? literal;

            internal Object Literal
            { get { return this.literal; } } 

            internal long LexemeType
            { get { return this.lexemeType;} }
        }
    }
}
