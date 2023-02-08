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
            case "--encrypt":
                AnsiConsole.MarkupLine($"[bold yellow]Encrypting File:[/]");
                File.Encrypt(args[1]);
                break;
            case "--decrypt":
                AnsiConsole.MarkupLine($"[bold yellow]Decrypting File:[/]");
                File.Decrypt(args[1]);
                break;
            case "-d":
            case "--debug":
                Console.WriteLine(new Compiler().Go(args[1], isFile: true,debug:1));
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
            case "-i":
            case "--inline":
            {
                Console.WriteLine(new Compiler().Go(args[1], isFile: false, debug: 0));
                break;
            }
            default:
                Console.WriteLine(new Compiler().Go(args[0], isFile: true,debug:0));
                break;
        }
    }
}