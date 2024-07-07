namespace JukaCompiler
{
    public static class DebugMe
    {
        // The debug mode value. The default is 1.
        private static int debugMode = 1;

        // Public property to get or set the debug mode.
        // This allows external code to control the debug mode.
        public static int DebugMode
        {
            get => debugMode;
            set => debugMode = value;
        }
    }
}