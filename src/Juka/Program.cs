string? sourceAsString;
string? readline;

if (args == null || args.Length == 0)
{
    Console.Title = "Juka Compiler";
    Console.WriteLine("♥ Welcome to Juka Compiler. If you need to run a file, pass it as an argument ♥");
    Console.WriteLine("Press Return 3 times to run the code");

    while (true)
    {
        Console.Write("→ ");
        sourceAsString = "";

        int counter = 0;
        while (true)
        {
            readline = Console.ReadLine();
            if (readline == "")
            {
                counter++;
                if (counter == 3)
                {
                    break;
                }
            }
            else
            {
                counter = 0;
            }
            sourceAsString += readline;

        }

        Console.WriteLine(Environment.NewLine + "=====OUTPUT:=======");
        Console.WriteLine(new JukaCompiler.Compiler().Go(sourceAsString, false));
    }
}
else
{
    sourceAsString = args[0];
    Console.WriteLine(new JukaCompiler.Compiler().Go(sourceAsString, true));
}