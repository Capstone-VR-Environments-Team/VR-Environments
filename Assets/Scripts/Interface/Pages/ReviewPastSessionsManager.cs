using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReviewPastSessionsManager : MonoBehaviour
{
    [Header("Session Data")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text participantIDText;
    [SerializeField] private TMP_Text notesText;

    [Header("Buttons")]
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button statisticalViewButton;
    [SerializeField] private Button interactiveViewButton;
    [SerializeField] private Button selectSessionButton;

    public void SetSessionInfo(JsonWrapper sessionInfo)
    {
        nameText.SetText(sessionInfo.TrialSessionInformation.SessionName);
        participantIDText.SetText(sessionInfo.TrialSessionInformation.ParticipantID);
        notesText.SetText(sessionInfo.TrialSessionInformation.Notes);
    }

    public void AddCancelOnCLick(UnityEngine.Events.UnityAction action)
    {
        cancelButton.onClick.AddListener(action);
    }

    public void AddInteractiveViewOnClick(UnityEngine.Events.UnityAction action)
    {
        interactiveViewButton.onClick.AddListener(action);
    }

    public void AddStatisticalViewOnClick(UnityEngine.Events.UnityAction action)
    {
        statisticalViewButton.onClick.AddListener(action);
    }

    public void AddSelectSessionOnClick(UnityEngine.Events.UnityAction action)
    {
        selectSessionButton.onClick.AddListener(action);
    }
}
