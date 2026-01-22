using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveTrialSessionInformation : MonoBehaviour
{
    [Header ("Session Information")]
    public TMP_InputField sessionNameInput;
    public TMP_InputField participantIDInput;
    public TMP_InputField notesInput;

    [Header("Trial Settings")]
    private TrialSettingsData _trialSettingsData;

   
    public void OnBeginTrialButtonClicked()
    {
        _trialSettingsData = LoadTrialSettings.Instance.GetTrialSettings();
        TrialSessionInformation trialSession = new TrialSessionInformation
        {
            SessionName = sessionNameInput.text,
            ParticipantID = participantIDInput.text,
            Notes = notesInput.text,
            TrialSettings = _trialSettingsData
        };
        //FileManager.Instance.SaveToFileDirectory(trialSession, trialSession.SessionName + "-" + trialSession.ParticipantID);
    }
}


