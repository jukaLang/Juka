using JukaCompiler.Exceptions;
using JukaCompiler.Lexer;
using JukaCompiler.Parse;
using JukaCompiler.Statements;
using Microsoft.Extensions.DependencyInjection;

namespace JukaCompiler.Interpreter
{
    internal class Resolver : Stmt.Visitor<object>, Expression.Visitor<object>
    {
        private JukaInterpreter interpreter;
        private FunctionType currentFunction = FunctionType.NONE;
        // private ClassType currentClass = ClassType.NONE;
        private ServiceProvider ServiceProvider;
        private  Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();
        private ICompilerError compilerError;

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
            this.ServiceProvider = interpreter.ServiceProvider;
            this.compilerError = ServiceProvider.GetService<ICompilerError>();
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
            BeginScope();
            Resolve(stmt.statements);
            EndScope();
            return null;
        }

        public object VisitCallExpr(Expression.Call expr)
        {
            Resolve(expr.callee);

            foreach(Expression arg in expr.arguments)
            {
                Resolve(arg);
            }

            return null;
        }

        public object VisitClassStmt(Stmt.Class stmt)
        {
            throw new NotImplementedException();
        }

        public object VisitExpressionStmt(Stmt.Expression stmt)
        {
            Resolve(stmt.expression);
            return null;
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
            Resolve(stmt.condition);
            Resolve(stmt.thenBranch);

            if (stmt.elseBranch != null)
            {
                Resolve(stmt.elseBranch);
            }

            return null;
        }

        public object VisitLiteralExpr(Expression.Literal expr)
        {
            return new Expression.Literal();
        }

        public object VisitLogicalExpr(Expression.Logical expr)
        {
            throw new NotImplementedException();
        }

        public object VisitPrintLine(Stmt.PrintLine stmt)
        {
            if (stmt == null || stmt.expr == null)
            {
                throw new ArgumentNullException("stmt and or expressoin are null");
            }

            Resolve(stmt.expr);
            return new Stmt.PrintLine();
        }

        public object VisitPrint(Stmt.Print stmt)
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
            if (currentFunction == FunctionType.NONE)
            {
                this.compilerError.AddError("Can't reach return. No function defined");
            }

            if (stmt.expr != null)
            {
                if (currentFunction == FunctionType.INITIALIZER)
                {
                    this.compilerError.AddError("can't return from an initializer function");
                }

                Resolve(stmt.expr);
            }

            return null;
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
            if (scopes.Count > 1)
            {
                if(scopes.Peek() != null && scopes.Peek().ContainsKey(expr.Name.ToString()))
                {
                    if (scopes.Peek()[expr.Name.ToString()] != false)
                    {
                        this.compilerError.AddError(expr.Name.ToString() + "Can't read local variable");
                    }
                }
            }

            ResolveLocal(expr, expr.Name);
            return null;
        }

        private void ResolveLocal(Expression expr, Lexeme name)
        {
            for(int i = scopes.Count() -1; i >=0; i--)
            {
                Dictionary<string, bool> locals = scopes.ElementAt(i);
                if (locals.ContainsKey(name.ToString()))
                {
                    this.interpreter.Resolve(expr, scopes.Count()-1-i);
                    return;
                }
            }
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
                var literalName = param.parameterName as Expression.Variable;
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
