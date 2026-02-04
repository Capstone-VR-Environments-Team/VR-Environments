using System;
using System.Collections.Generic;
using UnityEngine;

public enum AnalysisMode {
    LineToTarget,
    PointToTarget
}

[Serializable]
public class JsonWrapper {
    public TrialSessionInformation TrialSessionInformation;
    public List<KeyPoint> TargetHits;
    public List<KeyPoint> TargetProximityHits;
    public List<Note> Notes;
}

[Serializable]
public class Note {
    public double time;
    public string content;
}

[Serializable]
public class KeyPoint {
    public double time;
    public Vector3 location;
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
