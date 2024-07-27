using SDL2;
using static Juka.GUI.Globals;

namespace Juka.GUI
{
    public static class VirtualKeyboard
    {
        public static int keyWidth = 50;
        public static int keyHeight = 50;
        public static int keySpacing = 10;
        public static int xOffset = SCREEN_WIDTH/2 - (keyWidth*10/2 + keySpacing*9/2);
        public static int yOffset = keyboardy;

        public static List<string> qwerty = new List<string>
        {
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
            "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P",
            "A", "S", "D", "F", "G", "H", "J", "K", "L","Ent",
            "Z", "X", "C", "V", "B", "N", "M"," ","<-", "Ext"
        };



        public static void RenderKeyboard()
        {
            menuDescript[Menus.VirtualKeyboard] = new List<Descript<int>>();
            menuOptions[Menus.VirtualKeyboard] = new List<string>();

            // Calculate the total keyboard dimensions
            int numRows = (qwerty.Count + 9) / 10; // Round up to the nearest integer
            int totalWidth = 10 * keyWidth + 9 * keySpacing;
            int totalHeight = numRows * keyHeight + (numRows - 1) * keySpacing;

            // Create a background rectangle that encompasses the entire keyboard
            SDL.SDL_Rect backgroundRect = new SDL.SDL_Rect { x = xOffset-5, y = yOffset-5, w = totalWidth+10, h = totalHeight+10 };
            SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
            SDL.SDL_RenderFillRect(renderer, ref backgroundRect);




            for (int i = 0; i < qwerty.Count; i++)
            {
                int row = i / 10;
                int col = i % 10;
                int x = xOffset + col * (keyWidth + keySpacing);
                int y = yOffset + row * (keyHeight + keySpacing);
                SDL.SDL_Rect keyRect = new SDL.SDL_Rect { x = x, y = y, w = keyWidth, h = keyHeight };


                var menuOptionDescript = new Descript<int>
                {
                    X = x,
                    Y = y,
                    Width = keyWidth,
                    Height = keyHeight
                };
                menuDescript[Menus.VirtualKeyboard].Add(menuOptionDescript);
                menuOptions[Menus.VirtualKeyboard].Add(qwerty[i]);

                if (Helper.HandleSelection(mouseX, mouseY, keyRect))
                {
                    SDL.SDL_SetRenderDrawColor(renderer, 51, 52, 71, 255);
                    SDL.SDL_RenderFillRect(renderer, ref keyRect);
                    Helper.RenderText(qwerty[i], x + 10, y + 10, menufont, new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 });
                }
                else
                {
                    SDL.SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
                    SDL.SDL_RenderFillRect(renderer, ref keyRect);
                    Helper.RenderText(qwerty[i], x + 10, y + 10, menufont, new SDL.SDL_Color { r = 51, g = 52, b = 71, a = 255 });
                }
            }

        }
    }
}
