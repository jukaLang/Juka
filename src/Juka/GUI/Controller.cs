using SDL2;
using static Juka.GUI.Globals;

namespace Juka.GUI
{
    public static class Controller
    {
        public static void MouseHandler()
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
                        }
                        else if (mouseX > SCREEN_WIDTH - 12)
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
                            SDL.SDL_GameControllerRumble(controller, 0xFFFF, 0xFFFF, 2000); // Strong rumble for 1 second
                        }
                        SDL.SDL_HapticRunEffect(JoyHaptic, effectID, 1);



                        switch ((SDL.SDL_GameControllerButton)e.cbutton.button)
                        {
                            case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_GUIDE:
                                currentscreen = Menus.MenuMain;
                                break;
                            case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER:
                            case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X:
                            case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_Y:
                                currentscreen = Menus.MenuMain;
                                break;
                            case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A:
                            case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B:
                            case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START:
                            case SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER:
                                Clicked.itemclicked();
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
                                Clicked.itemclicked();
                                break;
                        }
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                        if (e.button.button == SDL.SDL_BUTTON_LEFT)
                        {
                            Clicked.itemclicked();
                        }
                        else if (e.button.button == SDL.SDL_BUTTON_RIGHT)
                        {
                            currentscreen = 0;
                        }
                        break;

                }
            }
        }
    }
}
