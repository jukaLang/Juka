namespace Juka;

public class CurrentVersion
{
    public static string Get()
    {
        string assemblyVersion = typeof(CurrentVersion).Assembly.GetName().Version?.ToString() ?? "DEBUG";
        if (assemblyVersion == "0.0.0.1")
        {
            assemblyVersion = "DEBUG";
        }
        return assemblyVersion;
    }
}