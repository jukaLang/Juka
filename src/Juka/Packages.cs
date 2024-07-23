using Newtonsoft.Json.Linq;

namespace Juka
{
    class Packages
    {
        public static List<Package> mypackages = new List<Package>();
        public static async Task<string> getList()
        {
            string output = "";
            HttpClient? client = new()
            {
                BaseAddress = null,
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower
            };

            // Set up request headers
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Juka HTTPClient");

            // Make API request to get latest version
            HttpResponseMessage? response = await client.GetAsync("https://raw.githubusercontent.com/jukaLang/Packages/main/packages.json");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            JArray jsonArray = JArray.Parse(responseBody);


            foreach (JObject sobj in jsonArray)
            {
                Package package = new Package()
                {
                    Name = (string)sobj["name"],
                    Author = (string)sobj["author"],
                    Description = (string)sobj["description"],
                    Date = (string)sobj["date"],
                    Version = (string)sobj["version"],
                    Dependencies = sobj["dependencies"].ToObject<List<string>>(),
                    Mainfilename = (string)sobj["mainfilename"],
                    Download = (string)sobj["download"],
                    Device = sobj["device"].ToObject<List<string>>()
                };

                mypackages.Add(package);
            }


            foreach (var package in mypackages)
            {
                output += $"Name: {package.Name}\n";
                output += $"Author: {package.Author}\n";
                output += $"Description: {package.Description}\n";
                output += $"Date: {package.Date}\n";
                output += $"Version: {package.Version}\n";
                output += $"Dependencies: {string.Join(", ", package.Dependencies)}\n";
                output += $"Mainfilename: {package.Mainfilename}\n";
                output += $"Download: {package.Download}\n";
                output += $"Device: {string.Join(", ", package.Device)}\n";
                output += "---------\n";
            }

            output += "Type !!get -help for more information.";


            return output;

        }
    }

    public class Package
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Version { get; set; }
        public List<string> Dependencies { get; set; }
        public string Mainfilename { get; set; }
        public string Download { get; set; }
        public List<string> Device { get; set; }
    }

}
