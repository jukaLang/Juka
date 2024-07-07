using JukaCompiler;
using Spectre.Console;
using System.Security.Cryptography;

namespace Juka;

class TerminalJuka
{
    public static async Task Perform(string[] args)
    {
        string userInput = args[0];

        switch (userInput)
        {
            case "--encrypt":
                var sourceFilename = args[1];
                var destinationFilename = args[1]+".encrypt";

                AnsiConsole.MarkupLine($"[bold yellow]Encrypting File: " + sourceFilename + " to "+ destinationFilename +" [/]");

                await using (var sourceStream = File.OpenRead(sourceFilename))
                await using (var destinationStream = File.Create(destinationFilename))
                using (var provider = Aes.Create())
                using (var cryptoTransform = provider.CreateEncryptor())
                await using (var cryptoStream = new CryptoStream(destinationStream, cryptoTransform, CryptoStreamMode.Write))
                {
                    destinationStream.Write(provider.IV, 0, provider.IV.Length);
                    await sourceStream.CopyToAsync(cryptoStream);
                    var mykey = Convert.ToBase64String(provider.Key);
                    await File.WriteAllTextAsync(sourceFilename + ".key", mykey);
                    Console.WriteLine(mykey);
                }
                break;
            case "--encrypted":
                var encFile = args[1] + ".encrypt";
                var encKeyfile = args[1] + ".key";

                var keyEncrypted = Convert.FromBase64String(await File.ReadAllTextAsync(encKeyfile));

                string? plainText;

                await using (var sourceStream = File.OpenRead(encFile))
                using (var provider = Aes.Create())
                {
                    var IV = new byte[provider.IV.Length];
                    sourceStream.Read(IV, 0, IV.Length);
                    using var cryptoTransform = provider.CreateDecryptor(keyEncrypted, IV);
                    await using var cryptoStream = new CryptoStream(sourceStream, cryptoTransform, CryptoStreamMode.Read);
                    using StreamReader reader = new(cryptoStream);
                    plainText = await reader.ReadToEndAsync();
                }
                DebugMe.DebugMode = 0;
                Console.WriteLine(new Compiler().Go(plainText, isFile: false));
                break;
            case "--decrypt":
                var encryptedFile = args[1]+".encrypt";
                var unencryptedFile = args[1];
                var keyfile = args[1] + ".key";

                var key = Convert.FromBase64String(await File.ReadAllTextAsync(keyfile));

                AnsiConsole.MarkupLine($"[bold yellow]Decrypting File: " + encryptedFile + " with key: " + keyfile + "[/]");
                await using (var sourceStream = File.OpenRead(encryptedFile))
                await using (var destinationStream = File.Create(unencryptedFile))
                using (var provider = Aes.Create())
                {
                    var IV = new byte[provider.IV.Length];
                    sourceStream.Read(IV, 0, IV.Length);
                    using ICryptoTransform cryptoTransform = provider.CreateDecryptor(key, IV);
                    await using var cryptoStream = new CryptoStream(sourceStream, cryptoTransform, CryptoStreamMode.Read);
                    await cryptoStream.CopyToAsync(destinationStream);
                }
                break;
            case "-d":
            case "--debug":
                DebugMe.DebugMode = 1;
                Console.WriteLine(new Compiler().Go(args[1], isFile: true));
                break;
            case "-v":
            case "--version":
            {
                AnsiConsole.MarkupLine($"[bold yellow]Current Version:[/] {CurrentVersion.Get()}");
                break;
            }
            case "-su":
            case "--self-update":
            {
                await SelfUpdate.Update();
                break;
            }
            case "-i":
            case "--inline":
            {
                DebugMe.DebugMode = 0;
                Console.WriteLine(new Compiler().Go(args[1], isFile: false));
                break;
            }
            default:
                DebugMe.DebugMode = 1;
                Console.WriteLine(new Compiler().Go(args[0], isFile: true));
                break;
        }
    }
}