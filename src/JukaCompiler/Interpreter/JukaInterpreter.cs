﻿using System.Diagnostics;
using JukaCompiler.Exceptions;
using JukaCompiler.Expressions;
using JukaCompiler.Lexer;
using JukaCompiler.Statements;
using JukaCompiler.SystemCalls;
using Microsoft.Extensions.DependencyInjection;
using static JukaCompiler.Interpreter.StackFrame;
using static JukaCompiler.Expressions.Expr;

namespace JukaCompiler.Interpreter
{
    internal class JukaInterpreter : Statement.IVisitor<Statement>, IVisitor<object>
    {
        private ServiceProvider serviceProvider;
        private JukaEnvironment globals;
        private JukaEnvironment environment;
        private Dictionary<Expr, int?> locals = [];
        private Stack<StackFrame> frames = new();
        private string globalScope = "__global__scope__";
        private int __max_stack_depth__ = 500;

        internal JukaInterpreter(ServiceProvider services)
        {
            environment = globals = new();
            serviceProvider = services;

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            globals.Define("CSharp", serviceProvider.GetService<ICSharp>());
            globals.Define("Clock", serviceProvider.GetService<ISystemClock>());
            globals.Define("FileOpen", services.GetService<IFileOpener>());
            globals.Define("GetAvailableMemory", services.GetService<IGetAvailableMemory>());
        }

        internal void Interpret(List<Statement> statements)
        {
            // populate the env with the function call locations
            // only functions are populated in the env
            // classes will need to be added. 
            // no local variables.
            frames.Clear();
            frames.Push(new(globalScope));
            foreach (Statement stmt in statements)
            {
                if (stmt is Statement.Function || stmt is Statement.Class)
                {
                    Execute(stmt);
                }
            }

            Lexeme lexeme = new(LexemeType.Types.IDENTIFIER, 0, 0);
            lexeme.AddToken("main");
            Statement.Expression expression = new(new Call(new Variable(lexeme), false, []));
            Execute(expression);
        }

        private void Execute(Statement stmt)
        {
            stmt.Accept(this);
        }

