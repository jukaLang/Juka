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

        protected Lexeme name;

        internal Lexeme Name
        {
            get
            {
                return name;
            }
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
            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
            }
        }

        internal class Binary : Expression
        {
            internal Binary(Expression expr, Lexeme lex, Expression right)
            {

            }
            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
            }
        }

        internal class Call : Expression
        {
            internal Call(Expression expr, Lexeme lex, Expression right)
            {

            }
            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
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
            internal Grouping(Expression expr, Lexeme lex, Expression right)
            {

            }
            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
            }
        }

        internal class Literal : Expression
        {
            internal Literal(Expression expr, Lexeme lex, Expression right)
            {

            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
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
    }
}
