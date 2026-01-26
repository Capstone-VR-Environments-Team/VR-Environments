using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public static class TargetAnalyzer
{
    public static Statistics AnalyzeData(List<KeyPoint> targetData)
    {
        if (targetData == null || targetData.Count == 0)
        {
            return null;
        }

        List<double> timeStamps = targetData.Select(keyPoint => keyPoint.time).ToList();

        List<double> times = new();
        for (int i = 1; i < timeStamps.Count; i++)
        {
            times.Add(timeStamps[i] - timeStamps[i - 1]);
        }

        var results = new Statistics();
        var sorted = times.OrderBy(n => n).ToList();

        results.Average = times.Average();
        results.Max = times.Max();
        results.Min = times.Min();
        results.StDev = Math.Sqrt(times.Average(v => Math.Pow(v - results.Average, 2)));
        results.Median = sorted.Count % 2 == 0
            ? (sorted[sorted.Count / 2 - 1] + sorted[sorted.Count / 2]) / 2.0
            : sorted[sorted.Count / 2];
        results.TotalDuration = times.Last() - times.First();
        results.TimeOfMax = times[times.IndexOf(results.Max)];
        results.TimeOfMin = times[times.IndexOf(results.Min)];

        return results;
    }
}