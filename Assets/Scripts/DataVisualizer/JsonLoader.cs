using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JsonLoader {
    [Serializable]
    private class JsonWrapper {
        public List<HitEntry> TargetHits;
        public List<HitEntry> TargetProximityHits;
        public List<Note> Notes;
    }

    [Serializable]
    private class HitEntry {
        public double time;
        public Vector3 location;
    }

    [Serializable]
    private class Note {
        public double time;
        public string content;
    }

    public static List<KeyPoint> LoadKeyPoints(string filePath) {
        var keyPoints = new List<KeyPoint>();
        if (!File.Exists(filePath)) {
            Debug.LogError("JSON file not found: " + filePath);
            return keyPoints;
        }

        string jsonContent = File.ReadAllText(filePath);
        JsonWrapper data = JsonUtility.FromJson<JsonWrapper>(jsonContent);

        if (data != null && data.TargetHits != null) {
            foreach (var hit in data.TargetHits) {
                keyPoints.Add(new KeyPoint {
                    Timestamp = hit.time,
                    Position = hit.location
                });
            }
        }
        return keyPoints;
    }
}