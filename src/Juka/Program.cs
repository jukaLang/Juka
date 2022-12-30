using Juka;

if (args.Length == 0)
{
    await Repl.RunRepl();
}
else
{
    await TerminalJuka.Perform(args);
}