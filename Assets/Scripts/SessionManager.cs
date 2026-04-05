using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SessionManager : Singleton<SessionManager>
{
    private static string _path;
    public static string BaseDataPath { get { _path ??= Application.persistentDataPath; return _path; } set => _path = value; }
    TrialSessionInformation _trialSessionInformation;
    TrialSettingsData _settings;
    private long _startTime;
    string _collectedDataDirectoryPath;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
    }

    private void OnEnable() {
        EventBus.StartExperiment += StartTrial;
    }

    private void OnDisable() {
        EventBus.StartExperiment -= StartTrial;
    }

    public void StartTrial(Vector3 headsetPosition) {
        _startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
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
        string folderPath = FileSelector.getFolderPath(rootPath);
        _collectedDataDirectoryPath = Path.Combine(folderPath, folderName);

        if (!Directory.Exists(_collectedDataDirectoryPath)) {
            Directory.CreateDirectory(_collectedDataDirectoryPath);
            Debug.Log($"Directory created at: {_collectedDataDirectoryPath}");
        }
    }

    public void SaveSettingsFile<T>(T data, string fileName) {
        string directoryPath = Path.Combine(Application.persistentDataPath, "TrialFiles");
        string filePath = FileSelector.getSaveFilePath(directoryPath, fileName, "json");
        FileManager.SaveJsonFile(data, filePath);
        Debug.Log($"JSON file saved to: {filePath}");
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

    public int GetOffsetType() {
        if (_settings != null) {
            if (_settings.OffsetSettings.OffsetType == "None") {
                return 0;
            } else if (_settings.OffsetSettings.OffsetType == "Fixed") {
                return 1;
            } else if (_settings.OffsetSettings.OffsetType == "Randomized") {
                return 2;
            }
        }
        return -1;
    }

    public String GetBackgroundType()
    {
        return _settings.BackgroundSettings.BackgroundType;
    }

    public String GetBackgroundImagePath()
    {
        return _settings.BackgroundSettings.ImageBackground;
    }

    public String GetBackgroundVideoPath()
    {
        return _settings.BackgroundSettings.VideoBackground;
    }

    public float GetHandFlickerOnDuration()
    {
        if (_settings != null)
        {
            return _settings.VisibilitySettings.HandsFlickerOnDuration;
        }
        return 0.0f;
    }
    public float GetHandFlickerOffDuration() {
        if (_settings != null) {
            return _settings.VisibilitySettings.HandsFlickerOffDuration;
        }
        return 0.0f;
    }

    public float GetTargetFlickerOnDuration()
    {
        if (_settings != null)
        {
            return _settings.VisibilitySettings.TargetFlickerOnDuration;
        }
        return 0.0f;
    }

    public float GetTargetFlickerOffDuration() {
        if (_settings != null) {
            return _settings.VisibilitySettings.TargetFlickerOffDuration;
        }
        return 0.0f;
    }

    public Vector3 GetOffsetValues() {
        if (_settings != null) {
            int offsetType = GetOffsetType();
            if (offsetType == 0)
            {
                return Vector3.zero;
            }
            else if (offsetType == 2)
            {
                float xOffset = GetRandomOffset(_settings.OffsetSettings.OffsetValues.x / 100.0f);
                float yOffset = GetRandomOffset(_settings.OffsetSettings.OffsetValues.y / 100.0f);
                float zOffset = GetRandomOffset(_settings.OffsetSettings.OffsetValues.z / 100.0f);
                return new Vector3(xOffset, yOffset, zOffset);
            }
            else if (offsetType == 1)
            {
                return _settings.OffsetSettings.OffsetValues / 100.0f;
            }
        }
        return Vector3.zero;
    }

    public float GetTargetProximity() {
        if (_settings != null) {
            return (_settings.OffsetSettings.TargetProximity / 100.0f); // Convert to cm
        }
        return 0.0f;
    }

    public string GetTrialName() {
        return _trialSessionInformation.SessionName;
    }

    public double GetTrialTime() {
        long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return (currentTime - _startTime);
    }

    public bool GetShowHandsInProximity()
    {
        if (_settings != null)
        {
            return _settings.OffsetSettings.ShowHandsInProximity;
        }
        return false;
    }

    public int GetHandsVisibilityType()
    {
        if (_settings != null)
        {
            if (_settings.VisibilitySettings.HandsVisibilityType == "Full")
            {
                return 2;
            }
            else if (_settings.VisibilitySettings.HandsVisibilityType == "Flicker")
            {
                return 1;
            }
            else if (_settings.VisibilitySettings.HandsVisibilityType == "None")
            {
                return 0;
            }
        }
        return -1;
    }

    public int GetTargetVisibilityType()
    {
        if (_settings != null)
        {
            if (_settings.VisibilitySettings.TargetVisibilityType == "Full")
            {
                return 2;
            }
            else if (_settings.VisibilitySettings.TargetVisibilityType == "Flicker")
            {
                return 1;
            }
            else if (_settings.VisibilitySettings.TargetVisibilityType == "None")
            {
                return 0;
            }
        }
        return -1;
    }

    public string GetLeftHandColor()
    {
        if (_settings != null)
        {
            return _settings.ColorSettings.LeftHandColor;
        }
        return "0000FF"; 
    }

    public string GetRightHandColor()
    {
        if (_settings != null)
        {
            return _settings.ColorSettings.RightHandColor;
        }
        return "FF0000";
    }

    public string GetTargetColor()
    {
        if (_settings != null)
        {
            return _settings.ColorSettings.TargetColor;
        }
        return "C0C0C0";
    }

    public int GetTimeBeforeStart()
    {
        if(_settings != null)
        {
            return _settings.TargetSettings.TimeBeforeStart;
        }
        return 0;
    }

    public float GetTargetSize() {
        if (_settings != null) {
            return _settings.TargetSettings.TargetSize / 100.0f;
        }
        return 0.0f;
    }

    public Vector3 getMovingBackgroundDirection() {
        int dirVal = (int)Enum.Parse<Direction>(_settings.BackgroundSettings.Direction);
        Vector3 direction = Vector3.zero;

        direction[dirVal / 2] = dirVal % 2 == 0 ? 1f : -1f;
        return direction;
    }

    public float getMovingBackgroundSpeed() {
        return _settings.BackgroundSettings.Speed;
    }

    public int getMovingBackgroundQuantity() {
        return _settings.BackgroundSettings.NumberOfObjects;
    }

    public Vector3 getMovingBackgroundSize() {
        return _settings.BackgroundSettings.ObjectSize / 100.0f;
    }
}
