using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Windows.Forms.VisualStyles;

public class LoadTrialSettings : Singleton<LoadTrialSettings>
{

    [Header("Data Storage")]
    public TrialSettingsData currentTrialData;

    public void OnUploadSettingsClicked()
    {
        TrialSettingsData loadedData = FileManager.Instance.LoadFromFile<TrialSettingsData>("TrialSettings");

        if (loadedData != null)
        {
            currentTrialData = loadedData;

            Debug.Log($"Loaded Configuration: {currentTrialData.ConfigurationName}");
            Debug.Log($"Target Count: {currentTrialData.TargetLocations.Count}");

        }
        else
        {
            Debug.LogError("Failed to load settings file.");
        }
    }

    public TrialSettingsData GetTrialSettings()
    {
        return currentTrialData;
    }

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

    public int GetOffsetType()
    {
        if (currentTrialData != null)
        {
            if (currentTrialData.OffsetSettings.OffsetType == "NONE")
            {
                return 0;
            }
            else if (currentTrialData.OffsetSettings.OffsetType == "STATIC")
            {
                return 1;
            }
            else if (currentTrialData.OffsetSettings.OffsetType == "RANDOM")
            {
                return 2;
            }
        }
        return -1;
    }

    public static float GetRandomOffset(float standardDeviation)
    {
        float u1 = 1.0f - UnityEngine.Random.value;
        float u2 = 1.0f - UnityEngine.Random.value;

        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

        return (float)(randStdNormal * standardDeviation);
    }

    public Vector3 GetOffsetValues()
    {
        if (currentTrialData != null)
        {
            int offsetType = GetOffsetType();
            if (offsetType == 0)
            {
                return Vector3.zero;
            } else if (offsetType == 2)
            {
                float xOffset = GetRandomOffset(currentTrialData.OffsetSettings.OffsetValues.x);
                float yOffset = GetRandomOffset(currentTrialData.OffsetSettings.OffsetValues.y);
                float zOffset = GetRandomOffset(currentTrialData.OffsetSettings.OffsetValues.z);
                return new Vector3(xOffset, yOffset, zOffset);
            }
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
