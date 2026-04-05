using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[Serializable]
public class TrialSession
{
    public TrialSessionInformation TrialSessionInformation;
    public CollectedTimingData CollectedTimingData;
}

[Serializable]
public class TrialSessionInformation
{
    public string SessionName;
    public string ParticipantID;
    public string Notes;
    public TrialSettingsData TrialSettings;
}

[Serializable]
public class TrialSettingsData : IJsonable
{
    public string ConfigurationName;
    public VisibilitySettings VisibilitySettings;
    public OffsetSettings OffsetSettings;
    public BackgroundSettings BackgroundSettings;
    public TargetSettings TargetSettings;
    public ColorSettings ColorSettings;
    public List<Vector3> TargetLocations;
}

[Serializable]
public class VisibilitySettings
{
    public string HandsVisibilityType;
    public string TargetVisibilityType;
    public float HandsFlickerOnDuration;
    public float HandsFlickerOffDuration;
    public float TargetFlickerOnDuration;
    public float TargetFlickerOffDuration;
}

[Serializable]
public class OffsetSettings
{
    public string OffsetType;
    public Vector3 OffsetValues;
    public float TargetProximity;
    public bool ShowHandsInProximity;
}

[Serializable]
public class BackgroundSettings
{
    public string BackgroundType;
    public string ImageBackground;
    public string VideoBackground;
    public string Direction;
    public float Speed;
    public int NumberOfObjects;
    public Vector3 ObjectSize;
}

[Serializable]
public class TargetSettings
{
    public int TimeBeforeStart;
    public float TargetSize;
}

[Serializable]
public class ColorSettings
{
    public string BackgroundObjectColor;
    public string LeftHandColor;
    public string RightHandColor;
    public string TargetColor;
}

[Serializable]
public class TargetImportData : IJsonable
{
    public List<Vector3> targets;

    public void From2dList(List<List<string>> data) {
        targets = new List<Vector3>();

        if (data == null || data.Count == 0) {
            Debug.LogError("Target Import Failed: The provided data list is null or completely empty.");
            return;
        }

        List<string> headers = data[0];
        if (headers.Count < 3) {
            Debug.LogError("Target Import Failed: The header row has fewer than 3 columns.");
            return;
        }

        int xIndex = -1, yIndex = -1, zIndex = -1;
        for (int i = 0; i < headers.Count; i++) {
            string header = headers[i].Trim().ToLower();
            if (header == "x") xIndex = i;
            else if (header == "y") yIndex = i;
            else if (header == "z") zIndex = i;
        }

        if (xIndex == -1 || yIndex == -1 || zIndex == -1) {
            Debug.LogError("Target Import Failed: The header row is missing 'x', 'y', or 'z'.");
            return;
        }

        for (int i = 1; i < data.Count; i++) {
            List<string> row = data[i];

            if (row == null || row.Count == 0) continue;

            if (row.Count <= xIndex || row.Count <= yIndex || row.Count <= zIndex) {
                Debug.LogWarning($"Target Import: Row {i} skipped. It does not have enough columns.");
                continue;
            }

            bool xSuccess = float.TryParse(row[xIndex].Trim(), out float xVal);
            bool ySuccess = float.TryParse(row[yIndex].Trim(), out float yVal);
            bool zSuccess = float.TryParse(row[zIndex].Trim(), out float zVal);

            if (xSuccess && ySuccess && zSuccess) {
                targets.Add(new Vector3(xVal, yVal, zVal));
            } else {
                Debug.LogWarning($"Target Import: Row {i} skipped. Invalid number format detected -> x: '{row[xIndex]}', y: '{row[yIndex]}', z: '{row[zIndex]}'");
            }
        }

        Debug.Log($"Target Import Complete: Successfully loaded {targets.Count} targets.");
    }
}



