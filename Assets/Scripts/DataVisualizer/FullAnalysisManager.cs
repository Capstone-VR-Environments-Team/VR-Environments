using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FullAnalysisManager : MonoBehaviour {

    // Internal Reference
    private string _currentFolderPath = "";

    [SerializeField] ReviewPastSessionsManager _reviewPastSessionsManager;
    [SerializeField] StatisticalViewManager _statisticalViewManager;
    [SerializeField] InteractiveViewManager _interactiveViewManager;

    void Start() {
        _reviewPastSessionsManager.SelectSessionButton.onClick.AddListener(OnBrowseAndAnalyze);
    }

    public void OnBrowseAndAnalyze() {
        string selectedPath = FileSelector.getFolderPath(Application.persistentDataPath);
        if (string.IsNullOrEmpty(selectedPath)) return;

        _currentFolderPath = selectedPath;
        RunAnalysis(_currentFolderPath);
    }

    private void RunAnalysis(string folderPath) {
        // Find Files
        string csvPath = Directory.GetFiles(folderPath, "*.csv").FirstOrDefault();
        string jsonPath = Directory.GetFiles(folderPath, "*.json").FirstOrDefault();

        if (string.IsNullOrEmpty(csvPath) || string.IsNullOrEmpty(jsonPath)) {
            Debug.LogError("Error: Missing .csv or .json in folder.");
            return;
        }

        // Load Data
        var (trialInfo, fileName) = FileManager.LoadFromFile<JsonWrapper>(jsonPath);
        _reviewPastSessionsManager.SetSessionInfo(trialInfo);
        List<TrackingData> rawData = FileManager.LoadCSVFile<RawData>(csvPath).trackingData;

        if (rawData == null || rawData.Count == 0) return;

        List<double> allTimes = rawData.Select(d => (double)d.timeStamp).ToList();

        // Prepare Lists for Both Hands
        List<Vector3> leftPos = rawData.Select(d => d.leftHandPos).ToList();
        List<Vector3> rightPos = rawData.Select(d => d.rightHandPos).ToList();
        Debug.Log($"Times = {allTimes.Count}, lefts = {leftPos.Count}");

        // Run Analysis
        var leftResults = ProcessHand(trialInfo.CollectedTimingData.TargetHits, trialInfo.CollectedTimingData.TargetProximityHits, leftPos, allTimes);
        var rightResults = ProcessHand(trialInfo.CollectedTimingData.TargetHits, trialInfo.CollectedTimingData.TargetProximityHits, rightPos, allTimes);

        Debug.Log($"Times = {allTimes.Count}, lefts2 = {leftResults.distVals.Count}");

        TargetAnalysisResults targetData = TargetAnalyzer.AnalyzeData(trialInfo.CollectedTimingData.TargetHits, trialInfo.CollectedTimingData.TargetProximityHits);

        // Display Combined Stats
        _statisticalViewManager.SetResults(leftResults, rightResults, targetData, allTimes);
        _interactiveViewManager.SetStatistics(leftResults.total.statsDist, rightResults.total.statsDist);
        _interactiveViewManager.SetPaths(rawData, trialInfo.CollectedTimingData.TargetHits);

    }

    private HandResultPackage ProcessHand(List<HitEvent> hits, List<HitEvent> prox, List<Vector3> positions, List<double> times) {
        // Slice
        SlicingResult segments = DataSlicer.AnalyzeSegments(hits, prox, positions, times);

        // Aggregate
        var totalDists = new List<double>();
        var totalXs = new List<double>();
        var totalYs = new List<double>();
        var totalZs = new List<double>();
        var totalTimes = new List<double>();
        var totalTypes = new List<AnalysisMode>();

        var searchDists = new List<double>();
        var searchXs = new List<double>();
        var searchYs = new List<double>();
        var searchZs = new List<double>();
        var searchTimes = new List<double>();

        var approachDists = new List<double>();
        var approachXs = new List<double>();
        var approachYs = new List<double>();
        var approachZs = new List<double>();
        var approachTimes = new List<double>();

        foreach (var seg in segments.SegmentResults) {
            if (seg.GeometryData != null) {

                var totalData = seg.GeometryData.total;
                int count = totalData.DistancesFromLine.Count;
                totalDists.AddRange(totalData.DistancesFromLine);
                totalXs.AddRange(totalData.DeviationsX);
                totalYs.AddRange(totalData.DevaitionsY);
                totalZs.AddRange(totalData.DevaitionsZ);
                totalTimes.AddRange(totalData.Timestamps);
                totalTypes.AddRange(Enumerable.Repeat(seg.Mode, count));

                var searchData = seg.GeometryData.search;
                searchDists.AddRange(searchData.DistancesFromLine);
                searchXs.AddRange(searchData.DeviationsX);
                searchYs.AddRange(searchData.DevaitionsY);
                searchZs.AddRange(searchData.DevaitionsZ);
                searchTimes.AddRange(searchData.Timestamps);

                var approachData = seg.GeometryData.approach;
                approachDists.AddRange(approachData.DistancesFromLine);
                approachXs.AddRange(approachData.DeviationsX);
                approachYs.AddRange(approachData.DevaitionsY);
                approachZs.AddRange(approachData.DevaitionsZ);
                approachTimes.AddRange(approachData.Timestamps);

            }
        }

        // Stats
        return new HandResultPackage {
            distVals = totalDists,
            xVals = totalXs,
            yVals = totalYs,
            zVals = totalZs,
            pointTypes = totalTypes,
            total = analyzeSetOfData(totalDists, totalXs, totalYs, totalZs, totalTimes),
            search = analyzeSetOfData(searchDists, searchXs, searchYs, searchZs, searchTimes),
            approach = analyzeSetOfData(approachDists, approachXs, approachYs, approachZs, approachTimes)
        };
    }

    private DeviationData analyzeSetOfData(List<double> dists, List<double> xs, List<double> ys, List<double> zs, List<double> times) {

        DeviationData result = new() {
            statsDist = DataAnalyzer.AnalyzeData(dists, times),
            statsX = DataAnalyzer.AnalyzeData(xs, times),
            statsY = DataAnalyzer.AnalyzeData(ys, times),
            statsZ = DataAnalyzer.AnalyzeData(zs, times)
        };

        return result;

    }
}