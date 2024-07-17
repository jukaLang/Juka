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

                string? readLine = AnsiConsole.Ask<string>("[bold green]Juka[/]([red]" + CurrentVersion.GetVersion() + "[/]){" + DateTime.Now.ToString("HH:mm:ss") + "}> ");


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

        // A method to initialize the REPL environment with specific configurations and setup.
        private static async Task InitializeRepl()
        {
            Console.Title = "Juka Programming Language";
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.OutputEncoding = Encoding.UTF8;


            subRoutineStack = new Stack<string>();
            isSubOrClass = false;
            startData = @"sub main() = {
                        ";
            endData = @"
                        }";


            try
            {
                // Create the layout
                Layout layout = new Layout("Root")
                .SplitColumns(
                    new Layout("Left").SplitRows(
                new Layout("Logo"),
                new Layout("Output")),
                    new Layout("Menu"));



            layout["Left"].Ratio(2);

            Dictionary<string, string> archicture = SelfUpdate.GetSystemInfo();




            layout["Output"].Update(new Padder(new Markup("[bold yellow]Hello[/] and [bold red]Welcome to 🍲 Juka Programming Language![/] For info visit [link blue]https://jukalang.com[/].\n\n" +
    "[bold blue]Your Operating System:[/] [green]" + archicture["platform"] + "[/]\n" +
    "[bold blue]Current Directory:[/] [green]" + archicture["dir"] + "[/]\n" +
    "[bold blue]Your Juka Assembly Architecture:[/] [green]" + archicture["architecture"] + "[/]\n" +
    "[bold blue]Your Juka Assembly Name:[/] [green]" + archicture["name"] + "[/]\n" +
    "[bold blue]Your Juka Assembly Extension:[/] [green]" + archicture["extension"] + "[/]\n"

    )));

            int paddingtop = 3;
            if (CurrentVersion.GetVersion() == "DEBUG")
            {
                paddingtop = 2;
            }

            
            FigletText logotext = new FigletText("Juka").Color(Color.Purple);
            Panel logopanel = new Panel(logotext).Expand();
            logopanel.Padding = new Padding(3, 3, 3, 3);
            logopanel.Expand = true;
            logopanel.Border = BoxBorder.None;
            logopanel.Header = new PanelHeader("Juka Version: " + CurrentVersion.GetVersion() + " ");
            layout["Logo"].Update(new Padder(logopanel).PadTop(paddingtop));
         




            layout["Menu"].Update(new Padder(DisplayMenuTable()).PadTop(paddingtop));




            AnsiConsole.Write(layout);
             } catch(Exception ex)
            {
                Console.WriteLine("Panel Error>>>");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("<<<");
            }

            await SelfUpdate.Check();

            compiler = new Compiler();

        }


        // A method that checks whether the input line is a command by checking if it starts with '!'.
        private static bool IsCommand(string line)
        {
            return line.StartsWith('!');
        }

        // Asynchronously handles different commands based on the input command string.
        private static async Task HandleCommandAsync(string command)
        {
            switch (command.Split(' ')[0].ToLower())
            {
                case "!!menu":
                    AnsiConsole.Write(DisplayMenuTable());
                    break;
                case "!!info":
                    Dictionary<string, string> archtecture = SelfUpdate.GetSystemInfo();
                    // Display system information
                    AnsiConsole.MarkupLine($"[bold blue]Your Operating System:[/] [green]"+archtecture["platform"]+"[/]");
                    AnsiConsole.MarkupLine($"[bold blue]Current Directory:[/] [green]"+archtecture["dir"]+"[/]");
                    AnsiConsole.MarkupLine($"[bold blue]Your Juka Assembly Architecture:[/] [green]"+archtecture["architecture"]+"[/]");
                    AnsiConsole.MarkupLine($"[bold blue]Your Juka Assembly Name:[/] [green]" + archtecture["name"] +"[/]");
                    AnsiConsole.MarkupLine($"[bold blue]Your Juka Assembly Extension:[/] [green]"+archtecture["extension"]+"[/]");
                    break;
                case "!!clear":
                    ClearConsole();
                    break;
                case "!!list":
                    ListCode();
                    break;
                case "!!get":
                    GetLibraries();
                    break;
                case "!!undo":
                    UndoLastCommand();
                    break;
                case "!!redo":
                    RedoLastCommand();
                    break;
                case "!!update":
                    string[] updateTypes = command.Split(' ');
                    if (updateTypes.Length > 1)
                    {
                        string updateType = updateTypes[1];
                        if (updateType == "-force")
                        {
                            await UpdateJuka("force");
                        }
                        else
                        {
                            await UpdateJuka();
                        }
                    }
                    else
                    {
                        await UpdateJuka();
                    }
                    break;
                case "!!restart":
                    string[] restartTypes = command.Split(' ');
                    if (restartTypes.Length > 1)
                    {
                        string restartType = restartTypes[1];
                        if (restartType == "-full")
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
                case "!!download":
                    string[] parts = command.Split(' ');
                    if (parts.Length > 1)
                    {
                        string url = parts[1];
                        await DownloadAFile(url);
                    }
                    else
                    {
                        Console.WriteLine("Invalid command format. Usage: !!download [url]");
                    }
                    break;
                case "!!exit":
                    ExitRepl();
                    break;
                default:
                    Console.WriteLine($"Unknown command: {command}");
                    break;
            }
        }

        // A method to handle user input in the REPL environment.
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

        // Displays the menu with various commands and their descriptions in a table format using AnsiConsole. After displaying the menu, it calls the DisplayPrompt method to show the REPL prompt.
        public static Table DisplayMenuTable()
        {
            Table table = new();

            table.AddColumn("Command");
            table.AddColumn(new TableColumn("Description"));

            table.AddRow("!!menu", "[yellow]Displays this menu[/]");
            table.AddRow("!!info", "[deeppink3]Lists system information[/]");
            table.AddRow("!!clear", "[green]Clears the REPL[/]");
            table.AddRow("!!list", "[red]Lists the current code[/]");
            table.AddRow("!!get", "[aqua]Get list of libraries for Juka[/]");
            table.AddRow("!!undo", "[blue]Undoes last entered command[/]");
            table.AddRow("!!redo", "[darkorange3_1]Redoes the undone command[/]");
            table.AddRow("!!download", "[aqua]Download a file from the web. Requires a url. [/] ");
            table.AddRow("!!update", "[navajowhite1]Update Juka to latest version[/] Flags: -force");
            table.AddRow("!!restart", "[fuchsia]Restart application. [/] Flags: -full or -normal");
            table.AddRow("!!exit", "[mistyrose3]Exits REPL[/]");
            return table;
        }



        // A method to clear the console, initialize a new compiler, reset the class or subroutine flag, clear the subroutine stack, set the starting data for the main subroutine, set the ending data, and display the prompt.
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
        }

       // Displays the code present in the subRoutineStack in reverse order and then calls the DisplayPrompt method.
        private static void ListCode()
        {
            foreach (string? data in subRoutineStack.Reverse())
            {
                Console.WriteLine(data);
            }

        }

       // A method to get the list of libraries for Juka.
        private static void GetLibraries()
        {
            // Implement your logic to get the list of libraries for Juka
        }

        // Undoes the last command by popping a line from the subRoutineStack, pushing it to the redoStack, marking it as removed in the console, and displaying the prompt.
        private static void UndoLastCommand()
        {
            string templine = subRoutineStack.Pop();
            redoStack.Push(templine);
            AnsiConsole.MarkupLine("[bold red]Removed: [/]" + templine);
        }


        // A method to redo the last command, pushing the popped command from the redo stack to the subRoutineStack, marking it as added, and displaying the prompt.
        private static void RedoLastCommand()
        {
            string templine = redoStack.Pop();
            subRoutineStack.Push(templine);
            AnsiConsole.MarkupLine("[bold green]Added: [/]" + templine);
        }

        // A method to update Juka asynchronously.
        private static async Task UpdateJuka(string update = "normal")
        {
            await SelfUpdate.Update(update);
        }

        // A method to download a file asynchronously from the specified URL and display a message upon completion.
        private static async Task DownloadAFile(string url)
        {
            await SelfUpdate.DownloadURLAsync(url);
            AnsiConsole.MarkupLine("[Green]Finished Downloading from: [/]" + url);
        }

        
        // Asynchronously restarts the application based on the provided type parameter.
        private static async void RestartApplication(string type)
        {
            IDictionary<string, string> info = SelfUpdate.GetSystemInfo();
            string jukaexepath = info["dir"] + info["name"] + info["extension"];
            await SelfUpdate.Restart(jukaexepath,type);
        }

        // Exits the REPL environment by terminating the application.
        private static void ExitRepl()
        {
            Environment.Exit(0);
        }

        // Executes the subroutines present in the subRoutineStack by iterating in reverse order, appending each item to userDataToExecute,
        // resetting the subRoutineStack, updating endData with the concatenated result, and finally displaying the prompt.
        private static void ExecuteSub()
        {
            StringBuilder userDataToExecute = new();
            foreach (string item in subRoutineStack.Reverse())
            {
                userDataToExecute.Append(item);
            }

            subRoutineStack = new Stack<string>();

            endData += userDataToExecute.ToString();
        }

        // Executes a line of code by compiling and running it, then displays the output.
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
                    AnsiConsole.WriteException(e);
                    Trace.WriteLine(e);
                }
            });

            Console.WriteLine(output);
        }

      

        internal static async Task RunSimpleRepl()
        {
            Console.WriteLine("Welcome to Juka version " + CurrentVersion.GetVersion() + "! This REPL works with Juka");
            DisplayPrompt();
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

        private static void DisplayPrompt()
        {
            string prompt = "Juka(" + CurrentVersion.GetVersion() + "){" + DateTime.Now.ToString("HH:mm:ss") + "}> ";
            Console.WriteLine(prompt);
        }
    }
}
