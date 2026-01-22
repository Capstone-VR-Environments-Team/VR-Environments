using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartNewSessionManager : MonoBehaviour
{
    [Header("Session Settings")]
    [SerializeField] private TMP_InputField sessionNameInput;
    public TMP_InputField SessionNameInput => sessionNameInput;

    [SerializeField] private TMP_InputField participantIDInput;
    public TMP_InputField ParticipantIDInput => participantIDInput;

    [SerializeField] private Button uploadConfigurationButton;
    public Button UploadConfigurationButton => uploadConfigurationButton;

    [SerializeField] private TMP_InputField notesInput;
    public TMP_InputField NotesInput => notesInput;

    [Header("Buttons")]
    [SerializeField] private Button cancelButton;
    public Button CancelButton => cancelButton;

    [SerializeField] private Button beginSessionButton;
    public Button BeginSessionButton => beginSessionButton;

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
        FileManager.Instance.SetTrialSessionInformation(trialSession);
    }
}
