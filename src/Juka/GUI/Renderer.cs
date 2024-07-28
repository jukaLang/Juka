using SDL2;
using static Juka.GUI.Globals;
using static Juka.GUI.Helper;
using static Juka.GUI.Colors;
using System;

namespace Juka.GUI
{
    public static class Renderer
    {

        public static void RenderHelper(int width = 300, Menus menu = Menus.MenuMain)
        {
            int height = 500;
            SDL.SDL_Rect menuBackgroundRect = new SDL.SDL_Rect { x = (SCREEN_WIDTH / 2)-width/2, y = 130, w = width, h = height};
            SDL.SDL_SetRenderDrawColor(renderer, 41, 42, 61, 255); // Juka background
            SDL.SDL_RenderFillRect(renderer, ref menuBackgroundRect);

            menuDescript[menu] = new List<Descript<int>>();
            for (int i = 0; i < menuOptions[menu].Count; ++i)
            {
                int textW, textH;
                SDL.SDL_QueryTexture(menuTextures[menu][i], out _, out _, out textW, out textH);
                SDL.SDL_Rect srcTextRect = new SDL.SDL_Rect { x = 0, y = 0, w = textW, h = textH };

                int x = (SCREEN_WIDTH / 2) - textW / 2;
                int y = 130 + textH + (height / menuOptions[menu].Count) * (i);
                int w = textW;
                int h = textH;

                var menuOptionDescript = new Descript<int>
                {
                    X = (SCREEN_WIDTH / 2) - width / 2,
                    Y = y - 10,
                    Width = width,
                    Height = h + 20
                };
                menuDescript[menu].Add(menuOptionDescript);

                SDL.SDL_Rect textRect = new SDL.SDL_Rect { x = x, y = y, w = textW, h = textH };

                // Render menu option text
                SDL.SDL_RenderCopy(renderer, menuTextures[menu][i], ref srcTextRect, ref textRect);

                SDL.SDL_Rect textRect2 = new SDL.SDL_Rect { x = (SCREEN_WIDTH / 2) - width / 2, y = y - 10, w = width, h = h+20 };
                if (HandleSelection(mouseX, mouseY, textRect2))
                {
                    SDL.SDL_SetRenderDrawColor(renderer, 255, 0, 0, 15); // Red highlight for selection
                    SDL.SDL_RenderDrawRect(renderer, ref textRect2);
                }
            }
        }
        public static void RenderMenu()
        {
            RenderHelper(300, Menus.MenuMain);
        }


        public static void RenderRun()
        {
            RenderHelper(600, Menus.MenuJuka);
        }


        public static void RenderMenuInstall()
        {
            RenderHelper(300, Menus.MenuInstall);
        }

        public static void RenderMenuJukaInstall()
        {
            RenderHelper(300, Menus.MenuInstallJuka);
        }


        public static void MediaStreamer(nint fontFooter, SDL.SDL_Color colorWhite, SDL.SDL_Color colorRed)
        {
            RenderText("Dowloaded to Juka/" + myfile + ". Press any button to continue", 160, 190, fontFooter, colorWhite);
        }



