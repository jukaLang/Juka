using Newtonsoft.Json.Linq;
using Spectre.Console;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

namespace Juka;

class SelfUpdate
{
    private static readonly string CurrentVersion = Juka.CurrentVersion.Get();
    public static async Task<string> Check()
    {
        if (CurrentVersion == "DEBUG")
        {
            AnsiConsole.MarkupLine("[yellow]You seem to be using a DEBUG version of Juka. Can't update![/]");
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
            client.DefaultRequestHeaders.Add("User-Agent", "Juka HTTPClient");
            HttpResponseMessage response = await client.GetAsync("https://api.github.com/repos/JukaLang/juka/releases/latest");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            string latestVersion = (string?)JObject.Parse(responseBody).SelectToken("tag_name") ?? "";
            AnsiConsole.MarkupLine($"[bold blue]Latest Version: {latestVersion}[/]");
            if (string.Compare(CurrentVersion, latestVersion, StringComparison.Ordinal) < 0)
            {
                AnsiConsole.MarkupLine("[red]New version of Juka is available![/]");
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

            string jukaexepath = info["dir"] + info["name"] + info["extension"];

            try
            {
                if (File.Exists(jukaexepath + ".backup"))
                {
                    File.Delete(jukaexepath + ".backup");
                }

                File.Move(jukaexepath, jukaexepath + ".backup");
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
                        using HttpResponseMessage response2 = await new HttpClient().GetAsync(url);
                        await using Stream streamToReadFrom = await response2.Content.ReadAsStreamAsync();


                        if (zipext == ".zip")
                        {
                            using ZipArchive zip = new(streamToReadFrom);
                            zip.ExtractToDirectory(info["dir"]);
                        }
                        else
                        {
                            await using var gzip = new GZipStream(streamToReadFrom, CompressionMode.Decompress);
                            const int chunk = 4096;
                            using var memStr = new MemoryStream();
                            int read;
                            var buffer = new byte[chunk];
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
                                {
                                    break;
                                }

                                memStr.Seek(24, SeekOrigin.Current);
                                memStr.Read(buffer, 0, 12);
                                long size = Convert.ToInt64(
                                    Encoding.UTF8.GetString(buffer, 0, 12).Trim('\0').Trim(), 8);

                                memStr.Seek(376L, SeekOrigin.Current);

                                string output = Path.Combine(info["dir"], fname);
                                if (!Directory.Exists(Path.GetDirectoryName(output)))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(output) ??
                                                              throw new Exception(
                                                                  "output path is invalid"));
                                }

                                if (!fname.Equals("./", StringComparison.InvariantCulture))
                                {
                                    await using FileStream str = File.Open(output, FileMode.OpenOrCreate,
                                        FileAccess.Write);
                                    byte[] buf = new byte[size];
                                    memStr.Read(buf, 0, buf.Length);
                                    str.Write(buf, 0, buf.Length);
                                }

                                var pos = memStr.Position;

                                long offset = 512 - (pos % 512);
                                if (offset == 512)
                                {
                                    offset = 0;
                                }

                                memStr.Seek(offset, SeekOrigin.Current);
                            }
                        }

                        AnsiConsole.MarkupLine("[green]Updated to version: " + latestVersion + "[/]");

                        //Start process, friendly name is something like MyApp.exe (from current bin directory)
                        System.Diagnostics.Process.Start(jukaexepath);

                        //Close the current process
                        Environment.Exit(0);
                    }
                    catch (Exception)
                    {
                        AnsiConsole.WriteLine("[bold red]Something went wrong downloading latest version of Juka...Download from Official Website [/][link]https://jukalang.com[/]");
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
}