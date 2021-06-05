using DreamCompiler;
using DreamCompiler.Scanner;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace DreamUnitTest
{
    using DreamCompiler.Lexer;
    using DreamCompiler.SyntaxAnalyzer;

    [TestClass]
    public class CompilerUnitTest
    {
        [TestMethod]
        public void TestEmptyMain()
        {
            string s =
            @"using System;
  
            class Testcompile {
  
                // Main Method
                protected static void Main()
                {
  
                    Console.WriteLine(""Main Method"");
                }
            }";
            try
            {
                var compiler = new Compiler();
                compiler.Go("testcompile", s);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[TestMethod]
        public void TestScanner()
        {
            LexicalAnalysis lexicalAnalysis = new LexicalAnalysis(new Scanner(@"..\..\..\..\examples\test.jlr"));
            var lexemeList = lexicalAnalysis.Analyze();

            SyntaxAnalyzer sa = new SyntaxAnalyzer();
            sa.Analyze(lexemeList);
        }


        //[TestMethod]
        public void TestScannerWithMemoryStream()
        {
            string program = "function main() = {{\r\n\tif ( 2<3 ) {{\r\n\t\tprintLine(\"foo\");\r\n\t}}\r\n}}\r\n";
            MemoryStream s = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(program));

            LexicalAnalysis lexicalAnalysis = new LexicalAnalysis(new Scanner(s));
            var lexemeList = lexicalAnalysis.Analyze();

            //Assert.AreEqual(s.Length, lexemeList.Count, "The numbers of tokens are not accurate");
        }



        private static MemoryStream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }



        //[TestMethod]
        public void TestAddBinaryExpression()
        {
            String binaryExpression = 
                @"function main() = { 
                    int x = 1 + 3 + 2; 
                    printLine(x);
                }";


            var node = new Compiler().Go("TestAddBinaryExpression", binaryExpression);
            Assert.IsNotNull(node);
        }


        //[TestMethod]
        public void TestBooleanExpression()
        {
            String binaryExpression =
                @"function main() = { 
                    int x = 1 + 3 + 2; 
                    printLine(x);
                }";

            var node = new Compiler().Go("TestAddBinaryExpression", binaryExpression);
            Assert.IsNotNull(node);
        }

        //[TestMethod]
        public void TestMultiplyParenthisizedExpression()
        {
            String binaryExpression =
                @"function main() = { 
                    int x = (2 + 3)+ 1 * 3; 
                    printLine(x);
                }";

            var node = new Compiler().Go("TestMultiplyParenthisizedExpression", binaryExpression);
            Assert.IsNotNull(node);
        }

        //[TestMethod]
        public void TestMultiplyBinaryExpression()
        {
            String binaryExpression =
                @"function main() = { 
                    int x = 1 * 3; 
                    printLine(x);
                }";

            var node = new Compiler().Go("TestMultiplyBinaryExpression", binaryExpression);
            Assert.IsNotNull(node);
        }


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

}
