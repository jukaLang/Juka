using JukaCompiler;
using Spectre.Console;
using System.Security.Cryptography;

namespace Juka;

/// <summary>
/// Represents the TerminalJuka.
/// </summary>
class TerminalJuka
{
    public static async Task Perform(string[] args)
    {
        string option = args[0];

        switch (option)
        {
            case "--encrypt":
                string sourceFile = args[1];
                string destFile = sourceFile + ".encrypt";

                AnsiConsole.MarkupLine($"[bold yellow]Encrypting File: {sourceFile} to {destFile}[/]");

                await using (FileStream sourceStream = File.OpenRead(sourceFile))
                await using (FileStream destStream = File.Create(destFile))
                using (Aes aesProvider = Aes.Create())
                using (ICryptoTransform encryptor = aesProvider.CreateEncryptor())
                await using (CryptoStream cryptoStream = new(destStream, encryptor, CryptoStreamMode.Write))
                {
                    destStream.Write(aesProvider.IV, 0, aesProvider.IV.Length);
                    await sourceStream.CopyToAsync(cryptoStream);
                    string key = Convert.ToBase64String(aesProvider.Key);
                    await File.WriteAllTextAsync(sourceFile + ".key", key);
                    Console.WriteLine(key);
                }
                break;

            case "--encrypted":
                string encryptedFile = args[1] + ".encrypt";
                string keyFile = args[1] + ".key";

                byte[] encryptedKey = Convert.FromBase64String(await File.ReadAllTextAsync(keyFile));

                string plaintext;

                await using (FileStream encStream = File.OpenRead(encryptedFile))
                using (Aes aes = Aes.Create())
                {
                    byte[] IV = new byte[aes.IV.Length];
                    encStream.Read(IV, 0, IV.Length);
                    using ICryptoTransform decryptor = aes.CreateDecryptor(encryptedKey, IV);
                    await using CryptoStream cryptoStream = new(encStream, decryptor, CryptoStreamMode.Read);
                    using StreamReader reader = new(cryptoStream);
                    plaintext = await reader.ReadToEndAsync();
                }
                Console.WriteLine(new Compiler().CompileJukaCode(plaintext, isFile: false));
                break;

            case "--decrypt":
                string encFile = args[1] + ".encrypt";
                string decFile = args[1];
                string deckeyFile = args[1] + ".key";

                byte[] decryptionKey = Convert.FromBase64String(await File.ReadAllTextAsync(deckeyFile));

                AnsiConsole.MarkupLine($"[bold yellow]Decrypting File: {encFile} with key: {deckeyFile}[/]");

                await using (FileStream encFileStream = File.OpenRead(encFile))
                await using (FileStream decFileStream = File.Create(decFile))
                using (Aes aesProvider = Aes.Create())
                {
                    byte[] IV = new byte[aesProvider.IV.Length];
                    encFileStream.Read(IV, 0, IV.Length);
                    using ICryptoTransform decryptor = aesProvider.CreateDecryptor(decryptionKey, IV);
                    await using CryptoStream cryptoStream = new(encFileStream, decryptor, CryptoStreamMode.Read);
                    await cryptoStream.CopyToAsync(decFileStream);
                }
                break;

            case "-d":
            case "--debug":
                DebugMe.DebugMode = 1;
                Console.WriteLine(new Compiler().CompileJukaCode(args[1], isFile: true));
                break;

            case "-v":
            case "--version":
                AnsiConsole.MarkupLine($"[bold yellow]Current Version:[/] {CurrentVersion.GetVersion()}");
                break;

            case "-su":
            case "--self-update":
                await SelfUpdate.Update();
                break;

            case "-i":
            case "--inline":
                DebugMe.DebugMode = 0;
                Console.WriteLine(new Compiler().CompileJukaCode(args[1], isFile: false));
                break;

            default:
                DebugMe.DebugMode = 1;
                Console.WriteLine(new Compiler().CompileJukaCode(args[0], isFile: true));
                break;
        }
    }

}
