using Juka;

if (args.Length == 0)
{
    await REPL.RunREPL();
}


await TerminalJuka.Perform(args);
