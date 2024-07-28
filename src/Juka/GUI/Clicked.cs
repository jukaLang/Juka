using SDL2;
using System.Diagnostics;
using VideoLibrary;
using static Juka.GUI.Globals;

namespace Juka.GUI
{
    public class Clicked
    {
        public static void PlayVideo(string filePath)
        {
            if (ffplayProcess == null || ffplayProcess.HasExited)
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "ffplay",
                    Arguments = $"-vf \"fps=30\" -fs -autoexit -preset ultrafast -maxrate 8000k -bufsize 48000k \"{filePath}\"",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                ffplayProcess = new Process { StartInfo = processStartInfo };
                ffplayProcess.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
                ffplayProcess.ErrorDataReceived += (sender, e) => Console.WriteLine($"ERROR: {e.Data}");

                ffplayProcess.Start();
                ffplayProcess.BeginOutputReadLine();
                ffplayProcess.BeginErrorReadLine();
                ffplayInput = ffplayProcess.StandardInput;
            }
        }

        public static void StopVideo()
        {
            if (ffplayProcess != null && !ffplayProcess.HasExited)
            {
                ffplayInput.WriteLine("q"); // Send 'q' to quit
                ffplayProcess.WaitForExit();
            }
        }

        public static void RewindVideo()
        {
            if (ffplayProcess != null && !ffplayProcess.HasExited)
            {
                ffplayInput.WriteLine("left"); // Send 'left' to rewind
            }
        }

        public static void PauseVideo()
        {
            if (ffplayProcess != null && !ffplayProcess.HasExited)
            {
                ffplayInput.WriteLine("p"); // Send 'p' to pause
            }
        }

        public static void ResumeVideo()
        {
            if (ffplayProcess != null && !ffplayProcess.HasExited)
            {
                ffplayInput.WriteLine("p"); // Send 'p' to resume (toggle pause)
            }
        }

