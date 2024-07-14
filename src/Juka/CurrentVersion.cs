namespace Juka;

// <summary>
/// Gets the version of the assembly.
/// This code defines a class CurrentVersion with a static method GetVersion that retrieves the version of the assembly.
/// Gets the version of the assembly.
/// If the version is "0.0.0.1", it returns "DEBUG"; otherwise, it returns the actual version number as a string.
/// </summary>
/// <returns>Returns a string representing the version.</returns>
public class CurrentVersion
{
    public static string GetVersion()
    {
        string debugversion = "DEBUG";
        string version = typeof(CurrentVersion).Assembly.GetName().Version?.ToString() ?? debugversion;
        if (version == "0.0.0.1")
        {
            version = debugversion;
        }
        return version;
    }
}