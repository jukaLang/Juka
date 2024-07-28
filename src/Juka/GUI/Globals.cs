using SDL2;
using System.Diagnostics;
using VideoLibrary;

namespace Juka.GUI
{
    public static class Globals
    {
        //Juka Related
        public static string openFont = "open-sans.ttf";
        public static string logo = "jukalogo.png";

        public static int fontSize = 64;
        public static int fontSizeC = 24;

        public static Dictionary<string, string> systemInfo = SelfUpdate.GetSystemInfo();
        public static string version = CurrentVersion.GetVersion();

        //SDL2
        public static nint JoyHaptic;
        public static int effectID;


        public static int SCREEN_WIDTH = 1280;
        public static int SCREEN_HEIGHT = 720;

        public static string keypressed = SDL.SDL_GetKeyName(SDL.SDL_Keycode.SDLK_UNDEFINED);
        public static bool rumble = false;
        public static bool rumble2 = false;

        public static nint window = nint.Zero;
        public static nint renderer = nint.Zero;
        public static nint font = nint.Zero;

        public static nint controller; //GET THE CONTROLLER
        public static bool quit = false; //Render UNTIL QUIT


        //keyboard
        public static string keyboardbuffer = "";

        // Screen Selection
        public static int mouseX = SCREEN_WIDTH / 2;
        public static int mouseY = SCREEN_HEIGHT / 2;

        public static nint fontBig;
        public static nint fontFooter;
        public static nint menufont;
        public static nint descriptionfont;
        public static bool keyboardOn = false;
        public static int keyboardy = SCREEN_HEIGHT / 2;

        public enum Menus
        {
            MenuMain,
            MenuJuka,
            RanJuka,
            MediaPlayer,
            MediaDownloaded,
            MenuInstall,
            MenuInstallJuka,
            VirtualKeyboard
        }



        //
        public static Dictionary<Menus, List<string>> menuOptions = new Dictionary<Menus, List<string>>();
        public static Dictionary<Menus, List<nint>> menuTextures= new Dictionary<Menus, List<nint>>();  // PRE-RENDER TEXTURES
        public static Dictionary<Menus, List<Descript<int>>> menuDescript = new Dictionary<Menus, List<Descript<int>>>();
        //

        // WHERE TO PUT MENU CONTENT AND OFFSET
        public static int boxX = 0;
        public static int boxY = 0;
        public static int boxW = 0;
        public static int boxH = 0;


        // Juka Run
        public static string filetoexecute = "";
        public static List<string> multioutput = new List<string>();

        // Video
        public static string myfile = "";
        public static List<VideoInfo> videoInfos = new List<VideoInfo>();
        
        //Current Screen
        public static Menus currentscreen = Menus.MenuMain;

        public static bool running = false;
        public static Process ffplayProcess;
        public static StreamWriter ffplayInput;

    }

    public class Descript<T>
    {
        public T X { get; set; }
        public T Y { get; set; }
        public T Width { get; set; }
        public T Height { get; set; }
    }

}
