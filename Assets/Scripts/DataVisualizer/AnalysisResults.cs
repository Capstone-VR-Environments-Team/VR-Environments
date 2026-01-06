using System;

public class AnalysisResults {
    // Basic Metrics
    public double Average { get; set; }
    public double Max { get; set; }
    public double Min { get; set; }
    public double Median { get; set; }

    // Deviation
    public double StandardDeviation { get; set; }

    // Time Context
    public double TimeOfMax { get; set; }
    public double TimeOfMin { get; set; }
    public double TotalDuration { get; set; }
}