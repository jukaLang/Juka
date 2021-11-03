using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DreamCompiler.Scan;
using DreamCompiler.RoslynCompile;
using Microsoft.CodeAnalysis.CSharp;

namespace DreamAzureFunction
{
    public static class DreamAzureFunction
    {
        [FunctionName("DreamAzureFunction")]
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

            var script = @"using System;
  
            public class MyClass {
  
                // Main Method
                public static void Main()
                {
                    " + code + @"
                }
            }";


            // string responseMessage = new Compiler().Go("Jukacompile", script).ToString();
            string responseMessage = CompileRoslyn.CompileSyntaxTree(SyntaxFactory.ParseSyntaxTree(script, null, ""),"Juka").ToString();

            return new OkObjectResult(responseMessage);
        }
    }
}
