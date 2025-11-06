using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class CSVFileLoader : IFileLoader {
    
    public override List<TrackingData> loadFile(string path) {
        List<TrackingData> trackingData = new List<TrackingData>();
        Dictionary<string, int> headerMap = new Dictionary<string, int>();
        try {
            using (StreamReader reader = new StreamReader(path)) {
                string header = reader.ReadLine();
                string[] headerParts = header.Split(',');
                for (int i = 0; i < headerParts.Length; i++) {
                    headerMap[headerParts[i].Trim()] = i;
                }
                while (!reader.EndOfStream) {
                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(',');

                    float GetValue(string key, float defaultVal = 0f) =>
                            headerMap.ContainsKey(key) ? float.Parse(parts[headerMap[key]]) : defaultVal;

                    TrackingData dataPoint = new TrackingData();

                    dataPoint.timeStamp = (long)GetValue("Timestamp");
                    dataPoint.leftHandPos = new Vector3(GetValue("Lx"), GetValue("Ly"), GetValue("Lz"));
                    dataPoint.rightHandPos = new Vector3(GetValue("Rx"), GetValue("Ry"), GetValue("Rz"));
                    dataPoint.leftHandRotation = new Quaternion(GetValue("LqX"), GetValue("LqY"), GetValue("LqZ"), GetValue("LqW"));
                    dataPoint.rightHandRotation = new Quaternion(GetValue("RqX"), GetValue("RqY"), GetValue("RqZ"), GetValue("RqW"));
                    trackingData.Add(dataPoint);
                }
            }
            
        } catch (System.Exception e) {
            Debug.LogError($"Error reading {path}: {e.Message}");
        }

        return trackingData;
    }

}
