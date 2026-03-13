using System;
using System.Collections.Generic;
using UnityEngine;

public enum AnalysisMode {
    LineToTarget,
    PointToTarget
}

[Serializable]
public class JsonWrapper : IJsonable {
    public TrialSessionInformation TrialSessionInformation;
    public CollectedTimingData CollectedTimingData;
}

[Serializable]
public class Note {
    public double time;
    public string content;
}

[Serializable]
public class SegmentAnalysisResult {
    public int SegmentIndex { get; set; }
    public Vector3 StartPoint { get; set; }
    public Vector3 EndPoint { get; set; }
    public GeometryResults GeometryData { get; set; }

    public AnalysisMode Mode { get; set; }
}

public class SlicingResult {
    public List<SegmentAnalysisResult> SegmentResults { get; set; } = new List<SegmentAnalysisResult>();
    public List<double> SearchTimes { get; set; } = new List<double>();
    public List<double> SearchTimeTimestamps { get; set; } = new List<double>();
}


public class TargetAnalysisResults {
    public Statistics targetToTargetTimes;
    public Statistics searchTimes;
    public Statistics preSearchTimes;
}

public class DeviationData {
    public Statistics statsDist;
    public Statistics statsX;
    public Statistics statsY;
    public Statistics statsZ;
}

[Serializable]
public class RawData : IJsonable {
    public List<TrackingData> trackingData;

    public void From2dList(List<List<string>> data) {
        List<TrackingData> trackingData = new List<TrackingData>();
        Dictionary<string, int> headerMap = new Dictionary<string, int>();
        List<string> headerParts = data[0];
        for (int i = 0; i < headerParts.Count; i++) {
            headerMap[headerParts[i].Trim()] = i;
        }
        for (int i = 1; i < data.Count; i++) {
            List<string> row = data[i];
            if (row == null || row.Count == 0) continue;

            if (row.Count <= headerMap.Count) {
                Debug.LogWarning($"Target Import: Row {i} skipped. It does not have enough columns.");
                continue;
            }

            float GetValue(string key, float defaultVal = 0f) =>
            headerMap.ContainsKey(key) ? float.Parse(row[headerMap[key]]) : defaultVal;

            TrackingData dataPoint = new TrackingData();

            dataPoint.timeStamp = (long)GetValue("Timestamp");
            dataPoint.leftHandPos = new Vector3(GetValue("Lx"), GetValue("Ly"), GetValue("Lz"));
            dataPoint.rightHandPos = new Vector3(GetValue("Rx"), GetValue("Ry"), GetValue("Rz"));
            dataPoint.leftHandRotation = new Quaternion(GetValue("LqX"), GetValue("LqY"), GetValue("LqZ"), GetValue("LqW"));
            dataPoint.rightHandRotation = new Quaternion(GetValue("RqX"), GetValue("RqY"), GetValue("RqZ"), GetValue("RqW"));
            trackingData.Add(dataPoint);
        }

        this.trackingData = trackingData;
    }
}
