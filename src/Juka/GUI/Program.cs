using SDL2;
using static Juka.GUI.Globals;
using static Juka.GUI.Helper;
using static Juka.GUI.Renderer;
using static SDL2.SDL;
using static Juka.GUI.VirtualKeyboard;
namespace Juka.GUI;


class Program
{
    public static Task GUI(string[] args)
    {
        //Create window
        if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_GAMECONTROLLER | SDL.SDL_INIT_HAPTIC | SDL.SDL_INIT_JOYSTICK) < 0)
        {
            Console.WriteLine("SDL could not initialize! SDL_Error: " + SDL.SDL_GetError());
        }
        window = SDL.SDL_CreateWindow("Juka Programming Language", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, SCREEN_WIDTH, SCREEN_HEIGHT, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
        if (window == nint.Zero)
        {
            Console.WriteLine("Failed to create window: " + SDL.SDL_GetError());
            return Task.CompletedTask;
        }
        renderer = SDL.SDL_CreateRenderer(window, -1,
                                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
        if (renderer == nint.Zero)
        {
            Console.WriteLine("Failed to create renderer: " + SDL.SDL_GetError());
            return Task.CompletedTask;
        }

        try
        {

            string mapping = "030000005e0400008e02000014010000,Xbox 360 Controller,a:b0,b:b1,x:b2,y:b3,back:b6,guide:b8,start:b7,leftstick:b9,rightstick:b10,leftshoulder:b4,rightshoulder:b5,dpup:h0.1,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:a4,righttrigger:a5,";
            SDL.SDL_GameControllerAddMapping(mapping);
            for (int i = 0; i < SDL.SDL_NumJoysticks(); i++)
            {
                if (SDL.SDL_IsGameController(i) == SDL.SDL_bool.SDL_TRUE)
                {
                    controller = SDL.SDL_GameControllerOpen(i);
                    if (controller != nint.Zero)
                    {

                        SDL_HapticEffect effect = new SDL_HapticEffect()
                        {
                            type = SDL.SDL_HAPTIC_SINE,
                            constant = new SDL_HapticConstant() { level = 0, length = 0x4000, attack_length = 1000 }
                        };
                        JoyHaptic = SDL.SDL_HapticOpenFromJoystick(i);
                        if (JoyHaptic == nint.Zero)
                        {
                            // Joystick isn't haptic
                            Console.WriteLine("Joystick isn't haptic");
                        }
                        effectID = SDL.SDL_HapticNewEffect(JoyHaptic, ref effect);
                        SDL.SDL_HapticRunEffect(JoyHaptic, effectID, 1);

                        Console.WriteLine("Controller opened successfully!");
                        if (SDL.SDL_GameControllerHasRumbleTriggers(controller) == SDL.SDL_bool.SDL_TRUE)
                        {
                            rumble2 = true;
                        }
                        if (SDL.SDL_GameControllerHasRumble(controller) == SDL.SDL_bool.SDL_TRUE)
                        {
                            rumble = true;
                            SDL.SDL_GameControllerRumbleTriggers(controller, 0xffff, 0xffff, 8000);
                            SDL.SDL_GameControllerRumble(controller, 0xffff, 0xffff, 8000);
                            Console.WriteLine("Rumble ON");
                        }
                        else
                        {
                            Console.WriteLine("Rumble OFF");
                        }
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Could not open gamecontroller! SDL_Error: " + SDL.SDL_GetError());
                    }
                }
                else
                {
                    Console.WriteLine("Controller not found!");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        SDL_ttf.TTF_Init();

        fontBig = SDL_ttf.TTF_OpenFont(openFont, fontSize);
        fontFooter = SDL_ttf.TTF_OpenFont(openFont, fontSizeC);
        menufont = SDL_ttf.TTF_OpenFont(openFont, 20);
        descriptionfont = SDL_ttf.TTF_OpenFont(openFont, 10);

        menuOptions[Menus.MenuMain] = new List<string>() { "Run File", "Update/Install", "Media Downloader", "Exit" };
        menuTextures[Menus.MenuMain] = Helper.GenerateMenu(menuOptions[Menus.MenuMain], menufont, Colors.colorWhite);

        menuOptions[Menus.MenuInstall] = new List<string>() { "Update Juka", "Update/Install Packages", "Back" };
        menuTextures[Menus.MenuInstall] = Helper.GenerateMenu(menuOptions[Menus.MenuInstall], menufont, Colors.colorWhite);

        menuOptions[Menus.MenuInstallJuka] = new List<string>() { "Install", "Back" };
        menuTextures[Menus.MenuInstallJuka] = Helper.GenerateMenu(menuOptions[Menus.MenuInstallJuka], menufont, Colors.colorWhite);


        //Juka Run
        menuOptions[Menus.MenuJuka] = new List<string>();
        try
        {
            string[] files = Directory.GetFiles(".", "*.juk", SearchOption.AllDirectories);
            menuOptions[Menus.MenuJuka].AddRange(files);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
        menuOptions[Menus.MenuJuka].Add("Back");
        menuTextures[Menus.MenuJuka] = Helper.GenerateMenu(menuOptions[Menus.MenuJuka], menufont, Colors.colorWhite);
        //


        // Main loop for handling input and rendering
        while (!quit)
        {
            SDL.SDL_RenderClear(renderer);
            SDL.SDL_SetRenderDrawColor(renderer, 51, 52, 71, 255); // Juka background

            // Render terminal content (text and formatting)
            // Render elements
            RenderLogo();
            RenderText("Juka Programming Language", 160, 10, fontBig, Colors.colorWhite);
            RenderText("Contribute to our project at https://github.com/jukaLang/juka", 50, 650, fontFooter, Colors.colorWhite);
            RenderText("Vesion: " + version, 1050, 650, fontFooter, Colors.colorWhite);
            RenderText("Key Pressed: " + keypressed, 1050, 620, fontFooter, Colors.colorWhite);

            if (currentscreen == Menus.MenuMain)
            {

                var infoOffsetX = 820;
                var infoOffsetY = 250;
                var infoOffsetYr = 30;
                RenderText("System: ", infoOffsetX, infoOffsetY + infoOffsetYr * 0, menufont, Colors.colorWhite);
                RenderText("OS: " + systemInfo["fullplatform"], infoOffsetX, infoOffsetY + infoOffsetYr * 1, menufont, Colors.colorWhite);
                RenderText("Directory: " + systemInfo["dir"], infoOffsetX, infoOffsetY + infoOffsetYr * 2, menufont, Colors.colorWhite);
                RenderText("Architecture: " + systemInfo["architecture"], infoOffsetX, infoOffsetY + infoOffsetYr * 3, menufont, Colors.colorWhite);
                RenderText("Assembly Name: " + systemInfo["name"], infoOffsetX, infoOffsetY + infoOffsetYr * 4, menufont, Colors.colorWhite);
                RenderText("Extension: " + systemInfo["extension"], infoOffsetX, infoOffsetY + infoOffsetYr * 5, menufont, Colors.colorWhite);
                RenderMenu();

            }
            else if (currentscreen == Menus.MenuJuka)
            {
                RenderRun();
            }
            else if (currentscreen == Menus.RanJuka)
            {
                RenderText("Click any button to go back", 160, 150, fontFooter, Colors.colorRed);
                RenderText("Output: ", 160, 190, fontFooter, Colors.colorWhite);
                for (int i = 0; i < multioutput.Count; i++)
                {
                    RenderText(multioutput[i], 160, 190 + fontSizeC * (i + 1), fontFooter, Colors.colorWhite);
                }
            }
            else if (currentscreen == Menus.MediaPlayer)
            {
                RenderMedia(fontFooter, Colors.colorWhite, Colors.colorRed);
            }
            else if (currentscreen == Menus.MediaDownloaded)
            {
                MediaStreamer(fontFooter, Colors.colorWhite, Colors.colorRed);
            }
            else if (currentscreen == Menus.MenuInstall)
            {
                RenderMenuInstall();
            }
            else if (currentscreen == Menus.MenuInstallJuka)
            {
                RenderMenuJukaInstall();
            }

            if (keyboardOn)
            {
                RenderKeyboard();
            }
            Controller.MouseHandler();
            SDL.SDL_Rect textRect2 = new SDL.SDL_Rect { x = mouseX, y = mouseY, w = 12, h = 12 };
            SDL.SDL_SetRenderDrawColor(renderer, 255, 0, 0, 15); // Mouse Simulation if Controller is used
            SDL.SDL_RenderFillRect(renderer, ref textRect2);

            SDL.SDL_SetRenderDrawColor(renderer, 51, 52, 71, 255);
            // Update the display
            SDL.SDL_RenderPresent(renderer);
        }


        //Destroy window
        SDL.SDL_DestroyWindow(window);


        //Quit SDL subsystems
        SDL.SDL_Quit();
        Environment.Exit(0);
        return Task.CompletedTask;
    }
}