using JukaCompiler.Parse;
using JukaCompiler.Statements;

namespace JukaCompiler.Interpreter
{
    internal class Resolver : Stmt.Visitor<object>, Expression.Visitor<object>
    {

        private Interpreter interpreter;
        private FunctionType currentFunction = FunctionType.NONE;

        private enum FunctionType
        {
            NONE,
            /* Resolving and Binding function-type < Classes function-type-method
                FUNCTION
            */
            //> Classes function-type-method
            FUNCTION,
            //> function-type-initializer
            INITIALIZER,
            //< function-type-initializer
            METHOD
            //< Classes function-type-method
        }

        private enum ClassType
        {
            NONE,
            /* Classes class-type < Inheritance class-type-subclass
                CLASS
             */
            //> Inheritance class-type-subclass
            CLASS,
            SUBCLASS
            //< Inheritance class-type-subclass
        }

        private ClassType currentClass = ClassType.NONE;

        internal Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        internal void Resolve(List<Stmt> statements)
        {
            foreach(Stmt stmt in statements)
            {
                Resolve(stmt);
            }
        }

        private void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        public object VisitAssignExpr(Expression.Assign expr)
        {
            throw new NotImplementedException();
        }

        public object VisitBinaryExpr(Expression.Binary expr)
        {
            throw new NotImplementedException();
        }

        public object visitBlockStmt(Stmt.Block stmt)
        {
            throw new NotImplementedException();
        }

        public object VisitCallExpr(Expression.Call expr)
        {
            throw new NotImplementedException();
        }

        public object visitClassStmt(Stmt.Class stmt)
        {
            throw new NotImplementedException();
        }

        public object visitExpressionStmt(Parse.Expression stmt)
        {
            throw new NotImplementedException();
        }

        public object visitFunctionStmt(Stmt.Function stmt)
        {
            throw new NotImplementedException();
        }

        public object VisitGetExpr(Expression.Get expr)
        {
            throw new NotImplementedException();
        }

        public object VisitGroupingExpr(Expression.Grouping expr)
        {
            throw new NotImplementedException();
        }

        public object visitIfStmt(Stmt.If stmt)
        {
            throw new NotImplementedException();
        }

        public object VisitLiteralExpr(Expression.Literal expr)
        {
            throw new NotImplementedException();
        }

        public object VisitLogicalExpr(Expression.Logical expr)
        {
            throw new NotImplementedException();
        }

        public object visitPrintStmt(Stmt.Print stmt)
        {
            throw new NotImplementedException();
        }

        public object visitReturnStmt(Stmt.Return stmt)
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

        public object visitVarStmt(Stmt.Var stmt)
        {
            throw new NotImplementedException();
        }

        public object visitWhileStmt(Stmt.While stmt)
        {
            throw new NotImplementedException();
        }
    }
}
