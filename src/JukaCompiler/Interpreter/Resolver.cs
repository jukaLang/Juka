using JukaCompiler.Exceptions;
using JukaCompiler.Expressions;
using JukaCompiler.Lexer;
using JukaCompiler.Statements;
using Microsoft.Extensions.DependencyInjection;
using static JukaCompiler.Expressions.Expr;

namespace JukaCompiler.Interpreter
{
    internal class Resolver : Statement.IVisitor<object>, Expr.IVisitor<object>
    {
        private JukaInterpreter interpreter;
        private FunctionType currentFunction = FunctionType.NONE;
        private ClassType currentClass = ClassType.NONE;
        private ServiceProvider? ServiceProvider;
        private Stack<Dictionary<string, bool>> scopes = new();
        private Dictionary<string, BlockScope?> processScope = [];
        private Stack<string> blockScope = new();
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

        internal void Resolve(List<Statement> statements)
        {
            foreach(Statement stmt in statements)
            {
                Resolve(stmt);
            }
        }

        private void Resolve(Statement stmt)
        {
            stmt.Accept(this);
        }

        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

        public object VisitAssignExpr(Expr.Assign expr)
        {
            var frame = blockScope.Peek() ?? throw new Exception(string.Format(errorMessage, "No stack frames"));
            if (processScope.ContainsKey(frame))
            {
                BlockScope? value = processScope[frame];
                if (value != null)
                {
                    string lexemeKey = expr.ExpressionLexeme?.ToString()!;
                    if (value.lexemeScope.ContainsKey(lexemeKey))
                    {
                        var lexeme = value.lexemeScope[lexemeKey];
                        //value.lexemeScope[expr.ExpressionLexeme.ToString] = expr.
                        //var xx= interpreter.LookUpVariable(lexeme, expr);
                    }
                }
            } 
            return new Statement.DefaultStatement();
        }

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            if (expr != null && expr.left != null && expr.right != null)
            {
                Resolve(expr.left);
                Resolve(expr.right);
            }
            return new Expr.Binary();
        }

        public object VisitBlockStmt(Statement.Block stmt)
        {
            //BeginScope(stmt.);
            Resolve(stmt.statements);
            //EndScope();
            return new Statement.DefaultStatement();
        }

        public object VisitCallExpr(Call expr)
        {
            if (expr is not null)
            {
                Call call = expr;
                BeginScope(expr.callee.ExpressionLexeme?.ToString()!);
                if (expr.callee.ExpressionLexeme != null)
                {
                    Declare(expr.callee.ExpressionLexeme);
                    Define(expr.callee.ExpressionLexeme);
                }

                if (!call.isJukaCallable)
                { 
                    Resolve(expr.callee);
                }
                EndScope();
            }
            return new Statement.DefaultStatement();
        }

        public object VisitClassStmt(Statement.Class stmt)
        {
            ClassType enclosingClass = currentClass;
            currentClass = ClassType.CLASS;
            BlockScope blockScope = new();

            Declare(stmt.name);
            Define(stmt.name);

            if (stmt.superClass != null && stmt.name.ToString().Equals(stmt.superClass.ExpressionLexeme?.ToString()))
            {
                this.compilerError?.AddError("can't inhert from itself");
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

            for (int i = 0; i < stmt.methods.Count; i++)
            {
                Statement.Function method = stmt.methods[i];
                //FunctionType decl = FunctionType.METHOD;
                //implement ctor
                // if(method.ExpressionLexeme.ToString

                ResolveFunction(method, FunctionType.METHOD);
            }

            EndScope();
            if(stmt.superClass != null)
            {
                EndScope();
            }

            currentClass = enclosingClass;
            return new Statement.DefaultStatement();
        }

        public object VisitExpressionStmt(Statement.Expression stmt)
        {
            Resolve(stmt.Expr);
            return new Statement.DefaultStatement();
        }

        public object VisitFunctionStmt(Statement.Function stmt)
        {
            BeginScope(stmt.StmtLexemeName);

            Declare(stmt.StmtLexeme);
            Define(stmt.StmtLexeme);

            ResolveFunction(stmt, FunctionType.FUNCTION);

            EndScope();
            return new Statement.DefaultStatement();
        }

        public object VisitLexemeTypeLiteral(Expr.LexemeTypeLiteral expr)
        {
            return new Statement.DefaultStatement(); 
        }

        public object VisitGetExpr(Expr.Get expr)
        {
            Resolve(expr.expr);
            return new Statement.DefaultStatement();
        }

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            if (expr == null || expr.expression == null)
            {
                throw new ArgumentNullException("Grouping has some null stuff");
            }

            Resolve(expr.expression);
            return new Expr.Grouping();
        }

        public object VisitIfStmt(Statement.If stmt)
        {
            Resolve(stmt.condition);
            Resolve(stmt.thenBranch);

            if (stmt.elseBranch != null)
            {
                Resolve(stmt.elseBranch);
            }

            return new Statement.DefaultStatement();
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return new Expr.Literal();
        }

