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

        if (rawDataPoints == null || rawDataPoints.Count == 0) {
            return results;
        }

        if (rawDataPoints.Count != rawDataTimes.Count) {
            return results;
        }

        List<SlicedEvent> chronologicalEvents = BuildChronologicalEvents(
            keyPoints,
            proximityPoints,
            previousSphereReentryPoints,
            previousSphereLeavePoints);

        if (chronologicalEvents.Count == 0) {
            return results;
        }

        List<ModeWindow> windows = BuildModeWindows(chronologicalEvents);
        double rawStartTime = rawDataTimes[0];
        double rawEndTime = rawDataTimes[rawDataTimes.Count - 1];
        Vector3 defaultReferenceLocation = chronologicalEvents[0].Location;

        int cursor = 0;
        double intervalStart = rawStartTime;

        foreach (ModeWindow window in windows.OrderBy(window => window.StartTime)) {
            if (window.StartTime > intervalStart) {
                AddChunk(
                    0,
                    intervalStart,
                    window.StartTime,
                    AnalysisMode.PREVIOUSSPHERE,
                    defaultReferenceLocation,
                    defaultReferenceLocation,
                    rawDataPoints,
                    rawDataTimes,
                    ref cursor,
                    results);
            }

            AddChunk(
                0,
                window.StartTime,
                window.EndTime,
                window.Mode,
                window.LinePointA,
                window.LinePointB,
                rawDataPoints,
                rawDataTimes,
                ref cursor,
                results);

            intervalStart = Math.Max(intervalStart, window.EndTime);
            defaultReferenceLocation = window.LinePointB;
        }

        if (intervalStart < rawEndTime) {
            AddChunk(
                0,
                intervalStart,
                rawEndTime,
                AnalysisMode.PREVIOUSSPHERE,
                defaultReferenceLocation,
                defaultReferenceLocation,
                rawDataPoints,
                rawDataTimes,
                ref cursor,
                results);
        }

        return results;
    }

    public static List<HitEvent> FilterValidProximityHits(List<HitEvent> keyPoints, List<HitEvent> proximityPoints) {
        if (keyPoints == null || proximityPoints == null || proximityPoints.Count == 0) {
            return new List<HitEvent>();
        }

        List<SlicedEvent> events = BuildChronologicalEvents(keyPoints, proximityPoints, null, null);
        List<ModeWindow> windows = BuildModeWindows(events);
        List<HitEvent> filtered = new List<HitEvent>();

        foreach (ModeWindow window in windows) {
            if (window.Mode == AnalysisMode.POINTTOTARGET) {
                filtered.Add(new HitEvent(window.StartTime, window.LinePointA));
            }
        }

        return filtered;
    }

    private static List<SlicedEvent> BuildChronologicalEvents(
        List<HitEvent> keyPoints,
        List<HitEvent> proximityPoints,
        List<HitEvent> previousSphereReentryPoints,
        List<HitEvent> previousSphereLeavePoints) {

        List<SlicedEvent> events = new List<SlicedEvent>();

        if (keyPoints != null) {
            events.AddRange(keyPoints.Select(hit => new SlicedEvent(hit.time, hit.location, SlicedEventType.TargetHit)));
        }

        if (proximityPoints != null) {
            events.AddRange(proximityPoints.Select(hit => new SlicedEvent(hit.time, hit.location, SlicedEventType.ProximityHit)));
        }

        if (previousSphereReentryPoints != null) {
            events.AddRange(previousSphereReentryPoints.Select(hit => new SlicedEvent(hit.time, hit.location, SlicedEventType.TargetReEntry)));
        }

        if (previousSphereLeavePoints != null) {
            events.AddRange(previousSphereLeavePoints.Select(hit => new SlicedEvent(hit.time, hit.location, SlicedEventType.TargetExit)));
        }

        return events.OrderBy(evt => evt.Time).ToList();
    }

    private static List<ModeWindow> BuildModeWindows(List<SlicedEvent> events) {
        List<ModeWindow> windows = new List<ModeWindow>();
        if (events == null || events.Count < 2) {
            return windows;
        }

        for (int i = 0; i < events.Count - 1; i++) {
            SlicedEvent startEvent = events[i];
            SlicedEvent endEvent = events[i + 1];
            AnalysisMode mode = ClassifyWindow(startEvent.Type, endEvent.Type);
            windows.Add(new ModeWindow(startEvent.Time, endEvent.Time, mode, startEvent.Location, endEvent.Location));
        }

        return windows;
    }

    private static AnalysisMode ClassifyWindow(SlicedEventType startType, SlicedEventType endType) {
        if (startType == SlicedEventType.TargetExit
            && endType == SlicedEventType.ProximityHit) {
            return AnalysisMode.LINETOTARGET;
        }

        if (startType == SlicedEventType.ProximityHit && endType == SlicedEventType.TargetHit) {
            return AnalysisMode.POINTTOTARGET;
        }

        if ((startType == SlicedEventType.TargetHit || startType == SlicedEventType.TargetReEntry)
            && endType == SlicedEventType.TargetExit) {
            return AnalysisMode.PREVIOUSSPHERE;
        }

        if (startType == SlicedEventType.TargetExit
            && endType == SlicedEventType.TargetReEntry) {
            return AnalysisMode.PREVIOUSSPHERE;
        }

        if (startType == SlicedEventType.ProximityHit && endType == SlicedEventType.TargetReEntry) {
            return AnalysisMode.PREVIOUSSPHERE;
        }

        return AnalysisMode.PREVIOUSSPHERE;
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

    private sealed class SlicedEvent {
        public double Time { get; }
        public Vector3 Location { get; }
        public SlicedEventType Type { get; }

        public SlicedEvent(double time, Vector3 location, SlicedEventType type) {
            Time = time;
            Location = location;
            Type = type;
        }
    }

    private sealed class ModeWindow {
        public double StartTime { get; }
        public double EndTime { get; }
        public AnalysisMode Mode { get; }
        public Vector3 LinePointA { get; }
        public Vector3 LinePointB { get; }

        public ModeWindow(double startTime, double endTime, AnalysisMode mode, Vector3 linePointA, Vector3 linePointB) {
            StartTime = startTime;
            EndTime = endTime;
            Mode = mode;
            LinePointA = linePointA;
            LinePointB = linePointB;
        }
    }

    private enum SlicedEventType {
        TargetHit,
        TargetExit,
        TargetReEntry,
        ProximityHit
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

