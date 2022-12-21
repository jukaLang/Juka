using JukaCompiler.Lexer;

namespace JukaCompiler.Expressions
{
    internal abstract class Expr
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
            R VisitArrayExpr(ArrayDeclarationExpr expr);

            R VisitNewExpr(NewDeclarationExpr expr);
            R VisitArrayAccessExpr(ArrayAccessExpr expr);
        }

        internal abstract R Accept<R>(Expr.IVisitor<R> visitor);

        private Lexeme? expressionLexeme;
        internal Lexeme? initializerContextVariableName;

        internal Lexeme? ExpressionLexeme
        {
            get => expressionLexeme;
            set { expressionLexeme = value; }
        }
        internal string ExpressionLexemeName
        {
            get => this.expressionLexeme?.ToString()!;
        }
        internal class Assign : Expr
        {
            internal readonly Expr value;

            internal Assign(Lexeme ExpressionLexeme, Expr value)
            {
                this.expressionLexeme = ExpressionLexeme;
                this.value = value;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitAssignExpr(this);
            }
        }
        internal class Variable : Expr
        {
            internal Decimal lexemeType;

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
        internal class Binary : Expr
        {
            internal Expr? left;
            internal Lexeme? op;
            internal Expr? right;

            internal Binary(Expr expr, Lexeme op, Expr right)
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

        internal class Increment : Expr
        {
            internal Lexeme variable;
            internal Lexeme opLexeme;

            internal Increment(Lexeme variable, Lexeme opLexeme)
            {
                this.variable = variable;
                this.opLexeme = opLexeme;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                throw new NotImplementedException();
            }
        }

        internal class NewDeclarationExpr : Expr
        {
            private Expr newDeclarationExprInit;

            internal NewDeclarationExpr(Expr expr)
            {
                this.newDeclarationExprInit = expr;
            }
            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitNewExpr(this);
            }
        }
        internal class ArrayDeclarationExpr : Expr
        {
            internal int ArraySize { get; }
            internal Lexeme ArrayDeclarationName { get; }

            internal ArrayDeclarationExpr(int size)
            {
                this.ArraySize = size;
                this.ArrayDeclarationName = new Lexeme(0,0,0);
            }

            internal ArrayDeclarationExpr(Lexeme arrayDeclarationName, Lexeme arraySize)
            {
                ArraySize = int.Parse(arraySize.ToString());
                ExpressionLexeme = arrayDeclarationName;
                this.ArrayDeclarationName = arrayDeclarationName;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitArrayExpr(this);
            }
        }
        internal class ArrayAccessExpr : Expr
        {
            internal int ArraySize { get; }
            internal Lexeme ArrayVariableName { get; }
            internal Expr LvalueExpr { get; }
            internal bool HasInitalizer { get; }

            internal ArrayAccessExpr(Lexeme arrayVariableName, Lexeme arraySize)
            {
                ArraySize = int.Parse(arraySize.ToString());
                ExpressionLexeme = ArrayVariableName = arrayVariableName;
                HasInitalizer = false;
                LvalueExpr = new DefaultExpr();
            }

            internal ArrayAccessExpr(Lexeme arrayVariableName, Lexeme arraySize, Expr expr)
            {
                ArraySize = int.Parse(arraySize.ToString());
                ExpressionLexeme = ArrayVariableName = arrayVariableName;
                LvalueExpr = expr;
                HasInitalizer = true;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitArrayAccessExpr(this);
            }
        }
        internal class Call : Expr
        {
            internal Expr callee;
            internal List<Expr> arguments;
            internal bool isJukaCallable = false;

            internal Call(Expr callee, bool isCallable, List<Expr> arguments)
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
        internal class Get : Expr
        {
            internal Expr expr;
            internal Get(Expr expr, Lexeme lex)
            {
                this.expr = expr;
                this.ExpressionLexeme = lex;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitGetExpr(this);
            }
        }
        internal class Unary : Expr
        {
            private long lexemeType;
            internal Unary(Lexeme lex, long lexemeType)
            {
                expressionLexeme = lex;
                this.lexemeType = lexemeType;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }

            public long LexemeType => lexemeType;
        }
        internal class Grouping : Expr
        {
            internal Expr? expression;

            internal Grouping(Expr expr)
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
        internal class Literal : Expr
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
        internal class Logical : Expr
        {
            internal Logical(Expr expr, Lexeme lex, Expr right)
            {
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                throw new NotImplementedException();
            }
        }
        internal class Set : Expr
        {
            internal Set(Expr expr, Lexeme lex, Expr right)
            {
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                throw new NotImplementedException();
            }
        }
        internal class Super : Expr
        {
            internal Super(Expr expr, Lexeme lex, Expr right)
            {

            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                throw new NotImplementedException();
            }
        }
        internal class This : Expr
        {
            internal This(Expr expr, Lexeme lex, Expr right)
            {
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                throw new NotImplementedException();
            }
        }
        internal class DefaultExpr : Expr
        {
            internal DefaultExpr()
            { }
            internal override R Accept<R>(IVisitor<R> visitor)
            {
                throw new NotImplementedException();
            }
        }
        internal class LexemeTypeLiteral : Expr
        {
            internal long lexemeType;
            internal object? literal;

            internal new object Literal
            {
                get => literal ?? string.Empty;
                set => literal = value;
            }

            internal long LexemeType
            {
                get => this.lexemeType;
                set => this.lexemeType = value;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitLexemeTypeLiteral(this);
            }
        }
    }
}
