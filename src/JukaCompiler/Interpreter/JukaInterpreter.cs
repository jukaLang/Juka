﻿using System.Diagnostics;
using JukaCompiler.Exceptions;
using JukaCompiler.Lexer;
using JukaCompiler.Parse;
using JukaCompiler.Statements;
using JukaCompiler.SystemCalls;
using Microsoft.Extensions.DependencyInjection;
using static JukaCompiler.Interpreter.StackFrame;
using static JukaCompiler.Parse.Expression;

namespace JukaCompiler.Interpreter
{
    internal class JukaInterpreter : Stmt.Visitor<Stmt>, Expression.IVisitor<object>
    {
        private readonly ServiceProvider serviceProvider;
        private readonly JukaEnvironment globals;
        private JukaEnvironment environment;
        private readonly Dictionary<Expression, int?> locals = new Dictionary<Expression, int?>();
        private Stack<StackFrame> frames = new();
        private readonly string globalScope = "__global__scope__";
        private readonly int __max_stack_depth__ = 500;

        internal JukaInterpreter(ServiceProvider services)
        {
            environment = globals = new JukaEnvironment();
            this.serviceProvider = services;

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
#pragma warning disable CS8604 // Possible null reference argument.
                globals.Define("clock", serviceProvider.GetService<ISystemClock>());
                globals.Define("fileOpen", services.GetService<IFileOpen>());
                globals.Define("getAvailableMemory", services.GetService<IGetAvailableMemory>());
#pragma warning restore CS8604 // Possible null reference argument.
        }

        internal void Interpret(List<Stmt> statements)
        {
            // populate the env with the function call locations
            // only functions are populated in the env
            // classes will need to be added. 
            // no local variables.
            frames.Clear();
            frames.Push(new StackFrame(globalScope));
            foreach (Stmt stmt in statements)
            {
                if (stmt is Stmt.Function || stmt is Stmt.Class)
                {
                    Execute(stmt);
                }
            }

            Lexeme? lexeme = new(LexemeType.IDENTIFIER, 0, 0);
            lexeme.AddToken("main");
            Expression.Variable functionName = new(lexeme);
            Expression.Call call = new(functionName, false, new List<Expression>());
            Stmt.Expression expression = new(call);
            Execute(expression);
        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        internal void ExecuteBlock(List<Stmt> statements, JukaEnvironment env)
        {
            JukaEnvironment previous = this.environment;

            try
            {
                this.environment = env;
                foreach(Stmt statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                this.environment = previous;
            }
        }
        Stmt Stmt.Visitor<Stmt>.VisitBlockStmt(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.statements, new JukaEnvironment(this.environment));
            return new Stmt.DefaultStatement();
        }

        Stmt Stmt.Visitor<Stmt>.VisitFunctionStmt(Stmt.Function stmt)
        {
            JukaFunction? functionCallable = new JukaFunction(stmt, this.environment, false);
            environment.Define(stmt.StmtLexemeName, functionCallable);
            return new Stmt.DefaultStatement();
        }
        Stmt Stmt.Visitor<Stmt>.VisitClassStmt(Stmt.Class stmt)
        {
            object? superclass = null;
            if (stmt.superClass != null)
            {
                superclass = Evaluate(stmt.superClass);
            }

            environment.Define(stmt.name.ToString(), null);

            if (stmt.superClass != null)
            {
                environment = new JukaEnvironment(environment);
                environment.Define("super", superclass);
            }

            Dictionary<string, JukaFunction> functions = new Dictionary<string, JukaFunction>();
            foreach(var method in stmt.methods)
            {
                JukaFunction jukaFunction = new JukaFunction(method, environment, false);
                functions.Add(method.StmtLexemeName, jukaFunction);
            }

            JukaClass? jukaClass = new JukaClass(stmt.name.ToString(), ((JukaClass)superclass!)!, functions);

            if (superclass != null)
            {
                environment = new JukaEnvironment(environment);
                environment.Define("super", superclass);
            }

            environment.Assign(stmt.name, jukaClass);
            return new Stmt.DefaultStatement();
        }

        public Stmt VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.expression);
            return new Stmt.DefaultStatement();
        }

