using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamCompiler.Lexer
{

    public class KeyWords
    {
        public const string BREAK = "break";
        public const string DO = "do";
        public const string INSTANCEOF = "instanceof";
        public const string TYPEOF = "typeof";
        public const string CASE = "case";
        public const string ELSE = "else";
        public const string NEW = "new";
        public const string VAR = "var";
        public const string CATCH = "catch";
        public const string FINALLY = "finally";
        public const string RETURN = "return";
        public const string VOID = "void";
        public const string CONTINUE = "continue";
        public const string FOR = "for";
        public const string SWITCH = "switch";
        public const string WHILE = "while";
        public const string DEBUGGER = "debugger";
        public const string FUNCTION = "function";
        public const string THIS = "this";
        public const string WITH = "with";
        public const string DEFAULT = "default";
        public const string IF = "if";
        public const string THROW = "throw";
        public const string DELETE = "delete";
        public const string IN = "in";
        public const string TRY = "try";
        public const string INT = "int";
        public const string FLOAT = "float";
        public const string DOUBLE = "double";
        public const string LONG = "long";
        public const string OBJECT = "object";
        public const string BOOLEAN = "boolean";
        public const string STRING = "string";
        public const string CLASS = "class";
        public const string MAIN = "main";

        public const string LPAREN = "(";
        public const string RPAREN = ")";

        public enum KeyWordsEnum
        {
            Notakeyword,
            Break,
            Do,
            Instanceof,
            Typeof,
            Case,
            Else,
            New,
            Var,
            Catch,
            Finally,
            Return,
            Void,
            Continue,
            For,
            Switch,
            While,
            Debugger,
            Function,
            This,
            With,
            Default,
            If,
            Throw,
            Delete,
            In,
            Try,
            Int,
            Float,
            Double,
            Long,
            Object,
            Boolean,
            String,
            Class,
            Main,
        };

        public static List<string> keyWordNames = new List<string>()
        {
            {"break"},
            {"do"},
            {"instanceof"},
            {"typeof"},
            {"case"},
            {"else"},
            {"new"},
            {"var"},
            {"catch"},
            {"finally"},
            {"return"},
            {"void"},
            {"continue"},
            {"for"},
            {"switch"},
            {"while"},
            {"debugger"},
            {"function"},
            {"this"},
            {"with"},
            {"default"},
            {"if"},
            {"throw"},
            {"delete"},
            {"in"},
            {"try"},
            {"int"},
            {"float"},
            {"double"},
            {"long"},
            {"object"},
            {"boolean"},
            {"string"},
            {"class"},
            {"main"},
        };

        public static Dictionary<string, KeyWordsEnum> keyValuePairs = new Dictionary<string, KeyWordsEnum>()
        {
            { "break",      KeyWordsEnum.Break },
            { "do",         KeyWordsEnum.Do},
            { "instanceof", KeyWordsEnum.Instanceof },   
            { "typeof",     KeyWordsEnum.Typeof },       
            { "case",       KeyWordsEnum.Case },         
            { "else",       KeyWordsEnum.Else },         
            { "new",        KeyWordsEnum.New },          
            { "var",        KeyWordsEnum.Var },          
            { "catch",      KeyWordsEnum.Catch },        
            { "finally",    KeyWordsEnum.Finally },      
            { "return",     KeyWordsEnum.Return },       
            { "void",       KeyWordsEnum.Void },         
            { "continue",   KeyWordsEnum.Continue },     
            { "for",        KeyWordsEnum.For },          
            { "switch",     KeyWordsEnum.Switch },       
            { "while",      KeyWordsEnum.While },        
            { "debugger",   KeyWordsEnum.Debugger },     
            { "function",   KeyWordsEnum.Function },     
            { "this",       KeyWordsEnum.This },         
            { "with",       KeyWordsEnum.With },         
            { "default",    KeyWordsEnum.Default },      
            { "if",         KeyWordsEnum.If },           
            { "throw",      KeyWordsEnum.Throw },        
            { "delete",     KeyWordsEnum.Delete },       
            { "in",         KeyWordsEnum.In },           
            { "try",        KeyWordsEnum.Try },          
            { "int",        KeyWordsEnum.Int },          
            { "float",      KeyWordsEnum.Float },        
            { "double",     KeyWordsEnum.Double },       
            { "long",       KeyWordsEnum.Long },         
            { "object",     KeyWordsEnum.Object },       
            { "boolean",    KeyWordsEnum.Boolean },      
            { "string",     KeyWordsEnum.String },       
            { "class",      KeyWordsEnum.Class },        
            { "main",       KeyWordsEnum.Main },         
        };
    }
}
