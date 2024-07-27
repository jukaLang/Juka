using SDL2;
using VideoLibrary;
using static Juka.GUI.Globals;

namespace Juka.GUI
{
    public class Clicked
    {

        public static void itemclicked()
        {

            if (currentscreen == Menus.MenuMain)
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
                for (int i = 0; i < videoInfos.Count + 1; i++)
                {
                    SDL.SDL_Rect optionRect = new SDL.SDL_Rect { x = 160, y = 190 + fontSizeC * (i + 1) + 5, w = 1280 - 160, h = fontSizeC };
                    if (Helper.HandleSelection(mouseX, mouseY, optionRect))
                    {
                        if (i == videoInfos.Count)
                        {
                            currentscreen = 0;
                        }
                        else
                        {

                            var VedioUrl = "https://www.youtube.com/embed/" + videoInfos[i].VideoId + ".mp4";
                            var youTube = YouTube.Default;
                            var video = youTube.GetVideo(VedioUrl);
                            File.WriteAllBytes(videoInfos[i].VideoId + ".mp4", video.GetBytes());

                            myfile = videoInfos[i].VideoId + ".mp4";

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
