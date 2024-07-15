using JukaCompiler.Exceptions;
using JukaCompiler.Expressions;
using JukaCompiler.Lexer;
using JukaCompiler.Scan;
using JukaCompiler.Statements;
using Microsoft.Extensions.DependencyInjection;
using static JukaCompiler.Expressions.Expr;

namespace JukaCompiler.Parse
{
    /// <summary>
    /// 
    /// </summary>
    public class Parser
    {
        private List<Lexeme> tokens = [];
        private int current = 0;
        private Scanner? scanner;
        public ServiceProvider Services { get; }
        private ICompilerError compilerError;

        internal Parser(Scanner scanner, ServiceProvider services)
        {
            this.scanner = scanner;
            this.Services = services;

            this.compilerError = services.GetRequiredService<ICompilerError>();
        }

        internal List<Statement> Parse()
        {
            if (scanner == null)
            {
                throw new ArgumentNullException("Scanner is null");
            }

            tokens = scanner.Scan()!;
            List<Statement> statements = [];
            while(!IsAtEnd())
            {
                statements.Add(Declaration());
            }

            return statements;
        }

        private bool IsAtEnd()
        {
            if (current == tokens.Count || Peek().LexemeType == LexemeType.Types.EOF)
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

        private Lexeme Peek(int lookAhead)
        {
            if (current + lookAhead > tokens.Count)
            {
                throw new JRuntimeException("Looked pass");
            }

            return tokens[current + lookAhead];
        }

        private Lexeme Advance()
        {
            if (!IsAtEnd())
            {
                current++;
            }

            return Previous();
        }

        private Statement Declaration()
        {
            if (Match(LexemeType.Types.CLASS))
            {
                return ClassDeclaration("class");
            }


            if (Match(LexemeType.Types.SUB))
            {
                return Function("sub");
            }

            if (MatchKeyWord())
            {
                return VariableDeclaration();
            }

            if (Match(LexemeType.Types.IDENTIFIER))
            {
                return VariableDeclaration();
            }

            return Statement();
        }

        private Statement Statement()
        {
            if (Match(LexemeType.Types.PRINTLINE))
            {
                return PrintLine();
            }

            if (Match(LexemeType.Types.PRINT))
            {
                return Print();
            }

            if (Match(LexemeType.Types.RETURN))
            {
                return ReturnStatement();
            }

            if (Match(LexemeType.Types.IF)) 
            {
                return IfStatement();
            }

            if (Match(LexemeType.Types.LEFT_BRACE))
            {
                return new Statement.Block(Block());
            }

            if (Match(LexemeType.Types.WHILE))
            {
                return WhileStatement();
            }

            if (Match(LexemeType.Types.FOR))
            {
                return ForStatement();
            }

            if (Match(LexemeType.Types.BREAK))
            {
                return BreakStatement();
            }

            return ExpressionStatement();
        }

        private Statement.If IfStatement()
        {
            Consume(LexemeType.Types.LEFT_PAREN, Previous());

            Expr? condition = Expr();

            if (condition == null)
            {
                compilerError.AddError("no if condition statement");
            }

            Consume(LexemeType.Types.RIGHT_PAREN, Previous());

            Statement thenBlock = Statement();
            Statement? elseBlock = null;

            if (Match(LexemeType.Types.ELSE))
            {
                elseBlock = Statement();
            }

            if (condition != null && elseBlock != null)
            {
                return new Statement.If(condition, thenBlock, elseBlock);
            }

            throw new JRuntimeException("If statement failure");
        }

        private Statement.While WhileStatement()
        {
            Consume(LexemeType.Types.LEFT_PAREN, Previous());

            Expr condition = Expr();

            Consume(LexemeType.Types.RIGHT_PAREN, Previous());

            Statement whileBlock = Statement();

            return new Statement.While(condition, whileBlock);
        }

        private Statement.For ForStatement()
        {
            Consume(LexemeType.Types.LEFT_PAREN, Previous());
            if (MatchKeyWord())
            {
                Statement.Var initCondition = VariableDeclaration();

                Expr.Binary breakCondition = (Expr.Binary)Equality();
                Consume(LexemeType.Types.SEMICOLON, Previous());

                Expr incrementCondition = Unary();

                Consume(LexemeType.Types.RIGHT_PAREN, Previous());

                Statement forBody = null!;
                if (Match(LexemeType.Types.LEFT_BRACE))
                {
                    forBody = Statement();
                }

                Consume(LexemeType.Types.RIGHT_BRACE, Peek());

                return new Statement.For(initCondition, breakCondition, incrementCondition, forBody);
            }

            throw new JRuntimeException("no valid variable");
        }

        private Statement.Break BreakStatement()
        {
            Consume(LexemeType.Types.SEMICOLON, Previous());
            Expr expr = Expr();
            return new Statement.Break(expr);
        }


        private Statement.Return ReturnStatement()
        {
            Lexeme keyword = Previous();
            Expr value = null!;

            if (!Check(LexemeType.Types.SEMICOLON))
            {
                value = Expr();
            }

            if (value == null)
            {
                throw new JRuntimeException("unable to parse return statement");
            }

            Consume(LexemeType.Types.SEMICOLON, Previous());
            return new Statement.Return(keyword, value);
        }

        private Statement.Expression ExpressionStatement()
        {
            Expr expr = Expr();
            Consume(LexemeType.Types.SEMICOLON, Peek());
            return new Statement.Expression(expr);
        }

        private Statement.PrintLine PrintLine()
        {
            Lexeme keyword = Previous();
            Consume(LexemeType.Types.LEFT_PAREN, Peek());

            Expr value = Expr();

            Consume(LexemeType.Types.RIGHT_PAREN, Peek());
            Consume(LexemeType.Types.SEMICOLON, Peek());

            return new Statement.PrintLine(value);
        }

        private Statement.Print Print()
        {
            Lexeme keyword = Previous();
            Consume(LexemeType.Types.LEFT_PAREN, Peek());

            Expr value = Expr();

            Consume(LexemeType.Types.RIGHT_PAREN, Peek());
            Consume(LexemeType.Types.SEMICOLON, Peek());

            return new Statement.Print(value);
        }

        private bool MatchKeyWord()
        {
            if (MatchInternalFunction())
            {
                return true;
            }

            if(Match(LexemeType.Types.INT)        || 
                Match(LexemeType.Types.STRING)    ||
                Match(LexemeType.Types.FLOAT)     ||
                Match(LexemeType.Types.DOUBLE)    ||
                Match(LexemeType.Types.VAR))
            {
                return true;
            }

            return false;
        }

        private Lexeme Consume(LexemeType.Types type, Lexeme currentLexeme)
        {
            if (Check(type))
            {
                return Advance();
            }

            compilerError.AddError($"Error trying to parse '{currentLexeme}' not valid at line:{currentLexeme.LineNumber} column:{currentLexeme.ColumnNumber} ");

            return new Lexeme(LexemeType.Types.UNDEFINED, 0, 0);
        }

        private Lexeme ConsumeKeyword()
        {
            if (CheckKeyWord())
            {
                return Advance();
            }

            compilerError.AddError("Unable to Keyword");
            return new Lexeme(LexemeType.Types.UNDEFINED, 0,0);
        }

        private bool Match(LexemeType.Types lexType)
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
            if (Check(LexemeType.Types.INTERNALFUNCTION))
            {
                Advance();
                return true;
            }

            return false;
        }

