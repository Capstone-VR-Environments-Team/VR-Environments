public class Statistics
{
    // Basic Metrics
    public double Average { get; set; }
    public double Max { get; set; }
    public double Min { get; set; }
    public double Median { get; set; }

    // Deviation
    public double StDev { get; set; }

    // Time Context
    public double TimeOfMax { get; set; }
    public double TimeOfMin { get; set; }
    public double TotalDuration { get; set; }

    public Statistics (double average, double maximum, double minimum, double median, double stDev, double timeOfMax, double timeOfMin, double totalDuration)
    {
        Average = average;
        Max = maximum;
        Min = minimum;
        Median = median;
        StDev = stDev;
        TimeOfMax = timeOfMax;
        TimeOfMin = timeOfMin;
        TotalDuration = totalDuration;
    }

    public Statistics() {
    }
}
