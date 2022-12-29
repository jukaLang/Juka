using Juka;

if (args.Length == 0)
{
    await REPL.RunRepl();
}


await TerminalJuka.Perform(args);
