using System.Collections.Generic;
using System.Linq;

public static class TargetAnalyzer
{
    public static TargetAnalysisResults AnalyzeData(List<HitEvent> targetData, List<HitEvent> proxHitData)
    {
        if (targetData == null || targetData.Count == 0)
        {
            return null;
        }

        List<double> hitTimeStamps = targetData.Select(keyPoint => keyPoint.time).ToList();
        List<double> proxTimeStamps = proxHitData.Select(keyPoint => keyPoint.time).ToList();

        List<double> totalTimes = new();
        List<double> searchTimes = new();
        List<double> preSearchTimes = new();
        for (int i = 1; i < hitTimeStamps.Count; i++)
        {
            totalTimes.Add(hitTimeStamps[i] - hitTimeStamps[i - 1]);
            if (i-1 < proxTimeStamps.Count){
                searchTimes.Add(hitTimeStamps[i] - proxTimeStamps[i-1]);
                preSearchTimes.Add(proxTimeStamps[i-1] - hitTimeStamps[i-1]);
            } else {
                preSearchTimes.Add(hitTimeStamps[i] - hitTimeStamps[i-1]);
            }

        }

        TargetAnalysisResults results = new() {
            targetToTargetTimes = DataAnalyzer.AnalyzeData(totalTimes, hitTimeStamps),
            searchTimes = DataAnalyzer.AnalyzeData(searchTimes, proxTimeStamps),
            preSearchTimes = DataAnalyzer.AnalyzeData(preSearchTimes, hitTimeStamps)
        };

        return results;
    }
}