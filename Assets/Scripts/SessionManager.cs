using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SessionManager : Singleton<SessionManager>
{
    public static string BaseDataPath = Application.persistentDataPath;
    TrialSessionInformation _trialSessionInformation;
    TrialSettingsData _settings;
    string _collectedDataDirectoryPath;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTrialSessionInformation(TrialSessionInformation info) {
        _trialSessionInformation = info;
        _settings = info.TrialSettings;
    }

    public TrialSessionInformation GetTrialSessionInformation() {
        return _trialSessionInformation;
    }

    public string SaveSessionInformation(CollectedTimingData collectedTimingData) {
        TrialSession trialSession = new TrialSession {
            TrialSessionInformation = _trialSessionInformation,
            CollectedTimingData = collectedTimingData
        };

        string fileName = _trialSessionInformation.SessionName + "-" + _trialSessionInformation.ParticipantID;
        CreateSaveDirectory(fileName);
        string filePath = Path.Combine(_collectedDataDirectoryPath, fileName + ".json");
        FileManager.SaveJsonFile(trialSession, filePath);
        return _collectedDataDirectoryPath;
    }

    private void CreateSaveDirectory(string name) {
        string folderName = $"{name}_{DateTime.Now:yyyyMMdd_HHmmss}";

        string rootPath = Path.Combine(Application.persistentDataPath, "TrialRuns");

        _collectedDataDirectoryPath = Path.Combine(rootPath, folderName);

        if (!Directory.Exists(_collectedDataDirectoryPath)) {
            Directory.CreateDirectory(_collectedDataDirectoryPath);
            Debug.Log($"Directory created at: {_collectedDataDirectoryPath}");
        }
    }

    public void SaveSettingsFile<T>(T data, string fileName) {
        string filePath = Path.Combine(BaseDataPath, "TrialFiles", fileName + ".json");
        FileManager.SaveJsonFile(data, filePath);
        Debug.Log($"JSON file saved to: {filePath}");
    }

    public void ResetFileManager() {
        _collectedDataDirectoryPath = null;
        _trialSessionInformation = null;
        _settings = null;
    }

    public static float GetRandomOffset(float standardDeviation) {
        float u1 = 1.0f - UnityEngine.Random.value;
        float u2 = 1.0f - UnityEngine.Random.value;

        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

        return (float)(randStdNormal * standardDeviation);
    }

    public TrialSettingsData GetTrialSettings() {
        return _settings;
    }

    public List<Vector3> GetLoadedTargets() {
        
        if (_trialSessionInformation.TrialSettings != null) {
            return _trialSessionInformation.TrialSettings.TargetLocations;
        }
        return new List<Vector3>();
    }

    public bool GetShowHands() {
        if (_settings != null) {
            return _settings.VisibilitySettings.ShowHands;
        }
        return true;
    }

    public bool GetShowTargets() {
        if (_settings != null) {
            return _settings.VisibilitySettings.ShowTargets;
        }
        return true;
    }


    public float GetHandVisibleTime() {
        if (_settings != null) {
            return _settings.VisibilitySettings.HandVisibleTime;
        }
        return 0.0f;
    }

    public int GetOffsetType() {
        if (_settings != null) {
            if (_settings.OffsetSettings.OffsetType == "NONE") {
                return 0;
            } else if (_settings.OffsetSettings.OffsetType == "STATIC") {
                return 1;
            } else if (_settings.OffsetSettings.OffsetType == "RANDOM") {
                return 2;
            }
        }
        return -1;
    }



    public Vector3 GetOffsetValues() {
        if (_settings != null) {
            int offsetType = GetOffsetType();
            if (offsetType == 0) {
                return Vector3.zero;
            } else if (offsetType == 2) {
                float xOffset = GetRandomOffset(_settings.OffsetSettings.OffsetValues.x);
                float yOffset = GetRandomOffset(_settings.OffsetSettings.OffsetValues.y);
                float zOffset = GetRandomOffset(_settings.OffsetSettings.OffsetValues.z);
                return new Vector3(xOffset, yOffset, zOffset);
            }
            return _settings.OffsetSettings.OffsetValues;
        }
        return Vector3.zero;
    }

    public float GetTargetProximity() {
        if (_settings != null) {
            return (_settings.OffsetSettings.TargetProximity / 100.0f); // Convert to cm
        }
        return 0.0f;
    }
}
