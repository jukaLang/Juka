using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JukaCompiler.Scan;
using JukaCompiler.Lexer;
using JukaCompiler.Statements;
using System.Reflection;

namespace JukaCompiler.Parse
{
    public class Parser
    {
        private List<Lexeme> tokens;
        private int current = 0;
        private Scanner scanner;
        internal Parser(Scanner scanner)
        {
            this.scanner = scanner;
        }

        internal List<Stmt> Parse()
        {
            tokens = this.scanner.Scan();
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

            Advance();
            return new Stmt.Block();
        }

        private bool MatchKeyWord()
        {
            if(Match(LexemeType.INT) || 
                Match(LexemeType.STRING) ||
                Match(LexemeType.FLOAT) ||
                Match(LexemeType.DOUBLE))
            {
                return true;
            }

            return false;
        }

        private Lexeme Consume(LexemeType type)
        {
            if (Check(type))
            {
                return Advance();
            }

            throw new Exception("");
        }

        private Lexeme ConsumeKeyword()
        {
            if (CheckKeyWord())
            {
                return Advance();
            }

            throw new Exception();
        }

        private bool Match(LexemeType lexType)
        {
            //foreach(LexemeType lex in Enum.GetValues(typeof(LexemeType)))
            //{
                if (Check(lexType))
                {
                    Advance();
                    return true;
                }
            //}

            return false;
        }


        private bool Check(LexemeType type)
        {
            if (IsAtEnd())
            {
                return false;
            }

            if (Peek().IsKeyWord)
            {
                return Peek().TypeOfKeyWord == type;
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
            Lexeme lparen = Consume(LexemeType.LEFT_PAREN);
            var typeMap = new List<TypeParameterMap>();

            if (!Check(LexemeType.RIGHT_PAREN))
            {
                do
                {
                    var parameterType = ConsumeKeyword();
                    var varName = Consume(LexemeType.IDENTIFIER);
                    if (parameterType.IsKeyWord)
                    {
                        typeMap.Add(new TypeParameterMap( parameterType,varName ));
                    }
                }
                while (Match(LexemeType.COMMA));
            }
            Lexeme rparen = Consume(LexemeType.RIGHT_PAREN);
            Lexeme equal = Consume(LexemeType.EQUAL);
            var leftBrace = Consume(LexemeType.LEFT_BRACE);

            List<Stmt> statements = new List<Stmt>();

            if (!Check(LexemeType.RIGHT_BRACE))
            {
                do
                {
                    statements = Block();
                }
                while(Match(LexemeType.RIGHT_BRACE));
            }
            var rightBrace = Consume(LexemeType.RIGHT_BRACE);

            var stmt = new Stmt.Function(name.ToString(), typeMap, statements);
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
            Expression expr;
            if (Match(LexemeType.EQUAL))
            {
                expr = Expr();
            }

            return new Stmt.Var();
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

                if (expr is Expression.Variable) {
                    Lexeme name = ((Expression.Variable)expr).Name;
                    return new Expression.Assign(name, value);
                    //> Classes assign-set
                } else if (expr is Expression.Get) {
                    Expression.Get get = (Expression.Get)expr;
                    return new Expression.Set(get, get.Name, value);
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
            return null;
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

            //return new Expr.Call(callee, paren, arguments);
            return null;
        }
    }
}