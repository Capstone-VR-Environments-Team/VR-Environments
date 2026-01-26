using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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
        _statisticalViewManager.SetResults(leftResults, rightResults, allTimes);
        _statisticalViewManager.SetTargets(targetData.targetToTargetTimes);
        _interactiveViewManager.SetStatistics(leftResults.statsDist, rightResults.statsDist);
        _interactiveViewManager.SetPaths(rawData, trialInfo.TargetHits);

    }

    private HandResultPackage ProcessHand(List<KeyPoint> hits, List<KeyPoint> prox, List<Vector3> positions, List<double> times) {
        // Slice
        SlicingResult segments = DataSlicer.AnalyzeSegments(hits, prox, positions, times);

        // Aggregate
        var dists = new List<double>();
        var hs = new List<double>();
        var vs = new List<double>();
        var types = new List<AnalysisMode>();

        foreach (var seg in segments.SegmentResults) {
            if (seg.GeometryData != null) {
                int count = seg.GeometryData.DistancesFromLine.Count;

                dists.AddRange(seg.GeometryData.DistancesFromLine);
                hs.AddRange(seg.GeometryData.PlaneAxisH);
                vs.AddRange(seg.GeometryData.PlaneAxisV);
                types.AddRange(Enumerable.Repeat(seg.Mode, count));
            }
        }

        // Stats
        return new HandResultPackage {
            distVals = dists,
            hVals = hs,
            vVals = vs,
            pointTypes = types,
            statsDist = DataAnalyzer.AnalyzeData(dists, times),
            statsH = DataAnalyzer.AnalyzeData(hs, times),
            statsV = DataAnalyzer.AnalyzeData(vs, times),
        };
    }
}