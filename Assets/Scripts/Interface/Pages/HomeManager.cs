using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startNewSessionButton;
    [SerializeField] private Button reviewPastSessionsButton;
    [SerializeField] private Button customizeSessionButton;

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
        SceneManager.LoadScene("StartNewSession");
    }
}
