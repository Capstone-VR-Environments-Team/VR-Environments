using System.Collections.Generic;

public static class ResultParser {
    // Flattens all segments into one continuous list of "Distance From Line"
    // Useful if you just want to graph "Error over Time" for the whole run.
    public static List<double> GetAllDistances(List<SegmentAnalysisResult> segments) {
        var allDistances = new List<double>();
        if (segments == null) return allDistances;

        foreach (var seg in segments) {
            if (seg.GeometryData != null) {
                allDistances.AddRange(seg.GeometryData.DistancesFromLine);
            }
        }
        return allDistances;
    }
}