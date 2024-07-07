using JukaCompiler;
using Spectre.Console;
using System.Diagnostics;
using System.Text;

namespace Juka;

public class Repl
{

    public static async Task RunRepl()
    {
        Console.Title = "Juka Programming Language";
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        Console.OutputEncoding = Encoding.UTF8;

        await SelfUpdate.Check();

        bool isFuncOrClass = false;
        string prompt = "[bold green]Juka[/]([red]" + CurrentVersion.Get() + "[/]){" + DateTime.Now.ToString("HH:mm:ss")+"}> ";

        AnsiConsole.Write(
            new FigletText("Juka").Color(Color.Purple));

        AnsiConsole.MarkupLine(
            "[bold yellow]Hello[/] and [bold red]Welcome to 🍲 Juka Programming Language![/] For info visit [link blue]https://jukalang.com[/]. Type [bold palegreen1]!menu[/] to see options");

        Compiler compiler = new();

        string dataStart = "func main() = {";
        string dataEnd = "}";

        Stack<string> funcData = new();
        AnsiConsole.Markup(prompt);

        Stack<string> operations = new();

        bool inloop = true;

        while (inloop)
        {
            string? readLine = Console.ReadLine();
            if (string.IsNullOrEmpty(readLine))
            {
                AnsiConsole.Markup(prompt);
                continue;
            }


            if (readLine.Equals("!menu", StringComparison.OrdinalIgnoreCase))
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
                AnsiConsole.Markup(prompt);
                continue;
            }

            if (readLine.Equals("!clear", StringComparison.OrdinalIgnoreCase))
            {
                Console.Clear();
                compiler = new Compiler();
                isFuncOrClass = false;
                funcData.Clear();
                dataStart = "func main() = {";
                dataEnd = "}";
                AnsiConsole.Markup(prompt);
                continue;
            }

            if (readLine.Equals("!list", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var data in funcData.Reverse())
                {
                    Console.WriteLine(data);
                }

                AnsiConsole.Markup(prompt);
                continue;
            }

            if (readLine.Equals("!get", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var data in funcData.Reverse())
                {
                    Console.WriteLine(data);
                }

                AnsiConsole.Markup(prompt);
                continue;
            }

            if (readLine.Equals("!undo", StringComparison.OrdinalIgnoreCase))
            {
                var templine = funcData.Pop();
                AnsiConsole.MarkupLine("[bold red]Removed: [/]" + templine);
                AnsiConsole.Markup(prompt);
                continue;
            }

            if(readLine.Equals("!update", StringComparison.OrdinalIgnoreCase))
            {
                await SelfUpdate.Update();
                AnsiConsole.Markup(prompt);
                continue;
            }

            if (readLine.Equals("!restart", StringComparison.OrdinalIgnoreCase))
            {
                IDictionary<string, string> info = SelfUpdate.Info();
                string jukaexepath = info["dir"] + info["name"] + info["extension"];
                SelfUpdate.Restart(jukaexepath);
                continue;
            }

            // Exit Juka
            if (readLine.Equals("!exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            if (readLine.StartsWith("func") || readLine.StartsWith("class"))
            {
                isFuncOrClass = true;
                funcData.Push(readLine);
                Trace.WriteLine("Starting Func: " + readLine);
                continue;
            }
            else if (isFuncOrClass)
            {
                if (readLine.StartsWith("}", StringComparison.OrdinalIgnoreCase))
                {
                    funcData.Push(readLine);
                    Trace.WriteLine("Ending Func: " + readLine);

                    StringBuilder userDataToExecute = new StringBuilder();
                    foreach (string item in funcData.Reverse())
                    {
                        userDataToExecute.Append(item);
                    }

                    funcData = new();

                    dataEnd += userDataToExecute.ToString();
                    isFuncOrClass = false;
                    AnsiConsole.Markup(prompt);
                }
                else
                {
                    Trace.WriteLine("Reading Func: " + readLine);
                    funcData.Push(readLine);
                }
            }
            else
            {
                if (readLine.StartsWith("var"))
                {
                    dataStart += readLine;
                    readLine = "";
                }

                funcData.Push(readLine);

                string codeToExecute = dataStart + readLine + dataEnd;

                Trace.WriteLine(codeToExecute);
                string output = "Something went wrong! Please restart the application";
                
                await AnsiConsole.Status().Spinner(Spinner.Known.Star).StartAsync("Computing...", async ctx =>
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
                AnsiConsole.Markup(prompt);
            }
        }
    }

}