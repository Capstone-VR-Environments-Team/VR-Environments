using UnityEngine;
using UnityEngine.InputSystem;

public class FullscreenToggle : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.f11Key.wasPressedThisFrame)
            {
                Screen.fullScreen = !Screen.fullScreen;
            }
        }
    }
}