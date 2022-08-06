using JukaCompiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace JukaUnitTest
{
    [TestClass]
    public class CompilerUnitTest
    {

        string sourceAsString =
            @"func main() = 
                {
                    test_func();
                }";


        private string Go(string source)
        {
            Compiler compiler = new Compiler();
            var outputValue = compiler.Go(sourceAsString, false);
            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }

            return outputValue;
        }

        [TestMethod]
        public void PrintLiteral()
        {
            sourceAsString +=
                @"func test_func() = 
                {
                    print(""print""); 
                }";

            var value = Go(sourceAsString);
            Assert.AreEqual("print", value);
        }

        [TestMethod]
        public void PrintVariable()
        {
            sourceAsString +=
                @"func test_func() = 
                {
                    var x = ""print"";
                    print(x); 
                }";

            Assert.AreEqual("print", Go(sourceAsString));
        }

        [TestMethod]
        public void TestEmptyString()
        {
            Compiler compiler = new Compiler();
            string sourceAsString = @"../../../../../examples/dontest.juk";

            var outputValue = compiler.Go(sourceAsString, true);
            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }
            Assert.AreEqual("process"+ Environment.NewLine+"foo"+ Environment.NewLine+"main"+Environment.NewLine, outputValue);
        }

        [TestMethod]
        public void TestEmptyComment()
        {
            Compiler compiler = new Compiler();
            string sourceAsString = "func main() = {} /*saddsadsa*dasdas/asdasd*//**///!sssd";

            var outputValue = compiler.Go(sourceAsString, false);
            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }
            Assert.AreEqual("", outputValue);
        }

        [TestMethod]
        public void TestFunctionCall()
        {
            Compiler compiler = new Compiler();
            string testfunccall = @"
            func x() = {
                var u = ""3"";
                print(u); 
            }

            func main() = 
            {
                x();
            }";


            var outputValue = compiler.Go(testfunccall, false);
            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }
            Assert.AreEqual("3", outputValue);
        }


        [TestMethod]
        public void TestMultipleVariables()
        {
            Compiler compiler = new Compiler();
            string sourceAsString = @"func main() = { 
                var x=32; 

                printLine(x);
            }";

            var outputValue = compiler.Go(sourceAsString, false);
            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }
            Assert.AreEqual("32" + Environment.NewLine, outputValue);
        }

        [TestMethod]
        public void TestOperationAdd()
        {
            Compiler compiler = new Compiler();
            string sourceAsString = @"func main() = {
                var x=32 + 33;
                printLine(x);
             }";

            var outputValue = compiler.Go(sourceAsString, false);
            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }
            Assert.AreEqual("65" + Environment.NewLine, outputValue);
        }

        [TestMethod]
        public void TestOperationSubtract()
        {
            Compiler compiler = new Compiler();
            string sourceAsString = @"func main() = {
                var x=32-33;
                printLine(x);
            }";

            var outputValue = compiler.Go(sourceAsString, false);
            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }
            Assert.AreEqual("-1" + Environment.NewLine, outputValue);
        }

        [TestMethod]
        public void TestOperationDivide()
        {
            Compiler compiler = new Compiler();
            string sourceAsString = @"func main() = {
                var x=10/3;
                printLine(x);
            }";

            var outputValue = compiler.Go(sourceAsString, false);
            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }
            Assert.AreEqual("3" + Environment.NewLine, outputValue);
        }

        [TestMethod]
        public void TestOperationMultiply()
        {
            Compiler compiler = new Compiler();
            string sourceAsString = @"func main() = {
                var x=3*3;
                printLine(x);
            }";

            var outputValue = compiler.Go(sourceAsString, false);
            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }
            Assert.AreEqual("9" + Environment.NewLine, outputValue);
        }


        [TestMethod]
        public void TestSourceAsFile()
        {
            Compiler compiler = new Compiler();

            var outputValue = compiler.Go(@"../../../../../examples/test.juk");
            if (compiler.HasErrors())
            {
                var errors = compiler.ListErrors();
                foreach(var error in errors)
                {
                    Assert.IsTrue(false, error);
                }
            }

            //Assert.AreEqual("AsdfA" + Environment.NewLine, outputValue);
        }

        [TestMethod]
        public void TestSourceAsFile2()
        {
            Compiler compiler = new Compiler();
            var outputValue = compiler.Go(@"../../../../../examples/test2.juk");
            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }
            Assert.AreEqual("AsdfA" + Environment.NewLine, outputValue);
        }

        [TestMethod]
        public void TestClass()
        {
            Compiler compiler = new Compiler();
            string sourceAsString = "class x = {  }; func main(){}";

            var outputValue = compiler.Go(sourceAsString, false);
            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }
            //Assert.AreEqual("", outputValue);
        }

        [TestMethod]
        public void TestMain()
        {
            Compiler compiler = new Compiler();
            string sourceAsString = "func main() = { print(\"Hello World\"); }";

            var outputValue = compiler.Go(sourceAsString, false);
            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }
            //Assert.AreEqual("Hello World", outputValue);
        }
    }
}