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
            //Try to RUN GUI Version using SDL2
            try
            {
                if (CurrentVersion.GetVersion() == "DEBUG")
                {
                   await SDL2_Gui.Program.GUI(arguments);
                   await Repl.RunRepl(); 
                }
                await SDL2_Gui.Program.GUI(arguments);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                //Try to run an advanced REPL
                try
                {
                    await Repl.RunRepl();
                }
                catch (Exception err) //Embedded Device or other error. Run a very simpel REPL
                {
                    Console.WriteLine(err.ToString());
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