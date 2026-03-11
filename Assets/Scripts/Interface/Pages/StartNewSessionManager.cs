using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField] private TMP_Text configurationFileNameText;
    public TMP_Text ConfigurationFileNameText => configurationFileNameText;

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
        TrialSessionInformation trialSession = new TrialSessionInformation
        {
            SessionName = sessionNameInput.text,
            ParticipantID = participantIDInput.text,
            Notes = notesInput.text,
            TrialSettings = _trialSettingsData
        };
        SessionManager.Instance.SetTrialSessionInformation(trialSession);
        clearInputs();
        SceneManager.LoadScene("SampleScene");

    }

    public void clearInputs()
    {
        sessionNameInput.text = "";
        participantIDInput.text = "";
        notesInput.text = "";
        configurationFileNameText.SetText("No File Uploaded");
    }

    public void ReturnToHome() {
        clearInputs();
        SceneManager.LoadScene("HomeScreen");
    }

    public void OnUploadSettingsClicked() {
        string filePath = FileSelector.getFilePath(SessionManager.BaseDataPath, "json");
        var (loadedData, fileName) = FileManager.LoadFromFile<TrialSettingsData>(filePath);

        if (loadedData != null) {
            _trialSettingsData = loadedData;
            ConfigurationFileNameText.SetText(fileName);
            Debug.Log($"Loaded Configuration: {_trialSettingsData.ConfigurationName}");
            Debug.Log($"Target Count: {_trialSettingsData.TargetLocations.Count}");

        } else {
            ConfigurationFileNameText.SetText("File Upload Failed");
            Debug.LogError("Failed to load settings file.");
        }
    }
}
