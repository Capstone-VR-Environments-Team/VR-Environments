using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DataSlicer {
    private const float LocationMatchTolerance = 0.02f;

    public static SlicingResult AnalyzeSegments(
        List<HitEvent> keyPoints,
        List<HitEvent> proximityPoints,
        List<HitEvent> previousSphereReentryPoints,
        List<HitEvent> previousSphereLeavePoints,
        List<Vector3> rawDataPoints,
        List<double> rawDataTimes) {

        SlicingResult results = new SlicingResult();

        if (keyPoints == null || keyPoints.Count < 2) {
            return results;
        }

        if (rawDataPoints == null || rawDataPoints.Count == 0) {
            return results;
        }

        if (rawDataPoints.Count != rawDataTimes.Count) {
            return results;
        }

        List<HitEvent> sortedKeys = keyPoints.OrderBy(k => k.time).ToList();
        List<HitEvent> sortedProx = proximityPoints != null
            ? proximityPoints.OrderBy(p => p.time).ToList()
            : new List<HitEvent>();

        Dictionary<int, HitEvent> proximityBySegment = BuildProximityBySegment(sortedKeys, sortedProx);
        List<SphereOccupancyWindow> occupancyWindows = BuildPreviousSphereWindows(sortedKeys, previousSphereReentryPoints, previousSphereLeavePoints);

        int cursor = 0;

        for (int i = 0; i < sortedKeys.Count - 1; i++) {
            HitEvent startKey = sortedKeys[i];
            HitEvent endKey = sortedKeys[i + 1];

            proximityBySegment.TryGetValue(i, out HitEvent proxHit);

            double intervalStart = startKey.time;
            List<SphereOccupancyWindow> windowsInSegment = occupancyWindows
                .Where(window =>
                    window.EndTime > startKey.time
                    && window.StartTime < endKey.time
                    && IsMatchingLocation(window.SphereLocation, startKey.location))
                .OrderBy(window => window.StartTime)
                .ToList();

            foreach (SphereOccupancyWindow window in windowsInSegment) {
                double windowStart = Math.Max(intervalStart, window.StartTime);
                double windowEnd = Math.Min(endKey.time, window.EndTime);
                HitEvent activeProxHit = proxHit != null && proxHit.time >= intervalStart && proxHit.time < windowStart
                    ? proxHit
                    : null;

                if (windowEnd <= windowStart) {
                    continue;
                }

                AddStandardChunks(
                    i,
                    startKey,
                    endKey,
                    activeProxHit,
                    intervalStart,
                    windowStart,
                    rawDataPoints,
                    rawDataTimes,
                    ref cursor,
                    results);

                AddChunk(
                    i,
                    windowStart,
                    windowEnd,
                    AnalysisMode.PREVIOUSSPHERE,
                    window.SphereLocation,
                    window.SphereLocation,
                    rawDataPoints,
                    rawDataTimes,
                    ref cursor,
                    results);

                intervalStart = windowEnd;
            }

            AddStandardChunks(
                i,
                startKey,
                endKey,
                proxHit != null && proxHit.time >= intervalStart ? proxHit : null,
                intervalStart,
                endKey.time,
                rawDataPoints,
                rawDataTimes,
                ref cursor,
                results);
        }

        return results;
    }

    public static List<HitEvent> FilterValidProximityHits(List<HitEvent> keyPoints, List<HitEvent> proximityPoints) {
        if (keyPoints == null || keyPoints.Count < 2 || proximityPoints == null || proximityPoints.Count == 0) {
            return new List<HitEvent>();
        }

        List<HitEvent> sortedKeys = keyPoints.OrderBy(k => k.time).ToList();
        List<HitEvent> sortedProx = proximityPoints.OrderBy(p => p.time).ToList();
        Dictionary<int, HitEvent> mapped = BuildProximityBySegment(sortedKeys, sortedProx);

        return mapped
            .OrderBy(kvp => kvp.Key)
            .Select(kvp => kvp.Value)
            .ToList();
    }

    private static void AddStandardChunks(
        int segmentIndex,
        HitEvent startKey,
        HitEvent endKey,
        HitEvent proxHit,
        double startTime,
        double endTime,
        List<Vector3> rawDataPoints,
        List<double> rawDataTimes,
        ref int cursor,
        SlicingResult results) {

        if (endTime <= startTime) {
            return;
        }

        if (proxHit == null) {
            AddChunk(
                segmentIndex,
                startTime,
                endTime,
                AnalysisMode.LINETOTARGET,
                startKey.location,
                endKey.location,
                rawDataPoints,
                rawDataTimes,
                ref cursor,
                results);
            return;
        }

        if (endTime <= proxHit.time) {
            AddChunk(
                segmentIndex,
                startTime,
                endTime,
                AnalysisMode.LINETOTARGET,
                startKey.location,
                endKey.location,
                rawDataPoints,
                rawDataTimes,
                ref cursor,
                results);
            return;
        }

        if (startTime >= proxHit.time) {
            AddChunk(
                segmentIndex,
                startTime,
                endTime,
                AnalysisMode.POINTTOTARGET,
                startKey.location,
                endKey.location,
                rawDataPoints,
                rawDataTimes,
                ref cursor,
                results);
            return;
        }

        AddChunk(
            segmentIndex,
            startTime,
            proxHit.time,
            AnalysisMode.LINETOTARGET,
            startKey.location,
            endKey.location,
            rawDataPoints,
            rawDataTimes,
            ref cursor,
            results);

        AddChunk(
            segmentIndex,
            proxHit.time,
            endTime,
            AnalysisMode.POINTTOTARGET,
            startKey.location,
            endKey.location,
            rawDataPoints,
            rawDataTimes,
            ref cursor,
            results);
    }

    private static void AddChunk(
        int segmentIndex,
        double startTime,
        double endTime,
        AnalysisMode mode,
        Vector3 linePointA,
        Vector3 linePointB,
        List<Vector3> rawDataPoints,
        List<double> rawDataTimes,
        ref int cursor,
        SlicingResult results) {

        if (endTime <= startTime) {
            return;
        }

        GeometryInputData chunk = ExtractChunk(startTime, endTime, rawDataPoints, rawDataTimes, ref cursor);
        if (chunk == null) {
            return;
        }

        chunk.LinePointA = linePointA;
        chunk.LinePointB = linePointB;
        chunk.Mode = mode;

        GeometryResults geometry = GeometryAnalyzer.AnalyzeGeometry(chunk);
        if (geometry == null) {
            return;
        }

        results.SegmentResults.Add(new SegmentAnalysisResult {
            SegmentIndex = segmentIndex,
            GeometryData = geometry,
            Mode = mode
        });
    }

    private static Dictionary<int, HitEvent> BuildProximityBySegment(List<HitEvent> sortedKeys, List<HitEvent> sortedProx) {
        Dictionary<int, HitEvent> segmentMatches = new Dictionary<int, HitEvent>();
        if (sortedKeys == null || sortedKeys.Count < 2 || sortedProx == null || sortedProx.Count == 0) {
            return segmentMatches;
        }

        int proxCursor = 0;

        for (int i = 0; i < sortedKeys.Count - 1; i++) {
            HitEvent startKey = sortedKeys[i];
            HitEvent endKey = sortedKeys[i + 1];

            while (proxCursor < sortedProx.Count && sortedProx[proxCursor].time <= startKey.time) {
                proxCursor++;
            }

            int scan = proxCursor;
            while (scan < sortedProx.Count && sortedProx[scan].time < endKey.time) {
                HitEvent candidate = sortedProx[scan];
                if (IsMatchingLocation(candidate.location, endKey.location)) {
                    segmentMatches[i] = candidate;
                    proxCursor = scan + 1;
                    break;
                }

                scan++;
            }
        }

        return segmentMatches;
    }

    private static List<SphereOccupancyWindow> BuildPreviousSphereWindows(List<HitEvent> keyPoints, List<HitEvent> reentryPoints, List<HitEvent> leavePoints) {
        List<SphereOccupancyWindow> windows = new List<SphereOccupancyWindow>();
        if (leavePoints == null || leavePoints.Count == 0) {
            return windows;
        }

        List<HitEvent> sortedLeaves = leavePoints.OrderBy(l => l.time).ToList();

        // Add initial occupancy windows: target hit (original enter) -> first leave of that same target.
        if (keyPoints != null && keyPoints.Count > 0) {
            int leaveCursorForInitial = 0;
            for (int i = 0; i < keyPoints.Count; i++) {
                HitEvent targetHit = keyPoints[i];
                double segmentEnd = i < keyPoints.Count - 1 ? keyPoints[i + 1].time : double.PositiveInfinity;

                while (leaveCursorForInitial < sortedLeaves.Count && sortedLeaves[leaveCursorForInitial].time <= targetHit.time) {
                    leaveCursorForInitial++;
                }

                int scan = leaveCursorForInitial;
                while (scan < sortedLeaves.Count && sortedLeaves[scan].time < segmentEnd) {
                    HitEvent leave = sortedLeaves[scan];
                    if (!IsMatchingLocation(targetHit.location, leave.location)) {
                        scan++;
                        continue;
                    }

                    windows.Add(new SphereOccupancyWindow(targetHit.time, leave.time, targetHit.location));
                    break;
                }
            }
        }

        if (reentryPoints == null || reentryPoints.Count == 0) {
            return windows;
        }

        List<HitEvent> sortedEntries = reentryPoints.OrderBy(e => e.time).ToList();

        int leaveCursor = 0;

        foreach (HitEvent entry in sortedEntries) {
            while (leaveCursor < sortedLeaves.Count && sortedLeaves[leaveCursor].time <= entry.time) {
                leaveCursor++;
            }

            int scan = leaveCursor;
            while (scan < sortedLeaves.Count) {
                HitEvent leave = sortedLeaves[scan];
                if (!IsMatchingLocation(entry.location, leave.location)) {
                    scan++;
                    continue;
                }

                windows.Add(new SphereOccupancyWindow(entry.time, leave.time, entry.location));
                leaveCursor = scan + 1;
                break;
            }
        }

        return windows;
    }

    private static bool IsMatchingLocation(Vector3 lhs, Vector3 rhs) {
        float toleranceSquared = LocationMatchTolerance * LocationMatchTolerance;
        return (lhs - rhs).sqrMagnitude <= toleranceSquared;
    }

    private static GeometryInputData ExtractChunk(double startTime, double endTime, List<Vector3> points, List<double> times, ref int cursor) {
        int total = times.Count;

        while (cursor < total && times[cursor] < startTime) {
            cursor++;
        }

        int startIndex = cursor;

        while (cursor < total && times[cursor] <= endTime) {
            cursor++;
        }

        int count = cursor - startIndex;
        if (count <= 0) {
            return null;
        }

        return new GeometryInputData {
            Points = points.GetRange(startIndex, count),
            Timestamps = times.GetRange(startIndex, count)
        };
    }

    private sealed class SphereOccupancyWindow {
        public double StartTime { get; }
        public double EndTime { get; }
        public Vector3 SphereLocation { get; }

        public SphereOccupancyWindow(double startTime, double endTime, Vector3 sphereLocation) {
            StartTime = startTime;
            EndTime = endTime;
            SphereLocation = sphereLocation;
        }
    }
}

