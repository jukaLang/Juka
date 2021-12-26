using System.Text;

namespace JukaCompiler.Lexer
{
    internal class LexemeType
    {
        internal static Int64 UNDEFINED             = 0x01;
        internal static Int64 CHAR                  = 0x02;
        internal static Int64 INTERNALFUNCTION      = 0x04;
        internal static Int64 STRING                = 0x08;
        internal static Int64 IDENTIFIER            = 0x10;
        internal static Int64 NUMBER                = 0x20;
        internal static Int64 WHITESPACE            = 0x40;
        internal static Int64 SYMBOL                = 0x80;
        internal static Int64 EOF                   = 0x100;
        internal static Int64 LEFT_PAREN            = 0x200;
        internal static Int64 RIGHT_PAREN           = 0x400;
        internal static Int64 LEFT_BRACE            = 0x800;
        internal static Int64 RIGHT_BRACE           = 0x1000;
        internal static Int64 COMMA                 = 0x2000;
        internal static Int64 DOT                   = 0x4000;
        internal static Int64 MINUS                 = 0x8000;
        internal static Int64 PLUS                  = 0x10000;
        internal static Int64 SEMICOLON             = 0x20000;
        internal static Int64 SLASH                 = 0x400000;
        internal static Int64 STAR                  = 0x800000;
        internal static Int64 BANG                  = 0x1000000;
        internal static Int64 BANG_EQUAL            = 0x2000000;
        internal static Int64 EQUAL                 = 0x4000000;
        internal static Int64 EQUAL_EQUAL           = 0x8000000;
        internal static Int64 GREATER               = 0x10000000;
        internal static Int64 GREATER_EQUAL         = 0x20000000;
        internal static Int64 LESS                  = 0x40000000;
        internal static Int64 LESS_EQUAL            = 0x80000000;
        internal static Int64 AND                   = 0x100000000;
        internal static Int64 CLASS                 = 0x200000000;
        internal static Int64 ELSE                  = 0x400000000;
        internal static Int64 FALSE                 = 0x800000000;
        internal static Int64 FUNC                  = 0x1000000000;
        internal static Int64 FOR                   = 0x2000000000;
        internal static Int64 IF                    = 0x4000000000;
        internal static Int64 NULL                  = 0x8000000000;
        internal static Int64 OR                    = 0x10000000000;
        internal static Int64 PRINTLINE             = 0x20000000000;
        internal static Int64 RETURN                = 0x40000000000;
        internal static Int64 SUPER                 = 0x80000000000;
        internal static Int64 THIS                  = 0x10000000000;
        internal static Int64 TRUE                  = 0x20000000000;
        internal static Int64 VAR                   = 0x40000000000;
        internal static Int64 WHILE                 = 0x80000000000;
        internal static Int64 INT                   = 0x100000000000;
        internal static Int64 LONG                  = 0x200000000000;
        internal static Int64 SHORT                 = 0x400000000000;
        internal static Int64 DOUBLE                = 0x800000000000;
        internal static Int64 FLOAT                 = 0x1000000000000;
        internal static Int64 BOOL                  = 0x2000000000000;
    }
    internal class Lexeme
    {
        private StringBuilder tokenBuilder = new StringBuilder();
        private bool isKeyWord = false;
        private Int64 typeOfKeyWord;
        internal Int64 LexemeType { get; set; }

        internal Lexeme()
        {
        }

        internal Lexeme(Int64 ltype) : this()
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

        internal Int64 TypeOfKeyWord
        {
            get { return this.typeOfKeyWord; } 
            set { this.typeOfKeyWord = value;}
        }

        public override string ToString()
        {
            return tokenBuilder.ToString();
        }

        internal string Literal()
        {
            return ToString();
        }
    }
}
