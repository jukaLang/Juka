using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace JukaUnitTest;

[TestClass]
public class CompilerUnitTest : UnitTestStructure
{
    [TestMethod]
    public void StackBasedArray()
    {
        SourceAsString +=
            @"func test_func() = 
                {
                    var x = array[3];
                    x[1] = ""test"";
                    x[0] = ""foo"";
                    x[2] = 3;
                    print(x[1]);
                }";

        Assert.AreEqual("test", Go());
    }


    [TestMethod]
    public void HeapBasedArray()
    {
        SourceAsString +=
            @"func test_func() = 
                {
                    var y = ""te"";
                    print(y);
                    //delete y;
                    print(""st"");
                }";

        Assert.AreEqual("test", Go());
    }



    [TestMethod]
    [DataRow(3,"3")]
    [DataRow(-1,"-1")]
    [DataRow(0,"0")]
    public void PrintLiteral(dynamic value, string expected)
    {
        SourceAsString +=
            @"func test_func() = 
                {
                    printLine("+value+@"); 
                }";
        Assert.AreEqual(expected + Environment.NewLine,  Go());
    }

    [TestMethod]
    [DataRow(3,"3")]
    [DataRow(-1,"-1")]
    [DataRow(0,"0")]
    public void PrintVariable(dynamic value, string expected)
    {
        SourceAsString +=
            @"func test_func() = 
                {
                    var x = "+value+@";
                    print(x); 
                }";

        Assert.AreEqual(expected, Go());
    }

    [TestMethod]
    [DataRow(3,"3")]
    [DataRow(-1,"-1")]
    [DataRow(0,"0")]
    public void PassVariable(dynamic value, string expected)
    {
        SourceAsString +=
            @"func test_func() = 
                {
                    var x = "+value+@";
                    varpass(x);
                }
                
                func varpass(var x) = 
                {
                    print(x); 
                }";

        Assert.AreEqual(expected, Go());
    }

    [TestMethod]
    [DataRow(3,"3")]
    [DataRow(-1,"-1")]
    [DataRow(0,"0")]
    public void PrintThreeLevelsNesting(dynamic value, string expected)
    {
        SourceAsString +=
            @"func test_func() = 
                {
                    var y = "+value+@";
                    nest1(y);
                }
                
                func nest1(var y) = 
                {
                    nest2(y);
                }

                func nest2(var z) = 
                {
                    print(z);
                }";

        Assert.AreEqual(expected, Go());
    }

        

    [TestMethod]
    [DataRow(3,"3")]
    [DataRow(-1,"-1")]
    [DataRow(0,"0")]
    public void MultipleVariables(dynamic value, string expected)
    {
        SourceAsString += @"
                func test_func() = 
                {
                    var z = 3;
                    var x="+value+@"; 
                    print(x);
                    print(z);
                }";

        Assert.AreEqual(expected+"3" , Go());
    }
}