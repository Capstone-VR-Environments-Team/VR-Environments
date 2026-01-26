using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using XUGL;

public class FullAnalysisManager : MonoBehaviour {

    // Internal Reference
    private CSVFileLoader _csvLoader;
    private string _currentFolderPath = "";

    [SerializeField] ReviewPastSessionsManager _reviewPastSessionsManager;
    [SerializeField] StatisticalViewManager _statisticalViewManager;
    [SerializeField] InteractiveViewManager _interactiveViewManager;


    void Awake() {
        _csvLoader = new CSVFileLoader();
    }

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
        JsonWrapper trialInfo = JsonLoader.LoadKeyPoints(jsonPath);
        List<TrackingData> rawData = _csvLoader.loadFile(csvPath);

        if (rawData == null || rawData.Count == 0) return;

        List<double> allTimes = rawData.Select(d => (double)d.timeStamp).ToList();

        // Prepare Lists for Both Hands
        List<Vector3> leftPos = rawData.Select(d => d.leftHandPos).ToList();
        List<Vector3> rightPos = rawData.Select(d => d.rightHandPos).ToList();
        Debug.Log($"Times = {allTimes.Count}, lefts = {leftPos.Count}");

        // Run Analysis
        var leftResults = ProcessHand(trialInfo.TargetHits, trialInfo.TargetProximityHits, leftPos, allTimes);
        var rightResults = ProcessHand(trialInfo.TargetHits, trialInfo.TargetProximityHits, rightPos, allTimes);

        Debug.Log($"Times = {allTimes.Count}, lefts2 = {leftResults.distVals.Count}");

        TargetAnalysisResults targetData = TargetAnalyzer.AnalyzeData(trialInfo.TargetHits, trialInfo.TargetProximityHits);

        // Display Combined Stats
        _statisticalViewManager.SetResults(leftResults, rightResults, targetData, allTimes);
        _interactiveViewManager.SetStatistics(leftResults.total.statsDist, rightResults.total.statsDist);
        _interactiveViewManager.SetPaths(rawData, trialInfo.TargetHits);

    }

    private HandResultPackage ProcessHand(List<KeyPoint> hits, List<KeyPoint> prox, List<Vector3> positions, List<double> times) {
        // Slice
        SlicingResult segments = DataSlicer.AnalyzeSegments(hits, prox, positions, times);

        // Aggregate
        var totalDists = new List<double>();
        var totalHs = new List<double>();
        var totalVs = new List<double>();
        var totalTimes = new List<double>();
        var totalTypes = new List<AnalysisMode>();

        var searchDists = new List<double>();
        var searchHs = new List<double>();
        var searchVs = new List<double>();
        var searchTimes = new List<double>();

        var approachDists = new List<double>();
        var approachHs = new List<double>();
        var approachVs = new List<double>();
        var approachTimes = new List<double>();

        foreach (var seg in segments.SegmentResults) {
            if (seg.GeometryData != null) {

                var totalData = seg.GeometryData.total;
                int count = totalData.DistancesFromLine.Count;
                totalDists.AddRange(totalData.DistancesFromLine);
                totalHs.AddRange(totalData.PlaneAxisH);
                totalVs.AddRange(totalData.PlaneAxisV);
                totalTimes.AddRange(totalData.Timestamps);
                totalTypes.AddRange(Enumerable.Repeat(seg.Mode, count));

                var searchData = seg.GeometryData.search;
                searchDists.AddRange(searchData.DistancesFromLine);
                searchHs.AddRange(searchData.PlaneAxisH);
                searchVs.AddRange(searchData.PlaneAxisV);
                searchTimes.AddRange(searchData.Timestamps);

                var approachData = seg.GeometryData.approach;
                approachDists.AddRange(approachData.DistancesFromLine);
                approachHs.AddRange(approachData.PlaneAxisH);
                approachVs.AddRange(approachData.PlaneAxisV);
                approachTimes.AddRange(approachData.Timestamps);

            }
        }

        // Stats
        return new HandResultPackage {
            distVals = totalDists,
            hVals = totalHs,
            vVals = totalVs,
            pointTypes = totalTypes,
            total = analyzeSetOfData(totalDists, totalHs, totalVs, totalTimes),
            search = analyzeSetOfData(searchDists, searchHs, searchVs, searchTimes),
            approach = analyzeSetOfData(approachDists, approachHs, approachVs, approachTimes)
        };
    }

    private DeviationData analyzeSetOfData(List<double> dists, List<double> hs, List<double> vs, List<double> times) {

        DeviationData result = new() {
            statsDist = DataAnalyzer.AnalyzeData(dists, times),
            statsH = DataAnalyzer.AnalyzeData(hs, times),
            statsV = DataAnalyzer.AnalyzeData(vs, times)
        };

        return result;

    }
}