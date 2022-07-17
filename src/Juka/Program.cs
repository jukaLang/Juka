using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Reflection;

string? userInput;
string? readline;

if (args.Length == 0)
{
    Console.Title = "Juka Compiler";
    Console.WriteLine("♥ Welcome to Juka Compiler. If you need to run a file, pass it as an argument ♥");
    Console.WriteLine("Press Return 4 times to run the code");

    while (true)
    {
        Console.Write("> ");
        userInput = "";

        int counter = 0;
        while (true)
        {
            readline = Console.ReadLine();
            if (readline == "")
            {
                counter++;
                if (counter == 3)
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
    if (userInput == "-su" || userInput == "--self-update")
    {
        Console.WriteLine("Updating Juka Programming Language...");
        string currentVersion = "0.0.0";
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
        GitHubApi myGitObj = JsonConvert.DeserializeObject<GitHubApi>(responseBody);
        string latestVersion = myGitObj.ToString();
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
            Console.WriteLine($"Your Processor: {processor}");
            Console.WriteLine($"Your OS: {platform} ");
            Console.WriteLine("You need to download a new version at https://jukalang.com/download");

        }
        else
        {
            Console.WriteLine("You are using the latest version!");
        }
    }
    else
    {
        Console.WriteLine(new JukaCompiler.Compiler().Go(userInput, isFile: true));
    }

}

internal class GitHubApi                     
{
    string tag_name { get; set; }

    public override string ToString()
    {
        return tag_name;
    }
}
