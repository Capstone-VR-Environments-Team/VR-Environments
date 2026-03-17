using UnityEngine;

public class CustomizeSessionSubpageController : MonoBehaviour
{
    [SerializeField] private GameObject visibilitySettings;
    [SerializeField] private GameObject offsetSettings;
    [SerializeField] private GameObject targetSettings;
    [SerializeField] private GameObject backgroundSettings;

    void Start()
    {
        HideAll();
    }

    public void HideAll()
    {
        visibilitySettings.SetActive(false);
        offsetSettings.SetActive(false);
        targetSettings.SetActive(false);
        backgroundSettings.SetActive(false);
    }

    public void ShowVisibilitySettings()
    {
        toggleSettings(visibilitySettings);
    }

    public void ShowOffsetSettings()
    {
        toggleSettings(offsetSettings);
    }

    public void ShowTargetSettings()
    {
        toggleSettings(targetSettings);
    }

    public void ShowBackgroundSettings()
    {
        toggleSettings(backgroundSettings);
    }

    public void toggleSettings(GameObject settings)
    {
        if (settings.activeSelf)
        {
            settings.SetActive(false);
        }
        else
        {
            HideAll();
            settings.SetActive(true);
        }
    }
}
