using SDL2;
using static Juka.GUI.Globals;
using static Juka.GUI.Helper;

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
            RenderText("Videos: ", 160, 190, fontFooter, colorWhite);
            for (int i = 0; i < videoInfos.Count; i++)
            {

                RenderText(videoInfos[i].Title, 160, 190 + fontSizeC * (i + 1), fontFooter, colorWhite);
            }
            RenderText("Back", 160, 190 + fontSizeC * (videoInfos.Count + 1), fontFooter, colorWhite);



            // Iterate through menu options and render text
            for (int i = 0; i < videoInfos.Count + 1; ++i)
            {
                SDL.SDL_Rect textRect2 = new SDL.SDL_Rect { x = 160, y = 190 + fontSizeC * (i + 1) + 5, w = 1280 - 160, h = fontSizeC };
                if (HandleSelection(mouseX, mouseY, textRect2))
                {
                    SDL.SDL_SetRenderDrawColor(renderer, 255, 0, 0, 15); // Red highlight for selection
                    SDL.SDL_RenderDrawRect(renderer, ref textRect2);
                }
            }
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
