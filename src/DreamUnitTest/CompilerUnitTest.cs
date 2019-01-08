using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using DreamCompiler;
using System.Linq.Expressions;


namespace DreamUnitTest
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis;
    using System.Runtime.CompilerServices;



    [TestClass]
    public class CompilerUnitTest
    {
        [TestMethod]
        public void TestRoslyn()
        {

            SyntaxTree tree = CSharpSyntaxTree.ParseText(
              @"using System;
 
                namespace HelloWorld
                {
                    class Program
                    {
                        static void Main(string[] args)
                        {
                            Console.WriteLine(args[0]);
                        }
                    }
                }");

            CompileRoslyn.Parse(tree);

        }

        [TestMethod]
        public void TestVar()
        {
            string s = @"function main() = { 
                int x = 3;
            }";
            var memoryStream = GenerateStreamFromString(s);
            try
            {
                var compiler = new Compiler();
                compiler.Go("testcompile", memoryStream);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static MemoryStream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        [TestMethod]
        public void TestEmpty()
        {
            try
            {
                if (File.Exists(@"..\..\..\..\examples\empty.jlr"))
                {
                    using (var stream = new FileStream(@"..\..\..\..\examples\empty.jlr", FileMode.Open))
                    {
                        CSharpSyntaxNode node = Compile(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [TestMethod]
        public void TestCompile()
        {
            try
            {
                if (File.Exists(@"..\..\..\..\examples\test.jlr"))
                {
                    using (var stream = new FileStream(@"..\..\..\..\examples\test.jlr", FileMode.Open))
                    {
                       CSharpSyntaxNode node = Compile(stream); 
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 

        private static CSharpSyntaxNode Compile(FileStream stream)
        {
            MemoryStream memoryStream;
            var byteArray = new byte[stream.Length];
            stream.Read(byteArray, 0, (int)stream.Length);
            memoryStream = new MemoryStream(byteArray);
            var compiler = new Compiler();
            return compiler.Go("testcompile", memoryStream);
        }

        [TestMethod]
        public void TestBinaryExpression()
        {
            String binaryExpression = @"function main() = { int x = 1 + 2 * 3; }";
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(binaryExpression);
            MemoryStream stream = new MemoryStream(byteArray);

            var node = new Compiler().Go("binaryexpression", stream);
            Assert.IsNotNull(node);
        }


        [TestMethod]
        public void TestCallSiteBinder()
        {
            CallSiteBinder binder = new Binder();

            var site = CallSite<Func<CallSite, object, object, object>>.Create(binder);

            var sum = site.Target(site, 5, 2);

            ParameterExpression x = Expression.Variable(typeof(int), "x");
            ParameterExpression y = Expression.Variable(typeof(int), "y");

            Expression blockExpression = Expression.Block(new ParameterExpression[] { x, y },
            Expression.Assign(x, Expression.Constant(2)),
            Expression.Assign(y, Expression.Constant(5)));


            ParameterExpression[] arguments = new[] { x, y };

            DynamicExpression exp = Expression.Dynamic(
                binder,
                typeof(object),
                arguments);

            var compiled = Expression.Lambda<Func<object>>(exp).Compile();
            var result = compiled();
            Console.WriteLine(result);
            Console.WriteLine("DONE");
        }

        [TestMethod]
        public void TestCallSiteBinder2()
        {

            CallSiteBinder binder = new Binder();

            ConstantExpression[] arguments = new[] { Expression.Constant(5), Expression.Constant(2) };

            DynamicExpression exp = Expression.Dynamic(
                binder,
                typeof(object),
                arguments);

            var compiled = Expression.Lambda<Func<object>>(exp).Compile();
            var result = compiled();
        }


        [TestMethod]
        public void TestExpressions()
        {
            var left = Expression.Add(Expression.Constant(3), Expression.Constant(4));
            var right = Expression.Divide(left, Expression.Constant(3));

            var block1 = Expression.Block(right);

            Expression.Lambda(block1).Compile().DynamicInvoke();


            string[,] gradeArray =
                {{"chemistry", "history", "mathematics"}, {"78", "61", "82"}};

            Expression arrayExpression = Expression.Constant(gradeArray);

            MethodCallExpression methodCall = Expression.ArrayIndex(arrayExpression, Expression.Constant(0), Expression.Constant(2));

            try
            {
                var theLambda = Expression.Lambda(methodCall).Compile().DynamicInvoke();
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.Message);
                throw;
            }


            BinaryExpression binary = Expression.MakeBinary(
                ExpressionType.Subtract,
                Expression.Constant(54),
                Expression.Constant(14));

            BinaryExpression binary1 = Expression.MakeBinary(
                ExpressionType.Subtract,
                Expression.Constant(Convert.ToInt32("45")),
                Expression.Constant(14));

            Expression[] expressions = new Expression[]
            {
                methodCall,
                binary,
                binary1
            };

            BlockExpression block = Expression.Block(expressions);

            foreach (Expression ex in block.Expressions)
            {
                Trace.WriteLine(ex.ToString());
                Trace.WriteLine(ex.Reduce().ToString());

                Trace.WriteLine(Expression.Lambda(ex).Compile().DynamicInvoke());
            }

        }

        [TestMethod]
        public void TestVariables()
        {
            ParameterExpression expression = Expression.Variable(typeof(string), "x");
            var binaryExpressionOne = Expression.Assign(expression, Expression.Constant("ASDF"));

            var x = 3;
            BinaryExpression binaryExpression =
                Expression.MakeBinary(
                    ExpressionType.Subtract,
                    Expression.Constant(53),
                    Expression.Constant(14)
               );

            Console.WriteLine(Expression.Add(Expression.Constant(x), binaryExpression).ToString());

        }


        [TestMethod]
        public void TestFunctionCall()
        {


            //Expression.Call()
            string[,] gradeArray =
                {{"chemistry", "history", "mathematics"}, {"78", "61", "82"}};

            System.Linq.Expressions.Expression arrayExpression =
                System.Linq.Expressions.Expression.Constant(gradeArray);

            // Create a MethodCallExpression that represents indexing
            // into the two-dimensional array 'gradeArray' at (0, 2).
            // Executing the expression would return "mathematics".
            System.Linq.Expressions.MethodCallExpression methodCallExpression =
                System.Linq.Expressions.Expression.ArrayIndex(
                    arrayExpression,
                    System.Linq.Expressions.Expression.Constant(0),
                    System.Linq.Expressions.Expression.Constant(2));

            var writeLineExpression = Expression.Call(null,
                typeof(Trace).GetMethod("WriteLine", new Type[] { typeof(object) }) ??
                throw new InvalidOperationException(),
                Expression.Constant("this is a test"));


            var innerLabel = Expression.Label();

            var expressionBlock = Expression.Block(Expression.Label(innerLabel), writeLineExpression);
            var block = Expression.Block(Expression.Goto(innerLabel), expressionBlock);
            var compiledCode = Expression.Lambda<Action>(block).Compile();

            compiledCode();
        }


        [TestMethod]
        public void TestBlock()
        {
            ParameterExpression value = Expression.Parameter(typeof(int), "value");
            ParameterExpression result = Expression.Parameter(typeof(int), "result");

            Func<string, string> action = (s) =>
            {
                BlockExpression writeHelloBlockExpression = Expression.Block(
                    Expression.Call(null,
                        typeof(Trace).GetMethod("WriteLine", new Type[] { typeof(String) }) ??
                        throw new InvalidOperationException(),
                        Expression.Constant("0s")
                    ));

                return String.Empty;
            };

            var actionType = action.GetType();


            LabelTarget label = Expression.Label(typeof(int));
            var lableExpression = Expression.Break(label, result);
            var greaterThanExpression = Expression.GreaterThan(value, Expression.Constant(1));
            var postDecrementExpression = Expression.PostDecrementAssign(value);
            var multiplyExpression = Expression.MultiplyAssign(result, postDecrementExpression);


            var callMethodExpression = Expression.Call(null,
                typeof(Trace).GetMethod("WriteLine", new Type[] { typeof(String) }) ?? throw new InvalidOperationException(),
                Expression.Constant("World!")
            );

            var tempBlock = Expression.Block(multiplyExpression, callMethodExpression);//, callWrite);
            var ifThanElseExpression = Expression.IfThenElse(greaterThanExpression, tempBlock, lableExpression);
            BlockExpression block = Expression.Block(
                new[] { result },
                Expression.Assign(result, Expression.Constant(1)),
                Expression.Loop(ifThanElseExpression, label)
            );

            int factorial = Expression.Lambda<Func<int, int>>(block, value).Compile()(5);


            if (factorial != 120)
            {
                throw new Exception("");
            }

            Console.WriteLine(factorial);
        }
    }

    class Binder : BinaryOperationBinder
    {
        public Binder() : base(ExpressionType.Subtract)
        {
        }

        public override DynamicMetaObject FallbackBinaryOperation(DynamicMetaObject target, DynamicMetaObject arg,
            DynamicMetaObject errorSuggestion)
        {
            if (target.RuntimeType == arg.RuntimeType)
            {
                var expression = Expression.Lambda<Func<int>>(Expression.MakeBinary(
                    ExpressionType.Add,
                    Expression.Constant(target.Value),
                    Expression.Constant(arg.Value))).Compile()();

                var convertedExpression = Expression.Convert(Expression.Constant(expression), typeof(object));

                return new DynamicMetaObject(convertedExpression, BindingRestrictions.Empty);
            }

            throw new Exception("can't bind");
        }

    }

    class CompileRoslyn
    {
        private static readonly System.Collections.Generic.IEnumerable<string> DefaultNamespaces =
            new[]
            {
                "System",
                "System.IO",
                "System.Net",
                "System.Linq",
                "System.Text",
                "System.Text.RegularExpressions",
                "System.Collections.Generic"
            };


        private static readonly string rt = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory() + "{0}.dll";
        //private static string runtimePath = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\{0}.dll";

        private static readonly System.Collections.Generic.IEnumerable<MetadataReference> DefaultReferences =
            new[]
            {
                MetadataReference.CreateFromFile(string.Format(rt, "mscorlib")),
                MetadataReference.CreateFromFile(string.Format(rt, "System")),
                MetadataReference.CreateFromFile(string.Format(rt, "System.Core"))
            };

        private static readonly CSharpCompilationOptions DefaultCompilationOptions =
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOverflowChecks(true).WithOptimizationLevel(OptimizationLevel.Release)
                    .WithUsings(DefaultNamespaces);

        public static string Rt =>System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory() + "{0}.dll";

        public static void Parse(SyntaxTree parsedSyntaxTree)
        {
            var compilation = CSharpCompilation.Create("Test", syntaxTrees: new[] {parsedSyntaxTree},references: DefaultReferences);
            try
            {
                var result = compilation.Emit(@"Test.exe", @"test.pdb");

                Console.WriteLine(result.Success ? "Sucess!!" : "Failed");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
