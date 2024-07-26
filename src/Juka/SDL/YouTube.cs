using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

public class YouTubeApiService
{
    private string _apiKey;

    public YouTubeApiService(string apiKey)
    {
        _apiKey = apiKey;
    }

    public List<VideoInfo> GetTopVideosSync(string regionCode = "US")
    {
        var chart = "mostPopular";
        var url = $"https://www.googleapis.com/youtube/v3/videos?part=snippet&chart={chart}&regionCode={Uri.EscapeDataString(regionCode)}&maxResults=10&key={Uri.EscapeDataString(_apiKey)}";

        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

        try
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            using (var response = (HttpWebResponse)request.GetResponse())

            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream stream = response.GetResponseStream())

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var jsonResponse
 = reader.ReadToEnd();
                        var youtubeResponse = DeserializeYouTubeResponse(jsonResponse); // Needs implementation
                        return youtubeResponse.Items;
                    }
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.StatusDescription}");
                    return null; // Handle error or return a defaultvalue
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    private YouTubeResponse DeserializeYouTubeResponse(string json)
{
    var youtubeResponse = new YouTubeResponse { Items = new List<VideoInfo>() };

    using (JsonDocument doc = JsonDocument.Parse(json))
    {
        if (doc.RootElement.TryGetProperty("items", out JsonElement items))
        {
            foreach (JsonElement item in items.EnumerateArray())
            {
                //Console.WriteLine($"Item: {item}");

                var videoInfo = new VideoInfo();

                if (item.TryGetProperty("id", out JsonElement id))
                {
                    videoInfo.VideoId = id.GetString();
                    //Console.WriteLine($"Video ID: {videoInfo.VideoId}");
                }

                if (item.TryGetProperty("snippet", out JsonElement snippet))
                {
                    //Console.WriteLine($"Snippet: {snippet}");
                    if (snippet.TryGetProperty("title", out JsonElement title))
                    {
                        videoInfo.Title = title.GetString();
                        //Console.WriteLine($"Video Title: {videoInfo.Title}");
                    }
                    else
                    {
                        //Console.WriteLine("Title not found in snippet");
                    }
                }
                else
                {
                    //Console.WriteLine("Snippet not found");
                }

                youtubeResponse.Items.Add(videoInfo);
            }
        }
    }

    return youtubeResponse;
}

public class YouTubeResponse
{
    public List<VideoInfo> Items { get; set; }
}

public class VideoInfo
{
    public string VideoId { get; set; }
    public string Title { get; set; }
}
}
