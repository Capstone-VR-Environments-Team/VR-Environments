using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startNewSessionButton;
    [SerializeField] private Button reviewPastSessionsButton;
    [SerializeField] private Button customizeSessionButton;
    [SerializeField] private Button quitButton;

    public void Awake() {
        startNewSessionButton.onClick.AddListener(() => SceneManager.LoadScene("StartNewSession"));
        reviewPastSessionsButton.onClick.AddListener(() => SceneManager.LoadScene("ReviewPastSession"));
        customizeSessionButton.onClick.AddListener(() => SceneManager.LoadScene("CustomizeSession"));
        quitButton.onClick.AddListener(Application.Quit);
    }
}
