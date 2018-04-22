using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DreamCompiler;
using System.IO;

namespace DreamCompilerUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCommpile()
        {
            try
            {
                var file = File.ReadAllBytes(@"..\..\source\test.jrl");

                MemoryStream memoryStream = null;

                using (var stream = new FileStream(@"..\..\source\test.jrl", FileMode.Open))
                {
                    var byteArray = new byte[stream.Length];
                    stream.Read(byteArray, 0, (int)stream.Length);
                    memoryStream = new MemoryStream(byteArray);
                    var compiler = new Compiler();
                    compiler.Go(memoryStream);

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
