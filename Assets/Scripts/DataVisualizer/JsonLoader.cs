using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JsonLoader {

    public static JsonWrapper LoadKeyPoints(string filePath) {
        var data = new JsonWrapper();

        if (!File.Exists(filePath)) {
            Debug.LogError("JSON file not found: " + filePath);
            return data;
        }

        string jsonContent = File.ReadAllText(filePath);
        data = JsonUtility.FromJson<JsonWrapper>(jsonContent);

        return data;
    }
}