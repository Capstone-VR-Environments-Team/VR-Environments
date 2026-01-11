using System;
using System.Collections.Generic;

public class AnalysisInputData {
    public List<double> Timestamps { get; set; }
    public List<double> Values { get; set; }

    public AnalysisInputData() {
        Timestamps = new List<double>();
        Values = new List<double>();
    }
}