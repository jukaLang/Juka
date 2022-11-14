using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            sourceAsString += @"func test_func() =
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
