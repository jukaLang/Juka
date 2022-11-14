using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using JukaCompiler;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using Color = Spectre.Console.Color;

string? assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
if (assemblyVersion == "0.0.0.1")
{
    assemblyVersion = "DEBUG";
}

bool isFuncOrClass = false;
string prompt = "[bold green]Juka[/]([red]" + assemblyVersion+"[/])> ";


if (args.Length == 0)
{

    Console.Title = "Juka Programming Language";
    Console.BackgroundColor = ConsoleColor.Black;
    Console.ForegroundColor = ConsoleColor.White;
    Console.OutputEncoding = Encoding.UTF8;
    
    

    AnsiConsole.Write(
        new FigletText("Juka")
            .LeftAligned().Color(Color.Purple));

    AnsiConsole.MarkupLine("[bold yellow]Hello[/] and [bold red]Welcome to 🍲 Juka Programming Language![/] For info visit [link blue]https://jukalang.com[/]");

    Compiler compiler = new();

    string dataStart = "func main() = {";
    string dataEnd = "}";

    List<string> funcData = new();
    AnsiConsole.Markup(prompt);
    
    while (true)
    {
        string? readLine = Console.ReadLine();
        if (string.IsNullOrEmpty(readLine))
        {
            AnsiConsole.Markup(prompt);
            continue;
        }

        if (readLine.Equals("clear", StringComparison.OrdinalIgnoreCase))
        {
            Console.Clear();
            compiler = new Compiler();
            isFuncOrClass = false;
            funcData.Clear();
            dataStart = "";
            dataEnd = "";
            AnsiConsole.Markup(prompt);
            continue;
        }

        if (readLine.Equals("list", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var data in funcData)
            {
                Console.WriteLine(data);
            }

            AnsiConsole.Markup(prompt);
            continue;
        }

        if (readLine.StartsWith("func") || readLine.StartsWith("class"))
        {
            isFuncOrClass = true;
            funcData.Add(readLine);
            Trace.WriteLine("Starting Func: " + readLine);
        }
        else if (isFuncOrClass)
        {
            if (readLine.StartsWith("}"))
            {
                funcData.Add(readLine);
                Trace.WriteLine("Ending Func: " + readLine);
                string userDataToExecute = string.Empty;
                foreach (string item in funcData)
                {
                    userDataToExecute += item;
                }

                dataEnd += userDataToExecute;
                isFuncOrClass = false;
                AnsiConsole.Markup(prompt);
            }
            else
            {
                Trace.WriteLine("Reading Func: " + readLine);
                funcData.Add(readLine);
            }
        }
        else
        {
            if (readLine.StartsWith("var"))
            {
                dataStart += readLine;
                readLine = "";
            }

            funcData.Add(readLine);

            string codeToExecute = dataStart + readLine+ dataEnd;
            try
            {
                Trace.WriteLine(codeToExecute);
                Console.WriteLine(compiler.Go(codeToExecute, isFile: false));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            AnsiConsole.Markup(prompt);
        }
    }
}
string userInput = args[0];
if (userInput == "-d" || userInput == "--debug")
{

}
if (userInput == "-v" || userInput == "--version")
{
    string currentVersion = "0.0.0.1";
    Version? curVer = Assembly.GetExecutingAssembly().GetName().Version;
    if (curVer != null)
    {
        currentVersion = curVer.ToString();
    }

    if (currentVersion == "0.0.0.1") currentVersion = "DEBUG BUILD";
    Console.WriteLine($"Current Version: {currentVersion}");
}
if (userInput == "-su" || userInput == "--self-update")
{
    Console.WriteLine("Updating Juka Programming Language...");
    string currentVersion = "0.0.0.1";
    Version? curVer = Assembly.GetExecutingAssembly().GetName().Version;
    if (curVer != null)
    {
        currentVersion = curVer.ToString();
    }

    if (currentVersion == "0.0.0.1")
    {
        Console.WriteLine("Current Version: DEBUG BUILD");
    }
    else
    {
        Console.WriteLine($"Current Version: {currentVersion}");
    }

    HttpClient client = new()
    {
        BaseAddress = null,
        DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower,
        MaxResponseContentBufferSize = 0,
        Timeout = default
    };
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(
        new("application/json"));
    client.DefaultRequestHeaders.Add("User-Agent", "Juka HTTPClient");
    HttpResponseMessage response = await client.GetAsync("https://api.github.com/repos/JukaLang/juka/releases/latest");
    response.EnsureSuccessStatusCode();
    string responseBody = await response.Content.ReadAsStringAsync();
    string? latestVersion = (string?)JObject.Parse(responseBody).SelectToken("tag_name");
    Console.WriteLine($"Latest Version: {latestVersion}");
    if (String.Compare(currentVersion, latestVersion, StringComparison.Ordinal) < 0)
    {
        string processor = Assembly.GetExecutingAssembly().GetName().ProcessorArchitecture.ToString();

        PlatformID pid = Environment.OSVersion.Platform;
        string platform = pid switch
        {
            PlatformID.Win32NT => "Windows",
            PlatformID.Win32S => "Windows",
            PlatformID.Win32Windows => "Windows",
            PlatformID.WinCE => "Windows",
            PlatformID.Unix => "Unix",
            PlatformID.MacOSX => "MacOS",
            PlatformID.Xbox => "Linux",
            PlatformID.Other => "Linux",
            _ => "Linux"
        };
        Console.WriteLine($"Your Juka Assembly Version: {processor}");
        Console.WriteLine($"Your Operating System: {platform} ");
        string dir = AppDomain.CurrentDomain.BaseDirectory;
        string? name = Assembly.GetExecutingAssembly().GetName().Name;

        if (File.Exists(dir + "JukaCompiler.pdb"))
        {
            File.Delete(dir + "JukaCompiler.pdb");
        }

        string extension = "";
        

        if (platform == "Windows")
        {
            extension = ".exe";
        }

        if (File.Exists(dir + name + ".backup" + extension))
        { 
            File.Delete(dir + name + ".backup" + extension);
        }

        File.Move(dir + name + extension, dir + name + extension);

        string zipext = ".zip";
        if (platform == "Unix" || (platform == "Linux" && processor == "X86"))
        {
            zipext = ".tar.gz";
        }

        switch (processor)
        {
            case "Amd64":
            case "X86":
            case "Arm":
            {
                string url = "https://github.com/jukaLang/Juka/releases/download/" + latestVersion + "/Juka_" +
                             platform + "_" + processor + "_" + latestVersion + zipext;
                using HttpResponseMessage response2 = await new HttpClient().GetAsync(url);
                await using Stream streamToReadFrom = await response2.Content.ReadAsStreamAsync();
                if(zipext == ".zip"){
                    using ZipArchive zip = new(streamToReadFrom);
                    zip.ExtractToDirectory(dir);
                } else
                {
                    using GZipStream gzip = new(streamToReadFrom, CompressionMode.Decompress);
                    const int chunk = 4096;
                    using MemoryStream memStr = new();
                    int read;
                    byte[] buffer = new byte[chunk];
                    do
                    {
                        read = gzip.Read(buffer, 0, chunk);
                        memStr.Write(buffer, 0, read);
                    } while (read == chunk);
                    memStr.Seek(0, SeekOrigin.Begin);

                    buffer = new byte[100];
                    while (true)
                    {
                        memStr.Read(buffer, 0, 100);
                        string fname = Encoding.ASCII.GetString(buffer).Trim('\0');
                        if (String.IsNullOrWhiteSpace(fname))
                            break;
                        memStr.Seek(24, SeekOrigin.Current);
                        memStr.Read(buffer, 0, 12);
                        long size = Convert.ToInt64(Encoding.UTF8.GetString(buffer, 0, 12).Trim('\0').Trim(), 8);

                        memStr.Seek(376L, SeekOrigin.Current);

                        string output = Path.Combine(dir, fname);
                        if (!Directory.Exists(Path.GetDirectoryName(output)))
                            Directory.CreateDirectory(Path.GetDirectoryName(output) ?? throw new Exception("output path is invalid"));
                        if (!fname.Equals("./", StringComparison.InvariantCulture))
                        {
                            using FileStream str = File.Open(output, FileMode.OpenOrCreate, FileAccess.Write);
                            byte[] buf = new byte[size];
                            memStr.Read(buf, 0, buf.Length);
                            str.Write(buf, 0, buf.Length);
                        }

                        long pos = memStr.Position;

                        long offset = 512 - (pos % 512);
                        if (offset == 512)
                            offset = 0;

                        memStr.Seek(offset, SeekOrigin.Current);
                    }
                }
                Console.WriteLine("Updated to version: "+latestVersion);
                break;
            }
            case "MSIL":
                Console.WriteLine("You seem to be using a debug version of Juka. Can't update!");
                break;
            default:
                Console.WriteLine("Something went wrong! Please post this on Juka's issues page");
                break;
        }

    } 
    else
    {
        Console.WriteLine("No need to update. You are using the latest version!");
    }
}
else
{
    Console.WriteLine(new Compiler().Go(userInput, isFile: true));
}