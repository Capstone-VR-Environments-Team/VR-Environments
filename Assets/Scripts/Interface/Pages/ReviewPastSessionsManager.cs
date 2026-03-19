using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReviewPastSessionsManager : MonoBehaviour
{
    [Header("Session Data")] 
    [SerializeField] private TMP_Text filePathText;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text participantIDText;
    [SerializeField] private TMP_Text notesText;

    [Header("Buttons")]
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button statisticalViewButton;
    [SerializeField] private Button interactiveViewButton;
    [SerializeField] private Button selectSessionButton;

    void Start ()
    {
        if (AnalysisResultsStore.Instance.HasSessionInfo)
        {
            SetSessionInfo();
        }
        UpdateScreen(AnalysisResultsStore.Instance.HasSessionInfo);
    }

    public void SetSessionInfo()
    {
        filePathText.SetText(AnalysisResultsStore.Instance.CurrentFilePath);

        TrialSessionInformation sessionInfo = AnalysisResultsStore.Instance.TrialInfo.TrialSessionInformation;
        nameText.SetText(sessionInfo.SessionName);
        participantIDText.SetText(sessionInfo.ParticipantID);
        notesText.SetText(sessionInfo.Notes);

        UpdateScreen(AnalysisResultsStore.Instance.HasSessionInfo);
    }

    private void UpdateScreen(bool mode)
    {
        infoPanel.SetActive(mode);
        statisticalViewButton.interactable = mode;
        interactiveViewButton.interactable = mode;

        Debug.Log("Interactable: " + mode);
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
