using JukaCompiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.CodeAnalysis;

namespace JukaUnitTest
{
    [TestClass]
    public class CompilerUnitTest
    {
        /// <summary>
        /// Default main function calls hard code test func"
        /// </summary>
        string sourceAsString =
            @"func main() = 
                {
                    test_func();
                }";


        private string Go()
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

            var value = Go();
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

            Assert.AreEqual("print", Go());
        }

        [TestMethod]
        public void PassVariable()
        {
            sourceAsString +=
                @"func test_func() = 
                {
                    var x = ""print"";
                    varpass(x);
                }
                
                func varpass(var x) = 
                {
                    print(x); 
                }";

            var value = Go();
            Assert.AreEqual("print", value);
        }

        [TestMethod]
        public void PrintThreeLevelsNesting()
        {
            sourceAsString +=
                @"func test_func() = 
                {
                    var y = ""one two three"";
                    nest1(y);
                }
                
                func nest1(var y) = 
                {
                    nest2(y);
                }

                func nest2(var z) = 
                {
                    print(z);
                }";

            var value = Go();
            Assert.AreEqual("one two three", value);
        }

        [TestMethod]
        public void EmptyComment()
        {
            Compiler compiler = new Compiler();
            sourceAsString += @"func test_func() =
                {
                    var y = ""one two three"";
                    /*nest1(y);*/
                }";

            var outputValue = compiler.Go(sourceAsString, false);
            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }
            Assert.AreEqual("", outputValue);
        }

        [TestMethod]
        public void MultipleVariables()
        {
            sourceAsString += @"
                func test_func() = 
                {
                    var z = 3;
                    var x=""a""; 
                    print(x);
                    print(z);
                }";

            Compiler compiler = new Compiler();
            var outputValue = new Compiler().Go(sourceAsString, false);

            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }

            Assert.AreEqual("a3" , outputValue);
        }

        [TestMethod]
        public void Add()
        {
            sourceAsString += @"
                func test_func() = {
                    var x=32; 
                    var y=33; 
                    var z=x+y;
                    printLine(z);
                 }";
            
            Compiler compiler = new Compiler();
            var outputValue = compiler.Go(sourceAsString, false);

            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }
            Assert.AreEqual("65" + Environment.NewLine, outputValue);
        }

        [TestMethod]
        public void Subtract()
        {
            Compiler compiler = new Compiler();
            sourceAsString += @"func test_func() = {
                var x=32; var y=33; var z=x-y;
                printLine(z);
            }";

            var outputValue = compiler.Go(sourceAsString, false);
            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }
            Assert.AreEqual("-1" + Environment.NewLine, outputValue);
        }

        [TestMethod]
        public void Divide()
        {
            sourceAsString += @"func test_func() =
            {
                var x=10; 
                var y=3;
                var z=x/y;
                printLine(z);
            }";

            Compiler compiler = new Compiler();
            var outputValue = compiler.Go(sourceAsString, false);
            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }
            Assert.AreEqual("3" + Environment.NewLine, outputValue);
        }

        [TestMethod]
        public void Multiply()
        {
            sourceAsString += @"func test_func() = 
            {
                var x=3;
                var y=3;
                var z=x*y;
                printLine(z);
            }";

            Compiler compiler = new Compiler();
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
        public void Class()
        {
            Compiler compiler = new Compiler();
            sourceAsString += @"
                class x = {  }
                func test_func() = 
                {
                }";

            var outputValue = compiler.Go(sourceAsString, false);
            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }
            
            Assert.AreEqual(String.Empty, outputValue);
        }

        [TestMethod]
        public void Primitives()
        {
            sourceAsString += @"
                func test_func() = 
                {
                    availableMemory();
                }

                func availableMemory() = 
                {
                    var v = getAvailableMemory();
                    print(v);
                }";

            //func test_func() = 
            //{
            //}
            //func test_func() = 
            //{
            //}";

            Compiler compiler = new Compiler();
            var outputValue = compiler.Go(sourceAsString, false);
            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }

            Assert.AreNotEqual(0, double.Parse(outputValue));
        }
    }
}