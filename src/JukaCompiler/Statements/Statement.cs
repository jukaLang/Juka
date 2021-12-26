using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JukaCompiler.Parse;

namespace JukaCompiler.Statements
{
    internal abstract class Stmt
    {
        internal interface Visitor<R>
        { 
            R visitBlockStmt(Block stmt);
            R visitFunctionStmt(Function stmt);
            R visitClassStmt(Class stmt);
            R visitExpressionStmt(Expression stmt);
            R visitIfStmt(If stmt);
            R visitPrintStmt(Print stmt);
            R visitReturnStmt(Return stmt);
            R visitVarStmt(Var stmt);
            R visitWhileStmt(While stmt);
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
                return vistor.visitPrintStmt(this);
            }
        }
        internal class Var : Stmt
        {
            internal override R Accept<R>(Visitor<R> vistor)
            {
                throw new NotImplementedException();
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