        public object VisitLogicalExpr(Expr.Logical expr)
        {
            throw new NotImplementedException("Resolver VisitLogicalExpr is not implemented");
        }

        public object VisitPrintLine(Statement.PrintLine stmt)
        {
            if (stmt == null || stmt.expr == null)
            {
                throw new ArgumentNullException("stmt and or expr are null");
            }

            Resolve(stmt.expr);
            return new Statement.PrintLine();
        }

        public object VisitPrint(Statement.Print stmt)
        {
            if (stmt == null || stmt.expr == null)
            {
                throw new ArgumentNullException("stmt and or expr are null");
            }

            Resolve(stmt.expr);
            return new Statement.Print();
        }

        public object VisitReturnStmt(Statement.Return stmt)
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

            return new Statement.DefaultStatement();
        }

        public object VisitBreakStmt(Statement.Break stmt)
        {
            Statement.Return returnStatement = new();
            return VisitReturnStmt(returnStatement);
        }

        public object VisitForStmt(Statement.For stmt)
        {
            return new Statement.DefaultStatement();
        }

        public object VisitArrayExpr(Expr.ArrayDeclarationExpr expr)
        {
            return new Statement.DefaultStatement();
        }

        public object VisitNewExpr(NewDeclarationExpr expr)
        {
            return new Statement.DefaultStatement();
        }

        public object VisitArrayAccessExpr(ArrayAccessExpr expr)
        {
            return new Statement.DefaultStatement();
        }

        public object VisitDeleteExpr(DeleteDeclarationExpr expr)
        {
            return new Statement.DefaultStatement();
        }

        public object VisitSetExpr(Expr.Set expr)
        {
            throw new NotImplementedException("Resolver VisitSetExpr is not implemented");
        }

        public object VisitSuperExpr(Expr.Super expr)
        {
            throw new NotImplementedException("Resolver VisitSuperExpr is not implemented");
        }

        public object VisitThisExpr(Expr.This expr)
        {
            throw new NotImplementedException("Resolver VisitThisExpr is not implemented");
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            throw new NotImplementedException("Resolver VisitUnaryExpr is not implemented");
        }

        public object VisitVariableExpr(Expr.Variable expr)
        {
            try
            {
                // the expr is a call to a function. 
                string currentScope = this.blockScope.Peek();
                
                if(processScope.TryGetValue(currentScope, out var localScope))
                {
                    if(localScope != null && localScope.lexemeScope.TryGetValue(expr.ExpressionLexeme!.ToString(), out var value))
                    {
                        this.interpreter.Resolve(expr,0);
                    }
                }
            }
            catch(Exception ex)
            {
                this.compilerError?.AddError(ex.Message);
            }

            return new Statement.DefaultStatement();
        }

        private void ResolveLocal(Expr expr, Lexeme name)
        {
            for (int i = scopes.Count -1; i >= 0; i--)
            {
                if (scopes.Peek().ContainsKey(name.ToString()))
                {
                    this.interpreter.Resolve(expr, scopes.Count - 1 - i);
                }
            }
        }

        public object VisitVarStmt(Statement.Var stmt)
        {
            if (stmt == null || stmt.name == null)
            {
                throw new ArgumentNullException("new vist stmt == null"+stmt);
            }

            Declare(stmt.name);

            if (stmt.isInitalizedVar && stmt.exprInitializer != null)
            {
                Resolve(stmt.exprInitializer);
            }

            Define(stmt.name);
            return new Statement.Var();
        }

        public object VisitWhileStmt(Statement.While stmt)
        {
            Resolve(stmt.condition);
            Resolve(stmt.whileBlock);

            return new Statement.DefaultStatement();
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

                if(this.processScope.TryGetValue(block, out BlockScope? bsLocal))
                {
                    bsLocal?.lexemeScope.Add(name.ToString(), name);
                    return;
                }

                BlockScope bsObject = new();
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
            if (scopes.Count == 0 || (scopes.Peek().Count == 0))
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

        private void ResolveFunction(Statement.Function function, FunctionType type)
        {
            FunctionType enclosingFunction = currentFunction;
            currentFunction = type;

            foreach (var param in function.typeParameterMaps)
            {
                Variable literalName = param.parameterName as Expr.Variable ?? throw new Exception("Something went wrong when resolving the function");
                if (literalName.ExpressionLexeme != null)
                {
                    Declare(literalName.ExpressionLexeme);
                }

                if (param.parameterType != null)
                {
                    Define(param.parameterType);
                }
            }

            if (function != null && function.body != null)
            {
                Resolve(function.body);
            }
            currentFunction = enclosingFunction;
        }

        private void BeginScope(string scopeName)
        {
            scopes.Push([]);
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
        internal Dictionary<string, Dictionary<string, Expr>> processScope = [];
        internal Dictionary<string, Lexeme> lexemeScope = [];

        public static implicit operator BlockScope?(string? v)
        {
            throw new NotImplementedException();
        }
    }
}
