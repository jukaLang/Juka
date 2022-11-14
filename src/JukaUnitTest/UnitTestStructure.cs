using JukaCompiler;
using System;

namespace JukaUnitTest
{
    public abstract class UnitTestStructure
    {
        /// <summary>
        /// Default main function calls hard code test func"
        /// </summary>
        public string sourceAsString =
            @"func main() = 
                {
                    test_func();
                }";


        public string Go()
        {
            Compiler compiler = new Compiler();
            var outputValue = compiler.Go(sourceAsString, false);
            if (compiler.HasErrors())
            {
                throw new Exception("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }

            return outputValue;
        }

    }
}
