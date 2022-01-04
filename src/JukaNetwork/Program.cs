using JukaNetwork;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;


//Based on Henry He code
class Program
{
    static async Task Main(string[] args)
    {

        /*var startTime = DateTime.Now;

        Blockchain JukaToken = new Blockchain();
        JukaToken.CreateTransaction(new Transaction("Person1", "Person2", 10));
        JukaToken.ProcessPendingTransactions("Person3");
        Console.WriteLine(JsonConvert.SerializeObject(JukaToken, Formatting.Indented));


        var endTime = DateTime.Now;

        Console.WriteLine($"Duration: {endTime - startTime}");

        Console.WriteLine("=========================");
        Console.WriteLine($"P1: {JukaToken.GetBalance("Person1")}");
        Console.WriteLine($"P2: {JukaToken.GetBalance("Person2")}");
        Console.WriteLine($"P3: {JukaToken.GetBalance("Person3")}");

        Console.WriteLine("=========================");
        Console.WriteLine(JsonConvert.SerializeObject(JukaToken, Formatting.Indented));*/

        // Connect to known Good Servers and download

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("JukaNetwork", "1"));
        string contents = await httpClient.GetStringAsync("https://raw.githubusercontent.com/jukaLang/juka-network/main/knownservers.txt");

       
        string[] addressList = contents.Split(
            new string[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None
        );

        // Create a Listener
        try
        {
            var jukaHost = new TcpListener(IPAddress.Parse("127.0.0.1"), 48048);
            jukaHost.Start();
            Console.WriteLine("Server waiting for connections...");
            jukaHost.BeginAcceptTcpClient(
                new AsyncCallback(DoAcceptTcpClientCallback), jukaHost);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        foreach (string address in addressList)
        {
            if (address.Trim() != "" && !address.Trim().StartsWith("/"))
            {
                Console.WriteLine(address.Trim());

                // TODO: Send IP + Port to known server
            }
        }




    }

    // Process the client connection.
    public static void DoAcceptTcpClientCallback(IAsyncResult ar)
    {
        // Get the listener that handles the client request.
        TcpListener listener = (TcpListener) ar.AsyncState;

        // End the operation and display the received data on
        // the console.
        TcpClient client = listener.EndAcceptTcpClient(ar);

        // Process the connection here. (Add the client to a
        // server table, read data, etc.)
        Console.WriteLine("Client connected completed");

    }
}