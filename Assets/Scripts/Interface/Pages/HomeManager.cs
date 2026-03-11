using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startNewSessionButton;
    public Button StartNewSessionButton => startNewSessionButton;

    [SerializeField] private Button reviewPastSessionsButton;
    public Button ReviewPastSessionsButton => reviewPastSessionsButton;

    [SerializeField] private Button customizeSessionButton;
    public Button CustomizeSessionButton => customizeSessionButton;

    public void Awake() {
        startNewSessionButton.onClick.AddListener(ShowNewSessionPage);
        reviewPastSessionsButton.onClick.AddListener(ShowReviewSessionPage);
        customizeSessionButton.onClick.AddListener(ShowCustomizationSessionPage);
    }

    public void ShowCustomizationSessionPage() {
        SceneManager.LoadScene("CustomizeSession");
    }

    public void ShowReviewSessionPage() {
        SceneManager.LoadScene("ReviewPastSession");
    }

    public void ShowNewSessionPage() {
        SceneManager.LoadScene("SampleScene");
    }
}
