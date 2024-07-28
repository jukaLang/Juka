using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Juka.GUI.Globals;

namespace Juka.GUI
{
    public class YouTubeApiService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly string _tempDirectory;

        public YouTubeApiService(string apiKey)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _httpClient = new HttpClient();
            _tempDirectory = Path.Combine(Directory.GetCurrentDirectory(), "temp");
            InitializeTempDirectory();
        }

        private void InitializeTempDirectory()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
            Directory.CreateDirectory(_tempDirectory);
        }

        public async Task<List<VideoInfo>> GetTopVideosAsync(string regionCode = "US")
        {
            var chart = "mostPopular";
            var url = $"https://www.googleapis.com/youtube/v3/videos?part=snippet&chart={chart}&regionCode={Uri.EscapeDataString(regionCode)}&maxResults=4&key={Uri.EscapeDataString(_apiKey)}";

            if (!string.IsNullOrEmpty(keyboardbuffer))
            {
                var encodedSearchTerm = Uri.EscapeDataString(keyboardbuffer);
                url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q={encodedSearchTerm}&regionCode={Uri.EscapeDataString(regionCode)}&maxResults=4&key={Uri.EscapeDataString(_apiKey)}";
            }

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var youtubeResponse = DeserializeYouTubeResponse(jsonResponse);

                    await DownloadThumbnailsAsync(youtubeResponse.Items);
                    return youtubeResponse.Items;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        private async Task DownloadThumbnailsAsync(List<VideoInfo> videos)
        {
            var tasks = new List<Task>();

            foreach (var video in videos)
            {
                var sanitizedTitle = SanitizeFileName(video.Title);
                var filePath = Path.Combine(_tempDirectory, $"{sanitizedTitle}.jpg");
                if (!File.Exists(filePath))
                {
                    tasks.Add(DownloadImageAsync($"https://img.youtube.com/vi/{video.VideoId}/0.jpg", filePath));
                }
            }

            await Task.WhenAll(tasks);
        }

        public static string SanitizeFileName(string title)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitizedTitle = new string(title.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
            return sanitizedTitle.Length > 25 ? sanitizedTitle.Substring(0, 25) : sanitizedTitle;
        }

        public void DeleteThumbnails(List<VideoInfo> videos)
        {
            foreach (var video in videos)
            {
                var sanitizedTitle = SanitizeFileName(video.Title);
                var filePath = Path.Combine(_tempDirectory, $"{sanitizedTitle}.jpg");
                if (File.Exists(filePath))
                {
                    DeleteImage(filePath);
                }
            }
        }

        public bool DeleteImage(string filePath)
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

        private async Task<bool> DownloadImageAsync(string url, string filePath)
        {
            try
            {
                var imageBytes = await _httpClient.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(filePath, imageBytes);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading thumbnail for {url}: {ex.Message}");
                return false;
            }
        }

        private YouTubeResponse DeserializeYouTubeResponse(string json)
        {
            return !string.IsNullOrEmpty(keyboardbuffer) ? DeserializeSearchResponse(json) : DeserializeVideoResponse(json);
        }

        private YouTubeResponse DeserializeVideoResponse(string json)
        {
            var youtubeResponse = new YouTubeResponse { Items = new List<VideoInfo>() };

            using (var doc = JsonDocument.Parse(json))
            {
                if (doc.RootElement.TryGetProperty("items", out var items))
                {
                    foreach (var item in items.EnumerateArray())
                    {
                        var videoInfo = new VideoInfo
                        {
                            VideoId = item.GetProperty("id").GetString()
                        };

                        if (item.TryGetProperty("snippet", out var snippet))
                        {
                            videoInfo.Title = snippet.GetProperty("title").GetString();
                            videoInfo.Description = snippet.GetProperty("description").GetString();
                            videoInfo.Published = snippet.GetProperty("publishedAt").GetString();
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

            using (var doc = JsonDocument.Parse(json))
            {
                if (doc.RootElement.TryGetProperty("items", out var items))
                {
                    foreach (var item in items.EnumerateArray())
                    {
                        var videoInfo = new VideoInfo();

                        if (item.TryGetProperty("id", out var idElement) && idElement.TryGetProperty("videoId", out var videoIdElement))
                        {
                            videoInfo.VideoId = videoIdElement.GetString();
                        }

                        if (item.TryGetProperty("snippet", out var snippet))
                        {
                            videoInfo.Title = snippet.GetProperty("title").GetString();
                            videoInfo.Description = snippet.GetProperty("description").GetString();
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
    }
}
