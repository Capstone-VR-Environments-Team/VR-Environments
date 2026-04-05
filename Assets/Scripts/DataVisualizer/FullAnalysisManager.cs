using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FullAnalysisManager : MonoBehaviour {

    // Internal Reference
    private string _currentFolderPath = "";

    [SerializeField] ReviewPastSessionsManager _reviewPastSessionsManager;

    void Start() {
        _reviewPastSessionsManager.AddSelectSessionOnClick(OnBrowseAndAnalyze);
    }

    public void OnBrowseAndAnalyze() {
        string selectedPath = FileSelector.getFolderPath(Path.Combine(Application.persistentDataPath, "TrialRuns"));
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
        AnalysisResultsStore.Instance.SetSessionInfo(trialInfo, folderPath, fileName);
        _reviewPastSessionsManager.SetSessionInfo();
        List<TrackingData> rawData = FileManager.LoadCSVFile<RawData>(csvPath).trackingData;

        if (rawData == null || rawData.Count == 0) return;

        if (trialInfo.CollectedTimingData == null) {
            trialInfo.CollectedTimingData = new CollectedTimingData();
        }

        IReadOnlyList<Vector3> configuredTargets = trialInfo?.TrialSessionInformation?.TrialSettings?.TargetLocations;
        double initialTime = rawData[0].timeStamp;
        trialInfo.CollectedTimingData.TargetHits = TargetHitSequenceBuilder.BuildWithInitialTarget(
            trialInfo.CollectedTimingData.TargetHits,
            configuredTargets,
            initialTime);

        bool includeProximityHits = ShouldIncludeProximityHits(trialInfo);
        ProcessedAnalysisData analysis = AnalysisProcessingService.Process(rawData, trialInfo.CollectedTimingData, includeProximityHits);
        Debug.Log($"Times = {analysis.AllTimes.Count}, leftPoints = {analysis.AnalyzedData.GetPoints(Hand.LEFT, MovementZone.OVERALL, DeviationType.TOTAL).Count}");
        AnalysisResultsStore.Instance.SetAnalysisResults(rawData, analysis);
    }

    private static bool ShouldIncludeProximityHits(JsonWrapper trialInfo) {
        OffsetSettings offsetSettings = trialInfo?.TrialSessionInformation?.TrialSettings?.OffsetSettings;
        if (offsetSettings == null) {
            return true;
        }

        return offsetSettings.TargetProximity > 0f;
    }
}