        private bool Check(LexemeType.Types type)
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

        private Statement.Function Function(string kind)
        {

            Lexeme name = Consume(LexemeType.Types.IDENTIFIER, Peek());

            Consume(LexemeType.Types.LEFT_PAREN, Peek());
            List<TypeParameterMap> typeMap = new List<TypeParameterMap>();

            if (!Check(LexemeType.Types.RIGHT_PAREN))
            {
                do
                {
                    Lexeme parameterType = ConsumeKeyword();
                    Expr varName = Expr();
                    if (parameterType.IsKeyWord)
                    {
                        typeMap.Add(new TypeParameterMap( parameterType,varName ));
                    }
                }
                while (Match(LexemeType.Types.COMMA));
            }

            Consume(LexemeType.Types.RIGHT_PAREN, Peek());
            Consume(LexemeType.Types.EQUAL, Peek());
            Consume(LexemeType.Types.LEFT_BRACE, Peek());

            List<Statement> statements = Block();

            Statement.Function stmt = new Statement.Function(name, typeMap, statements);
            return stmt;
        }

        private Statement.Class ClassDeclaration(string kind)
        {
            Lexeme name = Consume(LexemeType.Types.IDENTIFIER, Peek());
            Consume(LexemeType.Types.EQUAL, Peek());
            Consume(LexemeType.Types.LEFT_BRACE, Peek());
            List<Statement.Function> functions = [];
            List<Statement> variableDeclarations = [];

            if(!Check(LexemeType.Types.RIGHT_BRACE))
            {
                while(true)
                {
                    Lexeme isFunc = Peek();
                    if (!isFunc.IsKeyWord)
                    {
                        break;
                    }

                    if (isFunc.LexemeType == LexemeType.Types.SUB)
                    {
                        functions.Add((Statement.Function)Declaration());
                    }

                    if (MatchKeyWord())
                    {
                        variableDeclarations.Add(VariableDeclaration());
                    }
                }
            }

            Consume(LexemeType.Types.RIGHT_BRACE, Peek());

            return new Statement.Class(name, functions, variableDeclarations);
        }

