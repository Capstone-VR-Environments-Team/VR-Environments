using System;
using UnityEngine;

[Serializable]
public class KeyPoint {
    public double Timestamp { get; set; }
    public Vector3 Position { get; set; }
}

[Serializable]
public class SegmentAnalysisResult {
    public int SegmentIndex { get; set; }
    public Vector3 StartPoint { get; set; }
    public Vector3 EndPoint { get; set; }
    public GeometryResults GeometryData { get; set; }
}