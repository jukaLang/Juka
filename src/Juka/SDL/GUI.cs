using SDL2;

namespace SDL2_Gui;

class Program
{
    //Screen dimension constants
    private const int SCREEN_WIDTH = 1280;
    private const int SCREEN_HEIGHT = 720;


    private const int width = 80; // Characters
    private const int height = 25; // Lines
    private const int fontSize = 64;
    private const int fontSizeC = 24;


    static IntPtr window = IntPtr.Zero;
    static IntPtr renderer = IntPtr.Zero;
    static IntPtr font = IntPtr.Zero;

    static List<string> menulines = new List<string>();
    static List<nint> menuOptionsTexture = new List<nint>();
    static int selectedOption = 0;
    static string openFont = "Arial.ttf";


    public static async Task GUI(string[] args)
    {


        //Create window

        window = SDL.SDL_CreateWindow("Juka Programming Language", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, SCREEN_WIDTH, SCREEN_HEIGHT, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
        if (window == IntPtr.Zero)
        {
            Console.WriteLine("Failed to create window: " + SDL.SDL_GetError());
            return;
        }
        renderer = SDL.SDL_CreateRenderer(window, -1,
                                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
        if (renderer == IntPtr.Zero)
        {
            Console.WriteLine("Failed to create renderer: " + SDL.SDL_GetError());
            return;
        }

        SDL_ttf.TTF_Init();

        nint fontBig = SDL_ttf.TTF_OpenFont(openFont, fontSize);
        nint fontFooter = SDL_ttf.TTF_OpenFont(openFont, fontSizeC);
        nint menufont = SDL_ttf.TTF_OpenFont(openFont, 20);

        SDL.SDL_Color colorWhite;
        colorWhite.r = colorWhite.g = colorWhite.b = colorWhite.a = 255;


        menulines = new List<string>() { "REPL", "Run REPL", "Run File", "Package Update", "Exit" };


        GenerateMenu(menufont, colorWhite);

        bool keeploop = true;

        // Main loop for handling input and rendering
        while (keeploop)
        {
            SDL.SDL_RenderClear(renderer);
            SDL.SDL_SetRenderDrawColor(renderer, 51, 52, 71, 255); // Juka background
            // Render terminal content (text and formatting)
            // Render elements
            RenderLogo();
            RenderText("Juka Programming Language", 160,10,fontBig,colorWhite);
            RenderText("Contribute to our project at https://github.com/jukaLang/juka", 130,650,fontFooter,colorWhite);
            RenderMenu();


            // Update the display
            SDL.SDL_Delay(16);
            SDL.SDL_RenderPresent(renderer);
        }  



        //Destroy window
        SDL.SDL_DestroyWindow(window);

        //Quit SDL subsystems
        SDL.SDL_Quit();

        return;
    }


    static void RenderLogo()
    {
        // Load the logo image
        IntPtr jukalogo = SDL_image.IMG_Load("jukalogo.png");
        if (jukalogo == IntPtr.Zero)
        {
            Console.WriteLine("Failed to load image!" + SDL_image.IMG_GetError());
            return;
        }

        // Define the destination rectangle for the logo on the screen
        SDL.SDL_Rect imageRect = new SDL.SDL_Rect { x = 10, y = 10, w = 128, h = 128 }; // Adjust position and size as needed

        // Create a texture from the image surface
        nint texture = SDL.SDL_CreateTextureFromSurface(renderer, jukalogo);

        // Render the logo onto the renderer
        SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref imageRect);

        // Free the surface and destroy the texture after rendering
        SDL.SDL_FreeSurface(jukalogo);
        SDL.SDL_DestroyTexture(texture);
    }

    static void RenderText(string text, int x, int y, nint font, SDL.SDL_Color color)
    {
        nint surface = SDL_ttf.TTF_RenderText_Solid(font, text, color);
        if (surface == IntPtr.Zero)
        {
            Console.WriteLine("Failed to render text!");
            return;
        }

        nint message = SDL.SDL_CreateTextureFromSurface(renderer, surface);

        // Get the text surface dimensions
        int w, h;
        SDL.SDL_QueryTexture(message, out _, out _, out w, out h);
        SDL.SDL_Rect dstrect = new SDL.SDL_Rect { x = x, y = y, w = w, h = h }; // Adjust position if needed

        SDL.SDL_RenderCopy(renderer, message, IntPtr.Zero, ref dstrect);

        SDL.SDL_DestroyTexture(message);
        SDL.SDL_FreeSurface(surface);
    }

    static void RenderMenu()
    {

        int textW, textH;

        SDL.SDL_QueryTexture(menuOptionsTexture[1], out _, out _, out textW, out textH);


        // Calculate menu option positions (centered horizontally)
        int optionX = (SCREEN_WIDTH - textW) / 2; // Placeholder for text width calculation
        int optionY = ((SCREEN_HEIGHT)/ (menulines.Count + 1));


        SDL.SDL_Rect menuBackgroundRect = new SDL.SDL_Rect { x = optionX, y = optionY, w = 300, h = (optionY *(menulines.Count-1)) };
        SDL.SDL_SetRenderDrawColor(renderer, 41, 42, 61, 255); // Juka background
        SDL.SDL_RenderFillRect(renderer, ref menuBackgroundRect);



        // Iterate through menu options and render text
        for (int i = 0; i < menulines.Count; ++i)
        {
            SDL.SDL_QueryTexture(menuOptionsTexture[i], out _, out _, out textW, out textH);
            SDL.SDL_Rect srcTextRect = new SDL.SDL_Rect { x = 0, y = 0, w = textW, h = textH };
            SDL.SDL_Rect textRect = new SDL.SDL_Rect { x = optionX, y = optionY * (i + 1), w = textW, h = textH };

            // Render menu option text
            SDL.SDL_RenderCopy(renderer, menuOptionsTexture[i], ref srcTextRect, ref textRect);

            // Highlight the selected option
            if (i == selectedOption)
            {

                SDL.SDL_Rect textRect2 = new SDL.SDL_Rect { x = optionX-4, y = optionY * (i + 1)-2, w = textW+8, h = textH+4 };

                SDL.SDL_SetRenderDrawColor(renderer, 255, 0, 0, 15); // Red highlight for selection
                SDL.SDL_RenderDrawRect(renderer, ref textRect2);
            }
            SDL.SDL_SetRenderDrawColor(renderer, 51, 52, 71, 255);
        }
    }


    static void GenerateMenu(nint menuFont, SDL.SDL_Color color)
    {
        List<nint> menuOptions = new List<nint>();

        for (var i = 0; i < menulines.Count(); i++)
        {
            menuOptions.Add(SDL_ttf.TTF_RenderText_Solid(menuFont, menulines[i], color));
        }

        for (var i = 0; i < menulines.Count(); i++)
        {
            menuOptionsTexture.Add(SDL.SDL_CreateTextureFromSurface(renderer, menuOptions[i]));
        }
    }


    static bool HandleSelection(int x, int y, SDL.SDL_Rect rect)
    {
       if (x >= rect.x && x < rect.x + rect.w &&
      y >= rect.y && y < rect.y + rect.h)
        {
            return true; // Clicked inside the rectangle
        }

        return false;
    }

}