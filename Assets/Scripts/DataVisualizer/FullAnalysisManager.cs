using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FullAnalysisManager : MonoBehaviour {
    [Header("Input Settings")]
    public string folderPath = "C:/MyDataFolder";

    [Header("Visualization - Left Hand")]
    public SimpleGraph leftGraphDist;
    public SimpleGraph leftGraphAxisH;
    public SimpleGraph leftGraphAxisV;

    [Header("Visualization - Right Hand")]
    public SimpleGraph rightGraphDist;
    public SimpleGraph rightGraphAxisH;
    public SimpleGraph rightGraphAxisV;

    [Header("Text Output")]
    public Text statsDisplayText;

    // Internal Reference
    private CSVFileLoader _csvLoader;
    private string _currentFolderPath = "";

    void Awake() {
        _csvLoader = new CSVFileLoader();
    }

    public void OnBrowseAndAnalyze() {
        string selectedPath = FileSelector.getFolderPath("");
        if (string.IsNullOrEmpty(selectedPath)) return;

        _currentFolderPath = selectedPath;
        RunAnalysis(_currentFolderPath);
    }

    private void RunAnalysis(string folderPath) {
        // Find Files
        string csvPath = Directory.GetFiles(folderPath, "*.csv").FirstOrDefault();
        string jsonPath = Directory.GetFiles(folderPath, "*.json").FirstOrDefault();

        if (string.IsNullOrEmpty(csvPath) || string.IsNullOrEmpty(jsonPath)) {
            if (statsDisplayText) statsDisplayText.text = "Error: Missing .csv or .json in folder.";
            return;
        }

        // Load Data
        List<KeyPoint> targetHits = JsonLoader.LoadKeyPoints(jsonPath);
        List<TrackingData> rawData = _csvLoader.loadFile(csvPath);

        if (rawData == null || rawData.Count == 0) return;

        List<double> allTimes = rawData.Select(d => (double)d.timeStamp).ToList();

        // Prepare Lists for Both Hands
        List<Vector3> leftPos = rawData.Select(d => d.leftHandPos).ToList();
        List<Vector3> rightPos = rawData.Select(d => d.rightHandPos).ToList();

        // Run Analysis
        var leftResults = ProcessHand(targetHits, leftPos, allTimes);
        var rightResults = ProcessHand(targetHits, rightPos, allTimes);

        // Update Graphs
        if (leftGraphDist) leftGraphDist.ShowGraph(leftResults.distVals);
        if (leftGraphAxisH) leftGraphAxisH.ShowGraph(leftResults.hVals);
        if (leftGraphAxisV) leftGraphAxisV.ShowGraph(leftResults.vVals);

        if (rightGraphDist) rightGraphDist.ShowGraph(rightResults.distVals);
        if (rightGraphAxisH) rightGraphAxisH.ShowGraph(rightResults.hVals);
        if (rightGraphAxisV) rightGraphAxisV.ShowGraph(rightResults.vVals);

        // Display Combined Stats
        DisplayCombinedStats(leftResults, rightResults);
    }

    // Helper struct to keep the massive amounts of data organized
    private struct HandResultPackage {
        public AnalysisResults statsDist;
        public AnalysisResults statsH;
        public AnalysisResults statsV;
        public List<double> distVals;
        public List<double> hVals;
        public List<double> vVals;
    }

    private HandResultPackage ProcessHand(List<KeyPoint> hits, List<Vector3> positions, List<double> times) {
        // Slice
        List<SegmentAnalysisResult> segments = DataSlicer.AnalyzeSegments(hits, positions, times);

        // Aggregate
        var dists = new List<double>();
        var hs = new List<double>();
        var vs = new List<double>();

        foreach (var seg in segments) {
            if (seg.GeometryData != null) {
                dists.AddRange(seg.GeometryData.DistancesFromLine);
                hs.AddRange(seg.GeometryData.PlaneAxisH);
                vs.AddRange(seg.GeometryData.PlaneAxisV);
            }
        }

        // Stats
        return new HandResultPackage {
            distVals = dists,
            hVals = hs,
            vVals = vs,
            statsDist = DataAnalyzer.AnalyzeData(new AnalysisInputData { Values = dists }),
            statsH = DataAnalyzer.AnalyzeData(new AnalysisInputData { Values = hs }),
            statsV = DataAnalyzer.AnalyzeData(new AnalysisInputData { Values = vs })
        };
    }

    private void DisplayCombinedStats(HandResultPackage left, HandResultPackage right) {
        if (statsDisplayText == null) return;

        string report = $"<b>ANALYSIS REPORT</b>\nFolder: {Path.GetFileName(_currentFolderPath)}\n\n";

        // --- LEFT HAND ---
        report += "<b><color=blue>--- LEFT HAND ---</color></b>\n";
        if (left.statsDist != null) {
            report += $"Dist Error | Avg: {left.statsDist.Average:F3}, Max: {left.statsDist.Max:F3}\n";
            report += $"Horz Dev   | Avg: {left.statsH.Average:F3}, Range: {left.statsH.Min:F3} to {left.statsH.Max:F3}\n";
            report += $"Vert Dev   | Avg: {left.statsV.Average:F3}, Range: {left.statsV.Min:F3} to {left.statsV.Max:F3}\n\n";
        } else {
            report += "No valid data segments found for Left Hand.\n\n";
        }

        // --- RIGHT HAND ---
        report += "<b><color=red>--- RIGHT HAND ---</color></b>\n";
        if (right.statsDist != null) {
            report += $"Dist Error | Avg: {right.statsDist.Average:F3}, Max: {right.statsDist.Max:F3}\n";
            report += $"Horz Dev   | Avg: {right.statsH.Average:F3}, Range: {right.statsH.Min:F3} to {right.statsH.Max:F3}\n";
            report += $"Vert Dev   | Avg: {right.statsV.Average:F3}, Range: {right.statsV.Min:F3} to {right.statsV.Max:F3}\n";
        } else {
            report += "No valid data segments found for Right Hand.\n";
        }

        statsDisplayText.text = report;
    }
}