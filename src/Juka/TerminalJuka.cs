using JukaCompiler;
using Spectre.Console;

namespace Juka;

class TerminalJuka
{
    public static async Task Perform(string[] args)
    {
        string userInput = args[0];

        switch (userInput)
        {
            case "-d":
            case "--debug":
                Console.WriteLine(new Compiler().Go(userInput, isFile: true,debug:1));
                break;
            case "-v":
            case "--version":
            {
                AnsiConsole.MarkupLine($"[bold yellow]Current Version:[/] {CurrentVersion.Get()}");
                break;
            }
            case "-su":
            case "--self-update":
            {
                await SelfUpdate.Update();
                break;
            }
            default:
                Console.WriteLine(new Compiler().Go(userInput, isFile: true,debug:0));
                break;
        }
    }
}