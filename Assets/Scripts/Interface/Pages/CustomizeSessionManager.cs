using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomizeSessionManager : MonoBehaviour
{
    [Header("Configuration Name")]
    [SerializeField] private TMP_InputField configurationNameInput;

    [Header("Visibility Settings")]
    [SerializeField] private TMP_Dropdown handVisibility;
    [SerializeField] private TMP_Dropdown targetVisibility;
    [SerializeField] private TMP_InputField handFlickerFrequency;
    [SerializeField] private TMP_InputField targetFlickerFrequency;

    [SerializeField] private TMP_InputField leftHandColor;
    public TMP_InputField LeftHandColor => leftHandColor;

    [SerializeField] private TMP_InputField rightHandColor;
    public TMP_InputField RightHandColor => rightHandColor;

    [SerializeField] private TMP_InputField targetColor;
    public TMP_InputField TargetColor => targetColor;

    [Header("Offset Settings")]
    [SerializeField] private TMP_Dropdown offsetTypeDropdown;
    [SerializeField] private TMP_InputField offsetXInput;
    [SerializeField] private TMP_InputField offsetYInput;
    [SerializeField] private TMP_InputField offsetZInput;
    [SerializeField] private TMP_InputField targetRangeInput;

    [SerializeField] private Toggle showHandInProximityToggle;
    public Toggle ShowHandInProximityToggle => showHandInProximityToggle;

    [Header("Background Settings")]
    [SerializeField] private TMP_Dropdown backgroundTypeDropdown;
    [SerializeField] private Button uploadImage;
    [SerializeField] private TMP_Text uploadedImageFileNameText;
    [SerializeField] private TMP_Text uploadedVideoFileNameText;
    [SerializeField] private Button uploadVideo;
    [SerializeField] private TMP_Dropdown directionTypeDropdown;
    [SerializeField] private TMP_InputField speedInput;
    [SerializeField] private TMP_InputField numberOfObjectsInput;
    [SerializeField] private TMP_InputField objectSizeInput;
    [SerializeField] private TMP_InputField objectColorInput;

    [Header("Buttons")]
    [SerializeField] private Button saveConfigurationButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button uploadTargetLocationsButton;
    [SerializeField] private TMP_Text uploadedFileNameText;
    [SerializeField] private Button modifyConfigurationButton;

    private List<Vector3> _tempTargetLocations = new List<Vector3>();
    private String imageBackgroundFilePath = "";
    private String videoBackgroundFilePath = "";

    public void OnUploadLocationsClicked()
    {
        string filePath = FileSelector.getFilePath(SessionManager.BaseDataPath, new string[] { "json", "csv" });
        var (importedData, fileName) = FileManager.LoadFromFile<TargetImportData>(filePath);
        if (importedData != null && importedData.targets != null)
        {
            Debug.Log("filename: " + fileName);
            uploadedFileNameText.SetText(fileName);
            _tempTargetLocations = importedData.targets;
        }
        else
        {
            uploadedFileNameText.SetText("File Upload Failed");
            Debug.LogError("Failed to load target locations from file.");
        }
    }

    public float SafeParse(string input, float defaultValue)
    {
        if (float.TryParse(input, out float result))
        {
            return result;
        }
        return defaultValue;
    }

    public void onImageUpload()
    {
        string filePath = FileSelector.getFilePath(SessionManager.BaseDataPath, new string[] { "jpg", "png" });
        if (!string.IsNullOrEmpty(filePath))
        {
            imageBackgroundFilePath = filePath;
        }
    }

    public void onVideoUpload()
    {
        string filePath = FileSelector.getFilePath(SessionManager.BaseDataPath, new string[] { "mp4" });
        if (!string.IsNullOrEmpty(filePath))
        {
            videoBackgroundFilePath = filePath;
        }
    }


    public void OnSaveButtonClicked()
    {
        TrialSettingsData trial = new TrialSettingsData
        {
            ConfigurationName = configurationNameInput.text,
            VisibilitySettings = new VisibilitySettings
            {
                HandsVisibilityType = handVisibility.options[handVisibility.value].text,
                TargetVisibilityType = targetVisibility.options[targetVisibility.value].text,
                HandFlickerFrequency = SafeParse(handFlickerFrequency.text, 0),
                TargetFlickerFrequency = SafeParse(targetFlickerFrequency.text, 0),
                LeftHandColor = string.IsNullOrEmpty(leftHandColor.text) ? "#0000FF" : leftHandColor.text,
                RightHandColor = string.IsNullOrEmpty(rightHandColor.text) ? "#FF0000" : rightHandColor.text,
                TargetColor = string.IsNullOrEmpty(targetColor.text) ? "#C0C0C0" : targetColor.text
            },
            OffsetSettings = new OffsetSettings
            {
                OffsetType = offsetTypeDropdown.options[offsetTypeDropdown.value].text,
                OffsetValues = new Vector3(
                    SafeParse(offsetXInput.text, 0),
                    SafeParse(offsetYInput.text, 0),
                    SafeParse(offsetZInput.text, 0)
                ),
                TargetProximity = SafeParse(targetRangeInput.text, 0),
                ShowHandsInProximity = showHandInProximityToggle.isOn
            },
            BackgroundSettings = new BackgroundSettings
            {
                BackgroundType = backgroundTypeDropdown.options[backgroundTypeDropdown.value].text,
                ImageBackground = imageBackgroundFilePath,
                VideoBackground = videoBackgroundFilePath,
                Direction = directionTypeDropdown.options[directionTypeDropdown.value].text,
                Speed = SafeParse(speedInput.text, 0),
                NumberOfObjects = (int)SafeParse(numberOfObjectsInput.text, 0),
                ObjectColor = string.IsNullOrEmpty(objectColorInput.text) ? "#000000" : objectColorInput.text,
                ObjectSize = SafeParse(objectSizeInput.text, 0)
            },
            TargetLocations = _tempTargetLocations
        };
        SessionManager.Instance.SaveSettingsFile(trial, trial.ConfigurationName);
    }

    public void ResetInputs()
    {
        configurationNameInput.text = "";
        handVisibility.value = 0;
        targetVisibility.value = 0;
        handFlickerFrequency.text = "0";
        targetFlickerFrequency.text = "0";
        leftHandColor.text = "#0000FF";
        rightHandColor.text = "#FF0000";
        targetColor.text = "#C0C0C0";

        offsetTypeDropdown.value = 0;
        offsetXInput.text = "0";
        offsetYInput.text = "0";
        offsetZInput.text = "0";
        targetRangeInput.text = "0";
        showHandInProximityToggle.isOn = false;

        backgroundTypeDropdown.value = 0;
        directionTypeDropdown.value = 0;
        speedInput.text = "0";
        numberOfObjectsInput.text = "0";
        objectSizeInput.text = "0";
        objectColorInput.text = "#000000";

        uploadedImageFileNameText.text = "No file uploaded";
        uploadedVideoFileNameText.text = "No file uploaded";
        uploadedFileNameText.text = "No file uploaded";
    }

    public void OnGoHomeClicked() {
        ResetInputs();
        SceneManager.LoadScene("HomeScreen");
    }

}
