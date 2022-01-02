using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text;
using System.Runtime.InteropServices;

namespace JukaAzureFunction
{
    public static class JukaAzureFunction
    {
        [FunctionName("JukaAzureFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Running code");

            string code = "";
            code = req.Query["code"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            code = code ?? data?.code;


            //Based on Josh Varty and Damir code
            var tree = CSharpSyntaxTree.ParseText(@"
            using System;

            namespace ConsoleApp1
            {
                public class Program
                {
                    static void Main(string[] args)
                    {
                        Console.WriteLine("""+code+@""");
                        //Console.ReadLine();
                    }
                }
            }
            ");


            var assemblyPath = Path.ChangeExtension("output", "exe");



            File.WriteAllText(
                Path.ChangeExtension(assemblyPath, "runtimeconfig.json"),
                GenerateRuntimeConfig()
            );

            var dotNetCoreDir = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var console = MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location);
            var myruntime = MetadataReference.CreateFromFile(Path.Combine(dotNetCoreDir, "System.Runtime.dll"));

            //We first have to choose what kind of output we're creating: DLL, .exe etc.
            var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication);
            options = options.WithAllowUnsafe(true);                                //Allow unsafe code;
            options = options.WithOptimizationLevel(OptimizationLevel.Debug);     //Set optimization level
            options = options.WithPlatform(Platform.X64);                           //Set platform

            var compilation = CSharpCompilation.Create("MyCompilation",
                syntaxTrees: new[] { tree },
                references: new[] { mscorlib, console, myruntime },
                options: options);

            //Emitting to file is available through an extension method in the Microsoft.CodeAnalysis namespace
            var emitResult = compilation.Emit(assemblyPath);



            Process process = new Process();
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = assemblyPath; // Note the /c command (*)
            //process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            //* Read the output (or the error)
            string output = process.StandardOutput.ReadToEnd();
            //Console.WriteLine(output);
            string err = process.StandardError.ReadToEnd();
            //Console.WriteLine(err);
            process.WaitForExit();

            var responseMessage = output + err;
            return new OkObjectResult(responseMessage);


        }
        private static string GenerateRuntimeConfig()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(
                    stream,
                    new JsonWriterOptions() { Indented = true }
                ))
                {
                    writer.WriteStartObject();
                    writer.WriteStartObject("runtimeOptions");
                    writer.WriteStartObject("framework");
                    writer.WriteString("name", "Microsoft.NETCore.App");
                    writer.WriteString("version",Environment.Version.ToString());
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }


    }

    
}
