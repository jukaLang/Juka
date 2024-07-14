using JukaCompiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace JukaUnitTest;

[TestClass]
public class ExampleUnitTest : UnitTestStructure
{
    private Compiler compiler;

    [TestInitialize]
    public void TestSetup()
    {
        // Initialize the Compiler object before each test
        compiler = new Compiler();
    }

    [TestMethod]
    public void TestSourceAsFile()
    {
        // Use the Go method to compile and run the code
        var outputValue = compiler.CompileJukaCode(@"../../../../../examples/test2.juk");

        // If there are any compilation errors, fail the test and report the errors
        if (compiler.CheckForErrors())
        {
            var errors = compiler.GetErrorList();
            foreach (var error in errors)
            {
                Assert.Fail($"Compilation error: {error}");
            }
        }

        // Check the output value
        Assert.AreEqual("AsdfA" + Environment.NewLine, outputValue);
    }
}