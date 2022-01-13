using JukaCompiler.Lexer;
using JukaCompiler.Parse;
using JukaCompiler.RoslynEmiter;

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
        }
        internal abstract R Accept<R>(Stmt.Visitor<R> vistor);

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
            internal Lexeme name;
            internal List<TypeParameterMap> typeParameterMaps;

            internal Function(Lexeme name, List<TypeParameterMap> parametersMap, List<Stmt> body)
            {
                if (!(name.ToString().All(c => char.IsLetterOrDigit(c) || c == '_')))
                {
                    throw new Exception("Function {name.ToString()} has an invalid name");
                }
                this.name = name;
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
        }
        internal class Class : Stmt
        {
            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
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
                    throw new Exception("Variable {name.ToString()} has an invalid name");
                }
                this.name = name;
                this.exprInitializer = expr;
                this.isInitalizedVar = true;

            }

            internal Var(Lexeme name)
            {
                if (!(name.ToString().All(c => char.IsLetterOrDigit(c) || c == '_')))
                {
                    throw new Exception("Variable {name.ToString()} has an invalid name");
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
            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
            }
        }
        internal class Return : Stmt
        {
            internal Lexeme keyword;
            internal Parse.Expression expr;
            internal Return(Lexeme keyword, Parse.Expression expression)
            {
                this.keyword = keyword;
                this.expr = expression;
            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                return vistor.VisitReturnStmt(this);
            }
        }
    }
}
