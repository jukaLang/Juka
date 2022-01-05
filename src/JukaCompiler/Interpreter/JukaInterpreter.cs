using JukaCompiler.Lexer;
using JukaCompiler.Parse;
using JukaCompiler.Statements;
using Microsoft.Extensions.DependencyInjection;

namespace JukaCompiler.Interpreter
{
    internal class JukaInterpreter : Stmt.Visitor<Stmt>, Expression.Visitor<object>
    {
        private ServiceProvider serviceProvider;
        private JukaEnvironment globals;
        private JukaEnvironment environment;
        private Dictionary<Expression, int?> locals = new Dictionary<Expression, int?>();

        internal JukaInterpreter(ServiceProvider services)
        {
            environment = globals = new JukaEnvironment();
            this.serviceProvider = services;

            // var callable = new Callable();
            // globals.Define("clock",
        }

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

        internal void ExecuteBlock(List<Stmt> statements, JukaEnvironment environment)
        {
            JukaEnvironment prevEnvironment = environment;

            try
            {
                this.environment = environment;
                foreach(Stmt statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                this.environment = prevEnvironment;
            }
        }

        Stmt Stmt.Visitor<Stmt>.VisitBlockStmt(Stmt.Block stmt)
        {
            throw new NotImplementedException();
        }

        Stmt Stmt.Visitor<Stmt>.VisitFunctionStmt(Stmt.Function stmt)
        {
            JukaFunction functionCallable = new JukaFunction(stmt, null, false);
            environment.Define(stmt.name.ToString(), functionCallable);
            return null;
        }

        public Stmt VisitClassStmt(Stmt.Class stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.expression);
            return null;
        }

        public Stmt VisitIfStmt(Stmt.If stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt VisitPrintStmt(Stmt.Print stmt)
        {
            if (stmt.expr != null)
            {
                var lexemeTypeLiteral = Evaluate(stmt.expr) as Expression.LexemeTypeLiteral;
                Console.WriteLine(lexemeTypeLiteral.Literal);
            }
            return new Stmt.Print();
        }

        public Stmt VisitReturnStmt(Stmt.Return stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt VisitVarStmt(Stmt.Var stmt)
        {
            object value = null;
            if (stmt.isInitalizedVar != null)
            {
                value = Evaluate(stmt.exprInitializer);
            }

            environment.Define(stmt.name.ToString() , value);
            return null;
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
            //(Expression.Le ((Expression.Binary)expr).left.
            object left = Evaluate(expr.left);
            object right = Evaluate(expr.right);

            var leftLiteralType = left as Expression.LexemeTypeLiteral;
            var rightLiteralType = right as Expression.LexemeTypeLiteral;

            if (leftLiteralType == null || rightLiteralType == null)
            {
                throw new ArgumentNullException("unable to get literal");
            }
            
            long leftValueType = leftLiteralType.lexemeType;
            long rightValueType = rightLiteralType.lexemeType;

            object leftValue = leftLiteralType.Literal;
            object rightValue = rightLiteralType.Literal;

            switch (expr.op.ToString())
            {
                case "!=" :
                        return !IsEqual(leftValue, rightValue);
                case "==":
                        return IsEqual(leftValue, rightValue);
                //case ">":
                //case "<":
                //case "<=":
                //case ">=":
                //case "-":
                //case "/":
                //case "*":
                case "+":
                    if (leftValueType == LexemeType.NUMBER && rightValueType == LexemeType.NUMBER)
                    {
                        int lvalue = Convert.ToInt32(leftValue);
                        int rvalue = Convert.ToInt32(rightValue);
                        return lvalue + rvalue;
                    }

                    if (leftValueType == LexemeType.STRING && rightValueType == LexemeType.STRING)
                    { 
                        return (string)left + (string)right; 
                    }

                    throw new ArgumentNullException("can't add types");
            }

            return null;
        }

        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        private void CheckNumberOperand(Lexeme op, object operand)
        {
            if (operand is int)
            {
                return;
            }

            throw new ArgumentException(op.ToString() + " Operands must be numbers");
        }


        private void CheckNumberOperands(Lexeme op, object left, object right)
        {
            if (left is int && right is int )
            {
                return;
            }

            throw new ArgumentException(op.ToString() + " Operands must be numbers");
        }

        public object VisitCallExpr(Expression.Call expr)
        {
            object callee = Evaluate(expr.callee);
            List<object> arguments = new();
            foreach(Expression argument in expr.arguments)
            {
                arguments.Add(argument);
            }

            if (!(callee is Callable))
            {
                throw new ArgumentException("not a class or function");
            }

            Callable function = (Callable)callee;
            if (arguments.Count != function.Arity())
            {
                throw new ArgumentException("Wrong number of arguments");
            }

            return function.Call(this, arguments);
        }

        public object VisitGetExpr(Expression.Get expr)
        {
            throw new NotImplementedException();
        }

        public object VisitGroupingExpr(Expression.Grouping expr)
        {
            if (expr == null || expr.expression == null)
            {
                throw new ArgumentNullException("expr or expression == null");
            }

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
            return LookUpVariable(expr.Name, expr);
        }

        internal object LookUpVariable(Lexeme name, Expression expr)
        {
            locals.TryGetValue(expr, out var distance);

            if (distance != null)
            {
                return environment.GetAt(distance.Value, name.ToString());
            }
            else
            {
                return globals.Get(name);
            }

        }

        internal ServiceProvider ServiceProvider
        {
            get { return this.serviceProvider; }
        }

        internal void Resolve(Expression expr, int depth)
        {
            locals.Add(expr,depth);
        }
    }
}
