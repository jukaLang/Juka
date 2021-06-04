using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using DreamCompiler;

namespace DreamAzureFunction
{
    public static class DreamCompileFunction
    {
        [FunctionName("DreamCompileFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;


            if (string.IsNullOrEmpty(name))
            {
                return new OkObjectResult("The compiler is ready. Pass in the script into a name in the query string or in the request body for a response.");
            }
            

            string responseMessage = "";
            var memoryStream = GenerateStreamFromString(name);
            try
            {
                var x = new Compiler().Go("testcompile", memoryStream);
                //responseMessage = new Compiler().Go("testcompile", memoryStream).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new OkObjectResult(responseMessage);
        }


        private static MemoryStream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }


    }
}
