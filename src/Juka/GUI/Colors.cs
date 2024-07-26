using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juka.GUI
{
    public static class Colors
    {
        public static SDL.SDL_Color colorWhite = new SDL.SDL_Color() { a = 255, r=255, g=255, b=255 };
        public static SDL.SDL_Color colorBlack = new SDL.SDL_Color() { a = 255, r = 0, g = 0, b = 0 };
        public static SDL.SDL_Color colorRed = new SDL.SDL_Color() { a = 255, r = 255, g = 0, b = 0 };
    }
}
