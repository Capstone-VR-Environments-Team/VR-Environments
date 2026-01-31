using System;
using System.IO;
using System.Text;
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

    public TrialSessionInformation GetTrialSessionInformation()
    {
        return _trialSessionInformation;
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
        string filePath = FileSelector.getFilePath(Application.persistentDataPath, "json,csv");

        if (!File.Exists(filePath))
        {
            Debug.LogError($"File not found at: {filePath}");
            return default;
        }

        string fileContent = File.ReadAllText(filePath);
        string fileName = Path.GetFileName(filePath);
        string extension = Path.GetExtension(filePath).ToLower();

        string jsonToProcess = fileContent;

        if (extension == ".csv")
        {
            jsonToProcess = ConvertCsvToTargetJson(fileContent);
        }
        else if (extension != ".json")
        {
            Debug.LogWarning("Unknown file type loaded. Attempting to parse as JSON...");
        }

        return (JsonUtility.FromJson<T>(jsonToProcess), fileName);
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

    public string ConvertCsvToTargetJson(string csvContent)
    {
        StringBuilder jsonBuilder = new StringBuilder();

        jsonBuilder.Append("{\"targets\": [");

        string[] lines = csvContent.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        bool isFirstEntry = true;

        foreach (string line in lines)
        {
            string[] values = line.Split(',');

            if (values.Length >= 3)
            {
                if (float.TryParse(values[0], out float x) &&
                    float.TryParse(values[1], out float y) &&
                    float.TryParse(values[2], out float z))
                {
                    if (!isFirstEntry)
                    {
                        jsonBuilder.Append(",");
                    }

                    jsonBuilder.Append(string.Format("{{\"x\": {0}, \"y\": {1}, \"z\": {2}}}", x, y, z));

                    isFirstEntry = false;
                }
            }
        }
        jsonBuilder.Append("]}");

        return jsonBuilder.ToString();
    }

    public void ResetFileManager()
    {
        _collectedDataDirectoryPath = null;
        _trialSessionInformation = null;
    }

}