        public static void itemclicked()
        {
            
            if (keyboardOn == true)
            {
                for (int i = 0; i < menuOptions[Menus.VirtualKeyboard].Count; i++)
                {
                    SDL.SDL_Rect keyRect = new SDL.SDL_Rect { x = menuDescript[Menus.VirtualKeyboard][i].X, y = menuDescript[Menus.VirtualKeyboard][i].Y, w = menuDescript[Menus.VirtualKeyboard][i].Width, h = menuDescript[Menus.VirtualKeyboard][i].Height };

                    if (Helper.HandleSelection(mouseX, mouseY, keyRect))
                    {
                        if (menuOptions[Menus.VirtualKeyboard][i] == "<-")
                        {
                            if (keyboardbuffer.Length > 0)
                            {
                                keyboardbuffer = keyboardbuffer.Substring(0, keyboardbuffer.Length - 1);
                            }
                        } else if(menuOptions[Menus.VirtualKeyboard][i] == "Ext")
                        {
                            keyboardOn = false;
                        }
                        else if (menuOptions[Menus.VirtualKeyboard][i] == "Ent")
                        {
                            if (Menus.MediaPlayer == currentscreen)
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
                            keyboardOn = false;
                        }
                        else
                        {
                            keyboardbuffer += menuOptions[Menus.VirtualKeyboard][i];
                        }
                    }
                }
            }

            else if (currentscreen == Menus.MenuMain)
            {
                for (int i = 0; i < menuOptions[Menus.MenuMain].Count; i++)
                {
                    SDL.SDL_Rect optionRect = new SDL.SDL_Rect { x = menuDescript[Menus.MenuMain][i].X, y = menuDescript[Menus.MenuMain][i].Y, w = menuDescript[Menus.MenuMain][i].Width, h = menuDescript[Menus.MenuMain][i].Height };
                    if (Helper.HandleSelection(mouseX, mouseY, optionRect))
                    {
                        if (menuOptions[Menus.MenuMain][i] == "Exit")
                        {
                            quit = true;
                        }
                        else if (menuOptions[Menus.MenuMain][i] == "Run File")
                        {
                            currentscreen = Menus.MenuJuka;
                        }
                        else if (menuOptions[Menus.MenuMain][i] == "Update/Install")
                        {
                            currentscreen = Menus.MenuInstall;
                        }
                        else if (menuOptions[Menus.MenuMain][i] == "Media Downloader")
                        {
                            Helper.GenerateMedia();
                            currentscreen = Menus.MediaPlayer;
                        }
                        else
                        {
                            Console.WriteLine("Selected option: " + menuOptions[Menus.MenuMain][i]);
                        }
                    }
                }
            }
            else if (currentscreen == Menus.MenuJuka)
            {
                for (int i = 0; i < menuOptions[Menus.MenuJuka].Count; i++)
                {
                    SDL.SDL_Rect optionRect = new SDL.SDL_Rect { x = menuDescript[Menus.MenuJuka][i].X, y = menuDescript[Menus.MenuJuka][i].Y, w = menuDescript[Menus.MenuJuka][i].Width, h = menuDescript[Menus.MenuJuka][i].Height };
                    if (Helper.HandleSelection(mouseX, mouseY, optionRect))
                    {
                        if (menuOptions[Menus.MenuJuka][i] == "Back")
                        {
                            currentscreen = 0;
                        }
                        else
                        {
                            filetoexecute = menuOptions[Menus.MenuJuka][i];
                            Helper.generateProgram();
                            currentscreen = Menus.RanJuka;
                        }
                    }
                }
            }
            else if (currentscreen == Menus.RanJuka)
            {
                currentscreen = Menus.MenuJuka;
            }
            else if (currentscreen == Menus.MediaPlayer)
            {
                for (int i = 0; i < videoInfos.Count + 2; i++)
                {
                    SDL.SDL_Rect optionRect = new SDL.SDL_Rect { x = menuDescript[Menus.MediaPlayer][i].X, y = menuDescript[Menus.MediaPlayer][i].Y, w = menuDescript[Menus.MediaPlayer][i].Width, h = menuDescript[Menus.MediaPlayer][i].Height };
                    if (Helper.HandleSelection(mouseX, mouseY, optionRect))
                    {
                        if (menuOptions[Menus.MediaPlayer][i] == "Back")
                        {
                            YouTubeApiService.DeleteThumbnails(videoInfos);
                            currentscreen = Menus.MenuMain;
                        }
                        else if(menuOptions[Menus.MediaPlayer][i] == "Search")
                        {
                            keyboardy = 159;
                            keyboardOn = true;
                        }
                        else
                        {
                            running = true;

                            if (!File.Exists(videoInfos[i].VideoId + ".mp4"))
                            {
                                var VedioUrl = "https://www.youtube.com/embed/" + menuOptions[Menus.MediaPlayer][i] + ".mp4";
                                var youTube = YouTube.Default;
                                var video = youTube.GetVideo(VedioUrl);
                                File.WriteAllBytes(videoInfos[i].VideoId + ".mp4", video.GetBytes());
                            }

                            myfile = videoInfos[i].VideoId + ".mp4";
                            try
                            {
                                var ffplayThread = new Thread(() => PlayVideo(myfile));
                                ffplayThread.Start();
                            }
                            catch (Exception e) { 
                                Console.WriteLine("Can't plaay video: "+e); 
                            }
                            currentscreen = Menus.MediaDownloaded;
                        }
                    }
                }
            }
            else if (currentscreen == Menus.MediaDownloaded)
            {
                currentscreen = Menus.MediaPlayer;
            }
            else if (currentscreen == Menus.MenuInstall)
            {
                for (int i = 0; i < menuOptions[Menus.MenuInstall].Count; i++)
                {
                    SDL.SDL_Rect optionRect = new SDL.SDL_Rect { x = menuDescript[Menus.MenuInstall][i].X, y = menuDescript[Menus.MenuInstall][i].Y, w = menuDescript[Menus.MenuInstall][i].Width, h = menuDescript[Menus.MenuInstall][i].Height };
                    if (Helper.HandleSelection(mouseX, mouseY, optionRect))
                    {
                        if (menuOptions[Menus.MenuInstall][i] == "Back")
                        {
                            currentscreen = Menus.MenuMain;
                        }
                        else if(menuOptions[Menus.MenuInstall][i] ==  "Update Juka")
                        {
                            currentscreen = Menus.MenuInstallJuka;
                        }
                    }
                }
            }
            else if (currentscreen == Menus.MenuInstallJuka)
            {
                for (int i = 0; i < menuOptions[Menus.MenuInstallJuka].Count; i++)
                {
                    SDL.SDL_Rect optionRect = new SDL.SDL_Rect { x = menuDescript[Menus.MenuInstallJuka][i].X, y = menuDescript[Menus.MenuInstallJuka][i].Y, w = menuDescript[Menus.MenuInstallJuka][i].Width, h = menuDescript[Menus.MenuInstallJuka][i].Height };
                    if (Helper.HandleSelection(mouseX, mouseY, optionRect))
                    {
                        if (menuOptions[Menus.MenuInstallJuka][i] == "Back")
                        {
                            currentscreen = Menus.MenuInstall;
                        }
                        else if (menuOptions[Menus.MenuInstallJuka][i] == "Install")
                        {
                            

                        }
                    }
                }
            }
        }
        }

}
