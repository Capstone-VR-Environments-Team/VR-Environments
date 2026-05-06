using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TargetAnalyzer
{
    private const float LocationMatchTolerance = 0.02f;

    public static TargetAnalysisResults AnalyzeData(CollectedTimingData timingData)
    {
        List<TargetEventRecord> chronologicalEvents = BuildChronologicalEvents(timingData);
        if (chronologicalEvents.Count == 0)
        {
            return new TargetAnalysisResults();
        }

        List<double> totalTimes = new();
        List<double> totalTimeTimestamps = new();
        List<double> searchTimes = new();
        List<double> searchTimeTimestamps = new();
        List<double> preSearchTimes = new();
        List<double> preSearchTimeTimestamps = new();

        TargetEventRecord lastExit = null;
        Dictionary<int, TargetEventRecord> activeProximityByTarget = new Dictionary<int, TargetEventRecord>();
        Dictionary<int, TargetEventRecord> exitByTarget = new Dictionary<int, TargetEventRecord>();

        foreach (TargetEventRecord record in chronologicalEvents.OrderBy(evt => evt.time))
        {
            if (!record.TryGetEventType(out TargetEventType eventType))
            {
                continue;
            }

            int targetId = record.targetId;
            if (targetId <= 0)
            {
                continue;
            }

            if (eventType == TargetEventType.TargetExit)
            {
                lastExit = record;
                activeProximityByTarget.Remove(targetId);
                exitByTarget.Remove(targetId);
                continue;
            }

            if (eventType == TargetEventType.ProximityHit)
            {
                if (lastExit != null
                    && record.time > lastExit.time
                    && !activeProximityByTarget.ContainsKey(targetId))
                {
                    activeProximityByTarget[targetId] = record;
                    exitByTarget[targetId] = lastExit;
                }

                continue;
            }

            if (eventType != TargetEventType.TargetHit)
            {
                continue;
            }

            if (!exitByTarget.TryGetValue(targetId, out TargetEventRecord targetLeave)
                || !activeProximityByTarget.TryGetValue(targetId, out TargetEventRecord targetProximity))
            {
                continue;
            }

            if (!IsMatchingLocation(record.location, targetProximity.location))
            {
                continue;
            }

            double preSearchDelta = targetProximity.time - targetLeave.time;
            double searchDelta = record.time - targetProximity.time;
            if (preSearchDelta < 0.0 || searchDelta < 0.0)
            {
                continue;
            }

            preSearchTimes.Add(preSearchDelta);
            preSearchTimeTimestamps.Add(targetLeave.time);

            searchTimes.Add(searchDelta);
            searchTimeTimestamps.Add(targetProximity.time);

            totalTimes.Add(preSearchDelta + searchDelta);
            totalTimeTimestamps.Add(record.time);

            exitByTarget.Remove(targetId);
            activeProximityByTarget.Remove(targetId);
        }

        return new TargetAnalysisResults {
            targetToTargetTimes = DataAnalyzer.AnalyzeData(totalTimes, totalTimeTimestamps),
            searchTimes = DataAnalyzer.AnalyzeData(searchTimes, searchTimeTimestamps),
            preSearchTimes = DataAnalyzer.AnalyzeData(preSearchTimes, preSearchTimeTimestamps)
        };
    }

    private static List<TargetEventRecord> BuildChronologicalEvents(CollectedTimingData timingData) {
        List<TargetEventRecord> events = new List<TargetEventRecord>();
        if (timingData == null)
        {
            return events;
        }

        if (timingData.TargetEvents != null && timingData.TargetEvents.Count > 0)
        {
            events.AddRange(timingData.TargetEvents);
            return events;
        }

        events.AddRange(timingData.TargetHits.Select(hit => new TargetEventRecord(hit.time, TargetEventType.TargetHit, hit.location, hit.targetId)));
        events.AddRange(timingData.TargetProximityHits.Select(hit => new TargetEventRecord(hit.time, TargetEventType.ProximityHit, hit.location, hit.targetId)));
        events.AddRange(timingData.LeaveTargetHits.Select(hit => new TargetEventRecord(hit.time, TargetEventType.TargetExit, hit.location, hit.targetId)));
        events.AddRange(timingData.ReEnterTargetHits.Select(hit => new TargetEventRecord(hit.time, TargetEventType.TargetReEntry, hit.location, hit.targetId)));
        return events;
    }

    private static bool IsMatchingLocation(Vector3 lhs, Vector3 rhs) {
        float toleranceSquared = LocationMatchTolerance * LocationMatchTolerance;
        return (lhs - rhs).sqrMagnitude <= toleranceSquared;
    }
}