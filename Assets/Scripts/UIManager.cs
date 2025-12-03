using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject homePage;
    [SerializeField] private GameObject customizationPage;
    [SerializeField] private GameObject startNewPage;
    [SerializeField] private GameObject liveViewPage;

    void Start()
    {
        ShowHomePage();
    }

    public void ShowHomePage()
    {
        homePage.SetActive(true);
        customizationPage.SetActive(false);
        startNewPage.SetActive(false);
        liveViewPage.SetActive(false);
    }

    public void ShowCustomizationPage()
    {
        homePage.SetActive(false);
        customizationPage.SetActive(true);
        startNewPage.SetActive(false);
        liveViewPage.SetActive(false);
    }
    public void ShowStartNewPage()
    {
        homePage.SetActive(false);
        customizationPage.SetActive(false);
        startNewPage.SetActive(true);
        liveViewPage.SetActive(false);
    }
    public void ShowLiveViewPage()
    {
        homePage.SetActive(false);
        customizationPage.SetActive(false);
        startNewPage.SetActive(false);
        liveViewPage.SetActive(true);
    }
}