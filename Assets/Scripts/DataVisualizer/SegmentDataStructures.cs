using System;
using System.Collections.Generic;
using UnityEngine;

public enum AnalysisMode {
    LINETOTARGET,
    POINTTOTARGET,
    PREVIOUSSPHERE
}

public enum Hand {
    LEFT,
    RIGHT
}

public enum MovementZone {
    OVERALL,
    SEARCH,
    APPROACH
}

public enum DeviationType {
    TOTAL,
    X_DEV,
    Y_DEV,
    Z_DEV
}

public readonly struct AnalysisKey : IEquatable<AnalysisKey> {
    public Hand Hand { get; }
    public MovementZone MovementZone { get; }
    public DeviationType DeviationType { get; }

    public AnalysisKey(Hand hand, MovementZone movementZone, DeviationType deviationType) {
        Hand = hand;
        MovementZone = movementZone;
        DeviationType = deviationType;
    }

    public bool Equals(AnalysisKey other) {
        return Hand == other.Hand
            && MovementZone == other.MovementZone
            && DeviationType == other.DeviationType;
    }

    public override bool Equals(object obj) {
        return obj is AnalysisKey other && Equals(other);
    }

    public override int GetHashCode() {
        unchecked {
            int hash = 17;
            hash = (hash * 31) + (int)Hand;
            hash = (hash * 31) + (int)MovementZone;
            hash = (hash * 31) + (int)DeviationType;
            return hash;
        }
    }
}

public readonly struct SingleDataSet {
    private static readonly IReadOnlyList<double> EmptyValues = Array.Empty<double>();

    public Statistics Statistics { get; }
    public IReadOnlyList<double> Points { get; }
    public IReadOnlyList<double> Times { get; }

    public static SingleDataSet Empty => new SingleDataSet(new Statistics(), EmptyValues, EmptyValues);

    public SingleDataSet(Statistics statistics, IReadOnlyList<double> points, IReadOnlyList<double> times) {
        Statistics = statistics ?? new Statistics();
        Points = points ?? EmptyValues;
        Times = times ?? EmptyValues;
    }
}

public sealed class AnalyzedData {
    private static readonly IReadOnlyList<double> EmptyValues = Array.Empty<double>();
    private readonly Dictionary<AnalysisKey, SingleDataSet> _data = new Dictionary<AnalysisKey, SingleDataSet>();

    public IEnumerable<AnalysisKey> Keys => _data.Keys;

    public void SetData(Hand hand, MovementZone movementZone, DeviationType deviationType, SingleDataSet singleDataSet) {
        AnalysisKey key = new AnalysisKey(hand, movementZone, deviationType);
        _data[key] = singleDataSet;
    }

    public bool TryGetData(Hand hand, MovementZone movementZone, DeviationType deviationType, out SingleDataSet singleDataSet) {
        AnalysisKey key = new AnalysisKey(hand, movementZone, deviationType);
        return _data.TryGetValue(key, out singleDataSet);
    }

    public SingleDataSet GetDataOrEmpty(Hand hand, MovementZone movementZone, DeviationType deviationType) {
        return TryGetData(hand, movementZone, deviationType, out SingleDataSet data)
            ? data
            : SingleDataSet.Empty;
    }

    public IReadOnlyList<double> GetPoints(Hand hand, MovementZone movementZone, DeviationType deviationType) {
        return GetDataOrEmpty(hand, movementZone, deviationType).Points ?? EmptyValues;
    }

    public Statistics GetStatistics(Hand hand, MovementZone movementZone, DeviationType deviationType) {
        return GetDataOrEmpty(hand, movementZone, deviationType).Statistics ?? new Statistics();
    }
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
