using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JukaUnitTest
{
    [TestClass]
    public class SystemCallsUnitTest : UnitTestStructure
    {
        [TestMethod]
        [DataRow("getAvailableMemory()", "")]
        public void Primitives(string primitive, string expected)
        {
            SourceAsString += @"
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
