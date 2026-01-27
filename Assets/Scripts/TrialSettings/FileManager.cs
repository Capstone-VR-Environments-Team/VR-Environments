using System;
using System.IO;
using System.Web;
using UnityEngine;

public  class FileManager: Singleton<FileManager>
{
    private string _collectedDataDirectoryPath;

    private TrialSessionInformation _trialSessionInformation;

    // Save JSON to a file
    public void SaveSettingsFile<T>(T data, string fileName)
    {
        string json = JsonUtility.ToJson(data, true);
        string directoryPath = Path.Combine(Application.persistentDataPath, "TrialFiles");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string filePath = Path.Combine(directoryPath, fileName + ".json");
        File.WriteAllText(filePath, json);

        Debug.Log($"JSON file saved to: {filePath}");
    }

    public void SetTrialSessionInformation(TrialSessionInformation info)
    {
        _trialSessionInformation = info;
    }

    public string SaveSessionInformation()
    {
        string json = JsonUtility.ToJson(_trialSessionInformation, true);
        string fileName = _trialSessionInformation.SessionName + "-" + _trialSessionInformation.ParticipantID;
        CreateSaveDirectory(fileName);
        string filePath = Path.Combine(_collectedDataDirectoryPath, fileName + ".json");
        File.WriteAllText(filePath, json);

        return _collectedDataDirectoryPath;
    }

    public (T data, string fileName) LoadFromFile<T>()
    {
        string filePath = FileSelector.getFilePath(Application.persistentDataPath, "json");

        if (!File.Exists(filePath))
        {
            Debug.LogError($"File not found at: {filePath}");
            return default;
        }

        string json = File.ReadAllText(filePath);
        string fileName = Path.GetFileName(filePath);
        return (JsonUtility.FromJson<T>(json), fileName);
    }

    public void CreateSaveDirectory(string name)
    {
        string folderName = $"{name}_{DateTime.Now:yyyyMMdd_HHmmss}";

        string rootPath = Path.Combine(Application.persistentDataPath, "TrialRuns");

        _collectedDataDirectoryPath = Path.Combine(rootPath, folderName);

        if (!Directory.Exists(_collectedDataDirectoryPath))
        {
            Directory.CreateDirectory(_collectedDataDirectoryPath);
            Debug.Log($"Directory created at: {_collectedDataDirectoryPath}");
        }
    }

}
