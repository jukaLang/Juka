using JukaCompiler.Exceptions;
using JukaCompiler.Lexer;
using JukaCompiler.Parse;
using JukaCompiler.Statements;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace JukaCompiler.Interpreter
{
    internal class Resolver : Stmt.Visitor<object>, Expression.IVisitor<object>
    {
        private JukaInterpreter interpreter;
        private FunctionType currentFunction = FunctionType.NONE;
        private ClassType currentClass = ClassType.NONE;
        private ServiceProvider? ServiceProvider;
        private Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();
        Dictionary<string,BlockScope> processScope = new Dictionary<string, BlockScope>();
        private Stack<string> blockScope = new Stack<string>();
        private ICompilerError? compilerError;
        private string errorMessage = "Resolver error - message:{0}";

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
            var frame = blockScope.Peek();
            if (frame == null)
            {
                throw new Exception(string.Format(errorMessage,"No stack frames"));
            }

            if(processScope.TryGetValue(frame, out BlockScope value))
            {
                if (value.lexemeScope.TryGetValue(expr.Name.ToString(), out var lexeme))
                {
                    //value.lexemeScope[expr.name.ToString] = expr.
                    //var xx= interpreter.LookUpVariable(lexeme, expr);
                }
            }
            return new Stmt.DefaultStatement();
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
            //BeginScope(stmt.);
            Resolve(stmt.statements);
            //EndScope();
            return new Stmt.DefaultStatement();
        }

        public object VisitCallExpr(Expression.Call expr)
        {
            if (expr is Expression.Call)
            {
                var call = (Expression.Call)expr;
                BeginScope(expr.callee.Name.ToString());
                Declare(expr.callee.Name);
                Define(expr.callee.Name);

                if (!call.isJukaCallable)
                { 
                    Resolve(expr.callee);
                }
                EndScope();
            }
            return new Stmt.DefaultStatement();
        }

        public object VisitClassStmt(Stmt.Class stmt)
        {
            ClassType enclosingClass = currentClass;
            currentClass = ClassType.CLASS;
            BlockScope blockScope = new BlockScope();

            Declare(stmt.name);
            Define(stmt.name);

            if (stmt.superClass != null && stmt.name.ToString().Equals(stmt.superClass.name.ToString()))
            {
                this.compilerError.AddError("can't inhert from itself");
            }

            if (stmt.superClass != null)
            {
                currentClass = ClassType.SUBCLASS;
                Resolve(stmt.superClass);
            }

            if(stmt.superClass != null)
            {
                BeginScope(stmt.name.ToString());
                scopes.Peek().Add("super",true);
                
                processScope.Add("super", blockScope);
            }

            BeginScope(stmt.name.ToString());
            scopes.Peek().Add("this", true);
            processScope.Add("this", blockScope);

            foreach (Stmt.Function method in stmt.methods)
            {
                //FunctionType decl = FunctionType.METHOD;
                //implement ctor
                // if(method.name.ToString

                ResolveFunction(method, FunctionType.METHOD);
            }

            EndScope();
            if(stmt.superClass != null)
            {
                EndScope();
            }

            currentClass = enclosingClass;
            return null;
        }

        public object VisitExpressionStmt(Stmt.Expression stmt)
        {
            Resolve(stmt.expression);
            return new Stmt.DefaultStatement();
        }

        public object VisitFunctionStmt(Stmt.Function stmt)
        {
            BeginScope(stmt.name.ToString());

            Declare(stmt.name);
            Define(stmt.name);

            ResolveFunction(stmt, FunctionType.FUNCTION);

            EndScope();
            return new Stmt.DefaultStatement();
        }

        public object VisitLexemeTypeLiteral(Expression.LexemeTypeLiteral expr)
        {
            return new Stmt.DefaultStatement(); 
        }

        public object VisitGetExpr(Expression.Get expr)
        {
            Resolve(expr.expr);
            return null;
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

        public object VisitArrayExpr(Expression.ArrayExpression expr)
        {
            return new Stmt.DefaultStatement();
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
            try
            {
                // the expr is a call to a function. 
                string currentScope = this.blockScope.Peek();
                
                if(processScope.TryGetValue(currentScope, out var localScope))
                {
                    if(localScope.lexemeScope.TryGetValue(expr.name.ToString(), out Lexeme value))
                    {
                        this.interpreter.Resolve(expr,0);
                    }
                }
            }
            catch(Exception ex)
            {
                this.compilerError?.AddError(ex.Message);
            }

            return new Stmt.DefaultStatement();
        }

        private void ResolveLocal(Expression expr, Lexeme name)
        {
            for (int i = scopes.Count -1; i >= 0; i--)
            {
                if (scopes.Peek().ContainsKey(name.ToString()))
                {
                    this.interpreter.Resolve(expr, scopes.Count() - 1 - i);
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
            if (blockScope != null && blockScope.Count > 0)
            {
                var block = blockScope.Peek();
                if (block.Equals(name.ToString()))
                {
                    // current context is a function
                    return;
                }

                if(this.processScope.TryGetValue(block, out BlockScope bsLocal))
                {
                    bsLocal.lexemeScope.Add(name.ToString(), name);
                    return;
                }

                BlockScope bsObject = new BlockScope();
                if (bsObject.lexemeScope.ContainsKey(name.ToString()))
                {
                    throw new Exception("variable already exist");
                }

                bsObject.lexemeScope.Add(name.ToString(), name);
                this.processScope.Add(block, bsObject);
            }
        }

        private void Define(Lexeme name)
        {
            if (!scopes.Any() || (scopes.Peek().Count == 0))
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
            currentFunction = enclosingFunction;
        }

        private void BeginScope(string scopeName)
        {
            scopes.Push(new Dictionary<string, bool>());
            blockScope.Push(scopeName);
        }

        private void EndScope()
        {
            scopes.Pop();
            blockScope.Pop();
        }
    }

    internal class BlockScope
    {
        internal Dictionary<string, Dictionary<string, Expression>> processScope = new Dictionary<string, Dictionary<string, Expression>>();
        internal Dictionary<string, Lexeme> lexemeScope = new Dictionary<string, Lexeme>();
    }
}
