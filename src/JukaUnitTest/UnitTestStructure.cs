using JukaCompiler;
using System;

namespace JukaUnitTest
{
    public abstract class UnitTestStructure
    {
        /// <summary>
        /// Default main function calls hard code test func"
        /// </summary>
        public string SourceAsString { get; set; } =
            @"func main() = 
                {
                    test_func();
                }";


        public string Go()
        {
            Compiler compiler = new();
            var outputValue = compiler.Go(SourceAsString, false);
            if (compiler.HasErrors())
            {
                throw new ArgumentNullException("Parser exceptions:\r\n" + String.Join("\r\n", compiler.ListErrors()));
            }

            return outputValue;
        }

    }
}
