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

                using (var sourceStream = File.OpenRead(sourceFilename))
                using (var destinationStream = File.Create(destinationFilename))
                using (var provider = new AesCryptoServiceProvider())
                using (var cryptoTransform = provider.CreateEncryptor())
                using (var cryptoStream = new CryptoStream(destinationStream, cryptoTransform, CryptoStreamMode.Write))
                {
                    destinationStream.Write(provider.IV, 0, provider.IV.Length);
                    sourceStream.CopyTo(cryptoStream);
                    var mykey = System.Convert.ToBase64String(provider.Key);
                    File.WriteAllText(sourceFilename + ".key", mykey);
                    Console.WriteLine(mykey);
                }
                break;
            case "--decrypt":
                var encryptedFile = args[1]+".encrypt";
                var unencryptedFile = args[1];
                var keyfile = args[1] + ".key";

                var key = System.Convert.FromBase64String(File.ReadAllText(keyfile));

                AnsiConsole.MarkupLine($"[bold yellow]Decrypting File: " + encryptedFile + " with key: " + keyfile + "[/]");
                using (var sourceStream = File.OpenRead(encryptedFile))
                using (var destinationStream = File.Create(unencryptedFile))
                using (var provider = new AesCryptoServiceProvider())
                {
                    var IV = new byte[provider.IV.Length];
                    sourceStream.Read(IV, 0, IV.Length);
                    using (var cryptoTransform = provider.CreateDecryptor(key, IV))
                    using (var cryptoStream = new CryptoStream(sourceStream, cryptoTransform, CryptoStreamMode.Read))
                    {
                        cryptoStream.CopyTo(destinationStream);
                    }
                }
                break;
            case "-d":
            case "--debug":
                Console.WriteLine(new Compiler().Go(args[1], isFile: true,debug:1));
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
                Console.WriteLine(new Compiler().Go(args[1], isFile: false, debug: 0));
                break;
            }
            default:
                Console.WriteLine(new Compiler().Go(args[0], isFile: true,debug:0));
                break;
        }
    }
}