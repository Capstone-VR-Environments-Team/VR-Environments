using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SaveTrialSettings : MonoBehaviour
{
    [Header("Configuration Name")]
    public TMP_InputField configurationNameInput;

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
        TargetImportData importedData = JsonFileManager.LoadFromFile<TargetImportData>("ImportedTargets");
        if (importedData != null && importedData.targets != null)
        {
            _tempTargetLocations = importedData.targets;
        }
        else
        {
            Debug.LogError("Failed to load target locations from file.");
        }
    }
    public void OnSaveButtonClicked()
    {
        TrialSettingsData trial = new TrialSettingsData
        {
            ConfigurationName = configurationNameInput.text,
            VisibilitySettings = new VisibilitySettings
            {
                ShowTargets = showTargetsToggle.isOn,
                TargetVisibleTime = float.Parse(targetVisibleTimeInput.text),
                ShowHands = showHandsToggle.isOn,
                HandVisibleTime = float.Parse(handVisibleTimeInput.text),
            },
            OffsetSettings = new OffsetSettings
            {
                OffsetType = offsetTypeDropdown.options[offsetTypeDropdown.value].text,
                OffsetValues = new Vector3(
                    float.Parse(offsetXInput.text),
                    float.Parse(offsetYInput.text),
                    float.Parse(offsetZInput.text)
                ),
                TargetProximity = float.Parse(targetRangeInput.text)
            },
            TargetLocations = _tempTargetLocations
        };
        JsonFileManager.SaveToFile(trial, trial.ConfigurationName);
    }
}

