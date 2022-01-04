using JukaNetwork;
using Newtonsoft.Json;
using System.Xml;


//Based on Henry He code
class Program
{
    static void Main(string[] args)
    {

        var startTime = DateTime.Now;

        Blockchain JukaToken = new Blockchain();
        JukaToken.AddBlock(new Block(DateTime.Now, null, "{sender:Person1,receiver:Person2,amount:10}"));
        JukaToken.AddBlock(new Block(DateTime.Now, null, "{sender:Person1,receiver:Person2,amount:5}"));
        JukaToken.AddBlock(new Block(DateTime.Now, null, "{sender:Person1,receiver:Person2,amount:5}"));

        Console.WriteLine(JsonConvert.SerializeObject(JukaToken, Newtonsoft.Json.Formatting.Indented));


        Console.WriteLine($"Is Chain Valid: {JukaToken.IsValid()}");

        Console.WriteLine($"Updating amount received to 1000");
        JukaToken.Chain[1].Data = "{sender:Person1,receiver:Person2,amount:1000}";

        Console.WriteLine($"Is Chain Valid: {JukaToken.IsValid()}");


        //Re-calculate a new valid chain:
        Console.WriteLine($"Update the entire chain");
        JukaToken.Chain[2].PreviousHash = JukaToken.Chain[1].Hash;
        JukaToken.Chain[2].Hash = JukaToken.Chain[2].CalculateHash();
        JukaToken.Chain[3].PreviousHash = JukaToken.Chain[2].Hash;
        JukaToken.Chain[3].Hash = JukaToken.Chain[3].CalculateHash();


        var endTime = DateTime.Now;

        Console.WriteLine($"Duration: {endTime - startTime}");
    }
}