using JukaCompiler;
using Spectre.Console;
using System.Diagnostics;
using System.Text;


namespace Juka
{
    /// <summary>
    /// Represents the REPL.
    /// This code snippet defines a class Repl that implements a Read-Eval-Print Loop (REPL) for the Juka Programming Language. 
    /// It includes methods to handle user input, execute commands, display prompts, manage code execution, and perform various REPL operations 
    /// like clearing the console, listing code, updating the application, and more. 
    /// The Repl class initializes the REPL environment, processes user commands, and executes code snippets entered by the user.
    /// </summary>
    public class Repl
    {
        private static Compiler? compiler;
        private static Stack<string> subRoutineStack = new();
        private static Stack<string> redoStack = new();
        private static string? startData;
        private static string? endData;
        private static bool isSubOrClass;

        public static async Task RunRepl()
        {
            await InitializeRepl();

            while (true)
            {
                string? readLine = Console.ReadLine();
                if (string.IsNullOrEmpty(readLine))
                {
                    DisplayPrompt();
                    continue;
                }

                if (IsCommand(readLine))
                {
                    await HandleCommandAsync(readLine);
                }
                else
                {
                    HandleUserInput(readLine);
                }
            }
        }

        private static async Task InitializeRepl()
        {
            Console.Title = "Juka Programming Language";
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.OutputEncoding = Encoding.UTF8;

            await SelfUpdate.Check();

            isSubOrClass = false;
            startData =  @"sub main() = {
                        ";
            endData =  @"
                        }";

            subRoutineStack = new Stack<string>();

            AnsiConsole.Write(new FigletText("Juka").Color(Color.Purple));
            AnsiConsole.MarkupLine("[bold yellow]Hello[/] and [bold red]Welcome to 🍲 Juka Programming Language![/] For info visit [link blue]https://jukalang.com[/]. Type [bold palegreen1]!menu[/] to see options");

            compiler = new Compiler();

            DisplayPrompt();
        }

        private static void DisplayPrompt()
        {
            string prompt = "[bold green]Juka[/]([red]" + CurrentVersion.GetVersion() + "[/]){" + DateTime.Now.ToString("HH:mm:ss") + "}> ";
            AnsiConsole.Markup(prompt);
        }

        private static bool IsCommand(string line)
        {
            return line.StartsWith('!');
        }

        private static async Task HandleCommandAsync(string command)
        {
            switch (command.Split(' ')[0].ToLower())
            {
                case "!menu":
                    DisplayMenu();
                    break;
                case "!clear":
                    ClearConsole();
                    break;
                case "!list":
                    ListCode();
                    break;
                case "!get":
                    GetLibraries();
                    break;
                case "!undo":
                    UndoLastCommand();
                    break;
                case "!redo":
                    RedoLastCommand();
                    break;
                case "!update":
                    await UpdateJuka();
                    break;
                case "!restart":
                    string[] restartTypes = command.Split(' ');
                    if (restartTypes.Length > 1)
                    {
                        string restartType = restartTypes[1];
                        if (restartType == "full")
                        {
                            RestartApplication("full");
                        }
                        else
                        {
                            RestartApplication("normal");
                        }
                    }
                    else
                    {
                        RestartApplication("normal");
                    }
                    break;
                case "!download":
                    string[] parts = command.Split(' ');
                    if (parts.Length > 1)
                    {
                        string url = parts[1];
                        await DownloadAFile(url);
                    }
                    else
                    {
                        Console.WriteLine("Invalid command format. Usage: !download [url]");
                    }
                    break;
                case "!exit":
                    ExitRepl();
                    break;
                default:
                    Console.WriteLine($"Unknown command: {command}");
                    break;
            }
        }

        private static void HandleUserInput(string userInput)
        {
            if (userInput.StartsWith("sub") || userInput.StartsWith("class"))
            {
                isSubOrClass = true;
                subRoutineStack.Push(userInput);
                Trace.WriteLine("Starting Subroutine: " + userInput);
            }
            else if (isSubOrClass)
            {
                if (userInput.StartsWith("}", StringComparison.OrdinalIgnoreCase))
                {
                    subRoutineStack.Push(userInput);
                    Trace.WriteLine("Ending Subroutine: " + userInput);
                    ExecuteSub();
                    isSubOrClass = false;
                }
                else
                {
                    Trace.WriteLine("Reading Subroutine: " + userInput);
                    subRoutineStack.Push(userInput);
                }
            }
            else
            {
                startData += userInput;
                subRoutineStack.Push(userInput);
                ExecuteLine();
            }
        }

