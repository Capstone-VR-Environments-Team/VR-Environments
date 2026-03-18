using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomizeSessionManager : MonoBehaviour
{
    [Header("Configuration Name")]

    [SerializeField] private TMP_InputField configurationNameInput;
    public TMP_InputField ConfigurationNameInput => configurationNameInput;

    [Header("Visibility Settings")]
    [SerializeField] private TMP_Dropdown handVisibility;
    public TMP_Dropdown HandVisibility => handVisibility;

    [SerializeField] private TMP_Dropdown targetVisibility;
    public TMP_Dropdown TargetVisibility => targetVisibility;

    [SerializeField] private TMP_InputField handFlickerFrequency;
    public TMP_InputField HandFlickerFrequency => handFlickerFrequency;

    [SerializeField] private TMP_InputField targetFlickerFrequency;
    public TMP_InputField TargetFlickerFrequency => targetFlickerFrequency;

    [Header("Offset Settings")]
    [SerializeField] private TMP_Dropdown offsetTypeDropdown;
    public TMP_Dropdown OffsetTypeDropdown => offsetTypeDropdown;

    [SerializeField] private TMP_InputField offsetXInput;
    public TMP_InputField OffsetXInput => offsetXInput;

    [SerializeField] private TMP_InputField offsetYInput;
    public TMP_InputField OffsetYInput => offsetYInput;

    [SerializeField] private TMP_InputField offsetZInput;
    public TMP_InputField OffsetZInput => offsetZInput;

    [SerializeField] private TMP_InputField targetRangeInput;
    public TMP_InputField TargetRangeInput => targetRangeInput;

    [Header("Background Settings")]
    [SerializeField] private TMP_Dropdown backgroundTypeDropdown;
    public TMP_Dropdown BackgroundTypeDropdown => backgroundTypeDropdown;

    [SerializeField] private Button uploadImage;
    public Button UploadImage => uploadImage;

    [SerializeField] private TMP_Text uploadedImageFileNameText;
    public TMP_Text UploadedImageFileNameText => uploadedImageFileNameText;

    [SerializeField] private TMP_Text uploadedVideoFileNameText;
    public TMP_Text UploadedVideoFileNameText => uploadedVideoFileNameText;

    [SerializeField] private Button uploadVideo;
    public Button UploadVideo => uploadVideo;

    [SerializeField] private TMP_Dropdown directionTypeDropdown;
    public TMP_Dropdown DirectionTypeDropdown => directionTypeDropdown;

    [SerializeField] private TMP_InputField speedInput;
    public TMP_InputField SpeedInput => speedInput;

    [SerializeField] private TMP_InputField numberOfObjectsInput;
    public TMP_InputField NumberOfObjectsInput => numberOfObjectsInput;

    [SerializeField] private TMP_InputField objectSizeInput;
    public TMP_InputField ObjectSizeInput => objectSizeInput;

    [SerializeField] private TMP_InputField objectColorInput;
    public TMP_InputField ObjectColorInput => objectColorInput;

    [Header("Buttons")]
    [SerializeField] private Button saveConfigurationButton;
    public Button SaveConfigurationButton => saveConfigurationButton;

    [SerializeField] private Button cancelButton;
    public Button CancelButton => cancelButton;

    [SerializeField] private Button uploadTargetLocationsButton;
    public Button UploadTargetLocationsButton => uploadTargetLocationsButton;

    [SerializeField] private TMP_Text uploadedFileNameText;
    public TMP_Text UploadedFileNameText => uploadedFileNameText;

    [SerializeField] private Button modifyConfigurationButton;
    public Button ModifyConfigurationButton => modifyConfigurationButton;

    private List<Vector3> _tempTargetLocations = new List<Vector3>();

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
        //do something eventually
    }

    public void onVideoUpload()
    {
        //do something eventually
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
                TargetFlickerFrequency = SafeParse(targetFlickerFrequency.text, 0)
            },
            OffsetSettings = new OffsetSettings
            {
                OffsetType = offsetTypeDropdown.options[offsetTypeDropdown.value].text,
                OffsetValues = new Vector3(
                    SafeParse(offsetXInput.text, 0),
                    SafeParse(offsetYInput.text, 0),
                    SafeParse(offsetZInput.text, 0)
                ),
                TargetProximity = SafeParse(targetRangeInput.text, 0)
            },
            BackgroundSettings = new BackgroundSettings
            {
                BackgroundType = backgroundTypeDropdown.options[backgroundTypeDropdown.value].text,
                ImageBackground = null,
                VideoBackground = null,
                Direction = directionTypeDropdown.options[directionTypeDropdown.value].text,
                Speed = SafeParse(speedInput.text, 0),
                NumberOfObjects = (int)SafeParse(numberOfObjectsInput.text, 0)
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

        offsetTypeDropdown.value = 0;
        offsetXInput.text = "0";
        offsetYInput.text = "0";
        offsetZInput.text = "0";
        targetRangeInput.text = "0";

        backgroundTypeDropdown.value = 0;
        directionTypeDropdown.value = 0;
        speedInput.text = "0";
        numberOfObjectsInput.text = "0";
        objectSizeInput.text = "0";
        objectColorInput.text = "#FFFFFF";

        uploadedImageFileNameText.text = "No file uploaded";
        uploadedVideoFileNameText.text = "No file uploaded";
        uploadedFileNameText.text = "No file uploaded";
    }

    public void OnGoHomeClicked() {
        ResetInputs();
        SceneManager.LoadScene("HomeScreen");
    }

}
