using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DataSlicer {
    public static List<SegmentAnalysisResult> AnalyzeSegments(
        List<KeyPoint> keyPoints,
        List<Vector3> rawDataPoints,
        List<double> rawDataTimes) {
        var results = new List<SegmentAnalysisResult>();

        // Validation
        if (keyPoints == null || keyPoints.Count < 2) return results;
        if (rawDataPoints == null || rawDataPoints.Count == 0) return results;
        if (rawDataPoints.Count != rawDataTimes.Count) return results;

        var sortedKeys = keyPoints.OrderBy(k => k.Timestamp).ToList();

        // Cursor persists outside the loop for O(N) performance
        int cursor = 0;
        int totalCount = rawDataTimes.Count;

        for (int i = 0; i < sortedKeys.Count - 1; i++) {
            var startKey = sortedKeys[i];
            var endKey = sortedKeys[i + 1];

            // Fast-forward to start of segment
            while (cursor < totalCount && rawDataTimes[cursor] <= startKey.Timestamp) {
                cursor++;
            }

            int startIndex = cursor;

            // Scan to end of segment
            while (cursor < totalCount && rawDataTimes[cursor] <= endKey.Timestamp) {
                cursor++;
            }

            // Bulk copy data if points found
            int count = cursor - startIndex;
            if (count > 0) {
                var chunkInput = new GeometryInputData {
                    LinePointA = startKey.Position,
                    LinePointB = endKey.Position,
                    Points = rawDataPoints.GetRange(startIndex, count),
                    Timestamps = rawDataTimes.GetRange(startIndex, count)
                };

                results.Add(new SegmentAnalysisResult {
                    SegmentIndex = i,
                    StartPoint = startKey.Position,
                    EndPoint = endKey.Position,
                    GeometryData = GeometryAnalyzer.AnalyzeGeometry(chunkInput)
                });
            }
        }

        return results;
    }
}