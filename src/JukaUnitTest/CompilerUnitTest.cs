using JukaCompiler;
using JukaCompiler.Lexer;
using JukaCompiler.Scan;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace JukaUnitTest
{
    [TestClass]
    public class CompilerUnitTest
    {
        /*
        [TestMethod]
        public void TestEmptyFunc()
        {
            var mockScanner = new Mock<IScanner>();
            var tokenArray = StringToToken("func(){}");

            int bufferCount = -1;

            mockScanner.Setup(f => f.ReadToken() )
                .Returns(()=>
            {
                bufferCount++;

                if (bufferCount == tokenArray.Length)
                {
                    return new Token(TokenType.Eof);
                }

                return tokenArray[bufferCount];
            });

            mockScanner.Setup(f => f.PutTokenBack()).Callback(() => 
            {
                bufferCount--;
            });

            try
            {
                int sequenceCount = 5;

                ILexicalAnalysis lexical = new LexicalAnalysis();
                var llm = lexical.Analyze(mockScanner.Object);

                Assert.IsNotNull(llm);
                Assert.AreEqual(sequenceCount, llm.Count, $"Lexem count is off. Ensure the Token Array is accurate.");
            }
            catch (Exception)
            {
                throw;
            }
        }
        */
    }
}