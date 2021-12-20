using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

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

            string filePathReceipt = Path.GetTempFileName();
            File.WriteAllText(filePathReceipt, code.ToString());

            string fileOutput = Path.GetTempFileName();

            JukaCompiler.Compiler compiler = new JukaCompiler.Compiler();
            string responseMessage = "";
            /*try
            {
                compiler.Go(fileOutput, filePathReceipt);
                responseMessage = File.ReadAllText(fileOutput);
            }
            catch(Exception ex)
            {
                responseMessage = ex.ToString();
            }*/

            compiler.Go(fileOutput, filePathReceipt);
            responseMessage = File.ReadAllText(fileOutput);

            return new OkObjectResult(responseMessage);
        }
    }
}
