using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        Statistics targetData = TargetAnalyzer.AnalyzeData(targetHits);

        // Display Combined Stats
        _statisticalViewManager.SetResults(leftResults, rightResults, allTimes);
        _statisticalViewManager.SetTargets(targetData);
        _interactiveViewManager.SetStatistics(leftResults.statsDist, rightResults.statsDist);
        _interactiveViewManager.SetPaths(rawData, targetHits);

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
            statsDist = DataAnalyzer.AnalyzeData(new AnalysisInputData { Values = dists, Timestamps = times }),
            statsH = DataAnalyzer.AnalyzeData(new AnalysisInputData { Values = hs, Timestamps = times }),
            statsV = DataAnalyzer.AnalyzeData(new AnalysisInputData { Values = vs, Timestamps = times })
        };
    }
}