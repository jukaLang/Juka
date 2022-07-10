using JukaCompiler.Exceptions;
using JukaCompiler.Lexer;
using JukaCompiler.Scan;
using JukaCompiler.Statements;
using Microsoft.Extensions.DependencyInjection;

namespace JukaCompiler.Parse
{
    public enum CallableServices
    {
        GetAvailableMemory,
        FileOpen,
    }

    public class Parser
    {
        private List<Lexeme> tokens = new();
        private int current = 0;
        private Scanner? scanner;
        private ServiceProvider services;
        private ICompilerError compilerError;

        internal Parser(Scanner scanner, ServiceProvider services)
        {
            this.scanner = scanner;
            this.services = services;

            this.compilerError = services.GetRequiredService<ICompilerError>();
        }

        internal List<Stmt> Parse()
        {
            if (scanner == null)
            {
                throw new ArgumentNullException("Scanner is null");
            }

            tokens = scanner.Scan();
            List<Stmt> statements = new();
            while(!IsAtEnd())
            {
                statements.Add(Declaration());
            }

            return statements;
        }

        private bool IsAtEnd()
        {
            if (current == tokens.Count || Peek().LexemeType == LexemeType.EOF)
            {
                return true;
            }

            return false;
        }

        private Lexeme Peek()
        {
            return tokens[current];
        }

        private Lexeme Previous()
        {
            return tokens[current - 1]; 
        }

        private Lexeme Advance()
        {
            if (!IsAtEnd())
            {
                current++;
            }

            return Previous();
        }

        private Stmt Declaration()
        {
            if (Match(LexemeType.CLASS))
            {
                return ClassDeclaration("class");
            }


            if (Match(LexemeType.FUNC))
            {
                return Function("func");
            }

            if (MatchKeyWord())
            {
                return VariableDeclaration();
            }

            return Statement();
        }

        private Stmt Statement()
        {
            if (Match(LexemeType.INTERNALFUNCTION | LexemeType.PRINTLINE))
            {
                return PrintLine();
            }

            if (Match(LexemeType.INTERNALFUNCTION | LexemeType.PRINT))
            {
                return Print();
            }

            if (Match(LexemeType.RETURN))
            {
                return ReturnStatement();
            }

            if (Match(LexemeType.IF)) 
            {
                return IfStatement();
            }

            if (Match(LexemeType.LEFT_BRACE))
            {
                return new Stmt.Block(Block());
            }

            if (Match(LexemeType.WHILE))
            {
                return WhileStatement();
            }

            if (Match(LexemeType.BREAK))
            {
                return BreakStatement();
            }

            return ExpressionStatement();
        }

        private Stmt IfStatement()
        {
            Consume(LexemeType.LEFT_PAREN, Previous());

            var condition = Expr();

            if (condition == null)
            {
                compilerError.AddError("no if condition statement");
            }

            Consume(LexemeType.RIGHT_PAREN, Previous());

            var thenBlock = Statement();
            Stmt? elseBlock = null;

            if (Match(LexemeType.ELSE))
            {
                elseBlock = Statement();
            }

            return new Stmt.If(condition, thenBlock, elseBlock);
        }


        private Stmt WhileStatement()
        {
            Consume(LexemeType.LEFT_PAREN, Previous());

            var condition = Expr();

            if (condition == null)
            {
                compilerError.AddError("no while condition statement");
            }

            Consume(LexemeType.RIGHT_PAREN, Previous());

            var whileBlock = Statement();

            return new Stmt.While(condition, whileBlock);
        }

        private Stmt BreakStatement()
        {
            Consume(LexemeType.SEMICOLON, Previous());
            return new Stmt.Break();
        }


        private Stmt ReturnStatement()
        {
            var keyword = Previous();
            Expression value = null;

            if (!Check(LexemeType.SEMICOLON))
            {
                value = Expr();
            }

            if (value == null)
            {
                throw new JRuntimeException("unable to parse return statement");
            }

            Consume(LexemeType.SEMICOLON, Previous());
            return new Stmt.Return(keyword, value);
        }

        private Stmt ExpressionStatement()
        {
            Expression expression = Expr();
            Consume(LexemeType.SEMICOLON, Peek());
            return new Stmt.Expression(expression);
        }

        private Stmt PrintLine()
        {
            Lexeme keyword = Previous();
            Consume(LexemeType.LEFT_PAREN, Peek());

            Expression value = Expr();

            Consume(LexemeType.RIGHT_PAREN, Peek());
            Consume(LexemeType.SEMICOLON, Peek());

            return new Stmt.PrintLine(value);
        }

        private Stmt Print()
        {
            Lexeme keyword = Previous();
            Consume(LexemeType.LEFT_PAREN, Peek());

            Expression value = Expr();

            Consume(LexemeType.RIGHT_PAREN, Peek());
            Consume(LexemeType.SEMICOLON, Peek());

            return new Stmt.Print(value);
        }

        private bool MatchKeyWord()
        {
            if (MatchInternalFunction())
            {
                return true;
            }

            if(Match(LexemeType.INT) || 
                Match(LexemeType.STRING) ||
                Match(LexemeType.FLOAT) ||
                Match(LexemeType.DOUBLE))
            {
                return true;
            }

            return false;
        }

