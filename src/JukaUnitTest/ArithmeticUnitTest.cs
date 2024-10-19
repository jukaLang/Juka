using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JukaUnitTest;

[TestClass]
public class ArithmeticUnitTest : UnitTestStructure
{
    [TestMethod]
    [DataRow(32, 33, "65")]
    [DataRow(-5, -5, "-10")]
    public void Add(dynamic a, dynamic b, string expected)
    {
        SourceAsString = @" 
              sub test_func() = {
                    printLine(5);
                 }
              sub main() = {
                test_func();
               }";

        Assert.AreEqual(expected, Go());
    }
    /*var x=" + a + @";
                    var y=" + b + @";
                    var z=x+y;
                    print(z);*/


    [TestMethod]
    [DataRow(32, 33, "-1")]
    [DataRow(-5, -5, "0")]
    public void Subtract(dynamic a, dynamic b, string expected)
    {
        SourceAsString += @"sub test_func() = {
                var x=" + a + @"; var y=" + b + @"; var z=x-y;
                print(z);
            }";
        Assert.AreEqual(expected, Go());
    }

    [TestMethod]
    [DataRow(5, 5, "1")] // Expected result should be "1.0" for division
    [DataRow(-5, -5, "1")] // Expected result should be "1.0" for division
    public void Divide(dynamic a, dynamic b, string expected)
    {
        SourceAsString += @"sub test_func() =
            {
                var x=" + a + @";
                var y=" + b + @";
                var z=x/y;
                print(z);
            }";

        Assert.AreEqual(expected, Go());
    }

    [TestMethod]
    [DataRow(5, 5, "25")]
    [DataRow(-5, -5, "25")]
    public void Multiply(dynamic a, dynamic b, string expected)
    {
        SourceAsString += @"sub test_func() = 
            {
                var x=" + a + @";
                var y=" + b + @";
                var z=x*y;
                print(z);
            }";

        Assert.AreEqual(expected, Go());
    }
}