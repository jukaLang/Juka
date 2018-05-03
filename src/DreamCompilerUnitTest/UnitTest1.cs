using System;
using System.Diagnostics;
using System.IO;
using DreamCompiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace DreamCompilerUnitTest
{

    using System.Collections.ObjectModel;
    using static System.Console;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCommpile()
        {
            try
            {
                if (File.Exists(@"..\..\..\..\examples\test.jlr"))
                {

                    MemoryStream memoryStream = null;

                    using (var stream = new FileStream(@"..\..\..\..\examples\test.jlr", FileMode.Open))
                    {
                        var byteArray = new byte[stream.Length];
                        stream.Read(byteArray, 0, (int) stream.Length);
                        memoryStream = new MemoryStream(byteArray);
                        var compiler = new Compiler();
                        compiler.Go(memoryStream);

                    }
			
                }
                /*
                using (MemoryStream memStream = new MemoryStream((int) file.Length))
                {
                    memStream.Write(file, 0, file.Length);
                    var compiler = new Compiler();
                    compiler.Go(memStream);
                }
                */


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [TestMethod]
        public void TestExpressions()
        {
            /*
            string[,] gradeArray =
                {{"chemistry", "history", "mathematics"}, {"78", "61", "82"}};

            Expression arrayExpression = Expression.Constant(gradeArray);

            MethodCallExpression methodCall = Expression.ArrayIndex(arrayExpression, Expression.Constant(0), Expression.Constant(3));

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
            */
        }

        public static String lambda_method()
        {
            Trace.WriteLine("message");
            return ";";
        }
    }


}
