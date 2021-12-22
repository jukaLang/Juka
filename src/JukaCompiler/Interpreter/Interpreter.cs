using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JukaCompiler.Parse;
using JukaCompiler.Statements;

namespace JukaCompiler.Interpreter
{
    internal class Interpreter : Stmt.Visitor<Stmt>
    {
        internal void Interpert(List<Stmt> statements)
        {
            foreach(var stmt in statements)
            {
                Execute(stmt);
            }
        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        Stmt Stmt.Visitor<Stmt>.visitBlockStmt(Stmt.Block stmt)
        {
            throw new NotImplementedException();
        }

        Stmt Stmt.Visitor<Stmt>.visitFunctionStmt(Stmt.Function stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt visitClassStmt(Stmt.Class stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt visitExpressionStmt(Parse.Expression stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt visitIfStmt(Stmt.If stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt visitPrintStmt(Stmt.Print stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt visitReturnStmt(Stmt.Return stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt visitVarStmt(Stmt.Var stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt visitWhileStmt(Stmt.While stmt)
        {
            throw new NotImplementedException();
        }
    }
}