        internal void ExecuteBlock(List<Statement> statements, JukaEnvironment env)
        {
            JukaEnvironment previous = this.environment;

            try
            {
                this.environment = env;
                foreach (Statement statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            finally
            {
                this.environment = previous;
            }
        }
        Statement Statement.IVisitor<Statement>.VisitBlockStmt(Statement.Block stmt)
        {
            ExecuteBlock(stmt.statements, new(this.environment));
            return new Statement.DefaultStatement();
        }

        Statement Statement.IVisitor<Statement>.VisitFunctionStmt(Statement.Function stmt)
        {
            JukaFunction? functionCallable = new(stmt, this.environment, false);
            environment.Define(stmt.StmtLexemeName, functionCallable);
            return new Statement.DefaultStatement();
        }
        Statement Statement.IVisitor<Statement>.VisitClassStmt(Statement.Class stmt)
        {
            object? superclass = null;
            if (stmt.superClass != null)
            {
                superclass = Evaluate(stmt.superClass);
            }

            environment.Define(stmt.name.ToString(), null);

            if (stmt.superClass != null)
            {
                environment = new(environment);
                environment.Define("super", superclass);
            }

            Dictionary<string, JukaFunction> functions = [];
            foreach (var method in stmt.methods)
            {
                JukaFunction jukaFunction = new(method, environment, false);
                functions.Add(method.StmtLexemeName, jukaFunction);
            }

            JukaClass? jukaClass = new(stmt.name.ToString(), ((JukaClass)superclass!)!, functions);

            if (superclass != null)
            {
                environment = new(environment);
                environment.Define("super", superclass);
            }

            environment.Assign(stmt.name, jukaClass);
            return new Statement.DefaultStatement();
        }

        public Statement VisitExpressionStmt(Statement.Expression stmt)
        {
            Evaluate(stmt.Expr);
            return new Statement.DefaultStatement();
        }

        public Statement VisitIfStmt(Statement.If stmt)
        {
            Statement.DefaultStatement defaultStatement = new();

            if (IsTrue(Evaluate(stmt.condition)))
            {
                Execute(stmt.thenBranch);
            }
            else if (stmt.elseBranch != null)
            {
                Execute(stmt.elseBranch);
            }

            return defaultStatement;
        }
        public Statement VisitPrintLine(Statement.PrintLine stmt)
        {
            if (stmt.expr != null)
            {
                VisitPrintAllInternal(stmt.expr, Console.WriteLine);
            }

            return new Statement.PrintLine();
        }
        public Statement VisitPrint(Statement.Print stmt)
        {
            if (stmt.expr != null)
            {
                VisitPrintAllInternal(stmt.expr, Console.Write);
            }

            return new Statement.Print();
        }


        private Statement.Print VisitPrintAllInternal(Expr expr, Action<object> printAction)
        {
            if (expr != null)
            {
                if (expr is Expr.Literal || expr is Expr.LexemeTypeLiteral)
                {
                    if (Evaluate(expr) is LexemeTypeLiteral lexemeTypeLiteral)
                    {
                        PrintLiteral(lexemeTypeLiteral, printAction);
                    }
                }

                if(expr is Expr.Binary)
                {
                    object? binaryEval = Evaluate(expr);

                    if (binaryEval is Expr.LexemeTypeLiteral lexemeTypeLiteral)
                    {
                        object? literal = lexemeTypeLiteral.literal;
                        if (literal != null)
                            printAction(literal);
                    }
                }

                if (expr is Variable)
                {
                    PrintVariableLookUp((Expr.Variable)expr, o =>
                    {
                        printAction(o);
                    });
                }

                if (expr is ArrayAccessExpr)
                {
                    ArrayAccessExpr? variableExpression = expr as ArrayAccessExpr;
                    string? variableName = variableExpression?.ArrayVariableName.ToString();

                    if (variableName != null && frames.Peek().TryGetStackVariableByName(variableName, out StackVariableState? stackVariableState))
                    {
                        if (variableExpression != null)
                        {
                            object? arrayData = stackVariableState?.arrayValues[variableExpression.ArrayIndex];
                            if (arrayData is Expr.Literal literal)
                            {
                                PrintLiteral(literal, printAction);
                            }
                        }
                    }
                }
            }

            return new();
        }


        private void PrintLiteral(Expr.Literal expr, Action<object> printAction)
        {
            printAction(expr.ExpressionLexemeName);
        }

        private void PrintLiteral(Expr.LexemeTypeLiteral expr, Action<object> printAction)
        {
            if (expr?.Literal != null)
            {
               printAction(expr?.Literal!);
            }
        }

        private bool PrintVariableLookUp(Expr.Variable expr, Action<object> printTypeAction)
        {
            if (expr is not null)
            {
                if (expr.ExpressionLexeme != null)
                {
                    object? variable = LookUpVariable(expr.ExpressionLexeme, expr);

                    if (variable is StackVariableState stackVariable)
                    {
                        if (stackVariable.Value is Expr.LexemeTypeLiteral typeLiteral)
                        {
                            object? o = typeLiteral.literal;
                            if (o != null)
                                printTypeAction(o);
                        }

                        if (stackVariable.Value is Expr.Literal literal)
                        {
                            printTypeAction(literal.ExpressionLexeme?.ToString()!);
                        }

                        if (stackVariable.Value is string)
                        {
                            printTypeAction(stackVariable.Value);
                        }
                    }

                    if (variable is Expr.LexemeTypeLiteral lexemeTypeLiteral)
                    {
                        object? literal = lexemeTypeLiteral.literal;
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

        public Statement VisitReturnStmt(Statement.Return stmt)
        {
            object? value = null;
            if (stmt.expr != null)
            {
                value = Evaluate(stmt.expr);
            }

            throw new Return(value);
        }

        public Statement VisitBreakStmt(Statement.Break stmt)
        {
            Statement.Return returnStatement = new();
            return VisitReturnStmt(returnStatement);
        }

        public Statement VisitForStmt(Statement.For stmt)
        {
            stmt.Init.Accept(this);

            object? breakExpression = Evaluate(stmt.BreakExpr);
            while (IsTrue(breakExpression))
            {
                Execute(stmt.ForBody);
                stmt.IncExpr.Accept(this);
                breakExpression = Evaluate(stmt.BreakExpr);
            }

            return new Statement.DefaultStatement();
        }

        public Statement VisitVarStmt(Statement.Var stmt)
        {
            // Hack. If the expr is a method call assignment
            // i.e. var x = foo(); 
            // AddVariable does an Evaluate which will put the assigee
            // in the stack frame of foo().
            // doing the check of the initializer allows the code
            // to put the variable in the callers stack.
            object? value = frames.Peek().AddVariable(stmt, this);
            if (stmt.exprInitializer is Call)
            {
                frames.Pop();
                frames.Peek().AddVariable(stmt.name?.ToString()!, value, stmt.name?.GetType()!, stmt.exprInitializer);

                //This could be a problem in the future.
                //The stack frame is popped but never pushed.
                //There could be issues when/if CTOR is implemented
            }

            environment.Define(stmt.name?.ToString()!, value);
            return new Statement.DefaultStatement();
        }
        public Statement VisitWhileStmt(Statement.While stmt)
        {
            if (frames.Count >= __max_stack_depth__)
            {
                throw new JRuntimeException($"Stack depth exceeded. Depth = {frames.Count}");
            }
            while (IsTrue(Evaluate(stmt.condition)))
            {
                Execute(stmt.whileBlock);
            }

            return new Statement.DefaultStatement();
        }
        public object VisitLexemeTypeLiteral(Expr.LexemeTypeLiteral expr)
        {
            return expr.Accept(this);
        }

        internal object? Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }
        public object VisitAssignExpr(Expr.Assign expr)
        {
            object? value = Evaluate(expr.value);
            object? stackVariableState = new StackVariableState
            {
                Name = expr.ExpressionLexeme?.ToString()!,
                Value = value,
                type = expr.GetType(),
                expressionContext = expr,
            };

            StackFrame stackFrame = frames.Peek();

            if (!stackFrame.UpdateVariable(expr.ExpressionLexeme?.ToString() ?? string.Empty, stackVariableState))
            {
                if (expr.ExpressionLexeme != null)
                {
                    Statement.Var newVar = new(expr.ExpressionLexeme, expr);
                    stackFrame.AddVariable(newVar, this);
                }
            }

            Debug.Assert(value != null, nameof(value) + " != null");
            return value;
        }

        private (string LiteralValue, LexemeType.Types LiteralType) GetLiteralData(Expr expr)
        {
            if (expr is Variable)
            {
                StackVariableState? stackVariableState = Evaluate(expr) as StackVariableState;

                if (stackVariableState?.expressionContext is Literal context)
                {
                    Literal literal = context;
                    LexemeType.Types lexemeType = ((LexemeTypeLiteral)stackVariableState.Value!)!.LexemeType;

                    return (
                        literal.ExpressionLexeme?.ToString(),
                        lexemeType);
                }

                if (stackVariableState?.expressionContext is Assign assign)
                {
                    Literal literal = (Literal)assign.value;
                    LexemeType.Types lexemeType = assign.ExpressionLexeme!.LexemeType;

                    return (
                        literal.ExpressionLexeme?.ToString(),
                        lexemeType);
                }
            }

            if (expr is Expr.Binary binary)
            {
                Expr.LexemeTypeLiteral visited = (Expr.LexemeTypeLiteral)VisitBinaryExpr(binary);
                return (visited.literal.ToString(), visited.LexemeType);
            }
            if (expr is Literal expr1) return (expr1.ExpressionLexeme?.ToString(), expr1.Type);
            throw new JRuntimeException("Can't get literal data" + expr);

            //return VisitBinaryExpr(expr);
        }

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            (string literalValue, LexemeType.Types rightValueType) = GetLiteralData(expr.right);
            (string literal, LexemeType.Types leftValueType) = GetLiteralData(expr.left);
            string leftValue = literal;

            string rightValue = literalValue;


            //Console.WriteLine(leftValue);
            //Console.WriteLine(rightValue);

            object output = expr?.op?.ToString() switch
            {
                "!=" => !IsEqual(leftValue, rightValue),
                "==" => IsEqual(leftValue, rightValue),
                ">" => IsGreaterThan(leftValueType, rightValueType, leftValue, rightValue),
                "<" => IsLessThan(leftValueType, rightValueType, leftValue, rightValue),
                "/" => DivideTypes(leftValueType, rightValueType, leftValue, rightValue),
                "*" => MultiplyTypes(leftValueType, rightValueType, leftValue, rightValue),
                "-" => SubtractTypes(leftValueType, rightValueType, leftValue, rightValue),
                "+" => AddTypes(leftValueType, rightValueType, leftValue, rightValue),
                _ => new Expr.LexemeTypeLiteral()
            };

            //Console.WriteLine(((Expr.LexemeTypeLiteral)output).Literal.ToString());

            return output;
        }
        private static object IsLessThan(LexemeType.Types leftValueType, LexemeType.Types rightValueType, object leftValue, object rightValue)
        {
            if (leftValueType == LexemeType.Types.NUMBER && rightValueType == LexemeType.Types.NUMBER)
            {
                LexemeTypeLiteral literal = new()
                {
                    literal = Convert.ToDouble(leftValue) < Convert.ToDouble(rightValue),
                    lexemeType = LexemeType.Types.BOOL
                };
                return literal;
            }

            if (leftValueType == LexemeType.Types.STRING || rightValueType == LexemeType.Types.STRING)
            {
                throw new ArgumentException("can't apply less than operator to strings");
            }

            throw new ArgumentException("Can't compare types");
        }

        private static object IsGreaterThan(LexemeType.Types leftValueType, LexemeType.Types rightValueType, object leftValue, object rightValue)
        {
            if (leftValueType == LexemeType.Types.NUMBER && rightValueType == LexemeType.Types.NUMBER)
            {
                LexemeTypeLiteral literal = new()
                {
                    literal = Convert.ToDouble(leftValue) > Convert.ToDouble(rightValue),
                    lexemeType = LexemeType.Types.BOOL
                };
                return literal;
            }

            if (leftValueType == LexemeType.Types.STRING || rightValueType == LexemeType.Types.STRING)
            {
                throw new ArgumentException("can't apply less than operator to strings");
            }

            throw new ArgumentException("Can't compare types");
        }

        private static object AddTypes(LexemeType.Types leftValueType, LexemeType.Types rightValueType, object leftValue, object rightValue)
        {
            if (leftValueType == LexemeType.Types.NUMBER && rightValueType == LexemeType.Types.NUMBER)
            {
                LexemeTypeLiteral literalSum = new()
                {
                    literal = Convert.ToDouble(leftValue) + Convert.ToDouble(rightValue),
                    lexemeType = LexemeType.Types.NUMBER
                };
                return literalSum;
            }


            if ((leftValueType == LexemeType.Types.NUMBER && rightValueType == LexemeType.Types.STRING) || (leftValueType == LexemeType.Types.STRING && rightValueType == LexemeType.Types.NUMBER))
            {
                LexemeTypeLiteral literalStringSum = new()
                {
                    literal = Convert.ToString(leftValue) + Convert.ToString(rightValue),
                    lexemeType = LexemeType.Types.STRING
                };

                return literalStringSum;
            }

            if (leftValueType == LexemeType.Types.STRING && rightValueType == LexemeType.Types.STRING)
            {
                LexemeTypeLiteral literalStringSum = new()
                {
                    literal = Convert.ToString(leftValue) + Convert.ToString(rightValue),
                    lexemeType = LexemeType.Types.STRING
                };

                return literalStringSum;
            }

            throw new ArgumentNullException(nameof(leftValueType));
        }
        private static object SubtractTypes(LexemeType.Types leftValueType, LexemeType.Types rightValueType, object leftValue, object rightValue)
        {
            if (leftValueType == LexemeType.Types.NUMBER && rightValueType == LexemeType.Types.NUMBER)
            {
                LexemeTypeLiteral literalSum = new()
                {
                    literal = Convert.ToDouble(leftValue) - Convert.ToDouble(rightValue),
                    lexemeType = LexemeType.Types.NUMBER
                };
                return literalSum;
            }

            if (leftValueType == LexemeType.Types.STRING && rightValueType == LexemeType.Types.STRING)
            {
                throw new ArgumentException("Can't subtract strings");
            }

            throw new ArgumentNullException(nameof(leftValueType));
        }

        private static object MultiplyTypes(LexemeType.Types leftValueType, LexemeType.Types rightValueType, object leftValue, object rightValue)
        {
            if (leftValueType == LexemeType.Types.NUMBER && rightValueType == LexemeType.Types.NUMBER)
            {
                LexemeTypeLiteral literalProduction = new()
                {
                    literal = Convert.ToDouble(leftValue) * Convert.ToDouble(rightValue),
                    lexemeType = LexemeType.Types.NUMBER
                };
                return literalProduction;
            }

            if (leftValueType == LexemeType.Types.STRING && rightValueType == LexemeType.Types.STRING)
            {
                throw new ArgumentException("Can't multiply strings");
            }

            throw new ArgumentNullException(nameof(leftValueType));
        }
        private static object DivideTypes(LexemeType.Types leftValueType, LexemeType.Types rightValueType, object leftValue, object rightValue)
        {
            if (leftValueType == LexemeType.Types.NUMBER && rightValueType == LexemeType.Types.NUMBER)
            {
                LexemeTypeLiteral literalProduction = new();
                double divident = Convert.ToDouble(rightValue);

                if (divident == 0)
                {
                    throw new ArgumentException("Can't divide by zero");

                }
                literalProduction.literal = Convert.ToDouble(leftValue) / Convert.ToDouble(rightValue);
                literalProduction.lexemeType = LexemeType.Types.NUMBER;
                return literalProduction;
            }

            if (leftValueType == LexemeType.Types.STRING && rightValueType == LexemeType.Types.STRING)
            {
                throw new ArgumentException("can't divide strings");
            }

            throw new ArgumentNullException(nameof(leftValueType));
        }
        private static bool IsEqual(object a, object b)
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

#pragma warning disable CS8766
        public object? VisitCallExpr(Expr.Call expr)
#pragma warning restore CS8766
        {
            if (expr == null || expr.callee == null || expr.callee.ExpressionLexeme == null)
            {
                throw new("VisitCallExpr - runtime exception interperter");
            }

            switch (expr.callee)
            {
                case Get:
                    {
                        object instanceMethod = expr.callee.Accept(this);
                        Statement.Function? declaration = ((JukaFunction)instanceMethod).Declaration;
                        if (declaration != null)
                        {
                            StackFrame instanceStackFrame = new(declaration.StmtLexemeName);
                            frames.Push(instanceStackFrame);
                            object? instanceMethodReturn = ((JukaFunction)instanceMethod).Call(declaration.StmtLexemeName, this, null!);
                            frames.Pop();
                            return instanceMethodReturn;
                        }

                        break;
                    }
            }

            StackFrame currentStackFrame = new(expr.callee.ExpressionLexeme.ToString());
            frames.Push(currentStackFrame);

            List<object?> arguments = [];
            Dictionary<string, object?> argumentsMap = [];

            foreach (Expr argument in expr.arguments)
            {
                switch (argument)
                {
                    case Variable lexeme when lexeme.ExpressionLexeme != null:
                        {
                            object? variable = environment.Get(lexeme.ExpressionLexeme);
                            arguments.Add(variable);
                            argumentsMap.Add(lexeme.ExpressionLexeme.ToString(), variable);
                            break;
                        }
                    case Literal literal1:
                        {
                            Literal literal = literal1;
                            arguments.Add(literal.LiteralValue);
                            argumentsMap.Add(literal.ExpressionLexeme?.ToString()!, literal);
                            break;
                        }
                }
            }

            switch (argumentsMap.Count)
            {
                case > 0:
                    currentStackFrame.AddVariables(argumentsMap, this);
                    break;
            }

            switch (expr.isJukaCallable)
            {
                case true:
                    try
                    {
                        IJukaCallable? jukacall = (IJukaCallable)this.ServiceProvider.GetService(typeof(IJukaCallable))!;
                        return jukacall.Call(methodName: expr.callee.ExpressionLexeme.ToString(), this, arguments);
                    }
                    catch (SystemCallException? sce)
                    {
                        return sce;
                    }
            }

            object? callee = Evaluate(expr.callee);
            IJukaCallable? function = (IJukaCallable)callee!;
            if (arguments.Count != function.Arity())
            {
                throw new ArgumentException("Wrong number of arguments");
            }

            return function?.Call(expr.callee.ExpressionLexeme.ToString(), this, arguments);
        }

        public object VisitGetExpr(Expr.Get expr)
        {
            StackVariableState? getexpr = Evaluate(expr.expr) as StackVariableState;
            if (getexpr?.Value is JukaInstance instance)
            {
                if (expr.ExpressionLexeme != null)
                {
                    return instance.Get(expr.ExpressionLexeme);
                }
            }

            throw new("not a class instances");
        }

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            if (expr == null || expr.expression == null)
            {
                throw new ArgumentNullException(nameof(expr));
            }

            return Evaluate(expr.expression) ?? throw new JRuntimeException("Grouping is null");
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.LiteralValue() ?? throw new JRuntimeException("Literal is null");
        }

        public object VisitLogicalExpr(Expr.Logical expr)
        {
            throw new NotImplementedException();
        }

        public object VisitSetExpr(Expr.Set expr)
        {
            throw new NotImplementedException();
        }

        public object VisitSuperExpr(Expr.Super expr)
        {
            throw new NotImplementedException();
        }

        public object VisitThisExpr(Expr.This expr)
        {
            throw new NotImplementedException();
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            if (expr.ExpressionLexeme != null)
            {
                StackVariableState? stackVariableState = (StackVariableState)LookUpVariable(expr.ExpressionLexeme, expr)!;

                if (stackVariableState?.expressionContext is Literal context && ((Unary)expr).LexemeType == LexemeType.Types.PLUSPLUS)
                {
                    long unaryUpdate = Convert.ToInt64(context.ExpressionLexeme?.ToString()) + 1;

                    Lexeme lexeme = new(LexemeType.Types.NUMBER, 0, 0);
                    lexeme.AddToken(unaryUpdate.ToString());

                    Literal newLiteral = new(lexeme, lexeme.LexemeType);

                    stackVariableState.expressionContext = newLiteral;

                    LexemeTypeLiteral lexemeTypeLiteral = new()
                    {
                        LexemeType = lexeme.LexemeType,
                        ExpressionLexeme = lexeme,
                        literal = lexeme.Literal()
                    };

                    stackVariableState.Value = lexemeTypeLiteral;
                    return new Statement.DefaultStatement();
                }
            }

            throw new JRuntimeException("Bad unary expression");
        }

        public object VisitVariableExpr(Expr.Variable expr)
        {
            if (expr.ExpressionLexeme != null)
            {
                object? lookUp = LookUpVariable(expr.ExpressionLexeme, expr);
                return lookUp ?? throw new("Variable is null");
            }

            throw new JRuntimeException("Visit variable returned null");
        }

        public object VisitArrayExpr(Expr.ArrayDeclarationExpr expr)
        {
            object? currentFrame = frames.Peek();
            //if (expr.initializerContextVariableName != null)
            //    currentFrame.AddStackArray(expr.initializerContextVariableName, expr.ArrayIndex);
            return expr.ArraySize;
        }

        public object VisitNewExpr(NewDeclarationExpr expr)
        {
            //throw new NotImplementedException("The new keyword has not been implemented. I need to have a model for allocating and dealocating objects. Potentially a garbage collection model.");
            return new DefaultExpr();
        }

        public object VisitArrayAccessExpr(ArrayAccessExpr expr)
        {
            StackVariableState? stackVariableState = LookUpVariable(expr.ArrayVariableName, expr) as StackVariableState;

            int arraySize = 0;
            if (stackVariableState?.expressionContext is ArrayDeclarationExpr arrayContext)
            {
                arraySize = (int)(stackVariableState?.Value)!;
            }
            else if (stackVariableState?.expressionContext is NewDeclarationExpr context)
            {
                arraySize = ((ArrayDeclarationExpr)context.NewDeclarationExprInit).ArraySize;
            }

            if (expr.ArrayIndex > arraySize)
            {
                throw new JRuntimeException("JArray index out of bounds");
            }

            if (stackVariableState?.arrayValues == null)
            {
                stackVariableState!.arrayValues = new object[arraySize];
            }

            stackVariableState.arrayValues[expr.ArrayIndex] = expr.LvalueExpr;

            return new Statement.DefaultStatement();
        }

        public object VisitDeleteExpr(DeleteDeclarationExpr expr)
        {

            StackFrame currentFrame = frames.Peek();
            currentFrame.DeleteVariable(expr.variable.ExpressionLexemeName);

            //Console.WriteLine(currentFrame);

            return new Statement.DefaultStatement();
        }

        internal object? LookUpVariable(Lexeme name, Expr expr)
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

        internal void Resolve(Expr expr, int depth)
        {
            if (locals.Where(f => f.Key.ExpressionLexeme != null && expr.ExpressionLexeme != null && f.Key.ExpressionLexeme.ToString().Equals(expr.ExpressionLexeme.ToString())).Count() <= 1)
            {
                locals.Add(expr, depth);
            }
        }

        private bool IsTrue(object? o)
        {
            switch (o)
            {
                case null:
                    return false;
                case bool b:
                    return b;
                case LexemeTypeLiteral literal:
                    {
                        object boolValue = literal.Literal;
                        if (boolValue is bool)
                        {
                            return IsTrue(boolValue);
                        }

                        break;
                    }
            }

            return true;
        }
    }
}