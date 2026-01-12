using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class LoadTrialSettings : Singleton<LoadTrialSettings>
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

    public bool GetShowHands()
    {
        if (currentTrialData != null)
        {
            return currentTrialData.VisibilitySettings.ShowHands;
        }
        return true;
    }

    public bool GetShowTargets()
    {
        if (currentTrialData != null)
        {
            return currentTrialData.VisibilitySettings.ShowTargets;
        }
        return true;
    }

    public float GetTargetVisibleTime()
    {
        if (currentTrialData != null)
        {
            return currentTrialData.VisibilitySettings.TargetVisibleTime;
        }
        return 0.0f;
    }

    public float GetHandVisibleTime()
    {
        if (currentTrialData != null)
        {
            return currentTrialData.VisibilitySettings.HandVisibleTime;
        }
        return 0.0f;
    }

    public string GetOffsetType()
    {
        if (currentTrialData != null)
        {
            return currentTrialData.OffsetSettings.OffsetType;
        }
        return "None";
    }

    public Vector3 GetOffsetValues()
    {
        if (currentTrialData != null)
        {
            return currentTrialData.OffsetSettings.OffsetValues;
        }
        return Vector3.zero;
    }

    public float GetTargetProximity()
    {
        if (currentTrialData != null)
        {
            return currentTrialData.OffsetSettings.TargetProximity;
        }
        return 0.0f;
    }
}
