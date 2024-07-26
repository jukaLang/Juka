using SDL2;
using static Juka.GUI.Globals;
using static Juka.GUI.Helper;

namespace Juka.GUI
{
    public static class Renderer
    {
        public static void RenderMenu()
        {

            int textW, textH;

            SDL.SDL_QueryTexture(menuTextures[Menus.MenuMain][0], out _, out _, out textW, out textH);


            // Calculate menu option positions (centered horizontally)
            int optionX = (SCREEN_WIDTH - textW) / 2; // Placeholder for text width calculation
            int optionY = SCREEN_HEIGHT / (menuOptions[Menus.MenuMain].Count + 1);

            SDL.SDL_QueryTexture(menuTextures[Menus.MenuMain][0], out _, out _, out textW, out textH);
            SDL.SDL_Rect menuBackgroundRect = new SDL.SDL_Rect { x = optionX - textW / 2 - 150, y = optionY - 10, w = textW + 300, h = optionY * menuOptions[Menus.MenuMain].Count - 80 };
            SDL.SDL_SetRenderDrawColor(renderer, 41, 42, 61, 255); // Juka background
            SDL.SDL_RenderFillRect(renderer, ref menuBackgroundRect);

            var textWtemp = textW;

            // Iterate through menu options and render text
            for (int i = 0; i < menuOptions[Menus.MenuMain].Count; ++i)
            {
                SDL.SDL_QueryTexture(menuTextures[Menus.MenuMain][i], out _, out _, out textW, out textH);
                SDL.SDL_Rect srcTextRect = new SDL.SDL_Rect { x = 0, y = 0, w = textW, h = textH };
                SDL.SDL_Rect textRect = new SDL.SDL_Rect { x = optionX - textW / 2, y = optionY * (i + 1), w = textW, h = textH };

                // Render menu option text
                SDL.SDL_RenderCopy(renderer, menuTextures[Menus.MenuMain][i], ref srcTextRect, ref textRect);

                boxX = optionX - textWtemp / 2 - 150;
                boxY = optionY;
                boxW = textWtemp + 300;
                boxH = textH + 20;
                SDL.SDL_Rect textRect2 = new SDL.SDL_Rect { x = boxX, y = boxY * (i + 1) - 10, w = boxW, h = boxH };
                if (HandleSelection(mouseX, mouseY, textRect2))
                {
                    SDL.SDL_SetRenderDrawColor(renderer, 255, 0, 0, 15); // Red highlight for selection
                    SDL.SDL_RenderDrawRect(renderer, ref textRect2);
                }
            }
        }



        public static void RenderRun()
        {

            int textW, textH;

            SDL.SDL_QueryTexture(menuTextures[Menus.MenuJuka][0], out _, out _, out textW, out textH);


            // Calculate menu option positions (centered horizontally)
            int optionX = (SCREEN_WIDTH - textW) / 2; // Placeholder for text width calculation
            int optionY = SCREEN_HEIGHT / (menuOptions[Menus.MenuJuka].Count + 1);

            SDL.SDL_QueryTexture(menuTextures[Menus.MenuJuka][0], out _, out _, out textW, out textH);
            SDL.SDL_Rect menuBackgroundRect = new SDL.SDL_Rect { x = optionX - textW / 2 - 150, y = optionY - 10, w = textW + 300, h = optionY * menuOptions[Menus.MenuMain].Count - 80 };
            SDL.SDL_SetRenderDrawColor(renderer, 41, 42, 61, 255); // Juka background
            SDL.SDL_RenderFillRect(renderer, ref menuBackgroundRect);

            var textWtemp = textW;

            // Iterate through menu options and render text
            for (int i = 0; i < menuOptions[Menus.MenuJuka].Count; ++i)
            {
                SDL.SDL_QueryTexture(menuTextures[Menus.MenuJuka][i], out _, out _, out textW, out textH);
                SDL.SDL_Rect srcTextRect = new SDL.SDL_Rect { x = 0, y = 0, w = textW, h = textH };
                SDL.SDL_Rect textRect = new SDL.SDL_Rect { x = optionX - textW / 2, y = optionY * (i + 1), w = textW, h = textH };

                // Render menu option text
                SDL.SDL_RenderCopy(renderer, menuTextures[Menus.MenuJuka][i], ref srcTextRect, ref textRect);

                boxX = optionX - textWtemp / 2 - 150;
                boxY = optionY;
                boxW = textWtemp + 300;
                boxH = textH + 20;
                SDL.SDL_Rect textRect2 = new SDL.SDL_Rect { x = boxX, y = boxY * (i + 1) - 10, w = boxW, h = boxH };
                if (HandleSelection(mouseX, mouseY, textRect2))
                {
                    SDL.SDL_SetRenderDrawColor(renderer, 255, 0, 0, 15); // Red highlight for selection
                    SDL.SDL_RenderDrawRect(renderer, ref textRect2);
                }
            }
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
