JukaCompiler.Compiler compiler = new JukaCompiler.Compiler();
string? sourceAsString;

if (args == null || args.Length == 0)
{
    Console.WriteLine("🥣 Welcome to Juka Compiler. Either Pipe the code or use interactive interpreter below: 🥣");
    while (true)
    {
        Console.Write("➡️ ");
        sourceAsString = Console.ReadLine();
        Console.WriteLine(compiler.Go(sourceAsString, false));
    }
}
else
{
    sourceAsString = args[0];
    Console.WriteLine(compiler.Go(sourceAsString, true));
}