using System;
using System.IO;
using DreamCompiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DreamCompilerUnitTest
{

    using System.Collections.ObjectModel;
    using static System.Console;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCommpile()
        {
            try
            {
                if (File.Exists(@"..\..\..\..\examples\test.jlr"))
                {

                    MemoryStream memoryStream = null;

                    using (var stream = new FileStream(@"..\..\..\..\examples\test.jlr", FileMode.Open))
                    {
                        var byteArray = new byte[stream.Length];
                        stream.Read(byteArray, 0, (int) stream.Length);
                        memoryStream = new MemoryStream(byteArray);
                        var compiler = new Compiler();
                        compiler.Go(memoryStream);

                    }
                }
                /*
                using (MemoryStream memStream = new MemoryStream((int) file.Length))
                {
                    memStream.Write(file, 0, file.Length);
                    var compiler = new Compiler();
                    compiler.Go(memStream);
                }
                */


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }


}
