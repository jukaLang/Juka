using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.Hosting;

namespace JukaAzureFunction;

/// <summary>
/// This code defines a Main method in the Program class. 
/// It creates a new IHost using HostBuilder to configure a Functions Worker with NewtonsoftJson and then runs the host.
/// </summary>
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