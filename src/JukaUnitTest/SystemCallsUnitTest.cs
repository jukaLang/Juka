using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JukaUnitTest
{
    [TestClass]
    public class SystemCallsUnitTest : UnitTestStructure
    {
        [TestMethod]
        [DataRow("getAvailableMemory()", "0")]
        public void Primitives(string primitive, string expected)
        {
            sourceAsString += @"
                func test_func() = 
                {
                    testme();
                }

                func testme() = 
                {
                    var v = " + primitive + @";
                    print(v);
                }";

            Assert.AreNotEqual(expected, Go());
        }
    }
}
