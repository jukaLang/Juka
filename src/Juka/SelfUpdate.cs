using Newtonsoft.Json.Linq;
using Spectre.Console;
using System.IO.Compression;
using System.Runtime.InteropServices;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace Juka;

class SelfUpdate
{
    private static readonly string CurrentVersion = Juka.CurrentVersion.Get();
    public static async Task<string> Check()
    {
        if (CurrentVersion == "DEBUG")
        {
            AnsiConsole.MarkupLine("[yellow]You seem to be using a DEBUG version of Juka. Can't update a debug version![/]");
            return "";
        }
        AnsiConsole.MarkupLine("[bold yellow]Checking for updates for Juka Programming Language...[/]");
        AnsiConsole.MarkupLine($"[bold red]Current Version:[/] [red]{CurrentVersion}[/]");

        try
        {

            HttpClient client = new()
            {
                BaseAddress = null,
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new("application/json"));
            //Do not use this header as GitHub might disable access to the api
            //client.DefaultRequestHeaders.Add("User-Agent", "Juka HTTPClient");
            client.DefaultRequestHeaders.Add("User-Agent", "Juka HTTPClient");
            HttpResponseMessage response = await client.GetAsync("https://api.github.com/repos/JukaLang/juka/releases/latest");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            string latestVersion = (string?)JObject.Parse(responseBody).SelectToken("tag_name") ?? "";
            AnsiConsole.MarkupLine($"[bold blue]Latest Version: {latestVersion}[/]");
            if (string.Compare(CurrentVersion, latestVersion, StringComparison.Ordinal) < 0)
            {
                AnsiConsole.MarkupLine("[red]New version of Juka is available! Please update![/]");
                return latestVersion;
            }
            AnsiConsole.MarkupLine("[green]You are using the latest version![/] No need to update!");
            return "";
        }
        catch (Exception)
        {
            AnsiConsole.MarkupLine("[bold yellow]Cannot Update! Can't access the Network![/]");
        }
        return "";
    }

    public static Dictionary<string,string> Info()
    {
        string architecture = RuntimeInformation.ProcessArchitecture.ToString();

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
        string dir = AppDomain.CurrentDomain.BaseDirectory;
        string name = typeof(SelfUpdate).Assembly.GetName().Name ?? "";

        string extension = "";
        if (platform == "Windows")
        {
            extension = ".exe";
        }

        AnsiConsole.MarkupLine($"[bold blue]Your Operating System:[/] [green]{platform}[/]");
        AnsiConsole.MarkupLine($"[bold blue]Current Directory:[/] [green]{dir}[/]");
        AnsiConsole.MarkupLine($"[bold blue]Your Juka Assembly Architecture:[/] [green]{architecture}[/]");
        AnsiConsole.MarkupLine($"[bold blue]Your Juka Assembly Name:[/] [green]{name}[/]");
        AnsiConsole.MarkupLine($"[bold blue]Your Juka Assembly Extension:[/] [green]{extension}[/]");
        return new Dictionary<string, string>
        {
            { "platform", platform },
            { "dir", dir },
            { "architecture", architecture },
            {"name", name},
            { "extension", extension }
        };
    }
    public static async Task Update()
    {
        string latestVersion = await Check();

        if (latestVersion != "")
        {
            IDictionary<string, string> info = Info();

            try
            {
                if (File.Exists(info["dir"] + "JukaCompiler.pdb"))
                {
                    File.Delete(info["dir"] + "JukaCompiler.pdb");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            string jukaExePath = info["dir"] + info["name"] + info["extension"];

            try
            {
                if (File.Exists(jukaExePath + ".backup"))
                {
                    File.Delete(jukaExePath + ".backup");
                }

                File.Move(jukaExePath, jukaExePath + ".backup");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            AnsiConsole.MarkupLine(info["architecture"]);
            AnsiConsole.MarkupLine(latestVersion);

            switch (info["architecture"])
            {
                case "X64":
                case "X86":
                case "Arm":
                case "Arm64":
                {
                    AnsiConsole.MarkupLine("[bold green] Downloading...[/]" + latestVersion);
                    string zipext = ".zip";
                    if (info["platform"] == "Unix" || (info["platform"] == "Linux" && info["architecture"] == "X86"))
                    {
                        zipext = ".tar.gz";
                    }


                    try
                    {
                        string url = "https://github.com/jukaLang/Juka/releases/download/" + latestVersion +
                                     "/Juka_" +
                                     info["platform"] + "_" + info["architecture"] + "_" + latestVersion + zipext;
                        AnsiConsole.MarkupLine("[yellow]Downloading from [/]"+url);
                        using HttpResponseMessage response2 = await new HttpClient().GetAsync(url);
                        await using Stream streamToReadFrom = await response2.Content.ReadAsStreamAsync();


                        if (zipext == ".zip")
                        {
                            using ZipArchive zip = new(streamToReadFrom);
                            zip.ExtractToDirectory(info["dir"]);
                        }
                        else
                        {
                            Stream gzipStream = new GZipInputStream(streamToReadFrom);
                            TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
                            tarArchive.ExtractContents(info["dir"]);
                            tarArchive.Close();
                            gzipStream.Close();
                        }
                        streamToReadFrom.Close();

                        AnsiConsole.MarkupLine("[green]Updated to version: " + latestVersion + "[/]");

                        Restart(jukaExePath);
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLine("[bold red]Something went wrong downloading latest version of Juka...Download from Official Website [/][link]https://jukalang.com[/]");
                        AnsiConsole.WriteException(ex);
                        if (File.Exists(jukaExePath + ".backup"))
                        {
                            File.Move(jukaExePath + ".backup", jukaExePath);
                        }
                    }

                    break;
                }
                case "MSIL":
                    AnsiConsole.MarkupLine("[yellow]You seem to be using a DEBUG version of Juka. Can't update![/]");
                    break;
                default:
                    AnsiConsole.MarkupLine("[red]Something went wrong! We cannot determine your processor...If you see this, please post it on the Issues Page![/][link]https://github.com/jukaLang/juka/issues/ [/]");
                    break;
            }
        }
    }
    public static void Restart(string jukaExePath)
    {
        //Start process, friendly name is something like MyApp.exe (from current bin directory)
        System.Diagnostics.Process.Start(jukaExePath);

        //Close the current process
        Environment.Exit(0);
    }
}