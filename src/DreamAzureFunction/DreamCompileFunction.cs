using System;
using System.Net;
using DreamCompiler;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace DreamAzureFunction
{
    public static class DreamCompileFunction
    {
        [Function("DreamCompileFunction")]
        public static HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("DreamCompileFunction");
            logger.LogInformation("C# HTTP trigger function processed a request.");



            

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");


            var x = req.ReadAsString();

            response.WriteString("Welcome to Juka!\n");
            response.WriteString("Response:\n");

            var script = @"using System;
  
            public class MyClass {
  
                // Main Method
                public static void Main()
                {
                    " + x + @"
                }
            }";


            try
            {
                response.WriteString(new Compiler().Go("Jukacompile", script).ToString());
            }
            catch (Exception ex)
            {
                response.WriteString("FAILURE: " +ex);
            }


            return response;
        }
    }
}
