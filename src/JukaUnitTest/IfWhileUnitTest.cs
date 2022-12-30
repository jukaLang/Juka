using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JukaUnitTest
{

    [TestClass]
    public class IfWhileUnitTest : UnitTestStructure
    {
        [TestMethod]
        public void IfBoolean()
        {
            SourceAsString +=
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
            SourceAsString +=
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
        public void WhileBoolean()
        {
            SourceAsString +=
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
        [DataRow(3, "012")]
        [DataRow(-1, "")]
        [DataRow(0, "")]
        public void ForLoop(dynamic loops, string expected)
        {
            SourceAsString += @"
                func test_func() = 
                {
                    for(var i = 0; i<" + loops + @"; i++;)
                    {
                        print(i);
                    }
                }";

            Assert.AreEqual(expected, Go());
        }
    }
}
