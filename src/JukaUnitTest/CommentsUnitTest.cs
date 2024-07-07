using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JukaUnitTest;

[TestClass]
public class CommentsUnitTest : UnitTestStructure
{
    [TestMethod]
    [DataRow(3, "3")]
    [DataRow(-1, "-1")]
    [DataRow(0, "0")]
    public void EmptyComment(dynamic value, string expected)
    {
        SourceAsString += @"sub test_func() =
                {
                    var y = " + value + @";
                    print(y); // Uncommented this line
                    /*print(y);*/
                    //printLine(y);
                    // /*print(y);*/
                    /*nest(3)*/
                }";

        Assert.AreEqual(expected, Go());
    }
}