        private Lexeme Consume(Int64 type, Lexeme currentLexeme)
        {
            if (Check(type))
            {
                return Advance();
            }

            compilerError.AddError($"Error trying to parse '{currentLexeme.ToString()}' not valid at line:{currentLexeme.LineNumber} column:{currentLexeme.ColumnNumber} ");

            return new Lexeme(LexemeType.UNDEFINED, 0, 0);
        }

        private Lexeme ConsumeKeyword()
        {
            if (CheckKeyWord())
            {
                return Advance();
            }

            compilerError.AddError("Unable to Keyword");
            return new Lexeme(LexemeType.UNDEFINED, 0,0);
        }

        private bool Match(Int64 lexType)
        {
            if (Check(lexType))
            {
                Advance();
                return true;
            }

            return false;
        }

        private bool MatchInternalFunction()
        {
            if (Check(LexemeType.INTERNALFUNCTION))
            {
                Advance();
                return true;
            }

            return false;
        }

        private bool Check(Int64 type)
        {
            if (IsAtEnd())
            {
                return false;
            }

            if (Peek().IsKeyWord)
            {
                return Peek().LexemeType == type;
            }

            return Peek().LexemeType == type;
        }

        private bool CheckKeyWord()
        {
            if (IsAtEnd())
            {
                return false;
            }

            if (Peek().IsKeyWord)
            {
                return true;
            }

            return false;
        }

        private Stmt Function(string kind)
        {
            Lexeme name = Consume(LexemeType.IDENTIFIER, Peek());
            Consume(LexemeType.LEFT_PAREN, Peek());
            var typeMap = new List<TypeParameterMap>();

            if (!Check(LexemeType.RIGHT_PAREN))
            {
                do
                {
                    var parameterType = ConsumeKeyword();
                    var varName = Expr();
                    if (parameterType.IsKeyWord)
                    {
                        typeMap.Add(new TypeParameterMap( parameterType,varName ));
                    }
                }
               while (Match(LexemeType.COMMA));

            }

            Consume(LexemeType.RIGHT_PAREN, Peek());
            Consume(LexemeType.EQUAL, Peek());
            Consume(LexemeType.LEFT_BRACE, Peek());

            List<Stmt> statements = Block();

            var stmt = new Stmt.Function(name, typeMap, statements);
            return stmt;
        }

        private Stmt ClassDeclaration(string kind)
        {
            Lexeme name = Consume(LexemeType.IDENTIFIER, Peek());
            Consume(LexemeType.EQUAL, Peek());
            Consume(LexemeType.LEFT_BRACE, Peek());
            List<Stmt> functions = new List<Stmt>();
            List<Stmt> variableDeclarations = new List<Stmt>();

            if(!Check(LexemeType.RIGHT_BRACE))
            {
                while(true)
                {
                    var isFunc = Peek();
                    if (!isFunc.IsKeyWord)
                    {
                        break;
                    }

                    if (isFunc.LexemeType == LexemeType.FUNC)
                    {
                        functions.Add(Declaration());
                    }

                    if (MatchKeyWord())
                    {
                        variableDeclarations.Add(VariableDeclaration());
                    }
                }
            }

            Consume(LexemeType.RIGHT_BRACE, Peek());

            return new Stmt.Class(name, functions, variableDeclarations);
        }

        private List<Stmt> Block()
        {
            var stmts = new List<Stmt>();
            while(!Check(LexemeType.RIGHT_BRACE) && !IsAtEnd())
            {
                stmts.Add(Declaration());
            }

            Consume(LexemeType.RIGHT_BRACE, Peek());
            return stmts;
        }

        private Stmt VariableDeclaration()
        {
            Lexeme name = Consume(LexemeType.IDENTIFIER, Peek());
            Expression? initalizedState = null;

            if (Match(LexemeType.EQUAL))
            {
                initalizedState = Expr();
                Consume(LexemeType.SEMICOLON, Peek());
                return new Stmt.Var(name, initalizedState);
            }

            Consume(LexemeType.SEMICOLON, Peek());

            return new Stmt.Var(name);
        }

        private Expression Expr()
        {
            return Assignment();
        }

        private Expression Assignment()
        {
            Expression expr = Or();

            if (Match(LexemeType.EQUAL))
            {
                Lexeme equals = Previous();
                Expression value = Assignment();

                if (expr is Expression.Variable && 
                    ((Expression.Variable)expr) != null && 
                    ((Expression.Variable)expr).Name != null)
                {
                    Expression.Variable variable = (Expression.Variable)expr;
                    if (variable != null && variable.Name != null) 
                    {
                        return new Expression.Assign(variable.Name, value);
                    }
                    //> Classes assign-set
                }

                if (expr is Expression.Get &&
                    ((Expression.Get)expr) != null &&
                    ((Expression.Get)expr).Name != null) 
                {
                    Expression.Get get = (Expression.Get)expr;
                    if (get != null && get.Name != null)
                    {
                        return new Expression.Set(get, get.Name, value);
                    }
                    //< Classes assign-set
                }

                //error(equals, "Invalid assignment target."); // [no-throw]
            }

            return expr;
        }


