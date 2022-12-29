using Newtonsoft.Json.Linq;
using Spectre.Console;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Juka
{
    class SelfUpdate
    {
        public static async Task<string> Check()
        {
            AnsiConsole.MarkupLine("[bold yellow]Checking for updates for Juka Programming Language...[/]");
            AnsiConsole.MarkupLine($"[bold red]Current Version:[/] [red]{CurrentVersion.Get()}[/]");


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
            if (string.Compare(CurrentVersion.Get(), latestVersion, StringComparison.Ordinal) < 0)
            {
                AnsiConsole.MarkupLine("[red]New version of Juka is available![/]");
                return latestVersion;
            }
            AnsiConsole.MarkupLine("[green]You are using the latest version![/] No need to update!");
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
            string name = Assembly.GetExecutingAssembly().GetName().Name ?? "";

            string extension = "";
            if (platform == "Windows")
            {
                extension = ".exe";
            }

            AnsiConsole.MarkupLine($"[bold orange]Your Operating System:[/] [green]{platform}[/]");
            AnsiConsole.MarkupLine($"[bold orange]Current Directory:[/] [green]{dir}[/]");
            AnsiConsole.MarkupLine($"[bold orange]Your Juka Assembly Architecture:[/] [green]{architecture}[/]");
            AnsiConsole.MarkupLine($"[bold orange]Your Juka Assembly Name:[/] [green]{name}[/]");
            AnsiConsole.MarkupLine($"[bold orange]Your Juka Assembly Extension:[/] [green]{extension}[/]");
            return new Dictionary<string, string>
            {
                { "platform", platform },
                { "dir", dir },
                { "architecture", architecture },
                {"name", name},
                { "extension", extension }
            };
        }

        /*public static async Task InteractiveUpdate()
        {
            var items = new[] {
                ("Updating Juka")
            };
            await AnsiConsole.Progress()
                .Columns(new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new RemainingTimeColumn(),
                    new SpinnerColumn(),
                })
                .StartAsync(async ctx =>
                {

                    await Task.WhenAll(items.Select(async item =>
                    {
                        var task = ctx.AddTask("[green]Updating Juka...[/]", new ProgressTaskSettings
                        {
                            AutoStart = false
                        });

                        await SelfUpdate.Update(task);
                    }));
                });
        }*/

        public static async Task Update()
        {
            string latestVersion = await Check();
            if (latestVersion != "")
            {
                IDictionary<string, string> info = Info();

                if (File.Exists(info["dir"] + "JukaCompiler.pdb"))
                {
                    File.Delete(info["dir"] + "JukaCompiler.pdb");
                }

                string jukaexepath = info["dir"] + info["name"] + info["extension"];


                if (File.Exists(jukaexepath + ".backup"))
                {
                    File.Delete(jukaexepath + ".backup");
                }
                File.Move(jukaexepath, jukaexepath +".backup");

                string zipext = ".zip";
                if (info["platform"] == "Unix" || (info["platform"] == "Linux" && info["architecture"] == "X86"))
                {
                    zipext = ".tar.gz";
                }
                
                switch (info["architecture"])
                {
                    case "X64":
                    case "X86":
                    case "Arm":
                    case "Arm64":
                        {
                            string url = "https://github.com/jukaLang/Juka/releases/download/" + latestVersion + "/Juka_" +
                                         info["platform"] + "_" + info["architecture"] + "_" + latestVersion + zipext;
                            using HttpResponseMessage response2 = await new HttpClient().GetAsync(url);

                            // Set the max value of the progress task to the number of bytes
                            //task.MaxValue(response2.Content.Headers.ContentLength ?? 0);
                            // Start the progress task
                            //task.StartTask();

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
                            break;
                        }
                    case "MSIL":
                        AnsiConsole.MarkupLine("[yellow]You seem to be using a DEBUG version of Juka. Can't update![/]");
                        break;
                    default:
                        AnsiConsole.MarkupLine("[red]Something went wrong! We cannot determine your processor...If you see this, please post it on the Issues Page![/]");
                        break;
                }
            }
        }
    }
}
