using JukaCompiler.Lexer;
using JukaCompiler.Parse;

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
            internal Parse.Expression.Variable? superClass;

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
            internal Parse.Expression expression;

            internal Expression(Parse.Expression expression)
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
            internal Parse.Expression condition;
            internal Stmt thenBranch;
            internal Stmt elseBranch;

            internal If(Parse.Expression condition, Stmt thenBranch, Stmt elseBranch)
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
            internal Parse.Expression? expr;

            internal PrintLine(Parse.Expression expr)
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
            internal Parse.Expression? expr;

            internal Print(Parse.Expression expr)
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
            internal Parse.Expression? exprInitializer;
            internal bool isInitalizedVar = false;

            internal Var(Lexeme name, Parse.Expression expr)
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
            internal Parse.Expression condition;
            internal Stmt whileBlock;

            internal While(Parse.Expression condition, Stmt whileBlock)
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
            internal For(Parse.Expression init, Parse.Expression breakExpression, Parse.Expression incExpression, Stmt forBody)
            {
                this.Init = init;
                this.BreakExpression = breakExpression;
                this.IncExpression = incExpression;
                this.ForBody = forBody;
            }
            internal Parse.Expression Init { get; }

            internal Parse.Expression BreakExpression { get; }

            internal Parse.Expression IncExpression { get; }

            internal Stmt ForBody { get; }

            internal override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitForStmt(this);
            }
        }
        internal class Return : Stmt
        {
            internal Lexeme? keyword;
            internal Parse.Expression? expr;
            internal Return(Lexeme keyword, Parse.Expression expression)
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
            internal Parse.Expression expr;

            internal Break(Parse.Expression expr)
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
            internal Parse.Expression.LexemeTypeLiteral ltl;

            internal LiteralLexemeExpression(Parse.Expression.LexemeTypeLiteral lexemeTypeLiteral)
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
