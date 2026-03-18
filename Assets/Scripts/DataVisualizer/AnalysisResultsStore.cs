using System;
using System.Collections.Generic;
using UnityEngine;

public class AnalysisResultsStore : Singleton<AnalysisResultsStore> {


    public string CurrentFolderPath { get; private set; } = string.Empty;
    public JsonWrapper TrialInfo { get; private set; }
    public List<TrackingData> RawData { get; private set; }
    public ProcessedAnalysisData ProcessedData { get; private set; }

    public bool HasSessionInfo => TrialInfo != null;
    public bool HasAnalysisData => ProcessedData != null && RawData != null;

    public void SetSessionInfo(JsonWrapper trialInfo, string folderPath) {
        TrialInfo = trialInfo;
        CurrentFolderPath = folderPath ?? string.Empty;
        NotifyDataChanged();
    }

    public void SetAnalysisResults(List<TrackingData> rawData, ProcessedAnalysisData processedData) {
        RawData = rawData;
        ProcessedData = processedData;
        NotifyDataChanged();
    }

    public void Clear() {
        CurrentFolderPath = string.Empty;
        TrialInfo = null;
        RawData = null;
        ProcessedData = null;
        NotifyDataChanged();
    }

    private void NotifyDataChanged() {
        EventBus.DataChanged?.Invoke();
    }
}