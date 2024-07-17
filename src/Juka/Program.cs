namespace Juka;


class Program
{
    /// <summary>
    /// Main function of the program.
    /// This code defines the entry point of the program. 
    /// It checks if any command-line arguments are provided. If none are provided, it calls a method Repl.RunRepl(), 
    /// otherwise it calls TerminalJuka.Perform(arguments).
    /// </summary>
    /// <param name="arguments">Array of strings representing command-line arguments</param>
    static async Task Main(string[] arguments)
    {
        if (arguments.Length == 0)
        {
            try
            {
                await Repl.RunRepl();
            }
            catch (Exception e) //Embedded Device or other error
            {
                Console.WriteLine(e.ToString());
                await Repl.RunSimpleRepl();
            }
        }
        else
        {
            await TerminalJuka.Perform(arguments);
        }
    }
}