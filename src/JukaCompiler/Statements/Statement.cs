using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JukaCompiler.Lexer;
using JukaCompiler.Parse;

namespace JukaCompiler.Statements
{
    internal abstract class Stmt
    {
        internal interface Visitor<R>
        { 
            R VisitBlockStmt(Block stmt);
            R VisitFunctionStmt(Function stmt);
            R VisitClassStmt(Class stmt);
            R VisitExpressionStmt(Expression stmt);
            R VisitIfStmt(If stmt);
            R VisitPrintStmt(Print stmt);
            R VisitReturnStmt(Return stmt);
            R VisitVarStmt(Var stmt);
            R VisitWhileStmt(While stmt);
        }
        internal abstract R Accept<R>(Stmt.Visitor<R> vistor);
        internal class Block : Stmt
        {
            internal Block(List<Stmt> statements)
            {
                this.statements = statements;
            }

            internal List<Stmt> statements;
            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
            }
        }
        internal class Function : Stmt
        {
            internal List<Stmt> body = new List<Stmt>();
            internal string name;
            internal List<TypeParameterMap> typeParameterMaps;

            internal Function(string name, List<TypeParameterMap> parametersMap, List<Stmt> body)
            {
                this.name = name;
                this.typeParameterMaps = parametersMap;
                this.body = body;
            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
            }
        }
        internal class Class : Stmt
        {
            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
            }
        }
        //internal class Expression : Stmt
        //{
        //    internal override R Accept<R>(Visitor<R> vistor)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
        internal class If : Stmt
        {
            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
            }
        }
        internal class Print : Stmt
        {
            internal Expression expr;

            internal Print(Expression expr)
            {
                this.expr = expr;
            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                return vistor.VisitPrintStmt(this);
            }
        }
        internal class Var : Stmt
        {
            internal Lexeme name;
            internal Expression? exprInitializer;
            internal bool isInitalizedVar = false;

            internal Var(Lexeme name, Expression expr)
            {
                this.name=name;
                this.exprInitializer=expr;
                this.isInitalizedVar = true;
            }

            internal Var(Lexeme name)
            {
                this.name = name;
                exprInitializer = null;
            }

            internal override R Accept<R>(Visitor<R> vistor)
            {
                return vistor.VisitVarStmt(this);
            }
        }
        internal class While : Stmt
        {
            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
            }
        }
        internal class Return : Stmt
        {
            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
            }

        }
    }
}
