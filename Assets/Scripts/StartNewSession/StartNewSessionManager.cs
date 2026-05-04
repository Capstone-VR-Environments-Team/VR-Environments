using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartNewSessionManager : MonoBehaviour
{
    [Header("Session Settings")]
    [SerializeField] private TMP_InputField sessionNameInput;
    [SerializeField] private TMP_InputField participantIDInput;
    [SerializeField] private Button uploadConfigurationButton;
    [SerializeField] private TMP_Text configurationFileNameText;
    [SerializeField] private TMP_InputField notesInput;

    [Header("Buttons")]
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button beginSessionButton;
    private TrialSettingsData _trialSettingsData;

    void Start() {
        uploadConfigurationButton.onClick.AddListener(OnUploadSettingsClicked);
        beginSessionButton.onClick.AddListener(OnBeginTrialButtonClicked);
        cancelButton.onClick.AddListener(ReturnToHome);

        sessionNameInput.onValueChanged.AddListener(delegate { UpdateBeginButton(); });
        participantIDInput.onValueChanged.AddListener(delegate { UpdateBeginButton(); });
    }

    public void UpdateBeginButton()
    {
        beginSessionButton.interactable = !string.IsNullOrEmpty(sessionNameInput.text)
                                          && !string.IsNullOrEmpty(participantIDInput.text)
                                          && _trialSettingsData != null;
    }

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
        ClearInputs();
        SceneManager.LoadScene("LiveTrialView");

    }

    public void ClearInputs()
    {
        sessionNameInput.text = "";
        participantIDInput.text = "";
        notesInput.text = "";
        configurationFileNameText.SetText("No File Uploaded");
    }

    public void ReturnToHome() {
        ClearInputs();
        Destroy(SessionManager.Instance);
        SceneManager.LoadScene("HomeScreen");
    }

    public void OnUploadSettingsClicked() {
        string filePath = FileSelector.getFilePath(Path.Combine(SessionManager.BaseDataPath, "TrialFiles"), "json");
        if(string.IsNullOrEmpty(filePath))
        {
            return;
        }
        var (loadedData, fileName) = FileManager.LoadFromFile<TrialSettingsData>(filePath);

        if (loadedData != null) {
            _trialSettingsData = loadedData;
            configurationFileNameText.SetText(fileName);
            Debug.Log($"Loaded Configuration: {_trialSettingsData.ConfigurationName}");
            Debug.Log($"Target Count: {_trialSettingsData.TargetLocations.Count}");

        } else {
            configurationFileNameText.SetText("File Upload Failed");
            Debug.LogError("Failed to load settings file.");
        }

        UpdateBeginButton();
    }
}
