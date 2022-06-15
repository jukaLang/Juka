using Juka;

string? sourceAsString;
string? readline;

if (args == null || args.Length == 0)
{
    Console.Title = "Juka Compiler";
    Console.WriteLine("♥ Welcome to Juka Compiler (Press F10 to run the code). If you need to run a file, pass it as an argument ♥");
    while (true)
    {
        Console.Write("→ ");
        sourceAsString = "";

        while (true)
        {
            readline = XConsole.CancelableReadLine(out bool isCancelled);

            sourceAsString += readline;
            if (isCancelled)
            {
                break;
            }
        }
        Console.WriteLine("=====OUTPUT:=======");
        Console.WriteLine(new JukaCompiler.Compiler().Go(sourceAsString, false));
    }
}
else
{
    sourceAsString = args[0];
    Console.WriteLine(new JukaCompiler.Compiler().Go(sourceAsString, true));
}