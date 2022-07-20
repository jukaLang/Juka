using JukaCompiler.Exceptions;
using JukaCompiler.Lexer;
using JukaCompiler.Parse;
using JukaCompiler.Statements;
using Microsoft.Extensions.DependencyInjection;

namespace JukaCompiler.Interpreter
{
    internal class Resolver : Stmt.Visitor<object>, Expression.IVisitor<object>
    {
        private JukaInterpreter interpreter;
        private FunctionType currentFunction = FunctionType.NONE;
        // private ClassType currentClass = ClassType.NONE;
        private ServiceProvider? ServiceProvider;
        private Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();
        private Stack<StackFrame> frames = new Stack<StackFrame>();
        //Dictionary<string,bool>> processScope = new Stack<string, Dictionary<string, bool>>();
        private ICompilerError? compilerError;

        private enum FunctionType
        {
            NONE,
            FUNCTION,
            INITIALIZER,
            METHOD
        }

        private enum ClassType
        {
            NONE,
            CLASS,
            SUBCLASS
        }

        internal Resolver(JukaInterpreter interpreter)
        {
            this.interpreter = interpreter;

            this.ServiceProvider = interpreter.ServiceProvider;
            if(this.ServiceProvider != null)
            { 
                this.compilerError = this.ServiceProvider?.GetService<ICompilerError>();
            }
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
            throw new NotImplementedException("Resolver assign is not implemented"); 
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
            return new Stmt.DefaultStatement();
        }

        public object VisitCallExpr(Expression.Call expr)
        {
            if (expr is Expression.Call)
            {
                var call = (Expression.Call)expr;
                if (!call.isJukaCallable)
                { 
                    frames.Push(new StackFrame(expr.callee.Name.ToString()));
                    Declare(expr.callee.Name);
                    Resolve(expr.callee);
                    Define(expr.callee.Name);
                    frames.Pop();
                }
                else
                {
                    frames.Push(new StackFrame(expr.callee.Name.ToString()));
                    Declare(expr.callee.Name);
                    Define(expr.callee.Name);
                    frames.Pop();
                }
            }



            foreach(Expression arg in expr.arguments)
            {
                Resolve(arg);
            }

            return new Stmt.DefaultStatement();
        }

        public object VisitClassStmt(Stmt.Class stmt)
        {
            throw new NotImplementedException("Resolver VisitClassStmt is not implemented");
        }

        public object VisitExpressionStmt(Stmt.Expression stmt)
        {
            Resolve(stmt.expression);
            return new Stmt.DefaultStatement();
        }

        public object VisitFunctionStmt(Stmt.Function stmt)
        {
            Declare(stmt.name);
            Define(stmt.name);

            ResolveFunction(stmt, FunctionType.FUNCTION);
            return new Stmt.DefaultStatement();
        }

        public object VisitLexemeTypeLiteral(Expression.LexemeTypeLiteral expr)
        {
            //return new Stmt.DefaultStatement();
            return new Stmt.DefaultStatement(); 
        }

        public object VisitGetExpr(Expression.Get expr)
        {
            throw new NotImplementedException("Resolver VisitGetExpr is not implemented");
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

            return new Stmt.DefaultStatement();
        }

        public object VisitLiteralExpr(Expression.Literal expr)
        {
            return new Expression.Literal();
        }

        public object VisitLogicalExpr(Expression.Logical expr)
        {
            throw new NotImplementedException("Resolver VisitLogicalExpr is not implemented");
        }

        public object VisitPrintLine(Stmt.PrintLine stmt)
        {
            if (stmt == null || stmt.expr == null)
            {
                throw new ArgumentNullException("stmt and or expression are null");
            }

            Resolve(stmt.expr);
            return new Stmt.PrintLine();
        }

        public object VisitPrint(Stmt.Print stmt)
        {
            if (stmt == null || stmt.expr == null)
            {
                throw new ArgumentNullException("stmt and or expression are null");
            }

            Resolve(stmt.expr);
            return new Stmt.Print();
        }

        public object VisitReturnStmt(Stmt.Return stmt)
        {
            if (currentFunction == FunctionType.NONE)
            {
                this.compilerError?.AddError("Can't reach return. No function defined");
            }

            if (stmt.expr != null)
            {
                if (currentFunction == FunctionType.INITIALIZER)
                {
                    this.compilerError?.AddError("Can't return from an initializer function");
                }

                Resolve(stmt.expr);
            }

            return new Stmt.DefaultStatement();
        }

        public object VisitBreakStmt(Stmt.Break stmt)
        {
            Stmt.Return returnStatement = new Stmt.Return(null,null);
            return VisitReturnStmt(returnStatement);
        }

        public object VisitSetExpr(Expression.Set expr)
        {
            throw new NotImplementedException("Resolver VisitSetExpr is not implemented");
        }

        public object VisitSuperExpr(Expression.Super expr)
        {
            throw new NotImplementedException("Resolver VisitSuperExpr is not implemented");
        }

        public object VisitThisExpr(Expression.This expr)
        {
            throw new NotImplementedException("Resolver VisitThisExpr is not implemented");
        }

        public object VisitUnaryExpr(Expression.Unary expr)
        {
            throw new NotImplementedException("Resolver VisitUnaryExpr is not implemented");
        }

        public object VisitVariableExpr(Expression.Variable expr)
        {
            if(( scopes.Any() && scopes.Peek()[expr.Name.ToString()]) ||
                scopes.Any() && scopes.Peek().Count == 0 ||
                !scopes.Any())
            {
                ResolveLocal(expr, expr.Name);
                return new Stmt.DefaultStatement();
            }

            try
            {
                ResolveLocal(expr, expr.Name);
                return new Stmt.DefaultStatement();
            }
            catch
            {
                throw new NotImplementedException("Something went wrong visitng");
            }
        }

        private void ResolveLocal(Expression expr, Lexeme name)
        {
            for(int i = 0; i < scopes.Count(); i++)
            {
                if (scopes.ElementAt(i).ContainsKey(name.ToString()))
                {
                    this.interpreter.Resolve(expr, i);
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
            Resolve(stmt.condition);
            Resolve(stmt.whileBlock);

            return new Stmt.DefaultStatement();
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
            if (!scopes.Any())
            {
                return;
            }
            
            if (scopes.Peek().ContainsKey(name.ToString()))
            {
                scopes.Peek()[name.ToString()] = true;
            }
            else
            {
                scopes.Peek().Add(name.ToString(), false);
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
                    throw new Exception("Something went wrong when resolving the function");
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
