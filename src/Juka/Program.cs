namespace Juka
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                await Repl.RunRepl();
            }
            else
            {
                await TerminalJuka.Perform(args);
            }
        }
    }
}
