using UnityEngine;

public class EnableSecondDisplay : MonoBehaviour
{
    void Start()
    {
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate(); // Enables Display 2
        }
    }
}
