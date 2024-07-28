using Newtonsoft.Json.Linq;
using Spectre.Console;
using System.IO.Compression;
using System.Runtime.InteropServices;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System.Text;
using System.Diagnostics;

namespace Juka;

/// <summary>
/// Class responsible for updating the Juka programming language.
/// </summary>
class SelfUpdate
{
    private static string? _currentVersion = Juka.CurrentVersion.GetVersion();


    /// <summary>
    /// Check for updates for Juka Programming Language
    /// </summary>
    /// <returns>An async Task with a string representing the latest version if an update is available, otherwise an empty string</returns>

    public static async Task<string> Check()
    {
        if (_currentVersion == "DEBUG")
        {
            AnsiConsole.MarkupLine("[yellow]You seem to be using a DEBUG version of Juka. Can't update a debug version![/]" + "[gold3] Current Date/Time:" + DateTime.Now.ToString()+"[/]");
            return _currentVersion;
        }

        AnsiConsole.MarkupLine("[bold yellow]Checking for updates for Juka Programming Language...[/]"+$"[bold red]Current Version:[/] [red]{_currentVersion}[/]");

        try
        {
            HttpClient? client = new()
            {
                BaseAddress = null,
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower
            };

            // Set up request headers
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Juka HTTPClient");

            // Make API request to get latest version
            HttpResponseMessage? response = await client.GetAsync("https://api.github.com/repos/JukaLang/juka/releases/latest");
            response.EnsureSuccessStatusCode();
            string? responseBody = await response.Content.ReadAsStringAsync();
            string? latestVersion = (string?)JObject.Parse(responseBody).SelectToken("tag_name") ?? "";

            // Check if update is needed
            if (string.Compare(_currentVersion, latestVersion, StringComparison.Ordinal) < 0)
            {
                AnsiConsole.MarkupLine("[red]New version of Juka is available! Type !!update to update to the latest version[/]" + $"[bold blue]Latest Version: {latestVersion}[/]");
                //await SelfUpdate.Update("force");
                return latestVersion;
            }

            AnsiConsole.MarkupLine("[green]You are using the latest version: "+latestVersion+"[/]");
            return latestVersion;
        }
        catch (Exception)
        {
            AnsiConsole.MarkupLine("[bold yellow]Cannot Update! Can't access the Network! Using version: "+_currentVersion+"[/]");
        }
        return _currentVersion;
    }

    public static async Task DownloadURLAsync(string url)
    {
        AnsiConsole.MarkupLine("[yellow]Downloading from [/]" + url);
        using HttpResponseMessage? response2 = await new HttpClient().GetAsync(url);
        await using Stream? streamToReadFrom = await response2.Content.ReadAsStreamAsync();


        Uri uri = new Uri(url);
        string fileName = Path.GetFileName(uri.LocalPath);


        using (FileStream fileStream = File.Create(fileName))
        {
            streamToReadFrom.CopyTo(fileStream);
        }

    }


    public static string RunCommand(string command, string arguments)
    {
        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = new Process { StartInfo = processStartInfo })
        {
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
    }

    /// <summary>
    /// Get system information
    /// </summary>
    /// <returns>A dictionary containing system information such as platform, directory, architecture, name, and extension</returns>

    public static Dictionary<string, string> GetSystemInfo()
    {
        // Get system information
        string? architecture = RuntimeInformation.ProcessArchitecture.ToString();
        PlatformID? pid = Environment.OSVersion.Platform;

        // Map platform to user-friendly names
        string? platform = pid switch
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

        var fullplatform = platform;
        if(platform == "Unix")
        {
            string unameOutput = RunCommand("uname", "-a");

            if (unameOutput.Contains("Linux TinaLinux"))
            {
                fullplatform = unameOutput;
                platform = "Linux";
            }
        }

        string? dir = AppDomain.CurrentDomain.BaseDirectory;
        string? name = typeof(SelfUpdate).Assembly.GetName().Name ?? "";

        string? extension = platform == "Windows" ? ".exe" : "";


        return new Dictionary<string, string>
        {
            { "fullplatform", fullplatform },
            { "platform", platform },
            { "dir", dir },
            { "architecture", architecture },
            { "name", name },
            { "extension", extension }
        };
    }

    /// <summary>
    /// Update the Juka Programming Language to the latest version
    /// </summary>
    /// <returns>An async Task</returns>
    public static async Task Update(string update = "normal")
    {
        string? latestVersion = await Check();

        if (_currentVersion != latestVersion || update == "force")
        {
            if (latestVersion == "DEBUG")
            {
                HttpClient? client = new()
                {
                    BaseAddress = null,
                    DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower
                };

                // Set up request headers
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "Juka HTTPClient");

                // Make API request to get latest version
                HttpResponseMessage? response = await client.GetAsync("https://api.github.com/repos/JukaLang/juka/releases/latest");
                response.EnsureSuccessStatusCode();
                string? responseBody = await response.Content.ReadAsStringAsync();
                latestVersion = (string?)JObject.Parse(responseBody).SelectToken("tag_name") ?? "";
            }


            IDictionary<string, string>? info = GetSystemInfo();

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

            string? jukaExePath = info["dir"] + info["name"] + info["extension"];

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
                    string? zipext = ".zip";
                    if (info["platform"] == "Unix" || (info["platform"] == "Linux" && info["architecture"] == "X86"))
                    {
                        zipext = ".tar.gz";
                    }


                    try
                    {
                        string? url = "https://github.com/jukaLang/Juka/releases/download/" + latestVersion +
                                     "/Juka_" +
                                     info["platform"] + "_" + info["architecture"] + "_" + latestVersion + zipext;
                        AnsiConsole.MarkupLine("[yellow]Downloading from [/]"+url);
                        using HttpResponseMessage? response2 = await new HttpClient().GetAsync(url);
                        await using Stream? streamToReadFrom = await response2.Content.ReadAsStreamAsync();


                        if (zipext == ".zip")
                        {
                            using ZipArchive? zip = new(streamToReadFrom);
                            zip.ExtractToDirectory(info["dir"]);
                        }
                        else
                            {
                                Stream? gzipStream = new GZipInputStream(streamToReadFrom);
                                using (TarArchive? tarArchive = TarArchive.CreateInputTarArchive(gzipStream, Encoding.UTF8))
                                {
                                    tarArchive.ExtractContents(info["dir"]);
                                    tarArchive.Close();
                                }

                                gzipStream.Close();
                            }
                            streamToReadFrom.Close();

                        AnsiConsole.MarkupLine("[green]Updated to version: " + latestVersion + "[/]");

                        await Restart(jukaExePath,"normal");
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
    public static async Task Restart(string jukaExePath, string type)
    {
        if(type == "full") { 
            try
            {
                // Perform any cleanup or resource releasing before restarting
                // Example: Save user data, close connections, etc.

                ProcessStartInfo startInfo = new()
                {
                    FileName = jukaExePath,
                    UseShellExecute = true
                };

                Process newProcess = Process.Start(startInfo);

                // Close the current process gracefully
                await Task.Delay(1000); // Delay to ensure the new process starts
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during restart: {ex.Message}");
                // Log the exception for troubleshooting
            }
        }
        else
        {
            try
            {
                // Perform any cleanup or resource releasing before restarting
                // Example: Save user data, close connections, etc.

                Process.Start(jukaExePath);

                // Close the current process gracefully
                await Task.Delay(1000); // Delay to ensure the new process starts
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during restart: {ex.Message}");
                // Log the exception for troubleshooting
            }
        }
    }
}