        public static void RenderMedia(nint fontFooter, SDL.SDL_Color colorWhite, SDL.SDL_Color colorRed)
        {
            menuDescript[Menus.MediaPlayer] = new List<Descript<int>>();
            menuOptions[Menus.MediaPlayer] = new List<string>();


            for (int i = 0; i < videoInfos.Count; i++)
            {
                RenderYThumb(videoInfos[i].Title,videoInfos[i].Published, i);
                RenderText(videoInfos[i].Title, 160, 200 + (100 * i), menufont, colorWhite);
                var mydesc = videoInfos[i].Description;
                string[] lines = mydesc.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                for (int j = 0; j < Math.Min(lines.Length,4); j++)
                {
                    if (lines[j] != "")
                    {
                        RenderText(lines[j], 160, 200 + (100 * i) + 25 + (15 * j), descriptionfont, colorWhite);
                    }
                }
            }

            RenderText("Back", 160, 200 + (100 * videoInfos.Count), menufont, colorWhite);



            // Iterate through menu options and render text
            for (int i = 0; i < videoInfos.Count; ++i)
            {
                var menuOptionDescript = new Descript<int>
                {
                    X = 10,
                    Y = 200 + (100 * i),
                    Width = 1260,
                    Height = 72
                };
                menuDescript[Menus.MediaPlayer].Add(menuOptionDescript);
                menuOptions[Menus.MediaPlayer].Add(videoInfos[i].VideoId);

                SDL.SDL_Rect textRect2 = new SDL.SDL_Rect { x = 10, y = 200 + (100 * i), w = 1260, h = 72 };
                if (HandleSelection(mouseX, mouseY, textRect2))
                {

                    SDL.SDL_SetRenderDrawColor(renderer, 255, 0, 0, 15); // Red highlight for selection
                    SDL.SDL_RenderDrawRect(renderer, ref textRect2);
                }
            }





            SDL.SDL_Rect textRect3 = new SDL.SDL_Rect { x = 160, y = 204 + (100 * videoInfos.Count), w = 55, h = 24 };


            if (HandleSelection(mouseX, mouseY, textRect3))
            {
                SDL.SDL_SetRenderDrawColor(renderer, 255, 0, 0, 15); // Red highlight for selection
                SDL.SDL_RenderDrawRect(renderer, ref textRect3);
            }

            var menuOptionDescript2 = new Descript<int>
            {
                X = 160,
                Y = 204 + (100 * videoInfos.Count),
                Width = 55,
                Height = 24
            };
            menuDescript[Menus.MediaPlayer].Add(menuOptionDescript2);
            menuOptions[Menus.MediaPlayer].Add("Back");


            var menuOptionDescript3 = new Descript<int>
            {
                X = 430,
                Y = 122,
                Width = 400,
                Height = 32
            };
            menuDescript[Menus.MediaPlayer].Add(menuOptionDescript3);
            menuOptions[Menus.MediaPlayer].Add("Search");
            RenderText("Search: ", 300, 120, fontFooter, colorWhite);

            SDL.SDL_Rect searchRect = new SDL.SDL_Rect { x = 430, y = 122, w = 400, h = 32 };
            SDL.SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255); // Juka background
            SDL.SDL_RenderFillRect(renderer, ref searchRect);
            RenderText(keyboardbuffer,430,122,fontFooter, colorRed);
        }

        public static void RenderYThumb(string title, string published, int offset)
        {

            // Load the logo image
            nint ylogo = SDL_image.IMG_Load("temp/"+ YouTubeApiService.SanitizeFileName(title)+ ".jpg");
            if (ylogo == nint.Zero)
            {
                Console.WriteLine("Failed to load image!" + SDL_image.IMG_GetError());
                return;
            }

            // Define the destination rectangle for the logo on the screen
            SDL.SDL_Rect imageRect = new SDL.SDL_Rect { x = 10, y = 200+(100*offset), w = 128, h = 72 }; // Adjust position and size as needed

            // Create a texture from the image surface
            nint texture = SDL.SDL_CreateTextureFromSurface(renderer, ylogo);

            // Render the logo onto the renderer
            SDL.SDL_RenderCopy(renderer, texture, nint.Zero, ref imageRect);

            // Free the surface and destroy the texture after rendering
            SDL.SDL_FreeSurface(ylogo);
            SDL.SDL_DestroyTexture(texture);

            string[] parts = published.Split('T');
            string datePart = parts[0];
            RenderText(datePart, 10, 272 + (100 * offset), descriptionfont, colorWhite);
        }



        public static void RenderLogo()
        {
            // Load the logo image
            nint jukalogo = SDL_image.IMG_Load(logo);
            if (jukalogo == nint.Zero)
            {
                Console.WriteLine("Failed to load image!" + SDL_image.IMG_GetError());
                return;
            }

            // Define the destination rectangle for the logo on the screen
            SDL.SDL_Rect imageRect = new SDL.SDL_Rect { x = 10, y = 10, w = 128, h = 128 }; // Adjust position and size as needed

            // Create a texture from the image surface
            nint texture = SDL.SDL_CreateTextureFromSurface(renderer, jukalogo);

            // Render the logo onto the renderer
            SDL.SDL_RenderCopy(renderer, texture, nint.Zero, ref imageRect);

            // Free the surface and destroy the texture after rendering
            SDL.SDL_FreeSurface(jukalogo);
            SDL.SDL_DestroyTexture(texture);
        }
    }
}
