using JukaCompiler;
using System;

namespace JukaUnitTest;

public abstract class UnitTestStructure
{
    /// <summary>
    /// The source code string to be tested. The default main function calls a test function.
    /// </summary>
    public string SourceAsString { get; set; } =
        @"sub main() = 
                {
                    test_func();
                }";

    /// <summary>
    /// Compiles and runs the source code string, and returns the output.
    /// If there are any compilation errors, throws an exception with the error messages.
    /// </summary>
    public string Go()
    {
        // Create a new Compiler instance
        Compiler compiler = new();

        // Compile and run the source code string
        var outputValue = compiler.CompileJukaCode(SourceAsString, false);

        // If there are any compilation errors, throw an exception with the error messages
        if (compiler.CheckForErrors())
        {
            var errorMessage = "Compilation errors:\r\n" + string.Join("\r\n", compiler.GetErrorList());
            throw new InvalidOperationException(errorMessage);
        }

        // Return the output value
        return outputValue;
    }
}