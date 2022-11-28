﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JukaUnitTest
{

    [TestClass]
    public class IfWhileUnitTest : UnitTestStructure
    {
        [TestMethod]
        public void IfBoolean()
        {
            sourceAsString +=
                @"func test_func() = 
                {
                    var x = true;
                    if ( x == true)
                    {
                        print(""x"");
                    }
                    else
                    {
                        print(""y"");
                    }
                }";

            Assert.AreEqual("x", Go());
        }

        [TestMethod]
        public void IfBooleanElseBranch()
        {
            sourceAsString +=
                @"func test_func() = 
                {
                    var x = false;
                    if ( x == true)
                    {
                        print(""x"");
                    }
                    else
                    {
                        print(""y"");
                    }
                }";

            Assert.AreEqual("y", Go());
        }

        [TestMethod]
        public void WhileBoolean()
        {
            sourceAsString +=
                @"func test_func() = 
                {
                    var x = true;
                    while(x == true)
                    {
                        print(""y"");
                        x = false;
                    }
                }";

            Assert.AreEqual("y", Go());
        }



        [TestMethod]
        [DataRow(3, "012")]
        [DataRow(-1, "")]
        [DataRow(0, "")]
        public void ForLoop(dynamic loops, string expected)
        {
            sourceAsString += @"
                func test_func() = 
                {
                    for(var i = 0; i<" + loops + @"; i++;)
                    {
                        print(i);
                    }
                }";

            Assert.AreEqual(expected, Go());
        }
    }
}