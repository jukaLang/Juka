using System.Net;
using System.Text.Json;
using static Juka.GUI.Globals;

namespace Juka.GUI;

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
        var url = $"https://www.googleapis.com/youtube/v3/videos?part=snippet&chart={chart}&regionCode={Uri.EscapeDataString(regionCode)}&maxResults=4&key={Uri.EscapeDataString(_apiKey)}";

        if (keyboardbuffer.Length > 0)
        {
            var encodedSearchTerm = Uri.EscapeDataString(keyboardbuffer);
            url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q={encodedSearchTerm}&regionCode={Uri.EscapeDataString(regionCode)}&maxResults=4&key={Uri.EscapeDataString(_apiKey)}";

        }


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
                        string jsonResponse
 = reader.ReadToEnd();

                        YouTubeResponse youtubeResponse = DeserializeYouTubeResponse(jsonResponse); // Needs implementation

                        DownloadThumbnails(youtubeResponse.Items);
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
    private void DownloadThumbnails(List<VideoInfo> videos)
    {
        foreach (var video in videos)
        {
            if (!File.Exists(video.VideoId + ".jpg"))
            {
                DownloadImage("https://img.youtube.com/vi/" + video.VideoId + "/0.jpg", video.VideoId + ".jpg");
            }
        }
    }

    public static void DeleteThumbnails(List<VideoInfo> videos)
    {
        foreach (var video in videos)
        {
            if (File.Exists(video.VideoId + ".jpg"))
            {
                DeleteImage(video.VideoId + ".jpg");
            }
        }
    }

    public static bool DeleteImage(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting image: {ex.Message}");
            return false;
        }
    }

    private bool DownloadImage(string url, string filePath)
    {
        try
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(url, filePath);
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading thumbnail for {url}: {ex.Message}");
            return false;
        }
    }

    private YouTubeResponse DeserializeYouTubeResponse(string json)
    {
        if(keyboardbuffer.Length > 0)
        {
            return DeserializeSearchResponse(json);
        }
        else
        {
            return DeserializeVideoResponse(json);
        }
    }

    private YouTubeResponse DeserializeVideoResponse(string json)
    {
        // Your existing deserialization logic for videos
        var youtubeResponse = new YouTubeResponse { Items = new List<VideoInfo>() };

        using (JsonDocument doc = JsonDocument.Parse(json))
        {
            if (doc.RootElement.TryGetProperty("items", out JsonElement items))
            {
                foreach (JsonElement item in items.EnumerateArray())
                {
                    var videoInfo = new VideoInfo();

                    if (item.TryGetProperty("id", out JsonElement id))
                    {
                        videoInfo.VideoId = id.GetString();
                    }

                    if (item.TryGetProperty("snippet", out JsonElement snippet))
                    {
                        if (snippet.TryGetProperty("title", out JsonElement title))
                        {
                            videoInfo.Title = title.GetString();
                        }

                        if (snippet.TryGetProperty("description", out JsonElement description))
                        {
                            videoInfo.Description = description.GetString();
                        }
                    }

                    youtubeResponse.Items.Add(videoInfo);
                }
            }
        }
        return youtubeResponse;
    }

    private YouTubeResponse DeserializeSearchResponse(string json)
    {
        var youtubeResponse = new YouTubeResponse { Items = new List<VideoInfo>() };

        try
        {
            using (var document = JsonDocument.Parse(json))
            {
                if (document.RootElement.TryGetProperty("items", out JsonElement items))
                {
                    foreach (var item in items.EnumerateArray())
                    {
                        var videoInfo = new VideoInfo();

                        if (item.TryGetProperty("id", out JsonElement idElement))
                        {
                            if (idElement.TryGetProperty("videoId", out JsonElement videoIdElement))
                            {
                                videoInfo.VideoId = videoIdElement.GetString();
                            }
                        }

                        if (item.TryGetProperty("snippet", out JsonElement snippet))
                        {
                            if (snippet.TryGetProperty("title", out JsonElement title))
                            {
                                videoInfo.Title = title.GetString();
                            }

                            if (snippet.TryGetProperty("description", out JsonElement description))
                            {
                                videoInfo.Description = description.GetString();
                            }


                        }

                        youtubeResponse.Items.Add(videoInfo);
                    }
                }
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error deserializing YouTube search response: {ex.Message}");
            // Handle the error appropriately
        }

        return youtubeResponse;
    }


    public class YouTubeResponse
    {
        public List<VideoInfo> Items { get; set; }
    }
}
