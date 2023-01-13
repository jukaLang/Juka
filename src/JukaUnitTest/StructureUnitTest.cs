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
}