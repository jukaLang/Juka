using JukaCompiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
        public void ArrayTest()
        {
            sourceAsString +=
                @"func test_func() = 
                {
                    var x = array[3];
                    //x[1] = ""test"";
                    print(x[0]); 
                }";

            var value = Go();
            Assert.AreEqual("System.Object", value);
        }

        [TestMethod]
        public void WhileBoolean()
        {
            sourceAsString +=
                @"func test_func() = 
                {
                    var x = true;
                    while(x == true)
                    {
                        print(""y"");
                        x = false;
                    }
                }";

            Assert.AreEqual("y", Go());
        }

        [TestMethod]
        public void Fibonacci()
        {
            sourceAsString +=
                @"func test_func() = 
                {
                    var x = array[3];
                    print(""y"");
                }";

            Assert.AreEqual("y", Go());
        }

       [TestMethod]
        public void IfBoolean()
        {
            sourceAsString +=
                @"func test_func() = 
                {
                    var x = true;
                    if ( x == true)
                    {
                        print(""x"");
                    }
                    else
                    {
                        print(""y"");
                    }
                }";

            Assert.AreEqual("x", Go());
        }

        [TestMethod]
        public void IfBooleanElseBranch()
        {
            sourceAsString +=
                @"func test_func() = 
                {
                    var x = false;
                    if ( x == true)
                    {
                        print(""x"");
                    }
                    else
                    {
                        print(""y"");
                    }
                }";

            Assert.AreEqual("y", Go());
        }

        [TestMethod]
        public void PrintLiteral()
        {
            sourceAsString +=
                @"func test_func() = 
                {
                    printLine(""print""); 
                }";

            var value = Go();
            Assert.AreEqual("print\r\n", value);
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
            sourceAsString += @"func test_func() =
                {
                    var y = ""one two three"";
                    /*nest1(y);*/
                }";

            Assert.AreEqual("", Go());
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

            Assert.AreEqual("a3" , Go());
        }

        [TestMethod]
        public void Add()
        {
            sourceAsString += @"
                func test_func() = {
                    var x=32; 
                    var y=33; 
                    var z=x+y;
                    print(z);
                 }";

            var value = Go();
            Assert.AreEqual("65" , Go());
        }

        [TestMethod]
        public void Subtract()
        {
            sourceAsString += @"func test_func() = {
                var x=32; var y=33; var z=x-y;
                print(z);
            }";

            var x = Go();
            Assert.AreEqual("-1", x);
        }

        [TestMethod]
        public void Divide()
        {
            sourceAsString += @"func test_func() =
            {
                var x=10; 
                var y=3;
                var z=x/y;
                print(z);
            }";

            Assert.AreEqual("3", Go());
        }

        [TestMethod]
        public void Multiply()
        {
            sourceAsString += @"func test_func() = 
            {
                var x=3;
                var y=3;
                var z=x*y;
                print(z);
            }";

            Assert.AreEqual("9" , Go());
        }

        [TestMethod]
        public void TestSourceAsFile()
        {
            Compiler compiler = new Compiler();

            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            

            var outputValue = compiler.Go(@"..\..\..\..\..\examples\test2.juk");
            if (compiler.HasErrors())
            {
                var errors = compiler.ListErrors();
                foreach(var error in errors)
                {
                    Assert.IsTrue(false, error);
                }
            }

            Assert.AreEqual("AsdfA" + Environment.NewLine, outputValue);
        }


        [TestMethod]
        public void Class()
        {
            sourceAsString += @"
                class x = 
                {
                    func xmethod() = 
                    {
                        print(""foo"");
                    }

                    func zmethod() = 
                    {
                        print(""bar"");
                    }
                }

                func test_func() = 
                {
                    var v = x();
                    v.xmethod();
                    v.zmethod();
                }";

            Assert.AreEqual("foobar", Go());
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

            Assert.AreNotEqual(0, Go());
        }
    }
}