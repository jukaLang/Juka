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

        /*[TestMethod]
        public void ArrayTest()
        {
            sourceAsString +=
                @"func test_func() = 
                {
                    var x = array[3];
                    x[1] = ""test"";
                    print(x[1]); 
                }";

            var value = Go();
            Assert.AreEqual("test", value);
        }*/

        [TestMethod]
        public void VariablesPrint()
        {
            sourceAsString +=
                @"func test_func() = 
                {
                    var x = 32;
                    var z = x + 5;
                    var m = z + z;
                    print(z);
                }";

            Assert.AreEqual("37", Go());
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
        [DataRow(3,"3")]
        [DataRow(-1,"-1")]
        [DataRow(0,"0")]
        public void PrintLiteral(dynamic value, string expected)
        {
            sourceAsString +=
                @"func test_func() = 
                {
                    printLine("+value+@"); 
                }";
            Assert.AreEqual(expected + Environment.NewLine,  Go());
        }

        [TestMethod]
        [DataRow(3,"3")]
        [DataRow(-1,"-1")]
        [DataRow(0,"0")]
        public void PrintVariable(dynamic value, string expected)
        {
            sourceAsString +=
                @"func test_func() = 
                {
                    var x = "+value+@";
                    print(x); 
                }";

            Assert.AreEqual(expected, Go());
        }

        [TestMethod]
        [DataRow(3,"3")]
        [DataRow(-1,"-1")]
        [DataRow(0,"0")]
        public void PassVariable(dynamic value, string expected)
        {
            sourceAsString +=
                @"func test_func() = 
                {
                    var x = "+value+@";
                    varpass(x);
                }
                
                func varpass(var x) = 
                {
                    print(x); 
                }";

            Assert.AreEqual(expected, Go());
        }

        [TestMethod]
        [DataRow(3,"3")]
        [DataRow(-1,"-1")]
        [DataRow(0,"0")]
        public void PrintThreeLevelsNesting(dynamic value, string expected)
        {
            sourceAsString +=
                @"func test_func() = 
                {
                    var y = "+value+@";
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

            Assert.AreEqual(expected, Go());
        }

        [TestMethod]
        [DataRow(3)]
        [DataRow(-1)]
        [DataRow(0)]
        public void EmptyComment(dynamic value)
        {
            sourceAsString += @"func test_func() =
                {
                    var y = "+value+@";
                    /*print(y);*/
                    //printLine(y);
                    // /*print(y);*/
                    /*nest(3)*/
                }";

            Assert.AreEqual("", Go());
        }

        [TestMethod]
        [DataRow(3,"3")]
        [DataRow(-1,"-1")]
        [DataRow(0,"0")]
        public void MultipleVariables(dynamic value, string expected)
        {
            sourceAsString += @"
                func test_func() = 
                {
                    var z = 3;
                    var x="+value+@"; 
                    print(x);
                    print(z);
                }";

            Assert.AreEqual(expected+"3" , Go());
        }

        [TestMethod]
        [DataRow(32,33,"65")]
        [DataRow(-5,-5,"-10")]
        public void Add(dynamic a, dynamic b, string expected)
        {
            sourceAsString += @"
                func test_func() = {
                    var x="+a+@";
                    var y="+b+@";
                    var z=x+y;
                    print(z);
                 }";

            Assert.AreEqual(expected , Go());
        }

        [TestMethod]
        [DataRow(32,33,"-1")]
        [DataRow(-5,-5,"0")]
        public void Subtract(dynamic a, dynamic b, string expected)
        {
            sourceAsString += @"func test_func() = {
                var x="+a+@"; var y="+b+@"; var z=x-y;
                print(z);
            }";
            Assert.AreEqual(expected, Go());
        }

        [TestMethod]
        [DataRow(5,5,"1")]
        [DataRow(-5,-5,"1")]
        public void Divide(dynamic a, dynamic b, string expected)
        {
            sourceAsString += @"func test_func() =
            {
                var x="+a+@";
                var y="+b+@";
                var z=x/y;
                print(z);
            }";

            Assert.AreEqual(expected, Go());
        }

        [TestMethod]
        [DataRow(5,5,"25")]
        [DataRow(-5,-5,"25")]
        public void Multiply(dynamic a, dynamic b, string expected)
        {
            sourceAsString += @"func test_func() = 
            {
                var x="+a+@";
                var y="+b+@";
                var z=x*y;
                print(z);
            }";

            Assert.AreEqual(expected , Go());
        }

        [TestMethod]
        public void TestSourceAsFile()
        {
            Compiler compiler = new Compiler();

            var outputValue = compiler.Go(@"../../../../../examples/test2.juk");
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
        [DataRow("getAvailableMemory()","0")]
        public void Primitives(string primitive, string expected)
        {
            sourceAsString += @"
                func test_func() = 
                {
                    testme();
                }

                func testme() = 
                {
                    var v = "+primitive+@";
                    print(v);
                }";

            Assert.AreNotEqual(expected, Go());
        }

        [TestMethod]
        [DataRow(3,"012")]
        [DataRow(-1,"")]
        [DataRow(0,"")]
        public void ForLoop(dynamic loops, string expected)
        {
            sourceAsString += @"
                func test_func() = 
                {
                    for(var i = 0; i<"+loops+@"; i++;)
                    {
                        print(i);
                    }
                }";

            Assert.AreEqual(expected, Go());
        }
    }
}