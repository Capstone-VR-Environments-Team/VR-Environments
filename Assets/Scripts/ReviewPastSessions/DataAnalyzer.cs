using System;
using System.Collections.Generic;
using System.Linq;

public static class DataAnalyzer {
    public static Statistics AnalyzeData(List<double> values, List<double> times) {
        // Return null or empty result if data is invalid
        if (values == null || values.Count == 0) {
            return new Statistics();
        }

        int sampleCount = values.Count;
        if (times != null && times.Count > 0) {
            sampleCount = Math.Min(values.Count, times.Count);
        }

        if (sampleCount <= 0) {
            return new Statistics();
        }

        if (sampleCount != values.Count) {
            values = values.Take(sampleCount).ToList();
        }

        var results = new Statistics();
        var sorted = values.OrderBy(n => n).ToList();

        results.Average = values.Average();
        results.Max = values.Max();
        results.Min = values.Min();
        results.StDev = Math.Sqrt(values.Average(v => Math.Pow(v - results.Average, 2)));
        results.Median =  sorted.Count % 2 == 0
            ? (sorted[sorted.Count / 2 - 1] + sorted[sorted.Count / 2]) / 2.0
            : sorted[sorted.Count / 2];

        if (times == null || times.Count == 0) {
            results.TotalDuration = 0;
            results.TimeOfMax = 0;
            results.TimeOfMin = 0;
            return results;
        }

        results.TotalDuration = sampleCount > 1 ? times[sampleCount - 1] - times[0] : 0;

        int maxIndex = values.IndexOf(results.Max);
        int minIndex = values.IndexOf(results.Min);

        results.TimeOfMax = maxIndex >= 0 && maxIndex < sampleCount ? times[maxIndex] : 0;
        results.TimeOfMin = minIndex >= 0 && minIndex < sampleCount ? times[minIndex] : 0;

        return results;
    }
}