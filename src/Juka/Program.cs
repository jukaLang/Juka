using System.IO.Compression;
using System.Net.Http.Headers;
using System.Reflection;
using Newtonsoft.Json.Linq;

string? userInput;
string? readline;

string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

if (args.Length == 0)
{
    Console.Title = "Juka Compiler";
    Console.WriteLine("♥ Welcome to Juka Compiler Version: "+assemblyVersion+". If you need to run a file, pass it as an argument ♥");
    Console.WriteLine("Press Return 3 times to run the code");

    while (true)
    {
        Console.Write("Juka > ");
        userInput = "";

        int counter = 0;
        while (true)
        {
            readline = Console.ReadLine();
            if (readline == "")
            {
                if (userInput == "")
                {
                    Console.Write("Juka > ");
                    continue;
                }

                counter++;
                if (counter == 2)
                {
                    break;
                }
            }
            else
            {
                counter = 0;
            }
            userInput += readline;

        }

        Console.WriteLine(Environment.NewLine + "=====OUTPUT:=======");
        Console.WriteLine(new JukaCompiler.Compiler().Go(userInput, isFile: false));
    }
}
else
{
    userInput = args[0];
    if (userInput == "-d" || userInput == "--debug")
    {

    }
    if (userInput == "-v" || userInput == "--version")
    {
        string currentVersion = "0.0.0.1";
        var curVer = Assembly.GetExecutingAssembly().GetName().Version;
        if (curVer != null)
        {
            currentVersion = curVer.ToString();
        }

        Console.WriteLine($"Current Version: {currentVersion}");
    }
    if (userInput == "-su" || userInput == "--self-update")
    {
        Console.WriteLine("Updating Juka Programming Language...");
        string currentVersion = "0.0.0.1";
        var curVer = Assembly.GetExecutingAssembly().GetName().Version;
        if (curVer != null)
        {
            currentVersion = curVer.ToString();
        }

        Console.WriteLine($"Current Version: {currentVersion}");
        
        var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("User-Agent", "Juka HTTPClient");
        HttpResponseMessage response = await client.GetAsync("https://api.github.com/repos/JukaLang/juka/releases/latest");
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        string latestVersion = (string)JObject.Parse(responseBody).SelectToken("tag_name");
        Console.WriteLine($"Latest Version: {latestVersion}");
        if (String.Compare(currentVersion, latestVersion, StringComparison.Ordinal) < 0)
        {
            string processor = Assembly.GetExecutingAssembly().GetName().ProcessorArchitecture.ToString();

            PlatformID pid = Environment.OSVersion.Platform;
            string platform;
            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    platform = "Windows";
                    break;
                case PlatformID.Unix:
                    platform = "Unix";
                    break;
                case PlatformID.MacOSX:
                    platform = "Mac";
                    break;
                default:
                    platform = "Linux";
                    break;
            }
            Console.WriteLine($"Your Juka Assembly Version: {processor}");
            Console.WriteLine($"Your Operating System: {platform} ");
            if (platform == "Windows")
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                string name = Assembly.GetExecutingAssembly().GetName().Name;
                if (File.Exists(dir + name + ".backup.exe"))
                {
                    File.Delete(dir + name + ".backup.exe");
                }

                if (File.Exists(dir + "JukaCompiler.pdb"))
                {
                    File.Delete(dir + "JukaCompiler.pdb");
                }

                File.Move(dir + name + ".exe", dir + name + ".backup.exe");
                if (processor == "Amd64" || processor == "X86" || processor == "Arm")
                {
                    string url = "";
                    if (processor == "Amd64")
                    {
                        url =
                            "https://github.com/jukaLang/Juka/releases/download/" + latestVersion + "/Juka_WindowsX64_" +
                            latestVersion + ".zip";
                    } else if (processor == "X86")
                    {
                        url =
                            "https://github.com/jukaLang/Juka/releases/download/" + latestVersion + "/Juka_WindowsX86_" +
                            latestVersion + ".zip";
                    }
                    else
                    {
                        url =
                            "https://github.com/jukaLang/Juka/releases/download/" + latestVersion + "/Juka_WindowsARM64_" +
                            latestVersion + ".zip";
                    }
                    using var response2 = await new HttpClient().GetAsync(url);
                    await using var streamToReadFrom = await response2.Content.ReadAsStreamAsync();
                    using var zip = new ZipArchive(streamToReadFrom);
                    zip.ExtractToDirectory(dir);
                    Console.WriteLine("Updated to version: "+latestVersion);
                }
                else if (processor == "MSIL")
                {
                    Console.WriteLine("You seem to be using a debug version of Juka. Can't update!");
                }
                else
                {
                    Console.WriteLine("Something went wrong! Please post this on Juka's issues page");
                }
            }
            else
            {
                Console.WriteLine("You need to download a new version at https://jukalang.com/download");
            }
        }
        else
        {
            Console.WriteLine("No need to update. You are using the latest version!");
        }
    }
    else
    {
        Console.WriteLine(new JukaCompiler.Compiler().Go(userInput, isFile: true));
    }

}