        private static void DisplayMenu()
        {
            Table table = new();

            table.AddColumn("Command");
            table.AddColumn(new TableColumn("Description"));

            table.AddRow("!menu", "[yellow]Displays this menu[/]");
            table.AddRow("!clear", "[green]Clears the REPL[/]");
            table.AddRow("!list", "[red]Lists the current code[/]");
            table.AddRow("!get", "[aqua]Get list of libraries for Juka[/]");
            table.AddRow("!undo", "[blue]Undoes last entered command[/]");
            table.AddRow("!redo", "[red]Redoes the undone command[/]");
            table.AddRow("!download", "[aqua]Download a file from the web. Requires a url. [/]");
            table.AddRow("!update", "[yellow]Update Juka to latest version[/]");
            table.AddRow("!restart", "[fuchsia]Restart application. [/] Options: [/aqua]full[/] or [aqua]normal[/]");
            table.AddRow("!exit", "[yellow]Exits REPL[/]");

            AnsiConsole.Write(table);
            DisplayPrompt();
        }

        private static void ClearConsole()
        {
            Console.Clear();
            compiler = new Compiler();
            isSubOrClass = false;
            subRoutineStack.Clear();
            startData = @"sub main() = {
                        ";
            endData = @"
                        }";
            DisplayPrompt();
        }


        private static void ListCode()
        {
            foreach (string? data in subRoutineStack.Reverse())
            {
                Console.WriteLine(data);
            }

            DisplayPrompt();
        }

        private static void GetLibraries()
        {
            // Implement your logic to get the list of libraries for Juka
            DisplayPrompt();
        }

        private static void UndoLastCommand()
        {
            string templine = subRoutineStack.Pop();
            redoStack.Push(templine);
            AnsiConsole.MarkupLine("[bold red]Removed: [/]" + templine);
            DisplayPrompt();
        }

        private static void RedoLastCommand()
        {
            string templine = redoStack.Pop();
            subRoutineStack.Push(templine);
            AnsiConsole.MarkupLine("[bold green]Added: [/]" + templine);
            DisplayPrompt();
        }

        private static async Task UpdateJuka()
        {
            await SelfUpdate.Update();
            DisplayPrompt();
        }

        private static async Task DownloadAFile(string url)
        {
            await SelfUpdate.DownloadURLAsync(url);
            AnsiConsole.MarkupLine("[Green]Finished Downloading from: [/]" + url);
            DisplayPrompt();
        }

        

        private static async void RestartApplication(string type)
        {
            IDictionary<string, string> info = SelfUpdate.GetSystemInfo();
            string jukaexepath = info["dir"] + info["name"] + info["extension"];
            await SelfUpdate.Restart(jukaexepath,type);
        }

        private static void ExitRepl()
        {
            Environment.Exit(0);
        }

        private static void ExecuteSub()
        {
            StringBuilder userDataToExecute = new();
            foreach (string item in subRoutineStack.Reverse())
            {
                userDataToExecute.Append(item);
            }

            subRoutineStack = new Stack<string>();

            endData += userDataToExecute.ToString();
            DisplayPrompt();
        }

        private static void ExecuteLine()
        {
            string codeToExecute = startData + endData;

            Trace.WriteLine(codeToExecute);
            string output = "Something went wrong! Please restart the application";

            AnsiConsole.Status().Spinner(Spinner.Known.Star).Start("Computing...", ctx =>
            {
                try
                {
                    //Console.WriteLine(codeToExecute);
                    compiler = new Compiler(); // Clear OLD params -- In future, we don't need to do this
                    DebugMe.DebugMode = 1;
                    output = compiler.CompileJukaCode(codeToExecute, isFile: false);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
            });

            Console.WriteLine(output);
            DisplayPrompt();
        }
    }
}
