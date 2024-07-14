using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.Hosting;

namespace JukaAzureFunction;

public class Program
{
    /// <summary>
    /// 
    /// </summary>
    public static void Main()
    {
        IHost host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults(worker => worker.UseNewtonsoftJson())
            .Build();

        host.Run();
    }
}