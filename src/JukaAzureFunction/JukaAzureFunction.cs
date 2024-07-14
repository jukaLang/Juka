using System.Net;
using System.Reflection;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace JukaAzureFunction;

public class JukaAzureFunction(ILogger<JukaAzureFunction> log)
{
    private ILogger<JukaAzureFunction> logger = log;

    [Function("JukaAzureFunction")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "Pass Code as Get or Post", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "code", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Enter the **Code**")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]

    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        logger.LogInformation("JukaAzureFunction trigger is running.");

        System.Collections.Specialized.NameValueCollection query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
        string? code = query["code"];


        string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "DEBUG";
        if (assemblyVersion == "0.0.0.1")
        {
            assemblyVersion = "DEBUG";
        }

        if (string.IsNullOrEmpty(code))
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString("Welcome to Juka Azure Function version \"" + assemblyVersion + "\"! To execute a program, send a GET or a POST request to \"/api/JukaAzureFunction/?code=func main()={}\". Go to /api/swagger/ui to see OpenAPI documentation");
            return response;
        }

        logger.LogInformation("Running code: " + code);

        JukaCompiler.Compiler compiler = new();

        string outputValue = compiler.CompileJukaCode(code, false);

        if (compiler.CheckForErrors())
        {

            HttpResponseData response2 = req.CreateResponse(HttpStatusCode.OK);
            response2.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            string errors = string.Join(Environment.NewLine, compiler.GetErrorList());
            response2.WriteString(JsonConvert.SerializeObject(new { errors, original = code }));
            return response2;
        }

        HttpResponseData response3 = req.CreateResponse(HttpStatusCode.OK);
        response3.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        response3.WriteString(JsonConvert.SerializeObject(new { output = outputValue, original = code }));
        return response3;
    }
}