using JukaCompiler.Exceptions;
using JukaCompiler.Lexer;
using JukaCompiler.Scan;
using JukaCompiler.Statements;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace JukaCompiler.Parse
{
    public class Parser
    {
        private List<Lexeme> tokens = new List<Lexeme>();
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
                throw new ArgumentNullException("scanner is null");
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
            if (Match(LexemeType.FUNC))
            {
                return Function("func");
            }

            if (MatchKeyWord())
            {
                return VariableDeclaration(Previous());
            }

            //Advance();
            return Statement();
        }

        private Stmt Statement()
        {
            if (Match(LexemeType.INTERNALFUNCTION | LexemeType.PRINTLINE))
            {
                return PrintLine();
            }

            return ExpressionStatement();
        }

        private Stmt ExpressionStatement()
        {
            Expression expression = Expr();
            Consume(LexemeType.SEMICOLON);
            return new Stmt.Expression(expression);
        }

        private Stmt PrintLine()
        {
            Lexeme keyword = Previous();
            Consume(LexemeType.LEFT_PAREN);

            Expression value = Expr();

            Consume(LexemeType.RIGHT_PAREN);
            Consume(LexemeType.SEMICOLON);

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

        private Lexeme Consume(Int64 type)
        {
            if (Check(type))
            {
                return Advance();
            }

            compilerError.AddError("Unable to parser");

            return new Lexeme(LexemeType.UNDEFINED);
        }

        private Lexeme ConsumeKeyword()
        {
            if (CheckKeyWord())
            {
                return Advance();
            }

            compilerError.AddError("Unable to Keyword");
            return new Lexeme(LexemeType.UNDEFINED);
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
            Lexeme name = Consume(LexemeType.IDENTIFIER);
            Consume(LexemeType.LEFT_PAREN);
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

            Consume(LexemeType.RIGHT_PAREN);
            Consume(LexemeType.EQUAL);
            Consume(LexemeType.LEFT_BRACE);

            List<Stmt> statements = Block();

            var stmt = new Stmt.Function(name, typeMap, statements);
            return stmt;
        }

        private List<Stmt> Block()
        {
            var stmts = new List<Stmt>();
            while(!Check(LexemeType.RIGHT_BRACE) && !IsAtEnd())
            {
                stmts.Add(Declaration());
            }

            Consume(LexemeType.RIGHT_BRACE);
            return stmts;
        }

        private Stmt VariableDeclaration(Lexeme type)
        {
            Lexeme name = Consume(LexemeType.IDENTIFIER);
            Expression? initalizedState = null;

            if (Match(LexemeType.EQUAL))
            {
                initalizedState = Expr();
                Consume(LexemeType.SEMICOLON);
                return new Stmt.Var(name, initalizedState);
            }

            Consume(LexemeType.SEMICOLON);

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
        //< Control Flow or
        //> Control Flow and
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
        //< Control Flow and
        //> equality
        private Expression Equality()
        {
            Expression expr = Comparison();

            while (Match(LexemeType.BANG_EQUAL) || Match(LexemeType.EQUAL_EQUAL))
            {
                Lexeme op = Previous();
                Expression right = Comparison();
                expr = new Expression.Binary(expr, op, right);
            }

            return expr;
        }
        //< equality
        //> comparison
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
        //< comparison
        //> term
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
        //< term
        //> factor
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
        //< factor
        //> unary
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
                    Lexeme name = Consume(LexemeType.IDENTIFIER);
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
            if (Match(LexemeType.STRING) || Match(LexemeType.NUMBER))
            {
                return new Expression.Literal(Previous());
            }

            if ( Match(LexemeType.IDENTIFIER))
            {
                return new Expression.Variable(Previous());
            }

            if (Match(LexemeType.LEFT_PAREN))
            {
                Expression expr = Expr();
                Consume(LexemeType.RIGHT_PAREN);
                return new Expression.Grouping(expr);
            }

            throw new Exception(Peek() + "Expect expression");
        }

        private Expression FinishCall(Expression callee)
        {
            List<Expression> arguments = new List<Expression>();
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
                    
                    //****** arguments.Add(Expression());

                } while (Match(LexemeType.COMMA));
            }

            Lexeme paren = Consume(LexemeType.RIGHT_PAREN);

            return new Expression.Call(callee, paren, arguments);
        }
    }
}