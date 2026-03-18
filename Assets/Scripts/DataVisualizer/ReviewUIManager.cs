using UnityEngine;
using UnityEngine.SceneManagement;

public class ReviewUIManager : MonoBehaviour {
    [SerializeField] private GameObject reviewPage;

    void Start() {
        ShowReviewPage();
        ReviewPastSessionsManager reviewPastSessionsManager = reviewPage.GetComponent<ReviewPastSessionsManager>();
        reviewPastSessionsManager.AddInteractiveViewOnClick(ShowInteractiveViewPage);
        reviewPastSessionsManager.AddStatisticalViewOnClick(ShowStatisticalViewPage);
        reviewPastSessionsManager.AddCancelOnCLick(GoHome);
    }

    public void ShowReviewPage() {
        reviewPage.SetActive(true);
    }

    public void ShowStatisticalViewPage() {
        SceneManager.LoadScene("StatisticalView");
    }
    public void ShowInteractiveViewPage() {
        SceneManager.LoadScene("InteractiveView");
    }

    public void GoHome()
    {
        Destroy(AnalysisResultsStore.Instance);
        SceneManager.LoadScene("HomeScreen");
    }
}