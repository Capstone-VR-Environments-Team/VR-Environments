using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class ProcessedAnalysisData {
    public AnalyzedData AnalyzedData { get; }
    public List<double> AllTimes { get; }
    public IReadOnlyList<AnalysisMode> LeftPointTypes { get; }
    public IReadOnlyList<AnalysisMode> RightPointTypes { get; }
    public TargetAnalysisResults TargetAnalysisResults { get; }

    public ProcessedAnalysisData(
        AnalyzedData analyzedData,
        List<double> allTimes,
        IReadOnlyList<AnalysisMode> leftPointTypes,
        IReadOnlyList<AnalysisMode> rightPointTypes,
        TargetAnalysisResults targetAnalysisResults) {

        AnalyzedData = analyzedData ?? throw new ArgumentNullException(nameof(analyzedData));
        AllTimes = allTimes ?? new List<double>();
        LeftPointTypes = leftPointTypes ?? Array.Empty<AnalysisMode>();
        RightPointTypes = rightPointTypes ?? Array.Empty<AnalysisMode>();
        TargetAnalysisResults = targetAnalysisResults ?? new TargetAnalysisResults();
    }
}

public static class AnalysisProcessingService {
    public static ProcessedAnalysisData Process(List<TrackingData> rawData, CollectedTimingData timingData) {
        if (rawData == null || rawData.Count == 0) {
            throw new ArgumentException("Raw data cannot be null or empty.", nameof(rawData));
        }

        if (timingData == null) {
            throw new ArgumentNullException(nameof(timingData));
        }

        AnalysisContext context = CreateContext(rawData, timingData);
        Dictionary<Hand, IReadOnlyList<AnalysisMode>> pointTypesByHand = ProcessHands(context);

        TargetAnalysisResults targetAnalysisResults = TargetAnalyzer.AnalyzeData(timingData.TargetHits, timingData.TargetProximityHits);

        pointTypesByHand.TryGetValue(Hand.LEFT, out IReadOnlyList<AnalysisMode> leftPointTypes);
        pointTypesByHand.TryGetValue(Hand.RIGHT, out IReadOnlyList<AnalysisMode> rightPointTypes);

        return new ProcessedAnalysisData(
            context.AnalyzedData,
            context.AllTimes,
            leftPointTypes ?? Array.Empty<AnalysisMode>(),
            rightPointTypes ?? Array.Empty<AnalysisMode>(),
            targetAnalysisResults);
    }

    private static AnalysisContext CreateContext(List<TrackingData> rawData, CollectedTimingData timingData) {
        List<double> allTimes = rawData.Select(d => (double)d.timeStamp).ToList();
        Dictionary<Hand, List<Vector3>> positionsByHand = new Dictionary<Hand, List<Vector3>> {
            { Hand.LEFT, rawData.Select(d => d.leftHandPos).ToList() },
            { Hand.RIGHT, rawData.Select(d => d.rightHandPos).ToList() }
        };

        return new AnalysisContext(timingData, allTimes, positionsByHand, new AnalyzedData());
    }

    private static Dictionary<Hand, IReadOnlyList<AnalysisMode>> ProcessHands(AnalysisContext context) {
        Dictionary<Hand, IReadOnlyList<AnalysisMode>> pointTypesByHand = new Dictionary<Hand, IReadOnlyList<AnalysisMode>>();

        foreach (KeyValuePair<Hand, List<Vector3>> handData in context.PositionsByHand) {
            SlicingResult segments = DataSlicer.AnalyzeSegments(
                context.TimingData.TargetHits,
                context.TimingData.TargetProximityHits,
                handData.Value,
                context.AllTimes);

            Dictionary<MovementZone, ZoneAggregation> aggregates = CreateZoneAggregates();
            List<AnalysisMode> pointTypes = new List<AnalysisMode>();

            foreach (SegmentAnalysisResult segment in segments.SegmentResults) {
                if (segment.GeometryData == null) {
                    continue;
                }

                Geometry totalGeometry = segment.GeometryData.total;
                int pointCount = totalGeometry.DistancesFromLine.Count;

                AppendGeometry(aggregates[MovementZone.OVERALL], totalGeometry);
                pointTypes.AddRange(Enumerable.Repeat(segment.Mode, pointCount));

                AppendGeometry(aggregates[MovementZone.SEARCH], segment.GeometryData.search);
                AppendGeometry(aggregates[MovementZone.APPROACH], segment.GeometryData.approach);
            }

            WriteAggregatesToAnalyzedData(handData.Key, aggregates, context.AnalyzedData);
            pointTypesByHand[handData.Key] = pointTypes;
        }

        return pointTypesByHand;
    }

    private static Dictionary<MovementZone, ZoneAggregation> CreateZoneAggregates() {
        return new Dictionary<MovementZone, ZoneAggregation> {
            { MovementZone.OVERALL, new ZoneAggregation() },
            { MovementZone.SEARCH, new ZoneAggregation() },
            { MovementZone.APPROACH, new ZoneAggregation() }
        };
    }

    private static void WriteAggregatesToAnalyzedData(
        Hand hand,
        Dictionary<MovementZone, ZoneAggregation> aggregates,
        AnalyzedData analyzedData) {

        foreach (KeyValuePair<MovementZone, ZoneAggregation> zone in aggregates) {
            analyzedData.SetData(hand, zone.Key, DeviationType.TOTAL, BuildDataSet(zone.Value.Distances, zone.Value.Timestamps));
            analyzedData.SetData(hand, zone.Key, DeviationType.X_DEV, BuildDataSet(zone.Value.DeviationsX, zone.Value.Timestamps));
            analyzedData.SetData(hand, zone.Key, DeviationType.Y_DEV, BuildDataSet(zone.Value.DeviationsY, zone.Value.Timestamps));
            analyzedData.SetData(hand, zone.Key, DeviationType.Z_DEV, BuildDataSet(zone.Value.DeviationsZ, zone.Value.Timestamps));
        }
    }

    private static SingleDataSet BuildDataSet(List<double> values, List<double> times) {
        Statistics statistics = DataAnalyzer.AnalyzeData(values, times);
        return new SingleDataSet(statistics, values, times);
    }

    private static void AppendGeometry(ZoneAggregation aggregate, Geometry geometry) {
        aggregate.Distances.AddRange(geometry.DistancesFromLine);
        aggregate.DeviationsX.AddRange(geometry.DeviationsX);
        aggregate.DeviationsY.AddRange(geometry.DeviationsY);
        aggregate.DeviationsZ.AddRange(geometry.DeviationsZ);
        aggregate.Timestamps.AddRange(geometry.Timestamps);
    }

    private sealed class ZoneAggregation {
        public List<double> Distances { get; } = new List<double>();
        public List<double> DeviationsX { get; } = new List<double>();
        public List<double> DeviationsY { get; } = new List<double>();
        public List<double> DeviationsZ { get; } = new List<double>();
        public List<double> Timestamps { get; } = new List<double>();
    }

    private sealed class AnalysisContext {
        public CollectedTimingData TimingData { get; }
        public List<double> AllTimes { get; }
        public Dictionary<Hand, List<Vector3>> PositionsByHand { get; }
        public AnalyzedData AnalyzedData { get; }

        public AnalysisContext( CollectedTimingData timingData, List<double> allTimes, Dictionary<Hand, List<Vector3>> positionsByHand, AnalyzedData analyzedData) {
            TimingData = timingData;
            AllTimes = allTimes;
            PositionsByHand = positionsByHand;
            AnalyzedData = analyzedData;
        }
    }
}