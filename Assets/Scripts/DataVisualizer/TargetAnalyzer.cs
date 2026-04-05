using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TargetAnalyzer
{
    private const float LocationMatchTolerance = 0.02f;

    public static TargetAnalysisResults AnalyzeData(List<HitEvent> targetData, List<HitEvent> proxHitData)
    {
        if (targetData == null || targetData.Count == 0)
        {
            return new TargetAnalysisResults();
        }

        List<HitEvent> sortedHits = targetData.OrderBy(hit => hit.time).ToList();
        List<HitEvent> sortedProx = proxHitData != null
            ? proxHitData.OrderBy(hit => hit.time).ToList()
            : new List<HitEvent>();

        List<double> totalTimes = new();
        List<double> totalTimeTimestamps = new();
        List<double> searchTimes = new();
        List<double> searchTimeTimestamps = new();
        List<double> preSearchTimes = new();
        List<double> preSearchTimeTimestamps = new();

        int proxCursor = 0;
        for (int i = 1; i < sortedHits.Count; i++)
        {
            HitEvent startHit = sortedHits[i - 1];
            HitEvent endHit = sortedHits[i];

            double totalDelta = endHit.time - startHit.time;
            totalTimes.Add(totalDelta);
            totalTimeTimestamps.Add(endHit.time);

            while (proxCursor < sortedProx.Count && sortedProx[proxCursor].time <= startHit.time) {
                proxCursor++;
            }

            HitEvent proxHit = null;
            if (proxCursor < sortedProx.Count) {
                HitEvent candidate = sortedProx[proxCursor];
                bool inSegmentWindow = candidate.time < endHit.time;
                bool matchesTarget = IsMatchingLocation(candidate.location, endHit.location);
                if (inSegmentWindow && matchesTarget) {
                    proxHit = candidate;
                    proxCursor++;
                }
            }

            if (proxHit != null) {
                searchTimes.Add(endHit.time - proxHit.time);
                searchTimeTimestamps.Add(proxHit.time);
                preSearchTimes.Add(proxHit.time - startHit.time);
                preSearchTimeTimestamps.Add(startHit.time);
            } else {
                preSearchTimes.Add(totalDelta);
                preSearchTimeTimestamps.Add(startHit.time);
            }
        }

        TargetAnalysisResults results = new() {
            targetToTargetTimes = DataAnalyzer.AnalyzeData(totalTimes, totalTimeTimestamps),
            searchTimes = DataAnalyzer.AnalyzeData(searchTimes, searchTimeTimestamps),
            preSearchTimes = DataAnalyzer.AnalyzeData(preSearchTimes, preSearchTimeTimestamps)
        };

        return results;
    }

    private static bool IsMatchingLocation(Vector3 lhs, Vector3 rhs) {
        float toleranceSquared = LocationMatchTolerance * LocationMatchTolerance;
        return (lhs - rhs).sqrMagnitude <= toleranceSquared;
    }
}