using UnityEngine;
using UnityEngine.InputSystem;

public class FullscreenToggle : Singleton<FullscreenToggle>
{

    void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.f11Key.wasPressedThisFrame)
            {
                if (Screen.fullScreen)
                {
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                }
                else
                {
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                }
            }
        }
    }
}