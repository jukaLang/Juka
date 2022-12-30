using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JukaUnitTest
{
    [TestClass]
    public class CommentsUnitTest : UnitTestStructure
    {
        [TestMethod]
        [DataRow(3)]
        [DataRow(-1)]
        [DataRow(0)]
        public void EmptyComment(dynamic value)
        {
            SourceAsString += @"func test_func() =
                {
                    var y = " + value + @";
                    /*print(y);*/
                    //printLine(y);
                    // /*print(y);*/
                    /*nest(3)*/
                }";

            Assert.AreEqual("", Go());
        }
    }
}
