using UnityEngine;

public class CustomizeSessionSubpageController : MonoBehaviour
{
    [SerializeField] private GameObject visibilitySettings;
    [SerializeField] private GameObject offsetSettings;
    [SerializeField] private GameObject targetSettings;
        
    void Start()
    {
        HideAll();
    }

    public void HideAll()
    {
        visibilitySettings.SetActive(false);
        offsetSettings.SetActive(false);
        targetSettings.SetActive(false);
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
