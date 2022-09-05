using JukaCompiler.Lexer;
using JukaCompiler.Parse;
using static JukaCompiler.Expressions.Expression;

namespace JukaCompiler.Statements
{
    internal abstract class Stmt
    {
        internal interface Visitor<R>
        { 
            R VisitBlockStmt(Block stmt);
            R VisitFunctionStmt(Function stmt);
            R VisitClassStmt(Class stmt);
            R VisitExpressionStmt(Expression stmt);
            R VisitIfStmt(If stmt);
            R VisitPrintLine(PrintLine stmt);
            R VisitPrint(Print stmt);
            R VisitReturnStmt(Return stmt);
            R VisitVarStmt(Var stmt);
            R VisitWhileStmt(While stmt);
            R VisitBreakStmt(Break stmt);

            R VisitForStmt(For stmt);
        }
        internal abstract R Accept<R>(Stmt.Visitor<R> vistor);
        private Lexeme stmtLexeme = new Lexeme();

        internal Lexeme StmtLexeme
        {
            get => stmtLexeme;
            set => stmtLexeme = value;
        }

        internal string StmtLexemeName
        {
            get => stmtLexeme.ToString();
        }

        internal class Block : Stmt
        {
            internal Block(List<Stmt> statements)
            {
                this.statements = statements;
            }

            internal List<Stmt> statements;
            internal override R Accept<R>(Visitor<R> vistor)
            {
               return vistor.VisitBlockStmt(this);
            }
        }
        internal class Function : Stmt
        {
            internal List<Stmt> body = new List<Stmt>();
            internal List<TypeParameterMap> typeParameterMaps;

#pragma warning disable CS0659
            public override bool Equals(object? obj)
#pragma warning restore CS0659
            {
                return this.StmtLexemeName.Equals(obj);
            }
            internal Function(Lexeme stmtLexeme, List<TypeParameterMap> parametersMap, List<Stmt> body)
            {
                if (!(stmtLexeme.ToString().All(c => char.IsLetterOrDigit(c) || c == '_')))
                {
                    throw new Exception("Function {ExpressionLexeme.ToString()} has an invalid ExpressionLexeme");
                }
                this.StmtLexeme = stmtLexeme;
                this.typeParameterMaps = parametersMap;
                this.body = body;
            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                return vistor.VisitFunctionStmt(this);
            }

            internal List<TypeParameterMap> Params
            {
                get { return this.typeParameterMaps; }
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
        internal class Class : Stmt
        {
            internal Lexeme name;
            internal List<Stmt.Function> methods;
            internal List<Stmt> variableDeclarations;
            internal Variable? superClass;

            internal Class(Lexeme name, List<Stmt.Function> methods, List<Stmt> variableDeclarations)
            {
                this.name = name;
                this.methods = methods;
                this.variableDeclarations = variableDeclarations;
                this.superClass = null;
            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                return vistor.VisitClassStmt(this);
            }
        }
        internal class Expression : Stmt
         {
            internal Expressions.Expression expression;

            internal Expression(Expressions.Expression expression)
            {
                this.expression = expression;
            }

             internal override R Accept<R>(Visitor<R> vistor)
             {
                 return vistor.VisitExpressionStmt(this);
             }
        }
        internal class If : Stmt
        {
            internal Expressions.Expression condition;
            internal Stmt thenBranch;
            internal Stmt elseBranch;

            internal If(Expressions.Expression condition, Stmt thenBranch, Stmt elseBranch)
            {
                this.condition = condition;
                this.thenBranch = thenBranch;
                this.elseBranch = elseBranch;
            }

            internal override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitIfStmt(this);
            }

        }
        internal class PrintLine : Stmt
        {
            internal Expressions.Expression? expr;

            internal PrintLine(Expressions.Expression expr)
            {
                this.expr = expr;
            }

            internal PrintLine()
            {
            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                return vistor.VisitPrintLine(this);
            }
        }
        internal class Print : Stmt
        {
            internal Expressions.Expression? expr;

            internal Print(Expressions.Expression expr)
            {
                this.expr = expr;
            }

            internal Print()
            {
            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                return vistor.VisitPrint(this);
            }
        }
        internal class Var : Stmt
        {
            internal Lexeme? name;
            internal Expressions.Expression? exprInitializer;
            internal bool isInitalizedVar = false;

            internal Var(Lexeme name, Expressions.Expression expr)
            {
                if (!(name.ToString().All(c => char.IsLetterOrDigit(c) || c == '_')))
                {
                    throw new Exception("Variable {ExpressionLexeme.ToString()} has an invalid ExpressionLexeme");
                }
                this.name = name;
                this.exprInitializer = expr;
                this.exprInitializer.initializerContextVariableName = name;
                this.isInitalizedVar = true;

            }

            internal Var(Lexeme name)
            {
                if (!(name.ToString().All(c => char.IsLetterOrDigit(c) || c == '_')))
                {
                    throw new Exception("Variable {ExpressionLexeme.ToString()} has an invalid ExpressionLexeme");
                }
                this.name = name;
                exprInitializer = null;
            }

            internal Var()
            {
            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                return vistor.VisitVarStmt(this);
            }

        }
        internal class While : Stmt
        {
            internal Expressions.Expression condition;
            internal Stmt whileBlock;

            internal While(Expressions.Expression condition, Stmt whileBlock)
            {
                this.condition = condition;
                this.whileBlock = whileBlock;
            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                return vistor.VisitWhileStmt(this);
            }
        }

        internal class For : Stmt
        {
            private Expressions.Expression init;
            private Expressions.Expression breakExpression;
            private Expressions.Expression incExpression;
            private Stmt forBody;

            internal For(Expressions.Expression init, Expressions.Expression breakExpression, Expressions.Expression incExpression, Stmt forBody)
            {
                this.init = init;
                this.breakExpression = breakExpression;
                this.incExpression = incExpression;
                this.forBody = forBody;
            }

            internal Expressions.Expression Init => init;
            internal Expressions.Expression BreakExpression => breakExpression;
            internal Expressions.Expression IncExpression => incExpression;
            internal Stmt ForBody => forBody;

            internal override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitForStmt(this);
            }
        }
        internal class Return : Stmt
        {
            internal Lexeme? keyword;
            internal Expressions.Expression? expr;
            internal Return(Lexeme keyword, Expressions.Expression expression)
            {
                this.keyword = keyword;
                this.expr = expression;
            }

            internal Return()
            {
            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                return vistor.VisitReturnStmt(this);
            }
        }
        internal class Break : Stmt
        {
            internal Expressions.Expression expr;

            internal Break(Expressions.Expression expr)
            {
                this.expr = expr;
            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                return vistor.VisitBreakStmt(this);
            }
        }
        internal class DefaultStatement : Stmt
        {
            // Does nothing used for return values;
            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
            }
        }

        internal class LiteralLexemeExpression : Stmt
        {
            internal LexemeTypeLiteral ltl;

            internal LiteralLexemeExpression(LexemeTypeLiteral lexemeTypeLiteral)
            {
                this.ltl = lexemeTypeLiteral;
            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
            }
        }
    }
}
