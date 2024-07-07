using JukaCompiler.Interpreter;

namespace JukaCompiler.SystemCalls
{
    // Interface for a system call that opens a file
    internal interface IFileOpen : IJukaCallable
    {
        // Define methods for opening a file here
    }

    // Interface for a system call that interacts with C#
    internal interface ICSharp : IJukaCallable
    {
        // Define methods for interacting with C# here
    }

    // Interface for a system call that gets the system clock
    internal interface ISystemClock : IJukaCallable
    {
        // Define methods for getting the system clock here
    }

    // Interface for a system call that gets the available memory
    internal interface IGetAvailableMemory : IJukaCallable
    {
        // Define methods for getting the available memory here
    }

    // Interface for general system calls
    internal interface ISystemCalls : IJukaCallable
    {
        // Define methods for general system calls here
    }
}