        private List<Statement> Block()
        {
            List<Statement> stmts = new List<Statement>();
            while(!Check(LexemeType.Types.RIGHT_BRACE) && !IsAtEnd())
            {
                stmts.Add(Declaration());
            }

            Consume(LexemeType.Types.RIGHT_BRACE, Peek());
            return stmts;
        }

        private Statement.Var VariableDeclaration()
        {
            Lexeme name = Consume(LexemeType.Types.IDENTIFIER, Peek());

            if (Match(LexemeType.Types.EQUAL))
            {
                Expr initalizedState = Expr();
                Consume(LexemeType.Types.SEMICOLON, Peek());
                return new Statement.Var(name, initalizedState);
            }

            Consume(LexemeType.Types.SEMICOLON, Peek());

            return new Statement.Var(name);
        }

        private Expr Expr()
        {
            return Assignment();
        }

        private Expr Assignment()
        {
            Expr expr = Or();

            if (Match(LexemeType.Types.EQUAL))
            {
                Lexeme equals = Previous();
                Expr value = Assignment();

                //expr.ExpressionLexemeName =

                if (expr is Expr.Variable exprVariables &&
                    exprVariables != null &&
                    exprVariables.ExpressionLexeme != null)
                {
                    Expr.Variable variable = exprVariables;
                    //expr.ExpressionLexemeName = variable.ExpressionLexeme.
                    if (variable != null && variable.ExpressionLexeme != null) 
                    {
                        return new Expr.Assign(variable.ExpressionLexeme, value);
                    }
                    //> Classes assign-set
                }

                if (expr is Expr.Get exprGet &&
                    exprGet != null &&
                    exprGet.ExpressionLexeme != null) 
                {
                    Expr.Get get = exprGet;
                    if (get != null && get.ExpressionLexeme != null)
                    {
                        return new Expr.Set(get, get.ExpressionLexeme, value);
                    }
                    //< Classes assign-set
                }

                Console.WriteLine("Invalid assignment target."); // [no-throw]
            }

            return expr;
        }


