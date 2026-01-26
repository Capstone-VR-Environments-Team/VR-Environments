using System;
using System.Collections.Generic;
using System.Linq;

public static class DataAnalyzer {
    public static Statistics AnalyzeData(List<double> values, List<double> times) {
        // Return null or empty result if data is invalid
        if (values == null || values.Count == 0) {
            return null;
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
        results.TotalDuration = times.Last() - times.First();
        results.TimeOfMax = times[values.IndexOf(results.Max)];
        results.TimeOfMin = times[values.IndexOf(results.Min)];

        return results;
    }
}