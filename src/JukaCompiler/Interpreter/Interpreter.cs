using JukaCompiler.Parse;
using JukaCompiler.Statements;

namespace JukaCompiler.Interpreter
{
    internal class Interpreter : Stmt.Visitor<Stmt>, Expression.Visitor<object>
    {
        internal void Interpert(List<Stmt> statements)
        {
            foreach(var stmt in statements)
            {
                Execute(stmt);
            }
        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        Stmt Stmt.Visitor<Stmt>.VisitBlockStmt(Stmt.Block stmt)
        {
            throw new NotImplementedException();
        }

        Stmt Stmt.Visitor<Stmt>.VisitFunctionStmt(Stmt.Function stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt VisitClassStmt(Stmt.Class stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt VisitExpressionStmt(Parse.Expression stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt VisitIfStmt(Stmt.If stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt VisitPrintStmt(Stmt.Print stmt)
        {
            Console.WriteLine(Evaluate(stmt.expr));
            return null;
        }

        public Stmt VisitReturnStmt(Stmt.Return stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt VisitVarStmt(Stmt.Var stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt VisitWhileStmt(Stmt.While stmt)
        {
            throw new NotImplementedException();
        }

        private object Evaluate(Expression expr)
        {
            return expr.Accept(this);
        }

        public object VisitAssignExpr(Expression.Assign expr)
        {
            throw new NotImplementedException();
        }

        public object VisitBinaryExpr(Expression.Binary expr)
        {
            throw new NotImplementedException();
        }

        public object VisitCallExpr(Expression.Call expr)
        {
            throw new NotImplementedException();
        }

        public object VisitGetExpr(Expression.Get expr)
        {
            throw new NotImplementedException();
        }

        public object VisitGroupingExpr(Expression.Grouping expr)
        {
            return Evaluate(expr.expression);
        }

        public object VisitLiteralExpr(Expression.Literal expr)
        {
            return expr.LiteralValue();
        }

        public object VisitLogicalExpr(Expression.Logical expr)
        {
            throw new NotImplementedException();
        }

        public object VisitSetExpr(Expression.Set expr)
        {
            throw new NotImplementedException();
        }

        public object VisitSuperExpr(Expression.Super expr)
        {
            throw new NotImplementedException();
        }

        public object VisitThisExpr(Expression.This expr)
        {
            throw new NotImplementedException();
        }

        public object VisitUnaryExpr(Expression.Unary expr)
        {
            throw new NotImplementedException();
        }

        public object VisitVariableExpr(Expression.Variable expr)
        {
            throw new NotImplementedException();
        }
    }
}
