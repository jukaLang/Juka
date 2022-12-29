using System.Reflection;

namespace Juka
{
    public class CurrentVersion
    {
        public static string Get()
        {
            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "DEBUG";
            if (assemblyVersion == "0.0.0.1")
            {
                assemblyVersion = "DEBUG";
            }
            return assemblyVersion;
        }
    }
}
