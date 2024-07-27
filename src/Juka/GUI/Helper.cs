using JukaCompiler;
using SDL2;
using System.Net.Sockets;
using System.Net;
using static Juka.GUI.Globals;

namespace Juka.GUI
{
    public static class Helper
    {
        public static bool HandleSelection(int x, int y, SDL.SDL_Rect rect)
        {
            return x >= rect.x && x < rect.x + rect.w &&
                   y >= rect.y && y < rect.y + rect.h;
        }

        public static List<nint> GenerateMenu(List<string> menuLines, nint menuFont, SDL.SDL_Color color)
        {
            return menuLines.Select(line =>
            {
                var surface = SDL_ttf.TTF_RenderText_Solid(menuFont, line, color);
                if (surface == nint.Zero)
                {
                    Console.WriteLine($"Failed to render text: {line}");
                    return nint.Zero;
                }
                return SDL.SDL_CreateTextureFromSurface(renderer, surface);
            }).ToList();
        }

        public static void generateProgram()
        {
            var output = "";
            try
            {
                Compiler? compiler = new Compiler();
                output = compiler.CompileJukaCode(filetoexecute, isFile: true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                output = "Something went wrong: " + e.ToString();
            }
            multioutput = new List<string>(output.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None)).Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();
        }

        public static void RenderText(string text, int x, int y, nint font, SDL.SDL_Color color)
        {
            if (text.Length > 0)
            {
                nint surface = SDL_ttf.TTF_RenderText_Solid(font, text, color);
                if (surface == nint.Zero)
                {
                    Console.WriteLine("Failed to render in rendetext! " + text);
                    return;
                }

                nint message = SDL.SDL_CreateTextureFromSurface(renderer, surface);

                // Get the text surface dimensions
                int w, h;
                SDL.SDL_QueryTexture(message, out _, out _, out w, out h);
                SDL.SDL_Rect dstrect = new SDL.SDL_Rect { x = x, y = y, w = w, h = h }; // Adjust position if needed

                SDL.SDL_RenderCopy(renderer, message, nint.Zero, ref dstrect);

                SDL.SDL_DestroyTexture(message);
                SDL.SDL_FreeSurface(surface);
            }
        }

        public static async Task GeneratePackages()
        {
            string librariesList = await Packages.getList();
            //packages
        }

        public static void GenerateMedia()
        {
            var apikey = "AIzaSyBzpZzE4nQVxr_EQLgWqTfREpvWON - gWu8";
            var youtube = new YouTubeApiService(apikey);
            var videos = youtube.GetTopVideosSync();

            videoInfos = videos.Select(v => new VideoInfo
            {
                VideoId = v.VideoId,
                Title = v.Title,
                Description = v.Description
            }).ToList();

        }


    }
}