        private Expression Equality()
        {
            Expression expr = Comparison();

            while (Match(LexemeType.BANG_EQUAL) || Match(LexemeType.EQUAL_EQUAL))
            {
                Lexeme op = Previous();

                // Bug - I need to consume the second operator when doing comparisions.
                // Hack I need to figure out a better way to do this.
                Lexeme secondOp = Advance();
                if ( secondOp.LexemeType == LexemeType.EQUAL)
                {
                    op.AddToken(secondOp);
                    op.LexemeType = LexemeType.EQUAL_EQUAL;
                }
                // see what it is? maybe update the lexeme?

                Expression right = Comparison();
                expr = new Expression.Binary(expr, op, right);
            }

            return expr;
        }

        //< Statements and State parse-assignment
        //> Control Flow or
        private Expression Or()
        {
            Expression expr = And();

            while (Match(LexemeType.OR))
            {
                Lexeme op = Previous();
                Expression right = And();
                expr = new Expression.Logical(expr, op, right);
            }

            return expr;
        }

        private Expression And()
        {
            Expression expr = Equality();

            while (Match(LexemeType.AND))
            {
                Lexeme op = Previous();
                Expression right = Equality();
                expr = new Expression.Logical(expr, op, right);
            }

            return expr;
        }

        private Expression Comparison()
        {
            Expression expr = Term();

            while (Match(LexemeType.GREATER) || Match(LexemeType.GREATER_EQUAL) || Match(LexemeType.LESS) || Match(LexemeType.LESS_EQUAL))
            {
                Lexeme op = Previous();
                Expression right = Term();
                expr = new Expression.Binary(expr, op, right);
            }

            return expr;
        }

        private Expression Term()
        {
            Expression expr = Factor();

            while (Match(LexemeType.MINUS) || Match(LexemeType.PLUS))
            {
                Lexeme op = Previous();
                Expression right = Factor();
                expr = new Expression.Binary(expr, op, right);
            }

            return expr;
        }

        private Expression Factor()
        {
            Expression expr = Unary();

            while (Match(LexemeType.SLASH) || Match(LexemeType.STAR))
            {
                Lexeme op = Previous();
                Expression right = Unary();
                expr = new Expression.Binary(expr, op, right);
            }

            return expr;
        }

        private Expression Unary()
        {
            if (Match(LexemeType.BANG) || Match(LexemeType.MINUS))
            {
                Lexeme op = Previous();
                Expression right = Unary();
                //******* return new Expression.Unary(op, right);
            }

            return Call();
        }

        private Expression Call()
        {
            Expression expr = Primary();

            while (true)
            { // [while-true]
                if (Match(LexemeType.LEFT_PAREN))
                {
                    expr = FinishCall(expr);
                    //> Classes parse-property
                }
                else if (Match(LexemeType.DOT))
                {
                    Lexeme name = Consume(LexemeType.IDENTIFIER, Peek());
                    //******* expr = new Expression.Get(expr, name);
                    //< Classes parse-property
                }
                else
                {
                    break;
                }
            }
            return expr;
        }

        private Expression Primary()
        {
            if (Match(LexemeType.FALSE)) return new Expression.Literal(Previous(), LexemeType.FALSE);
            if (Match(LexemeType.TRUE)) return new Expression.Literal(Previous(), LexemeType.TRUE);
            if (Match(LexemeType.NULL)) return new Expression.Literal(Previous(), LexemeType.NULL);

            if (Match(LexemeType.STRING))
            {
                return new Expression.Literal(Previous(), LexemeType.STRING);
            }

            if (Match(LexemeType.NUMBER))
            {
                return new Expression.Literal(Previous(), LexemeType.NUMBER);
            }

            if ( Match(LexemeType.IDENTIFIER))
            {
                return new Expression.Variable(Previous());
            }

            if (Match(LexemeType.LEFT_PAREN))
            {
                Expression expr = Expr();
                Consume(LexemeType.RIGHT_PAREN, Peek());
                return new Expression.Grouping(expr);
            }

            throw new Exception(Peek() + "Expect expression");
        }

        private Expression FinishCall(Expression callee)
        {
            List<Expression> arguments = new();
            if (!Check(LexemeType.RIGHT_PAREN))
            {
                do
                {
                    ////> check-max-arity
                    //if (arguments.size() >= 255)
                    //{
                    //    error(peek(), "Can't have more than 255 arguments.");
                    //}
                    ////< check-max-arity
                    
                    arguments.Add(Expr());

                } while (Match(LexemeType.COMMA));
            }


            var callableServices = Enum.GetValues(typeof(CallableServices));
            bool isCallable = false;
            foreach(var callableService in callableServices)
            {
                if (callableService.ToString().Equals(callee.Name.ToString()))
                {
                    isCallable = true;
                }
            }

            Consume(LexemeType.RIGHT_PAREN, Peek());

            return new Expression.Call(callee, isCallable, arguments);
        }
    }
}