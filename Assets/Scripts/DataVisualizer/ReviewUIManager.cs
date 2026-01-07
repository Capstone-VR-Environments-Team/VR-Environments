using UnityEngine;
using UnityEngine.SceneManagement;

public class ReviewUIManager : MonoBehaviour {
    [SerializeField] private GameObject reviewPage;
    [SerializeField] private GameObject statisticalViewPage;
    [SerializeField] private GameObject interactiveViewPage;

    void Start() {
        ShowReviewPage();
        ReviewPastSessionsManager reviewPastSessionsManager = reviewPage.GetComponent<ReviewPastSessionsManager>();
        reviewPastSessionsManager.InteractiveViewButton.onClick.AddListener(ShowInteractiveViewPage);
        reviewPastSessionsManager.StatisticalViewButton.onClick.AddListener(ShowStatisticalViewPage);
        reviewPastSessionsManager.CancelButton.onClick.AddListener(GoHome);

        InteractiveViewManager interactiveViewManager = interactiveViewPage.GetComponent<InteractiveViewManager>();
        interactiveViewManager.EndReviewButton.onClick.AddListener(ShowReviewPage);

        StatisticalViewManager statisticalViewManager = statisticalViewPage.GetComponent<StatisticalViewManager>();
        statisticalViewManager.EndAnalysisButton.onClick.AddListener(ShowReviewPage);
    }

    public void ShowReviewPage() {
        reviewPage.SetActive(true);
        statisticalViewPage.SetActive(false);
        interactiveViewPage.SetActive(false);
    }

    public void ShowStatisticalViewPage() {
        reviewPage.SetActive(false);
        statisticalViewPage.SetActive(true);
        interactiveViewPage.SetActive(false);
    }
    public void ShowInteractiveViewPage() {
        reviewPage.SetActive(false);
        statisticalViewPage.SetActive(false);
        interactiveViewPage.SetActive(true);
        InteractiveViewManager interactiveViewManager = interactiveViewPage.GetComponent<InteractiveViewManager>();
        interactiveViewManager.EnterScreen();
    }

    public void GoHome()
    {
        SceneManager.LoadScene("SampleScene");
    }
}