        private Expr Equality()
        {
            Expr expr = Comparison();

            while (Match(LexemeType.Types.BANG_EQUAL) || Match(LexemeType.Types.EQUAL_EQUAL))
            {
                Lexeme op = Previous();

                // Bug - I need to consume the second operator when doing comparisions.
                // Hack I need to figure out a better way to do this.
                Lexeme secondOp = Advance();
                if ( secondOp.LexemeType == LexemeType.Types.EQUAL)
                {
                    op.AddToken(secondOp);
                    op.LexemeType = LexemeType.Types.EQUAL_EQUAL;
                }
                // see what it is? maybe update the lexeme?

                Expr right = Comparison();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        //< Statements and State parse-assignment
        //> Control Flow or
        private Expr Or()
        {
            Expr expr = And();

            while (Match(LexemeType.Types.OR))
            {
                Lexeme op = Previous();
                Expr right = And();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

        private Expr And()
        {
            Expr expr = Equality();

            while (Match(LexemeType.Types.AND))
            {
                Lexeme op = Previous();
                Expr right = Equality();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

        private Expr Comparison()
        {
            Expr expr = Term();

            while (Match(LexemeType.Types.GREATER) || Match(LexemeType.Types.GREATER_EQUAL) || Match(LexemeType.Types.LESS) || Match(LexemeType.Types.LESS_EQUAL))
            {
                Lexeme op = Previous();
                Expr right = Term();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Term()
        {
            Expr expr = Factor();

            while (Match(LexemeType.Types.MINUS) || Match(LexemeType.Types.PLUS))
            {
                Lexeme op = Previous();
                Expr right = Factor();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Factor()
        {
            Expr expr = Unary();

            while (Match(LexemeType.Types.SLASH) || Match(LexemeType.Types.STAR))
            {
                Lexeme op = Previous();
                Expr right = Unary();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Unary()
        {
            // Expr expr = Array();

            if (Match(LexemeType.Types.BANG))
            {
                //Lexeme op = Previous();
                //Expr right = Unary();
                //return new Expr.Unary(op, right);
            }

            Lexeme idLexeme = Peek();
            if (idLexeme.LexemeType == LexemeType.Types.NEW)
            {
                Match(LexemeType.Types.NEW);
                return new NewDeclarationExpr(Expr());
            }

            if (idLexeme.LexemeType == LexemeType.Types.DELETE)
            {
                Match(LexemeType.Types.DELETE);

                if (Peek().LexemeType == LexemeType.Types.IDENTIFIER)
                {
                    // If we get here then we have an identifier on the right hand side.
                    // the parent expression will need to validate that it is a variable.
                    Lexeme lexeme = Consume(LexemeType.Types.IDENTIFIER, Peek());
                    Variable variable = new Expr.Variable(lexeme);
                    return new DeleteDeclarationExpr(variable);
                }
            }

            if (idLexeme.LexemeType == LexemeType.Types.IDENTIFIER)
            {
                Lexeme isPlus = Peek(1);
                Lexeme isPlusPlus = Peek(2);
                if (isPlus.LexemeType == LexemeType.Types.PLUS && isPlusPlus.LexemeType == LexemeType.Types.PLUS)
                {
                    Match(LexemeType.Types.IDENTIFIER);
                    Match(LexemeType.Types.PLUS);
                    Match(LexemeType.Types.PLUS);
                    Match(LexemeType.Types.SEMICOLON);
                    return new Expr.Unary(idLexeme, LexemeType.Types.PLUSPLUS);
                }
            }

            return Call();
        }

        private Expr Call()
        {
            Expr expr = Primary();

            while (true)
            {
                if (Match(LexemeType.Types.LEFT_PAREN))
                {
                    expr = FinishCall(expr);
                }
                else if (Match(LexemeType.Types.DOT))
                {
                    Lexeme name = Consume(LexemeType.Types.IDENTIFIER, Peek());
                    expr = new Expr.Get(expr, name);
                }
                else
                {
                    break;
                }
            }
            return expr;
        }

        private Expr Primary()
        {
            if (Match(LexemeType.Types.FALSE)) return new Expr.Literal(Previous(), LexemeType.Types.FALSE);
            if (Match(LexemeType.Types.TRUE)) return new Expr.Literal(Previous(), LexemeType.Types.TRUE);
            if (Match(LexemeType.Types.NULL)) return new Expr.Literal(Previous(), LexemeType.Types.NULL);

            if (Match(LexemeType.Types.STRING))
            {
                return new Expr.Literal(Previous(), LexemeType.Types.STRING);
            }

            if (Match(LexemeType.Types.NUMBER))
            {
                return new Expr.Literal(Previous(), LexemeType.Types.NUMBER);
            }

            if (Match(LexemeType.Types.ARRAY))
            {
                Consume(LexemeType.Types.LEFT_BRACE, Peek());
                var size = Consume(LexemeType.Types.NUMBER, Peek());
                Consume(LexemeType.Types.RIGHT_BRACE, Peek());
                var name = new Lexeme(LexemeType.Types.ARRAY, 0, 0);
                name.AddToken("array");
                return new Expr.ArrayDeclarationExpr(name,size);
            }

            if (Match(LexemeType.Types.NEW))
            {
                var expr = Expr();
            }

            if ( Match(LexemeType.Types.IDENTIFIER))
            {
                Lexeme identifierName = Previous();
                if (Match(LexemeType.Types.LEFT_BRACE))
                {
                    var index = Consume(LexemeType.Types.NUMBER, Peek());
                    Consume(LexemeType.Types.RIGHT_BRACE, Peek());
                    if (Match(LexemeType.Types.EQUAL))
                    {
                        return new Expr.ArrayAccessExpr(identifierName, index, Expr());
                    }

                    return new Expr.ArrayAccessExpr(identifierName, index);
                }

                 //return Assignment();
                return new Expr.Variable(identifierName);
            }

            if (Match(LexemeType.Types.LEFT_PAREN))
            {
                Expr expr = Expr();
                Consume(LexemeType.Types.RIGHT_PAREN, Peek());
                return new Expr.Grouping(expr);
            }

            if (Match(LexemeType.Types.VAR))
            {
                return Assignment();
            }

            throw new Exception(Peek() + "Expect expr");
        }

        private Expr.Call FinishCall(Expr callee)
        {
            List<Expr> arguments = [];
            if (!Check(LexemeType.Types.RIGHT_PAREN))
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

                } while (Match(LexemeType.Types.COMMA));
            }


            Array callableServices = Enum.GetValues(typeof(JukaCompiler.SystemCalls.CallableServices));
            bool isCallable = false;
            foreach(object? callableService in callableServices)
            {
                if (callableService != null)
                {
                    string? serviceName = callableService.ToString();
                    if (!string.IsNullOrEmpty(serviceName))
                    {
                        isCallable = serviceName.Equals(callee.ExpressionLexeme?.ToString());
                    }
                }
            }

            Consume(LexemeType.Types.RIGHT_PAREN, Peek());

            return new Expr.Call(callee, isCallable, arguments);
        }
    }
}