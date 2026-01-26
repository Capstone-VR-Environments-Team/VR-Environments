using System.Collections.Generic;
using UnityEngine;

public class GeometryInputData {
    public AnalysisMode Mode { get; set; } = AnalysisMode.LineToTarget;
    public Vector3 LinePointA { get; set; }
    public Vector3 LinePointB { get; set; }
    public List<Vector3> Points { get; set; }
    public List<double> Timestamps { get; set; }

    public GeometryInputData() {
        Points = new List<Vector3>();
        Timestamps = new List<double>();
    }
}