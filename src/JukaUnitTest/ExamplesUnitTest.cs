using JukaCompiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace JukaUnitTest;

[TestClass]
public class ExampleUnitTest : UnitTestStructure
{
    [TestMethod]
    public void TestSourceAsFile()
    {
        Compiler compiler = new Compiler();

        var outputValue = compiler.Go(@"../../../../../examples/test2.juk");
        if (compiler.HasErrors())
        {
            var errors = compiler.ListErrors();
            foreach (var error in errors)
            {
                Assert.IsTrue(false, error);
            }
        }

        Assert.AreEqual("AsdfA" + Environment.NewLine, outputValue);
    }
}