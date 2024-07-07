using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JukaUnitTest;

[TestClass]
public class StructureUnitTest : UnitTestStructure
{
    [TestMethod]
    public void Class()
    {
        SourceAsString += @"
                class x = 
                {
                    sub xmethod() = 
                    {
                        print(""foo"");
                    }

                    sub zmethod() = 
                    {
                        print(""bar"");
                    }
                }

                sub test_func() = 
                {
                    var v = x();
                    v.xmethod();
                    v.zmethod();
                }";

        Assert.AreEqual("foobar", Go());
    }
}