using System;

namespace DreamCompiler
{
    public class CommandLineProvider
    {
        public static ICommandLineProvider InputArgs(string[] args, IServiceProvider service)
        {
            return new ArgumentParser(args);
        }
    }

    public class ArgumentParser : ICommandLineProvider
    {
        private string[] args;

        public ArgumentParser(string[] args)
        {
            this.args = args;
        }

        public string Check(int num)
        {
            return args[num];
        }
    }
}
