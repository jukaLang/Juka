using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JukaUnitTest;

[TestClass]
public class SystemCallsUnitTest : UnitTestStructure
{
    [TestMethod]
    [DataRow("getAvailableMemory()")]
    public void Primitives(string primitive)
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

        var result = Go();
        Assert.IsNotNull(result, $"The result of {primitive} should not be null or empty.");
    }
}