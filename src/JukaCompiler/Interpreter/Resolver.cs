using JukaCompiler.Lexer;
using JukaCompiler.Parse;
using JukaCompiler.Statements;

namespace JukaCompiler.Interpreter
{
    internal class Resolver : Stmt.Visitor<object>, Expression.Visitor<object>
    {

        private Interpreter interpreter;
        private FunctionType currentFunction = FunctionType.NONE;
        private  Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();

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

        private void Resolve(Expression expr)
        {
            expr.Accept(this);
        }

        public object VisitAssignExpr(Expression.Assign expr)
        {
            throw new NotImplementedException(); 
        }

        public object VisitBinaryExpr(Expression.Binary expr)
        {
            Resolve(expr.left);
            Resolve(expr.right);
            return null;
        }

        public object VisitBlockStmt(Stmt.Block stmt)
        {
            throw new NotImplementedException();
        }

        public object VisitCallExpr(Expression.Call expr)
        {
            throw new NotImplementedException();
        }

        public object VisitClassStmt(Stmt.Class stmt)
        {
            throw new NotImplementedException();
        }

        public object VisitExpressionStmt(Parse.Expression stmt)
        {
            throw new NotImplementedException();
        }

        public object VisitFunctionStmt(Stmt.Function stmt)
        {
            throw new NotImplementedException();
        }

        public object VisitGetExpr(Expression.Get expr)
        {
            throw new NotImplementedException();
        }

        public object VisitGroupingExpr(Expression.Grouping expr)
        {
            Resolve(expr.expression);
            return null;
        }

        public object VisitIfStmt(Stmt.If stmt)
        {
            throw new NotImplementedException();
        }

        public object VisitLiteralExpr(Expression.Literal expr)
        {
            return null;
        }

        public object VisitLogicalExpr(Expression.Logical expr)
        {
            throw new NotImplementedException();
        }

        public object VisitPrintStmt(Stmt.Print stmt)
        {
            Resolve(stmt.expr);
            return null;
        }

        public object VisitReturnStmt(Stmt.Return stmt)
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

        public object VisitVarStmt(Stmt.Var stmt)
        {
            Declare(stmt.name);
            if (stmt.isInitalizedVar && stmt.exprInitializer != null)
            {
                Resolve(stmt.exprInitializer);
            }

            Define(stmt.name);
            return null;
        }

        public object VisitWhileStmt(Stmt.While stmt)
        {
            throw new NotImplementedException();
        }

        private void Declare(Lexeme name)
        {
            if (this.scopes.Count() == 0)
            {
                return;
            }

            Dictionary<string,bool> scope = this.scopes.Peek();

            if (scope != null && scope.ContainsKey(name.ToString()))
            {
                throw new Exception("variable exist, - need to add to internal error log");
            }

            scope?.Add(name.ToString(), false);
        }

        private void Define(Lexeme name)
        {
            if (scopes.Count() == 0)
            {
                return;
            }

            scopes.Peek().Add(name.ToString(), true);
        }
    }
}
