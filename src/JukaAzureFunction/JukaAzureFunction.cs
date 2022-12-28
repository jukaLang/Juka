using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using AzureFunctions.Extensions.Swashbuckle;
using System.Net.Http;

namespace JukaAzureFunction
{
    public static class JukaAzureFunction
    {
        [FunctionName("JukaAzureFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            string code = req.Query["code"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            code ??= data?.code;

            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "DEBUG";
            if (assemblyVersion == "0.0.0.1") assemblyVersion = "DEBUG";
          

            if (code == null || code == "")
            {
                return new OkObjectResult(new { output = "\"Welcome to Juka Azure Function version \"+ assemblyVersion + \"! To execute a program, send a GET or a POST request to \\\"/api/JukaAzureFunction/code=func main()={}\\\" \");\r\n\r\n" });
            }

            log.LogInformation("Running code: " + code);

            JukaCompiler.Compiler compiler = new();

            var outputValue = compiler.Go(code, false);

            if (compiler.HasErrors())
            {
                var errors = string.Join(Environment.NewLine, compiler.ListErrors());
                return new OkObjectResult(new { errors, original = code });
            }
            return new OkObjectResult(new { output = outputValue, original = code });
        }
    }

    public static class SwaggerFunctions
    {
        [SwaggerIgnore]
        [FunctionName("Swagger")]
        public static Task<HttpResponseMessage> Swagger(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger/json")] HttpRequestMessage req,
                [SwashBuckleClient] ISwashBuckleClient swasBuckleClient)
        {
            return Task.FromResult(swasBuckleClient.CreateSwaggerJsonDocumentResponse(req));
        }
        [SwaggerIgnore]
        [FunctionName("SwaggerUI")]
        public static Task<HttpResponseMessage> SwaggerUI(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger/ui")] HttpRequestMessage req,
        [SwashBuckleClient] ISwashBuckleClient swasBuckleClient)
        {
            return Task.FromResult(swasBuckleClient.CreateSwaggerUIResponse(req, "swagger/json"));
        }
    }
}
