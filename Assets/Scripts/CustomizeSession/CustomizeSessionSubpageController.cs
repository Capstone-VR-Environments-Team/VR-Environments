using UnityEngine;

public class CustomizeSessionSubpageController : MonoBehaviour
{
    [SerializeField] private GameObject visibilitySettings;
    [SerializeField] private GameObject offsetSettings;
    [SerializeField] private GameObject targetSettings;
    [SerializeField] private GameObject backgroundSettings;
    [SerializeField] private GameObject colorSettings;

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
        colorSettings.SetActive(false);
    }

    public void ShowVisibilitySettings()
    {
        ToggleSettings(visibilitySettings);
    }

    public void ShowOffsetSettings()
    {
        ToggleSettings(offsetSettings);
    }

    public void ShowTargetSettings()
    {
        ToggleSettings(targetSettings);
    }

    public void ShowBackgroundSettings()
    {
        ToggleSettings(backgroundSettings);
    }

    public void ShowColorSettings()
    {
        ToggleSettings(colorSettings);
    }

    public void ToggleSettings(GameObject settings)
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