        public Stmt VisitIfStmt(Stmt.If stmt)
        {
            Stmt.DefaultStatement defaultStatement = new Stmt.DefaultStatement();

            if (IsTrue(Evaluate(stmt.condition)))
            {
                Execute(stmt.thenBranch);
            }
            else if(stmt.elseBranch != null)
            { 
                Execute(stmt.elseBranch);
            }

            return defaultStatement;
        }
        public Stmt VisitPrintLine(Stmt.PrintLine stmt)
        {
            if (stmt.expr != null)
            {
                VisitPrintAllInternal(stmt.expr, Console.WriteLine);
            }

            return new Stmt.PrintLine();
        }
        public Stmt VisitPrint(Stmt.Print stmt)
        {
            if (stmt.expr != null)
            {
                VisitPrintAllInternal(stmt.expr, Console.Write);
            }

            return new Stmt.Print();
        }


        private Stmt.Print VisitPrintAllInternal(Parse.Expression expr, Action<object> printAction)
        {
            if (expr != null)
            {
                if (expr is Expression.Literal || expr is Expression.LexemeTypeLiteral)
                {
                    var lexemeTypeLiteral = Evaluate(expr) as Expression.LexemeTypeLiteral;
                    if (lexemeTypeLiteral?.Literal != null)
                    {
                        if (lexemeTypeLiteral?.Literal != null)
                        {
                            printAction(lexemeTypeLiteral?.Literal!);
                        }
                    }
                }

                if (expr is Variable)
                {
                    PrintVariableLookUp((Expression.Variable)expr, o =>
                    {
                        printAction(o);
                    });
                }

                if (expr is ArrayAccessExpression)
                {
                    var variableExpression = expr as ArrayAccessExpression;
                    var variableName = variableExpression?.ArrayVariableName.ToString();

                    if (variableName != null && frames.Peek().TryGetStackArrayVariableByName(variableName, out ArrayImplementation? arrayImplementation))
                    {
                        if (frames.Peek()
                            .TryGetStackVariableByName(variableName, out var variableState))
                        {
                            int arrayIndex = (int)variableState?.Value!;
                            printAction(arrayImplementation?.GetAt(arrayIndex)!);
                        }
                    }
                }
            }

            return new Stmt.Print();
        }

