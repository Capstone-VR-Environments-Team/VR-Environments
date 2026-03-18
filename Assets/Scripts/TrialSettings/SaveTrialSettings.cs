using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Runtime.CompilerServices;

public class SaveTrialSettings : MonoBehaviour
{
    [Header("Configuration Name")]
    public TMP_InputField configurationNameInput;
    public TMP_Text fileUploaded;

    [Header("Visibility Settings")]

    public TMP_Dropdown handsVisibility;
    public TMP_Dropdown targetsVisibility;
    public TMP_InputField handFlickerFrequency;
    public TMP_InputField targetFlickerFrequency;


    [Header("Offset Settings")]
    public TMP_Dropdown offsetTypeDropdown;
    public TMP_InputField offsetXInput;
    public TMP_InputField offsetYInput;
    public TMP_InputField offsetZInput;
    public TMP_InputField targetRangeInput;

    [Header("Background Settings")]
    public TMP_Dropdown backgroundTypeDropdown;
    public TMP_Dropdown directionTypeDropdown;
    public TMP_InputField speedInput;
    public TMP_InputField numberOfObjectsInput;

    private List<Vector3> _tempTargetLocations = new List<Vector3>();

    public void OnUploadLocationsClicked()
    {
        string filePath = FileSelector.getFilePath(SessionManager.BaseDataPath, new string[] { "json", "csv" });
        var (importedData, fileName) = FileManager.LoadFromFile<TargetImportData>(filePath);
        if (importedData != null && importedData.targets != null)
        {
            Debug.Log("filename: " + fileName);
            fileUploaded.SetText(fileName);
            _tempTargetLocations = importedData.targets;
        }
        else
        {
            fileUploaded.SetText("File Upload Failed");
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
                HandsVisibilityType = handsVisibility.options[handsVisibility.value].text,
                TargetVisibilityType = targetsVisibility.options[targetsVisibility.value].text,
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
}

