using JukaCompiler.Lexer;
using JukaCompiler.Parse;
using JukaCompiler.Statements;
using System.Net.Mail;

namespace JukaCompiler.Interpreter
{
    internal class Resolver : Stmt.Visitor<object>, Expression.Visitor<object>
    {
        private JukaInterpreter interpreter;
        private FunctionType currentFunction = FunctionType.NONE;
        // private ClassType currentClass = ClassType.NONE;
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

        internal Resolver(JukaInterpreter interpreter)
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
            if (expr != null && expr.left != null && expr.right != null)
            {
                Resolve(expr.left);
                Resolve(expr.right);
            }
            return new Expression.Binary();
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
            Declare(stmt.name);
            Define(stmt.name);

            ResolveFunction(stmt, FunctionType.FUNCTION);
            return null;
        }

        public object VisitGetExpr(Expression.Get expr)
        {
            throw new NotImplementedException();
        }

        public object VisitGroupingExpr(Expression.Grouping expr)
        {
            if (expr == null || expr.expression == null)
            {
                throw new ArgumentNullException("grouping has some null stuff");
            }

            Resolve(expr.expression);
            return new Expression.Grouping();
        }

        public object VisitIfStmt(Stmt.If stmt)
        {
            throw new NotImplementedException();
        }

        public object VisitLiteralExpr(Expression.Literal expr)
        {
            return new Expression.Literal();
        }

        public object VisitLogicalExpr(Expression.Logical expr)
        {
            throw new NotImplementedException();
        }

        public object VisitPrintStmt(Stmt.Print stmt)
        {
            if (stmt == null || stmt.expr == null)
            {
                throw new ArgumentNullException("stmt and or expressoin are null");
            }

            Resolve(stmt.expr);
            return new Stmt.Print();
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
            if (stmt==null || stmt.name == null)
            {
                throw new ArgumentNullException("new vist stmt == null");
            }

            Declare(stmt.name);

            if (stmt.isInitalizedVar && stmt.exprInitializer != null)
            {
                Resolve(stmt.exprInitializer);
            }

            Define(stmt.name);
            return new Stmt.Var();
        }

        public object VisitWhileStmt(Stmt.While stmt)
        {
            throw new NotImplementedException();
        }

        private void Declare(Lexeme name)
        {
            if (scopes.Count() == 0)
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
            
            if (scopes.Peek().ContainsKey(name.ToString()))
            {
                scopes.Peek()[name.ToString()] = true;
            }
            else
            {
                scopes.Peek().Add(name.ToString(), true);
            }
        }

        private void ResolveFunction(Stmt.Function function, FunctionType type)
        {
            FunctionType enclosingFunction = currentFunction;
            currentFunction = type;

            //< set-current-function
            BeginScope();
            foreach (var param in function.typeParameterMaps)
            {
                var literalName = param.parameterName as Expression.Literal;
                if (literalName == null)
                {
                    // throw something;
                }
                Declare(literalName.Name);
                Define(param.parameterType);
            }
            Resolve(function.body);
            EndScope();
            //> restore-current-function
            currentFunction = enclosingFunction;
            //< restore-current-function
        }

        private void BeginScope()
        {
            scopes.Push(new Dictionary<string, bool>());
        }
        //< begin-scope
        //> end-scope
        private void EndScope()
        {
            scopes.Pop();
        }
    }
}
