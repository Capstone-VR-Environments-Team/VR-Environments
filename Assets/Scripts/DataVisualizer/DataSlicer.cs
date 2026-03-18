using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DataSlicer {
    public static SlicingResult AnalyzeSegments(
        List<HitEvent> keyPoints,
        List<HitEvent> proximityPoints,
        List<Vector3> rawDataPoints,
        List<double> rawDataTimes) {
        var results = new SlicingResult();

        // Validation
        if (keyPoints == null || keyPoints.Count < 2) return results;
        if (rawDataPoints == null || rawDataPoints.Count == 0) return results;
        if (rawDataPoints.Count != rawDataTimes.Count) return results;

        var sortedKeys = keyPoints.OrderBy(k => k.time).ToList();

        var sortedProx = proximityPoints != null
            ? proximityPoints.OrderBy(p => p.time).ToList()
            : new List<HitEvent>();

        // Cursor persists outside the loop for O(N) performance
        int cursor = 0;
        int totalCount = rawDataTimes.Count;

        for (int i = 0; i < sortedKeys.Count - 1; i++) {
            var startKey = sortedKeys[i];
            var endKey = sortedKeys[i + 1];

            var proxHit = sortedProx.FirstOrDefault(p => p.time > startKey.time && p.time < endKey.time);

            if (proxHit != null) {
                // --- SPLIT SEGMENT CASE ---
                
                // PART A: Approach (Start -> Proximity)
                var approachData = ExtractChunk(startKey.time, proxHit.time, rawDataPoints, rawDataTimes, ref cursor);
                if (approachData != null) {
                    approachData.LinePointA = startKey.location;
                    approachData.LinePointB = endKey.location; // Direction is still towards final target
                    approachData.Mode = AnalysisMode.LINETOTARGET;
                    
                    results.SegmentResults.Add(new SegmentAnalysisResult {
                        SegmentIndex = i, 
                        GeometryData = GeometryAnalyzer.AnalyzeGeometry(approachData),
                        Mode = AnalysisMode.LINETOTARGET
                    });
                }

                // PART B: Homing (Proximity -> Target)
                var homingData = ExtractChunk(proxHit.time, endKey.time, rawDataPoints, rawDataTimes, ref cursor);
                if (homingData != null) {
                    homingData.LinePointA = startKey.location; // Keep same A for consistent Axis calculation
                    homingData.LinePointB = endKey.location;
                    homingData.Mode = AnalysisMode.POINTTOTARGET;

                    results.SegmentResults.Add(new SegmentAnalysisResult {
                        SegmentIndex = i,
                        GeometryData = GeometryAnalyzer.AnalyzeGeometry(homingData),
                        Mode = AnalysisMode.POINTTOTARGET
                    });
                }

            }
            else {
                // --- STANDARD SEGMENT CASE ---
                var chunk = ExtractChunk(startKey.time, endKey.time, rawDataPoints, rawDataTimes, ref cursor);
                if (chunk != null) {
                    chunk.LinePointA = startKey.location;
                    chunk.LinePointB = endKey.location;
                    chunk.Mode = AnalysisMode.LINETOTARGET;

                    results.SegmentResults.Add(new SegmentAnalysisResult {
                        SegmentIndex = i,
                        GeometryData = GeometryAnalyzer.AnalyzeGeometry(chunk),
                        Mode = AnalysisMode.LINETOTARGET
                    });
                }
            }
        }

        return results;
    }

    private static GeometryInputData ExtractChunk(double startTime, double endTime, List<Vector3> points, List<double> times, ref int cursor) {
        int total = times.Count;

        // Fast-forward
        while (cursor < total && times[cursor] < startTime) cursor++;
        int startIndex = cursor;

        // Scan
        while (cursor < total && times[cursor] <= endTime) cursor++;
        int count = cursor - startIndex;

        if (count <= 0) return null;

        return new GeometryInputData {
            Points = points.GetRange(startIndex, count),
            Timestamps = times.GetRange(startIndex, count)
        };
    }
}

