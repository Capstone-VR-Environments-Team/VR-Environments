using System.IO;
using UnityEngine;

public static class JsonFileManager
{
    // Save JSON to a file
    public static void SaveToFile<T>(T data, string fileName)
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

    // Load JSON from a file
    public static T LoadFromFile<T>(string fileName)
    {
        string filePath = FileSelector.getFilePath(Application.persistentDataPath, "json");

        if (!File.Exists(filePath))
        {
            Debug.LogError($"File not found at: {filePath}");
            return default;
        }

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<T>(json);
    }
}
