using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;


namespace JukaAzureFunction
{
    public class JukaAzureFunction
    {
        private readonly ILogger<JukaAzureFunction> _logger;

        public JukaAzureFunction(ILogger<JukaAzureFunction> log)
        {
            _logger = log;
        }


        [FunctionName("JukaAzureFunction")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "Pass Code as Get or Post", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "code", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Code** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("JukaAzureFunction trigger is running.");

            string code = req.Query["code"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            code ??= data?.code;

            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "DEBUG";
            if (assemblyVersion == "0.0.0.1")
            {
                assemblyVersion = "DEBUG";
            }
          

            if (string.IsNullOrEmpty(code))
            {
                return new OkObjectResult(new { output = "\"Welcome to Juka Azure Function version "+ assemblyVersion + "! To execute a program, send a GET or a POST request to \\\"/api/JukaAzureFunction/code=func main()={}\\\" \");\r\n\r\n" });
            }

            _logger.LogInformation("Running code: " + code);

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
}
