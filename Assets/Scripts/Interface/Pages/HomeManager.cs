using UnityEngine;
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
}
