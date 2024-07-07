using JukaCompiler;
using Spectre.Console;
using System.Diagnostics;
using System.Text;

namespace Juka
{
    public class Repl
    {
        private static Compiler? compiler;
        private static Stack<string> funcData = new();
        private static string? dataStart;
        private static string? dataEnd;
        private static bool isFuncOrClass;

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
                    HandleCode(readLine);
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

            isFuncOrClass = false;
            dataStart = "func main() = {";
            dataEnd = "}";

            funcData = new Stack<string>();

            AnsiConsole.Write(new FigletText("Juka").Color(Color.Purple));
            AnsiConsole.MarkupLine("[bold yellow]Hello[/] and [bold red]Welcome to 🍲 Juka Programming Language![/] For info visit [link blue]https://jukalang.com[/]. Type [bold palegreen1]!menu[/] to see options");

            compiler = new Compiler();

            DisplayPrompt();
        }

        private static void DisplayPrompt()
        {
            string prompt = "[bold green]Juka[/]([red]" + CurrentVersion.Get() + "[/]){" + DateTime.Now.ToString("HH:mm:ss") + "}> "; 
            AnsiConsole.Markup(prompt);
        }

        private static bool IsCommand(string line)
        {
            return line.StartsWith('!');
        }

        private static async Task HandleCommandAsync(string command)
        {
            switch (command.ToLower())
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
                case "!update":
                    await UpdateJuka();
                    break;
                case "!restart":
                    RestartApplication();
                    break;
                case "!exit":
                    ExitRepl();
                    break;
                default:
                    Console.WriteLine($"Unknown command: {command}");
                    break;
            }
        }

        private static void HandleCode(string code)
        {
            if (code.StartsWith("func") || code.StartsWith("class"))
            {
                isFuncOrClass = true;
                funcData.Push(code);
                Trace.WriteLine("Starting Func: " + code);
            }
            else if (isFuncOrClass)
            {
                if (code.StartsWith("}", StringComparison.OrdinalIgnoreCase))
                {
                    funcData.Push(code);
                    Trace.WriteLine("Ending Func: " + code);
                    ExecuteFunction();
                    isFuncOrClass = false;
                }
                else
                {
                    Trace.WriteLine("Reading Func: " + code);
                    funcData.Push(code);
                }
            }
            else
            {
                if (code.StartsWith("var"))
                {
                    dataStart += code;
                    code = "";
                }

                funcData.Push(code);
                ExecuteLine();
            }
        }

        private static void DisplayMenu()
        {
            var table = new Table();

            // Add some columns
            table.AddColumn("Command");
            table.AddColumn(new TableColumn("Description"));

            // Add some rows
            table.AddRow("!list", "[red]Lists the current code[/]");
            table.AddRow("!clear", "[green]Clears The REPL[/]");
            table.AddRow("!undo", "[blue]Undoes last entered command[/]");
            table.AddRow("!update", "[yellow]Update Juka to latest version[/]");
            table.AddRow("!restart", "[fuchsia]Restart Application[/]");
            table.AddRow("!get", "[aqua]Get List of Libraries for Juka[/]");
            table.AddRow("!exit", "[darkred_1]Exits REPL[/]");
            AnsiConsole.Write(table);
            DisplayPrompt();
        }

        private static void ClearConsole()
        {
            Console.Clear();
            compiler = new Compiler();
            isFuncOrClass = false;
            funcData.Clear();
            dataStart = "func main() = {";
            dataEnd = "}";
            DisplayPrompt();
        }

        private static void ListCode()
        {
            foreach (var data in funcData.Reverse())
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
            var templine = funcData.Pop();
            AnsiConsole.MarkupLine("[bold red]Removed: [/]" + templine);
            DisplayPrompt();
        }

        private static async Task UpdateJuka()
        {
            await SelfUpdate.Update();
            DisplayPrompt();
        }

        private static void RestartApplication()
        {
            IDictionary<string, string> info = SelfUpdate.Info();
            string jukaexepath = info["dir"] + info["name"] + info["extension"];
            SelfUpdate.Restart(jukaexepath);
        }

        private static void ExitRepl()
        {
            Environment.Exit(0);
        }

        private static void ExecuteFunction()
        {
            StringBuilder userDataToExecute = new();
            foreach (string item in funcData.Reverse())
            {
                userDataToExecute.Append(item);
            }

            funcData = new Stack<string>();

            dataEnd += userDataToExecute.ToString();
            DisplayPrompt();
        }

        private static void ExecuteLine()
        {
            string codeToExecute = dataStart + funcData.Peek() + dataEnd;

            Trace.WriteLine(codeToExecute);
            string output = "Something went wrong! Please restart the application";

            AnsiConsole.Status().Spinner(Spinner.Known.Star).Start("Computing...", ctx =>
            {
                try
                {
                    //Console.WriteLine(codeToExecute);
                    compiler = new Compiler(); // Clear OLD params -- In future, we don't need to do this
                    DebugMe.DebugMode = 1;
                    output = compiler.Go(codeToExecute, isFile: false);
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
