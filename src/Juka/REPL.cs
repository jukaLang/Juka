using JukaCompiler;
using Spectre.Console;
using System.Diagnostics;
using System.Text;

namespace Juka
{
    public class REPL
    {
        public static async Task RunREPL()
        {
            Console.Title = "Juka Programming Language";
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.OutputEncoding = Encoding.UTF8;

            await SelfUpdate.Update();



            bool isFuncOrClass = false;
            string prompt = "[bold green]Juka[/]([red]" + CurrentVersion.Get() + "[/])> ";

            AnsiConsole.Write(
                new FigletText("Juka")
                    .LeftAligned().Color(Color.Purple));

            AnsiConsole.MarkupLine(
                "[bold yellow]Hello[/] and [bold red]Welcome to 🍲 Juka Programming Language![/] For info visit [link blue]https://jukalang.com[/]. Type [bold palegreen1]menu[/] to see options");

            Compiler compiler = new();

            string dataStart = "func main() = {";
            string dataEnd = "}";

            List<string> funcData = new();
            AnsiConsole.Markup(prompt);

            while (true)
            {
                string? readLine = Console.ReadLine();
                if (string.IsNullOrEmpty(readLine))
                {
                    AnsiConsole.Markup(prompt);
                    continue;
                }


                if (readLine.Equals("menu", StringComparison.OrdinalIgnoreCase))
                {
                    var table = new Table();

                    // Add some columns
                    table.AddColumn("Command");
                    table.AddColumn(new TableColumn("Description"));

                    // Add some rows
                    table.AddRow("list", "[red]Lists the current code[/]");
                    table.AddRow("clear", "[green]Clears The REPL[/]");
                    AnsiConsole.Write(table);
                    AnsiConsole.Markup(prompt);
                    continue;
                }

                if (readLine.Equals("clear", StringComparison.OrdinalIgnoreCase))
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

                if (readLine.Equals("list", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var data in funcData)
                    {
                        Console.WriteLine(data);
                    }

                    AnsiConsole.Markup(prompt);
                    continue;
                }

                if (readLine.StartsWith("func") || readLine.StartsWith("class"))
                {
                    isFuncOrClass = true;
                    funcData.Add(readLine);
                    Trace.WriteLine("Starting Func: " + readLine);
                }
                else if (isFuncOrClass)
                {
                    if (readLine.StartsWith("}"))
                    {
                        funcData.Add(readLine);
                        Trace.WriteLine("Ending Func: " + readLine);
                        string userDataToExecute = string.Empty;
                        foreach (string item in funcData)
                        {
                            userDataToExecute += item;
                        }

                        dataEnd += userDataToExecute;
                        isFuncOrClass = false;
                        AnsiConsole.Markup(prompt);
                    }
                    else
                    {
                        Trace.WriteLine("Reading Func: " + readLine);
                        funcData.Add(readLine);
                    }
                }
                else
                {
                    if (readLine.StartsWith("var"))
                    {
                        dataStart += readLine;
                        readLine = "";
                    }

                    funcData.Add(readLine);

                    string codeToExecute = dataStart + readLine + dataEnd;
                    try
                    {
                        Trace.WriteLine(codeToExecute);
                        string output = compiler.Go(codeToExecute, isFile: false,debug:1);
                        Console.WriteLine(output);
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.ToString());
                        AnsiConsole.WriteException(e);
                        Console.WriteLine("Something went wrong! Please restart the application");
                    }

                    AnsiConsole.Markup(prompt);
                }
            }
        }

    }
}
