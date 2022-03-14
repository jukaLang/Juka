using JukaCompiler.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace JukaCompiler.RoslynEmiter
{
    internal class RoslynGenerate : Stmt.Visitor<Stmt>
    {

        internal RoslynGenerate()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
            using System;

            namespace ConsoleApp1
            {
                public class Program
                {
                    static void Main(string[] args)
                    {
                        //Console.WriteLine("");
                        //Console.ReadLine();
                    }
                }
            }
            ");

            ParameterExpression value = Expression.Parameter(typeof(int), "value");

            // Creating an expression to hold a local variable.
            ParameterExpression result = Expression.Parameter(typeof(int), "result");

            // Creating a label to jump to from a loop.  
            LabelTarget label = Expression.Label(typeof(int));

            // Creating a method body.  
            BlockExpression block = Expression.Block(
                // Adding a local variable.  
                new[] { result },
                // Assigning a constant to a local variable: result = 1  
                Expression.Assign(result, Expression.Constant(1)),
                    // Adding a loop.  
                    Expression.Loop(
                       // Adding a conditional block into the loop.  
                       Expression.IfThenElse(
                           // Condition: value > 1  
                           Expression.GreaterThan(value, Expression.Constant(1)),
                           // If true: result *= value --  
                           Expression.MultiplyAssign(result,
                               Expression.PostDecrementAssign(value)),
                           // If false, exit the loop and go to the label.  
                           Expression.Break(label, result)
                       ),
                   // Label to jump to.  
                   label
                )
            );

            var dotNetCoreDir = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var console = MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location);
            var myruntime = MetadataReference.CreateFromFile(Path.Combine(dotNetCoreDir, "System.Runtime.dll"));

            var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication);
            options = options.WithAllowUnsafe(true);
            options = options.WithOptimizationLevel(OptimizationLevel.Debug);
            options = options.WithPlatform(Platform.X64);

            var compilation = CSharpCompilation.Create("MyCompilation",
            syntaxTrees: new[] { tree },
            references: new[] { mscorlib, console, myruntime },
            options: options);

            /*
            CallSiteBinder binder = new DynamicMetaObjectBinder();

            ConstantExpression[] arguments = new[] { Expression.Constant(5), Expression.Constant(2) };
            DynamicExpression exp = Expression.Dynamic(
                binder,
                typeof(object),
                arguments);

            var compiled = Expression.Lambda<Func<object>>(exp).Compile();
            var result = compiled();
            */
        }

        public Stmt VisitBlockStmt(Stmt.Block stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt VisitBreakStmt(Stmt.Break stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt VisitClassStmt(Stmt.Class stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt VisitExpressionStmt(Stmt.Expression stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt VisitFunctionStmt(Stmt.Function stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt VisitIfStmt(Stmt.If stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt VisitPrint(Stmt.Print stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt VisitPrintLine(Stmt.PrintLine stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt VisitReturnStmt(Stmt.Return stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt VisitVarStmt(Stmt.Var stmt)
        {
            throw new NotImplementedException();
        }

        public Stmt VisitWhileStmt(Stmt.While stmt)
        {
            throw new NotImplementedException();
        }

        internal void Generate(List<Stmt> stmts)
        {
            foreach(Stmt stmt in stmts)
            {
                stmt.Accept(this);
            }
        }
    }
}
