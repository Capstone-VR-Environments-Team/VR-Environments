using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SaveTrialSettings : MonoBehaviour
{
    [Header("Configuration Name")]
    public TMP_InputField configurationNameInput;
    public TMP_Text fileUploaded;

    [Header("Visibility Settings")]

    public Toggle showTargetsToggle;
    public TMP_InputField targetVisibleTimeInput;
    public Toggle showHandsToggle;
    public TMP_InputField handVisibleTimeInput;

    [Header("Offset Settings")]
    public TMP_Dropdown offsetTypeDropdown;
    public TMP_InputField offsetXInput;
    public TMP_InputField offsetYInput;
    public TMP_InputField offsetZInput;
    public TMP_InputField targetRangeInput;

    private List<Vector3> _tempTargetLocations = new List<Vector3>();

    public void OnUploadLocationsClicked()
    {
        var (importedData, fileName) = FileManager.Instance.LoadFromFile<TargetImportData>();
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

    public void OnSaveButtonClicked()
    {
        TrialSettingsData trial = new TrialSettingsData
        {
            ConfigurationName = configurationNameInput.text,
            VisibilitySettings = new VisibilitySettings
            {
                ShowTargets = showTargetsToggle.isOn,
                TargetVisibleTime = SafeParse(targetVisibleTimeInput.text, 0),
                ShowHands = showHandsToggle.isOn,
                HandVisibleTime = SafeParse(handVisibleTimeInput.text, 0)
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
            TargetLocations = _tempTargetLocations
        };
        FileManager.Instance.SaveSettingsFile(trial, trial.ConfigurationName);
    }
}

