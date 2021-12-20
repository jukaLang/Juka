using System.Text;

namespace JukaCompiler.Lexer
{
    internal enum LexemeType
    {
        UNDEFINED,
        CHAR,

        // Literals.
        STRING,
        IDENTIFIER,
        NUMBER,
        WHITESPACE,
        SYMBOL,
        EOF,

        // Single-character tokens.
        LEFT_PAREN,
        RIGHT_PAREN,
        LEFT_BRACE,
        RIGHT_BRACE,
        COMMA,
        DOT,
        MINUS,
        PLUS,
        SEMICOLON,
        SLASH,
        STAR,

        // One or two character tokens.
        BANG,
        BANG_EQUAL,
        EQUAL,
        EQUAL_EQUAL,
        GREATER,
        GREATER_EQUAL,
        LESS,
        LESS_EQUAL,

        // Keywords.
        AND,
        CLASS,
        ELSE,
        FALSE,
        FUNC,
        FOR,
        IF,
        NULL,
        OR,
        PRINT,
        RETURN,
        SUPER,
        THIS,
        TRUE,
        VAR,
        WHILE,
        INT,
        LONG,
        SHORT,
        DOUBLE,
        FLOAT,
        BOOL
    }

    internal class Lexeme
    {
        private StringBuilder tokenBuilder = new StringBuilder();
        private bool isKeyWord = false;
        private LexemeType typeOfKeyWord;
        internal LexemeType LexemeType { get; set; }

        internal Lexeme()
        {
        }

        internal Lexeme(LexemeType ltype) : this()
        {
            this.LexemeType = ltype;
        }

        internal void AddToken(char token)
        {
            tokenBuilder.Append(token);
        }

        internal bool IsKeyWord
        {
            set { isKeyWord = true; }
            get { return isKeyWord; }
        }

        internal LexemeType TypeOfKeyWord
        {
            get { return this.typeOfKeyWord; } 
            set { this.typeOfKeyWord = value;}
        }

        public override string ToString()
        {
            return tokenBuilder.ToString();
        }
    }
}
