using System.Net;
using System.Reflection;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using AzureFunctions.Extensions.Swashbuckle;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs;
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


        [SwaggerIgnore]
        [FunctionName("SwaggerUi")]
        public static Task<HttpResponseMessage> Run2(
            [Microsoft.Azure.Functions.Worker.HttpTrigger(AuthorizationLevel.Function, "get", Route = "Swagger/ui")] HttpRequestMessage req,
            [SwashBuckleClient] ISwashBuckleClient swashBuckleClient)
        {
            return Task.FromResult(swashBuckleClient.CreateSwaggerUIResponse(req, "swagger/json"));
        }


        [Function("JukaAzureFunction")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "Pass Code as Get or Post", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "code", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Code** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]

        public HttpResponseData Run([Microsoft.Azure.Functions.Worker.HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("JukaAzureFunction trigger is running.");

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var code = query["code"];


            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "DEBUG";
            if (assemblyVersion == "0.0.0.1")
            {
                assemblyVersion = "DEBUG";
            }

            if (string.IsNullOrEmpty(code))
            {
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.WriteString("Welcome to Juka Azure Function version \""+ assemblyVersion + "\"! To execute a program, send a GET or a POST request to \"/api/JukaAzureFunction/?code=func main()={}\"");
                return response;
            }

            _logger.LogInformation("Running code: " + code);

            JukaCompiler.Compiler compiler = new();

            var outputValue = compiler.Go(code, false);

            if (compiler.HasErrors())
            {

                var response2 = req.CreateResponse(HttpStatusCode.OK);
                response2.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                var errors = string.Join(Environment.NewLine, compiler.ListErrors());
                response2.WriteString(JsonConvert.SerializeObject(new { errors, original = code }));
                return response2;
            }

            var response3 = req.CreateResponse(HttpStatusCode.OK);
            response3.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response3.WriteString(JsonConvert.SerializeObject(new { output = outputValue, original = code }));
            return response3;
        }
    }
}