        private bool PrintVariableLookUp(Expression.Variable expr, Action<object> printTypeAction)
        {
            if (expr is not null)
            {
                if (expr.ExpressionLexeme != null)
                {
                    var variable = LookUpVariable(expr.ExpressionLexeme, expr);

                    if (variable is StackVariableState stackVariable)
                    {
                        if (stackVariable.Value is Expression.LexemeTypeLiteral typeLiteral)
                        {
                            var o = typeLiteral.literal;
                            if (o != null)
                                printTypeAction(o);
                        }
                        
                        if (stackVariable.Value is Expression.Literal literal)
                        {
                            printTypeAction(literal.ExpressionLexeme?.ToString()!);
                        }

                        if (stackVariable.Value is string)
                        {
                            printTypeAction(stackVariable.Value);
                        }
                    }

                    if (variable is Expression.LexemeTypeLiteral lexemeTypeLiteral)
                    {
                        var literal = lexemeTypeLiteral.literal;
                        if (literal != null)
                            printTypeAction(literal);
                    }
                    else
                    {
                        if (variable is Literal literal)
                        {
                            printTypeAction(literal.ExpressionLexeme?.ToString()!);
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public Stmt VisitReturnStmt(Stmt.Return stmt)
        {
            object? value = null;
            if (stmt.expr != null)
            {
                value = Evaluate(stmt.expr);
            }

            throw new Return(value);
        }

        public Stmt VisitBreakStmt(Stmt.Break stmt)
        {
            Stmt.Return returnStatement = new Stmt.Return();
            return VisitReturnStmt(returnStatement);
        }

        public Stmt VisitForStmt(Stmt.For stmt)
        {
            var init = Evaluate(stmt.Init);
            environment.Define(stmt.Init.ExpressionLexemeName,init);

            while (IsTrue(Evaluate(stmt.IncExpression)))
            {
                var breakExpression = Evaluate(stmt.BreakExpression);
                if (!IsTrue(breakExpression))
                {
                    Execute(stmt.ForBody);
                }
            }

            return new Stmt.DefaultStatement();
        }

        public Stmt VisitVarStmt(Stmt.Var stmt)
        {
            // Hack. If the expression is a method call assignment
            // i.e. var x = foo(); 
            // AddVariable does an Evaluate which will put the assigee
            // in the stack frame of foo().
            // doing the check of the initializer allows the code
            // to put the variable in the callers stack.
            object? value = frames.Peek().AddVariable(stmt,this);
            if (stmt.exprInitializer is Call)
            {
                frames.Pop();
                frames.Peek().AddVariable(stmt.name?.ToString()!, value, stmt.name?.GetType()!, stmt.exprInitializer);

                //This could be a problem in the future.
                //The stack frame is popped but never pushed.
                //There could be issues when/if CTOR is implemented
            }

            environment.Define(stmt.name?.ToString()! , value);
            return new Stmt.DefaultStatement(); 
        }
        public Stmt VisitWhileStmt(Stmt.While stmt)
        {
            if (frames.Count >= __max_stack_depth__)
            {
                throw new JRuntimeException($"Stack depth exceeded. Depth = {frames.Count}");
            }
            while (IsTrue(Evaluate(stmt.condition)))
            {
                Execute(stmt.whileBlock);
            }

            return new Stmt.DefaultStatement();
        }
        public object VisitLexemeTypeLiteral(Expression.LexemeTypeLiteral expr)
        {
            return expr.Accept(this);
        }

        internal object? Evaluate(Expression expr)
        {
            return expr.Accept(this);
        }
        public object VisitAssignExpr(Expression.Assign expr)
        {
            object? value = Evaluate(expr.value);
            var stackVariableState = new StackVariableState
            {
                Name = expr.ExpressionLexeme?.ToString()!,
                Value = value,
                type = expr.GetType(),
                expressionContext = expr,
            };

            var stackFrame = frames.Peek();

            if (!stackFrame.UpdateVariable(expr.ExpressionLexeme?.ToString() ?? string.Empty, stackVariableState))
            {
                var newVar = new Stmt.Expression.Var(expr.ExpressionLexeme, expr);
                stackFrame.AddVariable(newVar, this);
            }

            Debug.Assert(value != null, nameof(value) + " != null");
            return value;
        }

        private (Literal LiteralValue, long LiteralType) GetLiteralData(Expression expression)
        {
            if (expression is Variable)
            {
                var stackVariableState = Evaluate(expression) as StackVariableState;

                if (stackVariableState?.expressionContext is Literal context)
                {
                    var literal = context;
                    var lexemeType = ((LexemeTypeLiteral) stackVariableState.Value!)!.LexemeType;

                    return (
                        literal,
                        lexemeType);
                }

                if (stackVariableState?.expressionContext is Assign assign)
                {
                    var literal = (Literal)assign.value;
                    var lexemType = assign.ExpressionLexeme!.LexemeType;

                    return (
                        literal, 
                        lexemType);
                }
            }

            if (expression is Literal)
            {
                return ((Literal)expression, ((Literal)expression).Type);
            }

            throw new JRuntimeException("Can't get literal data");

        }

        public object VisitBinaryExpr(Expression.Binary expr)
        {
            var r = GetLiteralData(expr.right!);
            var l = GetLiteralData(expr.left!);
            long leftValueType =  l.LiteralType;
            object leftValue = l.LiteralValue.ExpressionLexeme?.ToString()!;

            long rightValueType = r.LiteralType;
            object rightValue = r.LiteralValue.ExpressionLexeme?.ToString()!;

            switch (expr.op?.ToString())
            {
                case "!=" :
                        return !IsEqual(leftValue, rightValue);
                case "==":
                        return IsEqual(leftValue, rightValue);
                case ">":
                    return IsLessThan(leftValueType, rightValueType, leftValue, rightValue);
                case "/":
                    return DivideTypes(leftValueType, rightValueType, leftValue, rightValue);
                case "*":
                    return MultiplyTypes(leftValueType, rightValueType, leftValue, rightValue);
                case "-":
                    return SubtractTypes(leftValueType, rightValueType, leftValue, rightValue);
                case "+":
                    return AddTypes(leftValueType, rightValueType, leftValue, rightValue);
                //case "<":
                //case "<=":
                //case ">=":
            }

            return new Expression.LexemeTypeLiteral();
        }
        private static object IsLessThan(long leftValueType, long rightValueType, object leftValue, object rightValue)
        {
            if (leftValueType == LexemeType.NUMBER && rightValueType == LexemeType.NUMBER)
            {
                var literal = new Expression.LexemeTypeLiteral();
                literal.literal = Convert.ToInt32(leftValue) < Convert.ToInt32(rightValue);
                literal.lexemeType = LexemeType.BOOL;
                return literal;
            }

            if (leftValueType == LexemeType.STRING || rightValueType == LexemeType.STRING)
            {
                throw new ArgumentException("can't apply less than operator to strings");
            }

            throw new ArgumentException("Can't compare types");
        }

        private static object AddTypes(long leftValueType, long rightValueType, object leftValue, object rightValue)
        {
            if (leftValueType == LexemeType.NUMBER && rightValueType == LexemeType.NUMBER)
            {
                var literalSum = new Expression.LexemeTypeLiteral();
                literalSum.literal = Convert.ToInt32(leftValue) + Convert.ToInt32(rightValue);
                literalSum.lexemeType = LexemeType.NUMBER;
                return literalSum;
            }

            if (leftValueType == LexemeType.STRING && rightValueType == LexemeType.STRING)
            {
                var literalStringSum = new Expression.LexemeTypeLiteral();

                literalStringSum.literal = Convert.ToString(leftValue) + Convert.ToString(rightValue);
                literalStringSum.lexemeType = LexemeType.STRING;
                return literalStringSum;
            }

            throw new ArgumentNullException("Can't add types");
        }
        private static object SubtractTypes(long leftValueType, long rightValueType, object leftValue, object rightValue)
        {
            if (leftValueType == LexemeType.NUMBER && rightValueType == LexemeType.NUMBER)
            {
                var literalSum = new Expression.LexemeTypeLiteral();
                literalSum.literal = Convert.ToInt32(leftValue) - Convert.ToInt32(rightValue);
                literalSum.lexemeType = LexemeType.NUMBER;
                return literalSum;
            }

            if (leftValueType == LexemeType.STRING && rightValueType == LexemeType.STRING)
            {
                throw new ArgumentException("Can't subtract strings");
            }

            throw new ArgumentNullException("Can't subtract types");
        }

        private static object MultiplyTypes(long leftValueType, long rightValueType, object leftValue, object rightValue)
        {
            if (leftValueType == LexemeType.NUMBER && rightValueType == LexemeType.NUMBER)
            {
                var literalProduction = new Expression.LexemeTypeLiteral();
                literalProduction.literal = Convert.ToInt32(leftValue) * Convert.ToInt32(rightValue);
                literalProduction.lexemeType = LexemeType.NUMBER;
                return literalProduction;
            }

            if (leftValueType == LexemeType.STRING && rightValueType == LexemeType.STRING)
            {
                throw new ArgumentException("Can't multiply strings");
            }

            throw new ArgumentNullException("Can't multiply types");
        }
        private static object DivideTypes(long leftValueType, long rightValueType, object leftValue, object rightValue)
        {
            if (leftValueType == LexemeType.NUMBER && rightValueType == LexemeType.NUMBER)
            {
                var literalProduction = new Expression.LexemeTypeLiteral();
                int divident = Convert.ToInt32(rightValue);

                if (divident == 0)
                {
                    throw new ArgumentException("Can't divide by zero");

                }
                literalProduction.literal = Convert.ToInt32(leftValue) / Convert.ToInt32(rightValue);
                literalProduction.lexemeType = LexemeType.NUMBER;
                return literalProduction;
            }

            if (leftValueType == LexemeType.STRING && rightValueType == LexemeType.STRING)
            {
                throw new ArgumentException("can't divide strings");
            }

            throw new ArgumentNullException("can't divide types");
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

#pragma warning disable CS8766
        public object? VisitCallExpr(Expression.Call expr)
#pragma warning restore CS8766
        {
            if (expr == null || expr.callee == null || expr.callee.ExpressionLexeme == null)
            {
                throw new Exception("VisitCallExpr - runtime exception interperter");
            }

            if (expr.callee is Expression.Get)
            {
                var instanceMethod = expr.callee.Accept(this);
                var declaration = ((JukaFunction)instanceMethod).Declaration;
                if (declaration != null)
                {
                    var instanceStackFrame = new StackFrame(declaration.StmtLexemeName);
                    frames.Push(instanceStackFrame);
                    var instanceMethodReturn = ((JukaFunction)instanceMethod).Call(declaration.StmtLexemeName, this, null!);
                    frames.Pop();
                    return instanceMethodReturn;
                }
            }

            var currentStackFrame = new StackFrame(expr.callee.ExpressionLexeme.ToString());
            frames.Push(currentStackFrame);

            var arguments = new List<object?>();
            Dictionary<string, object?> argumentsMap = new Dictionary<string, object?>();

            foreach (Expression argument in expr.arguments)
            {
                if (argument is Expression.Variable)
                {
                    var lexeme = (Expression.Variable)argument;
                    if (lexeme.ExpressionLexeme != null)
                    {
                        object? variable = environment.Get(lexeme.ExpressionLexeme);
                        arguments.Add(variable);
                        argumentsMap.Add(lexeme.ExpressionLexeme.ToString(), variable);
                    }
                }

                if (argument is Expression.Literal)
                {
                    var literal = (Expression.Literal)argument;
                    arguments.Add(literal.LiteralValue);
                    argumentsMap.Add(literal.ExpressionLexeme?.ToString()!, literal);
                }
            }
            
            if (argumentsMap.Count > 0)
            {
                currentStackFrame.AddVariables(argumentsMap, this);
            }

            if (expr.isJukaCallable)
            {
                try
                { 
                    var jukacall = (IJukaCallable)this.ServiceProvider.GetService(typeof(IJukaCallable));
                    return jukacall.Call(methodName: expr.callee.ExpressionLexeme.ToString(), this, arguments);
                }
                catch(SystemCallException? sce)
                {
                    return sce;
                }
            }
            else
            {
                object? callee = Evaluate(expr.callee);
                IJukaCallable? function = (IJukaCallable)callee!;
                if (arguments.Count != function.Arity())
                {
                    throw new ArgumentException("Wrong number of arguments");
                }

                return function?.Call(expr.callee.ExpressionLexeme.ToString(),this, arguments);
            }
        }

        public object VisitGetExpr(Expression.Get expr)
        {
            StackVariableState? getexpr = Evaluate(expr.expr) as StackVariableState;
            if (getexpr?.Value is JukaInstance instance)
            {
                if (expr.ExpressionLexeme != null)
                {
                    return instance.Get(expr.ExpressionLexeme);
                }
            }

            throw new Exception("not a class instances");
        }

        public object VisitGroupingExpr(Expression.Grouping expr)
        {
            if (expr == null || expr.expression == null)
            {
                throw new ArgumentNullException("expr or expression == null");
            }

            return Evaluate(expr.expression) ?? throw new JRuntimeException("Grouping is null");
        }

        public object VisitLiteralExpr(Expression.Literal expr)
        {
            return expr.LiteralValue() ?? throw new JRuntimeException("literal is null");
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
            if (expr.ExpressionLexeme != null)
            {
                var lookUp = LookUpVariable(expr.ExpressionLexeme, expr);
                if (lookUp == null)
                {
                    throw new Exception("variable is null");
                }

                return lookUp;
            }

            throw new JRuntimeException("visit variable returned null");
        }

        public object VisitArrayExpr(Expression.ArrayDeclarationExpression expr)
        {
            var currentFrame = frames.Peek();
            if (expr.initializerContextVariableName != null)
                currentFrame.AddStackArray(expr.initializerContextVariableName, expr.ArraySize);
            return expr.ArraySize;
        }

        public object VisitArrayAccessExpr(ArrayAccessExpression expr)
        {
            return new Stmt.DefaultStatement();
        }

        internal object? LookUpVariable(Lexeme name, Expression expr)
        {
            locals.TryGetValue(expr, out int? distance);

            if (frames.Peek().TryGetStackVariableByName(name.ToString(), out StackVariableState? variable))
            {
                return variable;
            }

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
            if (locals.Where( f => f.Key.ExpressionLexeme != null && expr.ExpressionLexeme != null && f.Key.ExpressionLexeme.ToString().Equals(expr.ExpressionLexeme.ToString()) ).Count() <= 1)
            {
                locals.Add(expr,depth);
            }
        }

        private bool IsTrue(object? o)
        {
            if (o == null)
            {
                return false;
            }

            if (o is bool)
            {
                return (bool)o;
            }

            return true;
        }
    }
}
