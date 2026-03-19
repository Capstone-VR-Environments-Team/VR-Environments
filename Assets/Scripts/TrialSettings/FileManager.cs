using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class FileManager {
    
    public static (T data, string fileName) LoadFromFile<T>(string filePath) where T : IJsonable, new() {

        if (!File.Exists(filePath)) {
            Debug.LogError($"File not found at: {filePath}");
            return default;
        }

        string fileContent = File.ReadAllText(filePath);
        string fileName = Path.GetFileName(filePath);
        string extension = Path.GetExtension(filePath).ToLower();

        string jsonToProcess = fileContent;

        if (extension == ".csv") {
            return (LoadCSVFile<T>(filePath), fileName); 
        } else if (extension == ".json") {
            return (LoadJsonFile<T>(filePath), fileName);
        } else {
            Debug.LogError($"Incorrect file type! Should be .json");
            return default;
        }
    }

    public static T LoadJsonFile<T>(string filePath){
        string fileContent = File.ReadAllText(filePath);
        return JsonUtility.FromJson<T>(fileContent);
    }

    public static T LoadCSVFile<T>(string filePath) where T : IJsonable, new() {
        List<List<string>> data = new List<List<string>>();
        StreamReader reader = new StreamReader(filePath);
        while (!reader.EndOfStream) {
            string line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] parts = line.Split(',');
            data.Add(parts.ToList());
        }
        T results = new T();
        results.From2dList(data);
        return results;
    }

    public static bool SaveJsonFile<T>(T data, string filePath) {

        string json = JsonUtility.ToJson(data, true);
        string directoryPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath)) {
            Directory.CreateDirectory(directoryPath);
        }

        File.WriteAllText(filePath, json);
        return true;
    }

    //public void Load360Media()
    //{
    //    string filePath = FileSelector.getFilePath(Application.persistentDataPath, "png,jpg,jpeg,mp4,mov,mkv");

    //    if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
    //    {
    //        Debug.LogWarning("No file selected or file does not exist.");
    //        return;
    //    }

    //    string extension = Path.GetExtension(filePath).ToLower();

    //    if (extension == ".mp4" || extension == ".mov" || extension == ".mkv")
    //    {
    //        if (videoPlayer == null)
    //        {
    //            Debug.LogError("VideoPlayer reference is missing in FileManager.");
    //            return;
    //        }

    //        videoPlayer.source = VideoSource.Url;
    //        videoPlayer.url = filePath;
    //        videoPlayer.Play();

    //        Debug.Log($"Loaded 360 Video from: {filePath}");
    //    }
    //    else if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
    //    {
    //        if (skyboxMaterial == null)
    //        {
    //            Debug.LogError("Skybox Material reference is missing in FileManager.");
    //            return;
    //        }

    //        if (videoPlayer != null && videoPlayer.isPlaying)
    //        {
    //            videoPlayer.Stop();
    //        }
    //        byte[] fileData = File.ReadAllBytes(filePath);
    //        Texture2D tex = new Texture2D(2, 2);
    //        tex.LoadImage(fileData); 
    //        skyboxMaterial.mainTexture = tex;

    //        Debug.Log($"Loaded 360 Image from: {filePath}");
    //    }
    //    else
    //    {
    //        Debug.LogError($"Unsupported media type selected: {extension}");
    //    }
    //}

   

}
