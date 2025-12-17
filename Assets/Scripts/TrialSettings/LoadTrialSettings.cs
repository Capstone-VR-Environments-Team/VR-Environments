using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LoadTrialSettings : MonoBehaviour
{

    [Header("Data Storage")]
    public TrialSettingsData currentTrialData;

    public void OnUploadSettingsClicked()
    {
        TrialSettingsData loadedData = JsonFileManager.LoadFromFile<TrialSettingsData>("TrialSettings");

        if (loadedData != null)
        {
            currentTrialData = loadedData;

            Debug.Log($"Loaded Configuration: {currentTrialData.ConfigurationName}");
            Debug.Log($"Target Count: {currentTrialData.TargetLocations.Count}");

            // 3. (Optional) Apply settings immediately
            // ApplySettingsToGame(); 
        }
        else
        {
            Debug.LogError("Failed to load settings file.");
        }
    }

    // Example of how other scripts might access the targets later
    public List<Vector3> GetLoadedTargets()
    {
        if (currentTrialData != null)
        {
            return currentTrialData.TargetLocations;
        }
        return new List<Vector3>();
    }
}
