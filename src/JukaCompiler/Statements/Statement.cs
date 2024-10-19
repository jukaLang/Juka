using JukaCompiler.Expressions;
using JukaCompiler.Lexer;
using JukaCompiler.Parse;

namespace JukaCompiler.Statements
{
    internal abstract class Statement
    {
        internal interface IVisitor<R>
        {
            R VisitBlockStmt(Block statement);
            R VisitFunctionStmt(Function statement);
            R VisitClassStmt(Class statement);
            R VisitExpressionStmt(Expression statement);
            R VisitIfStmt(If statement);
            R VisitPrintLine(PrintLine statement);
            R VisitPrint(Print statement);
            R VisitReturnStmt(Return statement);
            R VisitVarStmt(Var statement);
            R VisitWhileStmt(While statement);
            R VisitBreakStmt(Break statement);

            R VisitForStmt(For statement);
            object VisitArrayExpr(Expr.ArrayDeclarationExpr expression);
        }
        internal abstract R Accept<R>(IVisitor<R> vistor);
        private Lexeme stmtLexeme = new();

        internal Lexeme StmtLexeme
        {
            get => stmtLexeme;
            set => stmtLexeme = value;
        }

        internal string StmtLexemeName
        {
            get => stmtLexeme.ToString();
        }

        internal class Block : Statement
        {
            internal Block(List<Statement> statements) => this.statements = statements;

            internal List<Statement> statements;
            internal override R Accept<R>(IVisitor<R> vistor) => vistor.VisitBlockStmt(this);
        }
        internal class Function : Statement
        {
            internal List<Statement>? body = [];
            internal List<TypeParameterMap> typeParameterMaps;

            public override bool Equals(object? obj) => StmtLexemeName.Equals(obj);
            internal Function(Lexeme stmtLexeme, List<TypeParameterMap> parametersMap, List<Statement> body)
            {
                if (!stmtLexeme.ToString().All(c => char.IsLetterOrDigit(c) || c == '_'))
                {
                    throw new Exception("Function {ExpressionLexeme.ToString()} has an invalid ExpressionLexeme");
                }
                StmtLexeme = stmtLexeme;
                typeParameterMaps = parametersMap;
                this.body = body;
            }

            internal override R Accept<R>(IVisitor<R> vistor) => vistor.VisitFunctionStmt(this);

            internal List<TypeParameterMap> Params => typeParameterMaps;

            public override int GetHashCode() => base.GetHashCode();
        }
        internal class Class : Statement
        {
            internal Lexeme name;
            internal List<Function> methods;
            internal List<Statement> variableDeclarations;
            internal Expr.Variable? superClass;

            internal Class(Lexeme name, List<Function> methods, List<Statement> variableDeclarations)
            {
                this.name = name;
                this.methods = methods;
                this.variableDeclarations = variableDeclarations;
                this.superClass = null;
            }

            internal override R Accept<R>(IVisitor<R> vistor)
            {
                return vistor.VisitClassStmt(this);
            }
        }
        internal class Expression : Statement
         {
            internal Expr Expr;

            internal Expression(Expr expr)
            {
                Expr = expr;
            }

             internal override R Accept<R>(IVisitor<R> vistor)
             {
                 return vistor.VisitExpressionStmt(this);
             }
        }
        internal class If : Statement
        {
            internal Expr condition;
            internal Statement thenBranch;
            internal Statement elseBranch;

            internal If(Expr condition, Statement thenBranch, Statement elseBranch)
            {
                this.condition = condition;
                this.thenBranch = thenBranch;
                this.elseBranch = elseBranch;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitIfStmt(this);
            }

        }
        internal class PrintLine : Statement
        {
            internal Expr? expr;

            internal PrintLine(Expr expr)
            {
                this.expr = expr;
            }

            internal PrintLine()
            {
                expr = null;
            }

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitPrintLine(this);
            }
        }
        internal class Print : Statement
        {
            internal Expr? expr;

            internal Print(Expr expr)
            {
                this.expr = expr;
            }

            internal Print()
            {
                expr = null;
            }

            internal override R Accept<R>(IVisitor<R> vistor)
            {
                return vistor.VisitPrint(this);
            }
        }
        internal class Var : Statement
        {
            internal Lexeme? name;
            internal Expr? exprInitializer;
            internal bool isInitalizedVar = false;

            internal Var(Lexeme name, Expr expr)
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

            internal override R Accept<R>(IVisitor<R> vistor)
            {
                return vistor.VisitVarStmt(this);
            }
        }

        internal class While : Statement
        {
            internal Expr condition;
            internal Statement whileBlock;

            internal While(Expr condition, Statement whileBlock)
            {
                this.condition = condition;
                this.whileBlock = whileBlock;
            }

            internal override R Accept<R>(IVisitor<R> vistor)
            {
                return vistor.VisitWhileStmt(this);
            }
        }

        internal class For : Statement
        {
            private Var init;
            private Expr breakExpr;
            private Expr incExpr;
            private Statement forBody;

            internal For(Statement.Var init, Expr breakExpr, Expr incExpr, Statement forBody)
            {
                this.init = init;
                this.breakExpr = breakExpr;
                this.incExpr = incExpr;
                this.forBody = forBody;
            }

            internal Statement.Var Init => init;
            internal Expr BreakExpr => breakExpr;
            internal Expr IncExpr => incExpr;
            internal Statement ForBody => forBody;

            internal override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitForStmt(this);
            }
        }
        internal class Return : Statement
        {
            internal Lexeme? keyword;
            internal Expr? expr;
            internal Return(Lexeme keyword, Expr expr)
            {
                this.keyword = keyword;
                this.expr = expr;
            }

            internal Return()
            {
                expr = null;
                keyword = null;
            }

            internal override R Accept<R>(IVisitor<R> vistor)
            {
                return vistor.VisitReturnStmt(this);
            }
        }
        internal class Break : Statement
        {
            internal Expr expr;

            internal Break(Expr expr)
            {
                this.expr = expr;
            }

            internal override R Accept<R>(IVisitor<R> vistor)
            {
                return vistor.VisitBreakStmt(this);
            }
        }
        internal class DefaultStatement : Statement
        {
            // Does nothing used for return values;
            internal override R Accept<R>(IVisitor<R> vistor)
            {
                throw new NotImplementedException();
            }
        }

        internal class LiteralLexemeExpression : Statement
        {
            internal Expr.LexemeTypeLiteral ltl;

            internal LiteralLexemeExpression(Expr.LexemeTypeLiteral lexemeTypeLiteral)
            {
                ltl = lexemeTypeLiteral;
            }

            internal override R Accept<R>(IVisitor<R> vistor)
            {
                throw new NotImplementedException();
            }
        }
    }
}
