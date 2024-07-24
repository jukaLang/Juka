using System.Diagnostics;

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
            //Try to run an advanced REPL
            try
            {
                if (CurrentVersion.GetVersion() == "DEBUG")
                {
                    await SDL2_Gui.Program.GUI(arguments);
                }
                await Repl.RunRepl();
            }
            catch (Exception err) //Embedded Device? Try SDL Interface  
            {
                try
                {
                    Console.WriteLine(err.ToString());
                }
                catch (Exception) { // Run a very simple REPL (for embedded devices or devices that can't render SDL)
                    await Repl.RunSimpleRepl();
                }
            }

        }
        else
        {
            await TerminalJuka.Perform(arguments);
        }
    }
}