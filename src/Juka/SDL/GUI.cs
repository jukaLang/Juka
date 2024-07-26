using SDL2;
using JukaCompiler;
using Microsoft.CodeAnalysis;
using static SDL2.SDL;
using VideoLibrary;
using Spectre.Console;
using Juka;


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
    static string openFont = "open-sans.ttf";
    static string logo = "jukalogo.png";

    static List<string> jukalines = new List<string>();
    static List<nint> jukaOptionsTexture = new List<nint>();

    static int mouseX = 600;
    static int mouseY = 290;


    static bool quit = false;

    static int boxX = 0;
    static int boxY = 0;
    static int boxW = 0;
    static int boxH = 0;

    static int currentscreen = 0;
    static string filetoexecute = "";

    static List<string> multioutput = new List<string>();


    static List<Juka.SDL.VideoInfo> videoInfos = new List<Juka.SDL.VideoInfo>();
    static string myfile = "";

    static IntPtr controller;
    static bool rumble = false;

    static Dictionary<string, string> systemInfo = Juka.SelfUpdate.GetSystemInfo();
    static string version = Juka.CurrentVersion.GetVersion();

    static string keypressed = SDL.SDL_GetKeyName(SDL_Keycode.SDLK_UNDEFINED);

    public static Task GUI(string[] args)
    {
        //Create window
        //SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);
        if (SDL.SDL_Init(SDL.SDL_INIT_GAMECONTROLLER) < 0)
        {
            Console.WriteLine("SDL could not initialize! SDL_Error: " + SDL.SDL_GetError());
        }
        window = SDL.SDL_CreateWindow("Juka Programming Language", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, SCREEN_WIDTH, SCREEN_HEIGHT, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
        if (window == IntPtr.Zero)
        {
            Console.WriteLine("Failed to create window: " + SDL.SDL_GetError());
            return Task.CompletedTask;
        }
        renderer = SDL.SDL_CreateRenderer(window, -1,
                                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
        if (renderer == IntPtr.Zero)
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
                    if (controller != IntPtr.Zero)
                    {
                        Console.WriteLine("Controller opened successfully!");
                        SDL.SDL_GameControllerRumbleTriggers(controller, 0xffff, 0xffff, 8000);
                        SDL.SDL_GameControllerRumble(controller, 0xffff, 0xffff, 8000);
                        if (SDL.SDL_GameControllerHasRumble(controller) == SDL.SDL_bool.SDL_TRUE)
                        {
                            rumble = true;
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
        catch(Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        SDL_ttf.TTF_Init();

        nint fontBig = SDL_ttf.TTF_OpenFont(openFont, fontSize);
        nint fontFooter = SDL_ttf.TTF_OpenFont(openFont, fontSizeC);
        nint menufont = SDL_ttf.TTF_OpenFont(openFont, 20);

        SDL.SDL_Color colorWhite;
        colorWhite.r = colorWhite.g = colorWhite.b = colorWhite.a = 255;

        SDL.SDL_Color colorRed;
        colorRed.r = colorRed.a = 255;
        colorRed.g = colorRed.b = 0;


        menulines = new List<string>() { "Run File", "Update/Install", "Media Downloader", "Exit" };
        menuOptionsTexture = GenerateMenu(menulines, menufont, colorWhite);

        jukalines = new List<string>();
        try
        {
            string[] files = Directory.GetFiles(".", "*.juk", SearchOption.AllDirectories);
            jukalines.AddRange(files);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
        jukalines.Add("Back");
        jukaOptionsTexture = GenerateMenu(jukalines, menufont, colorWhite);

        // Main loop for handling input and rendering
        while (!quit)
        {
            SDL.SDL_RenderClear(renderer);
            SDL.SDL_SetRenderDrawColor(renderer, 51, 52, 71, 255); // Juka background

            // Render terminal content (text and formatting)
            // Render elements
            RenderLogo();
            RenderText("Juka Programming Language", 160, 10, fontBig, colorWhite);
            RenderText("Contribute to our project at https://github.com/jukaLang/juka", 50, 650, fontFooter, colorWhite);
            RenderText("Vesion: "+version, 1050, 650, fontFooter, colorWhite);
            RenderText("Key Pressed: " +keypressed, 1050, 620, fontFooter, colorWhite);



            if (currentscreen == 0)
            {

                var infoOffsetX = 800;
                var infoOffsetY = 300;
                var infoOffsetYr = 30;
                RenderText("System: ", infoOffsetX, infoOffsetY + infoOffsetYr*0, menufont, colorWhite);
                RenderText("OS: " + systemInfo["platform"], infoOffsetX, infoOffsetY + infoOffsetYr * 1, menufont, colorWhite);
                RenderText("Directory: " + systemInfo["dir"], infoOffsetX, infoOffsetY + infoOffsetYr * 2, menufont, colorWhite);
                RenderText("Architecture: " + systemInfo["architecture"], infoOffsetX, infoOffsetY + infoOffsetYr * 3, menufont, colorWhite);
                RenderText("Assembly Name: " + systemInfo["name"], infoOffsetX, infoOffsetY + infoOffsetYr * 4, menufont, colorWhite);
                RenderText("Extension: " + systemInfo["extension"], infoOffsetX, infoOffsetY + infoOffsetYr * 5, menufont, colorWhite);
                RenderMenu();

            }
            else if (currentscreen == 1)
            {
                RenderRun();
            }
            else if (currentscreen == 2)
            {
                RenderText("Click any button to go back", 160, 150, fontFooter, colorRed);
                RenderText("Output: ", 160, 190, fontFooter, colorWhite);
                for (int i = 0; i < multioutput.Count; i++)
                {
                    RenderText(multioutput[i], 160, 190 + fontSizeC * (i + 1), fontFooter, colorWhite);
                }
            }
            else if (currentscreen == 10)
            {
                RenderMedia(fontFooter, colorWhite, colorRed);
            }
            else if (currentscreen == 11)
            {
                MediaStreamer(fontFooter, colorWhite, colorRed);
            }
            else if (currentscreen == 20)
            {
                RenderInstallMenu(fontFooter, colorWhite, colorRed);
            }


            MouseHandler();
            SDL.SDL_Rect textRect2 = new SDL.SDL_Rect { x = mouseX, y = mouseY, w = 12, h = 12 };
            SDL.SDL_SetRenderDrawColor(renderer, 255, 0, 0, 15); // Red highlight for selection
            SDL.SDL_RenderFillRect(renderer, ref textRect2);

            SDL.SDL_SetRenderDrawColor(renderer, 51, 52, 71, 255);
            // Update the display
            SDL.SDL_RenderPresent(renderer);
            //SDL.SDL_Delay(16);
        }


        //Destroy window
        SDL.SDL_DestroyWindow(window);


        //Quit SDL subsystems
        SDL.SDL_Quit();
        Environment.Exit(0);
        return Task.CompletedTask;
    }

    static void clicked()
    {
        if (currentscreen == 0)
        {
            for (int i = 0; i < menulines.Count; i++)
            {
                SDL.SDL_Rect optionRect = new SDL.SDL_Rect { x = boxX, y = boxY * (i + 1) - 10, w = boxW, h = boxH };
                if (HandleSelection(mouseX, mouseY, optionRect))
                {
                    if (menulines[i] == "Exit")
                    {
                        quit = true;
                    }
                    else if (menulines[i] == "Run File")
                    {
                        currentscreen = 1;

                    }
                    else if (menulines[i] == "Update/Install")
                    {
                        currentscreen = 20;
                    }
                    else if (menulines[i] == "Media Downloader")
                    {
                        GenerateMedia();
                        currentscreen = 10;
                    }
                    else
                    {
                        Console.WriteLine("Selected option: " + menulines[i]);
                    }
                }
            }
        }
        else if (currentscreen == 1)
        {
            for (int i = 0; i < jukalines.Count; i++)
            {
                SDL.SDL_Rect optionRect = new SDL.SDL_Rect { x = boxX, y = boxY * (i + 1) - 10, w = boxW, h = boxH };
                if (HandleSelection(mouseX, mouseY, optionRect))
                {
                    if (jukalines[i] == "Back")
                    {
                        currentscreen = 0;
                    }
                    else
                    {
                        filetoexecute = jukalines[i];
                        generateProgram();
                        currentscreen = 2;
                    }
                }
            }
        }
        else if (currentscreen == 2)
        {
            currentscreen = 1;
        }
        else if (currentscreen == 10)
        {
            for (int i = 0; i < videoInfos.Count + 1; i++)
            {
                SDL.SDL_Rect optionRect = new SDL.SDL_Rect { x = 160, y = 190 + fontSizeC * (i + 1) + 5, w = 1280 - 160, h = fontSizeC };
                if (HandleSelection(mouseX, mouseY, optionRect))
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

                        currentscreen = 11;
                    }
                }
            }
        }
        else if (currentscreen == 11)
        {
            currentscreen = 10;
        }
    }


    static void MouseHandler()
    {
        SDL.SDL_Event e;
        while (SDL.SDL_PollEvent(out e) != 0)
        {
            switch (e.type)
            {
                case SDL.SDL_EventType.SDL_CONTROLLERAXISMOTION:
                    switch ((SDL.SDL_GameControllerAxis)e.caxis.axis)
                    {
                        case SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY:
                        case SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTY:
                        case SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_MAX:
                        case SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_INVALID:
                            mouseY += e.caxis.axisValue / 1000;
                            break;

                        case SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX:
                        case SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX:
                        case SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERLEFT:
                        case SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERRIGHT:
                            mouseX += e.caxis.axisValue / 1000;
                            break;
                        default:
                            break;
                    }
                    if (mouseX < 0)
                    {
                        mouseX = 0;
                    } else if (mouseX > SCREEN_WIDTH - 12)
                    {
                        mouseX = SCREEN_WIDTH - 12;
                    }
                    if (mouseY < 0)
                    {
                        mouseY = 0;
                    }
                    else if (mouseY > SCREEN_HEIGHT - 12)
                    {
                        mouseY = SCREEN_HEIGHT - 12;
                    }
                    break;
                case SDL.SDL_EventType.SDL_KEYDOWN:
                    keypressed = SDL.SDL_GetKeyName(e.key.keysym.sym);

                    if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_DOWN)
                    {
                        mouseY += 10;
                    }
                    else if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_UP)
                    {
                        mouseY -= 10;
                    }
                    else if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_LEFT)
                    {
                        mouseX -= 10;
                    }
                    else if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_RIGHT)
                    {
                        mouseX += 10;
                    }
                    break;
                case SDL.SDL_EventType.SDL_MOUSEMOTION:
                    // Get the current mouse position
                    mouseX = e.motion.x;
                    mouseY = e.motion.y;
                    break;
                case SDL.SDL_EventType.SDL_CONTROLLERBUTTONDOWN:
                    keypressed = e.cbutton.button.ToString();
                    if (rumble)
                    {
                        // Rumble the controller
                        SDL_GameControllerRumble(controller, 0xFFFF, 0xFFFF, 2000); // Strong rumble for 1 second
                    }

                    SDL_GameControllerRumble(controller, 0xFFFF, 0xFFFF, 2000); // Strong rumble for 1 second


                    switch ((SDL.SDL_GameControllerButton)e.cbutton.button)
                    {
                        case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_GUIDE:
                            currentscreen = 0;
                            break;
                        case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER:
                        case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X:
                        case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_Y:
                            currentscreen = 0;
                            break;
                        case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A:
                        case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B:
                        case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START:
                        case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER:
                            clicked();
                            break;
                        case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_UP:
                            mouseY -= 10;
                            break;
                        case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_DOWN:
                            mouseY += 10;
                            break;
                        case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_LEFT:
                            mouseX -= 10;
                            break;
                        case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_RIGHT:
                            mouseX += 10;
                            break;
                        default:
                            clicked();
                            break;
                    }
                    break;
                case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    if (e.button.button == SDL.SDL_BUTTON_LEFT)
                    {
                        clicked();
                    }
                    else if (e.button.button == SDL.SDL_BUTTON_RIGHT)
                    {
                        currentscreen = 0;
                    }
                    break;
          
            }
        }
    }

    static void RenderInstallMenu(nint fontFooter, SDL.SDL_Color colorWhite, SDL.SDL_Color colorRed)
    {
        int textW, textH;

        SDL.SDL_QueryTexture(menuOptionsTexture[1], out _, out _, out textW, out textH);


        // Calculate menu option positions (centered horizontally)
        int optionX = (SCREEN_WIDTH - textW) / 2; // Placeholder for text width calculation
        int optionY = ((SCREEN_HEIGHT) / (menulines.Count + 1));

        SDL.SDL_QueryTexture(menuOptionsTexture[0], out _, out _, out textW, out textH);
        SDL.SDL_Rect menuBackgroundRect = new SDL.SDL_Rect { x = optionX - textW / 2 - 150, y = optionY - 10, w = textW + 300, h = (optionY * (menulines.Count)) - 80 };
        SDL.SDL_SetRenderDrawColor(renderer, 41, 42, 61, 255); // Juka background
        SDL.SDL_RenderFillRect(renderer, ref menuBackgroundRect);

        var textWtemp = textW;

        // Iterate through menu options and render text
        for (int i = 0; i < menulines.Count; ++i)
        {
            SDL.SDL_QueryTexture(menuOptionsTexture[i], out _, out _, out textW, out textH);
            SDL.SDL_Rect srcTextRect = new SDL.SDL_Rect { x = 0, y = 0, w = textW, h = textH };
            SDL.SDL_Rect textRect = new SDL.SDL_Rect { x = optionX - textW / 2, y = optionY * (i + 1), w = textW, h = textH };

            // Render menu option text
            SDL.SDL_RenderCopy(renderer, menuOptionsTexture[i], ref srcTextRect, ref textRect);

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

    static void MediaStreamer(nint fontFooter, SDL.SDL_Color colorWhite, SDL.SDL_Color colorRed)
    {
        RenderText("Dowloaded to Juka/"+myfile+". Press any button to continue", 160, 190, fontFooter, colorWhite);
    }

    static void RenderMedia(nint fontFooter, SDL.SDL_Color colorWhite, SDL.SDL_Color colorRed)
    {
        RenderText("Videos: ", 160, 190, fontFooter, colorWhite);
        for (int i = 0; i < videoInfos.Count; i++)
        {
           
            RenderText(videoInfos[i].Title, 160, 190 + fontSizeC * (i + 1), fontFooter, colorWhite);
        }
        RenderText("Back", 160, 190 + fontSizeC * (videoInfos.Count + 1), fontFooter, colorWhite);



        // Iterate through menu options and render text
        for (int i = 0; i < videoInfos.Count+1; ++i)
        {
            SDL.SDL_Rect textRect2 = new SDL.SDL_Rect { x = 160, y = 190 + fontSizeC * (i + 1)+5, w = 1280-160, h = fontSizeC };
            if (HandleSelection(mouseX, mouseY, textRect2))
            {
                SDL.SDL_SetRenderDrawColor(renderer, 255, 0, 0, 15); // Red highlight for selection
                SDL.SDL_RenderDrawRect(renderer, ref textRect2);
            }
        }
    }

    static async Task GeneratePackages()
    {
        string librariesList = await Packages.getList();
        //packages
    }

    static void GenerateMedia()
    {
        var apikey = "AIzaSyBzpZzE4nQVxr_EQLgWqTfREpvWON - gWu8";
        var youtube = new YouTubeApiService(apikey);
        var videos = youtube.GetTopVideosSync();

        videoInfos = videos.Select(v => new Juka.SDL.VideoInfo
        {
            VideoId = v.VideoId,
            Title = v.Title
        }).ToList();

    }

    static void RenderLogo()
    {
        // Load the logo image
        IntPtr jukalogo = SDL_image.IMG_Load(logo);
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
            Console.WriteLine("Failed to render in rendetext! " + text);
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

        SDL.SDL_QueryTexture(menuOptionsTexture[0], out _, out _, out textW, out textH);
        SDL.SDL_Rect menuBackgroundRect = new SDL.SDL_Rect { x = optionX - textW / 2 -150, y = optionY-10, w = textW + 300, h = (optionY *(menulines.Count))-80 };
        SDL.SDL_SetRenderDrawColor(renderer, 41, 42, 61, 255); // Juka background
        SDL.SDL_RenderFillRect(renderer, ref menuBackgroundRect);

        var textWtemp = textW;
 
        // Iterate through menu options and render text
        for (int i = 0; i < menulines.Count; ++i)
        {
            SDL.SDL_QueryTexture(menuOptionsTexture[i], out _, out _, out textW, out textH);
            SDL.SDL_Rect srcTextRect = new SDL.SDL_Rect { x = 0, y = 0, w = textW, h = textH };
            SDL.SDL_Rect textRect = new SDL.SDL_Rect { x = optionX -textW/2, y = optionY * (i + 1), w = textW, h = textH };

            // Render menu option text
            SDL.SDL_RenderCopy(renderer, menuOptionsTexture[i], ref srcTextRect, ref textRect);

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

    static void generateProgram()
    {
        var output = "";
        try
        {
            Compiler? compiler = new Compiler();
            output = compiler.CompileJukaCode(filetoexecute, isFile: true);
        }catch(Exception e)
        {
            Console.WriteLine(e);
            output = "Something went wrong: " + e.ToString();
        }
        multioutput = new List<string>(output.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None)).Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList(); 
    }

    static void RenderRun()
    {

        int textW, textH;

        SDL.SDL_QueryTexture(jukaOptionsTexture[0], out _, out _, out textW, out textH);


        // Calculate menu option positions (centered horizontally)
        int optionX = (SCREEN_WIDTH - textW) / 2; // Placeholder for text width calculation
        int optionY = ((SCREEN_HEIGHT) / (menulines.Count + 1));

        SDL.SDL_QueryTexture(jukaOptionsTexture[0], out _, out _, out textW, out textH);
        SDL.SDL_Rect menuBackgroundRect = new SDL.SDL_Rect { x = optionX - textW / 2 - 150, y = optionY - 10, w = textW + 300, h = (optionY * (menulines.Count)) - 80 };
        SDL.SDL_SetRenderDrawColor(renderer, 41, 42, 61, 255); // Juka background
        SDL.SDL_RenderFillRect(renderer, ref menuBackgroundRect);

        var textWtemp = textW;

        // Iterate through menu options and render text
        for (int i = 0; i < jukalines.Count; ++i)
        {
            SDL.SDL_QueryTexture(jukaOptionsTexture[i], out _, out _, out textW, out textH);
            SDL.SDL_Rect srcTextRect = new SDL.SDL_Rect { x = 0, y = 0, w = textW, h = textH };
            SDL.SDL_Rect textRect = new SDL.SDL_Rect { x = optionX - textW / 2, y = optionY * (i + 1), w = textW, h = textH };

            // Render menu option text
            SDL.SDL_RenderCopy(renderer, jukaOptionsTexture[i], ref srcTextRect, ref textRect);

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


    static List<nint> GenerateMenu(List<string> menulines, nint menuFont, SDL.SDL_Color color)
    {
        List<nint> menuOptionsTexture = new List<nint>();
        List<nint> menuOptions = new List<nint>();

        for (var i = 0; i < menulines.Count(); i++)
        {
            menuOptions.Add(SDL_ttf.TTF_RenderText_Solid(menuFont, menulines[i], color));
        }

        for (var i = 0; i < menulines.Count(); i++)
        {
            menuOptionsTexture.Add(SDL.SDL_CreateTextureFromSurface(renderer, menuOptions[i]));
        }
        return menuOptionsTexture;
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