using System;
using System.Linq;

public static class DataAnalyzer {
    public static Statistics AnalyzeData(AnalysisInputData input) {
        // Return null or empty result if data is invalid
        if (input == null || input.Values == null || input.Values.Count == 0) {
            return null;
        }

        var results = new Statistics();
        var values = input.Values; 
        var sorted = values.OrderBy(n => n).ToList();

        results.Average = values.Average();
        results.Max = values.Max();
        results.Min = values.Min();
        results.StDev = Math.Sqrt(values.Average(v => Math.Pow(v - results.Average, 2)));
        results.Median =  sorted.Count % 2 == 0
            ? (sorted[sorted.Count / 2 - 1] + sorted[sorted.Count / 2]) / 2.0
            : sorted[sorted.Count / 2];
        results.TotalDuration = input.Timestamps.Last() - input.Timestamps.First();
        results.TimeOfMax = input.Timestamps[values.IndexOf(results.Max)];
        results.TimeOfMin = input.Timestamps[values.IndexOf(results.Min)];

        return results;
    }
}