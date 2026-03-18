using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
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

    public void ExportAnalysisResults() {
        if (!TryValidateExportData(out string validationError)) {
            Debug.LogError($"Export canceled: {validationError}");
            return;
        }

        string defaultBaseName = BuildDefaultBaseName();
        string startDirectory = Directory.Exists(CurrentFolderPath) ? CurrentFolderPath : Application.persistentDataPath;
        string selectedPath = FileSelector.getSaveFilePath(startDirectory, defaultBaseName, "csv");

        if (string.IsNullOrWhiteSpace(selectedPath)) {
            Debug.Log("Export canceled by user.");
            return;
        }

        string exportDirectory = Path.GetDirectoryName(selectedPath);
        string selectedBaseName = Path.GetFileNameWithoutExtension(selectedPath);
        if (string.IsNullOrWhiteSpace(exportDirectory) || string.IsNullOrWhiteSpace(selectedBaseName)) {
            Debug.LogError("Export canceled: invalid save location or file name.");
            return;
        }

        string leftPath = Path.Combine(exportDirectory, $"{selectedBaseName}-LeftHand.csv");
        string rightPath = Path.Combine(exportDirectory, $"{selectedBaseName}-RightHand.csv");
        string statsPath = Path.Combine(exportDirectory, $"{selectedBaseName}-Statistics.csv");

        try {
            Directory.CreateDirectory(exportDirectory);
            WriteHandCsv(Hand.LEFT, leftPath);
            WriteHandCsv(Hand.RIGHT, rightPath);
            WriteStatisticsCsv(statsPath);

            Debug.Log(
                "Export complete.\n" +
                $"Left hand: {leftPath}\n" +
                $"Right hand: {rightPath}\n" +
                $"Statistics: {statsPath}");
        } catch (Exception ex) {
            Debug.LogError($"Export failed: {ex.Message}");
        }
    }

    private bool TryValidateExportData(out string errorMessage) {
        errorMessage = string.Empty;

        if (TrialInfo == null || TrialInfo.TrialSessionInformation == null) {
            errorMessage = "Session metadata is missing.";
            return false;
        }

        if (RawData == null || RawData.Count == 0) {
            errorMessage = "Raw hand data is missing.";
            return false;
        }

        if (ProcessedData == null || ProcessedData.AnalyzedData == null) {
            errorMessage = "Processed analysis data is missing.";
            return false;
        }

        IReadOnlyList<double> leftTotalPoints = ProcessedData.AnalyzedData.GetPoints(Hand.LEFT, MovementZone.OVERALL, DeviationType.TOTAL);
        IReadOnlyList<double> rightTotalPoints = ProcessedData.AnalyzedData.GetPoints(Hand.RIGHT, MovementZone.OVERALL, DeviationType.TOTAL);

        if (leftTotalPoints == null || leftTotalPoints.Count == 0) {
            errorMessage = "Left-hand processed deviation data is missing.";
            return false;
        }

        if (rightTotalPoints == null || rightTotalPoints.Count == 0) {
            errorMessage = "Right-hand processed deviation data is missing.";
            return false;
        }

        if (ProcessedData.LeftPointTypes == null || ProcessedData.LeftPointTypes.Count == 0) {
            errorMessage = "Left-hand point type labels are missing.";
            return false;
        }

        if (ProcessedData.RightPointTypes == null || ProcessedData.RightPointTypes.Count == 0) {
            errorMessage = "Right-hand point type labels are missing.";
            return false;
        }

        return true;
    }

    private string BuildDefaultBaseName() {
        string sessionName = TrialInfo?.TrialSessionInformation?.SessionName;
        string participantId = TrialInfo?.TrialSessionInformation?.ParticipantID;
        string timestamp = GetTrialTimestampString();

        if (string.IsNullOrWhiteSpace(sessionName)) {
            sessionName = "Session";
        }

        if (string.IsNullOrWhiteSpace(participantId)) {
            participantId = "Participant";
        }

        return SanitizeFileName($"{sessionName}-{participantId}-{timestamp}");
    }

    private string GetTrialTimestampString() {
        if (RawData != null && RawData.Count > 0) {
            long firstTimestamp = RawData[0].timeStamp;
            try {
                return DateTimeOffset.FromUnixTimeMilliseconds(firstTimestamp).ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            } catch {
                // Fall through to current time when timestamps are not Unix epoch milliseconds.
            }
        }

        return DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
    }

    private void WriteHandCsv(Hand hand, string outputPath) {
        IReadOnlyList<double> deviations = ProcessedData.AnalyzedData.GetPoints(hand, MovementZone.OVERALL, DeviationType.TOTAL);
        IReadOnlyList<double> xDeviations = ProcessedData.AnalyzedData.GetPoints(hand, MovementZone.OVERALL, DeviationType.X_DEV);
        IReadOnlyList<double> yDeviations = ProcessedData.AnalyzedData.GetPoints(hand, MovementZone.OVERALL, DeviationType.Y_DEV);
        IReadOnlyList<double> zDeviations = ProcessedData.AnalyzedData.GetPoints(hand, MovementZone.OVERALL, DeviationType.Z_DEV);
        SingleDataSet dataSet = ProcessedData.AnalyzedData.GetDataOrEmpty(hand, MovementZone.OVERALL, DeviationType.TOTAL);
        IReadOnlyList<double> times = dataSet.Times;

        if (times == null || times.Count == 0) {
            times = ProcessedData.AllTimes;
        }

        IReadOnlyList<AnalysisMode> pointTypes = hand == Hand.LEFT ? ProcessedData.LeftPointTypes : ProcessedData.RightPointTypes;

        int count = RawData.Count;
        count = Mathf.Min(count, deviations.Count);
        count = Mathf.Min(count, xDeviations.Count);
        count = Mathf.Min(count, yDeviations.Count);
        count = Mathf.Min(count, zDeviations.Count);
        count = Mathf.Min(count, times.Count);
        count = Mathf.Min(count, pointTypes.Count);

        if (count == 0) {
            throw new InvalidOperationException($"No aligned rows available for {hand} export.");
        }

        StringBuilder builder = new StringBuilder();
        builder.AppendLine("Time,X,Y,Z,TotalDeviation,XDeviation,YDeviation,ZDeviation,PointType");

        for (int i = 0; i < count; i++) {
            TrackingData row = RawData[i];
            Vector3 position = hand == Hand.LEFT ? row.leftHandPos : row.rightHandPos;

            string[] columns = {
                FormatNumber(times[i]),
                FormatNumber(position.x),
                FormatNumber(position.y),
                FormatNumber(position.z),
                FormatNumber(deviations[i]),
                FormatNumber(xDeviations[i]),
                FormatNumber(yDeviations[i]),
                FormatNumber(zDeviations[i]),
                MapPointType(pointTypes[i])
            };

            builder.AppendLine(ToCsvRow(columns));
        }

        File.WriteAllText(outputPath, builder.ToString(), Encoding.UTF8);
    }

    private void WriteStatisticsCsv(string outputPath) {
        List<StatisticsRow> dataRows = BuildStatisticsRows();

        StringBuilder builder = new StringBuilder();

        List<string> header = new List<string> {
            "DataSet",
            "Hand",
            "Zone",
            "DeviationType",
            "Average",
            "StDev",
            "Min",
            "Max",
            "Median",
            "TimeOfMin",
            "TimeOfMax",
            "TotalDuration"
        };
        builder.AppendLine(ToCsvRow(header));

        foreach (StatisticsRow rowData in dataRows) {
            Statistics stats = rowData.Stats ?? new Statistics();
            List<string> row = new List<string> {
                rowData.DataSet,
                rowData.Hand,
                rowData.Zone,
                rowData.DeviationType,
                FormatNumber(stats.Average),
                FormatNumber(stats.StDev),
                FormatNumber(stats.Min),
                FormatNumber(stats.Max),
                FormatNumber(stats.Median),
                FormatNumber(stats.TimeOfMin),
                FormatNumber(stats.TimeOfMax),
                FormatNumber(stats.TotalDuration)
            };

            builder.AppendLine(ToCsvRow(row));
        }

        File.WriteAllText(outputPath, builder.ToString(), Encoding.UTF8);
    }

    private List<StatisticsRow> BuildStatisticsRows() {
        List<StatisticsRow> rows = new List<StatisticsRow>();

        foreach (Hand hand in Enum.GetValues(typeof(Hand))) {
            foreach (MovementZone zone in Enum.GetValues(typeof(MovementZone))) {
                foreach (DeviationType deviationType in Enum.GetValues(typeof(DeviationType))) {
                    Statistics stats = ProcessedData.AnalyzedData.GetStatistics(hand, zone, deviationType) ?? new Statistics();
                    rows.Add(new StatisticsRow(
                        "Analysis",
                        hand.ToString(),
                        zone.ToString(),
                        deviationType.ToString(),
                        stats));
                }
            }
        }

        TargetAnalysisResults timing = ProcessedData.TargetAnalysisResults ?? new TargetAnalysisResults();
        rows.Add(new StatisticsRow("Timing_TargetToTarget", "N/A", "N/A", "N/A", timing.targetToTargetTimes ?? new Statistics()));
        rows.Add(new StatisticsRow("Timing_PreSearch", "N/A", "N/A", "N/A", timing.preSearchTimes ?? new Statistics()));
        rows.Add(new StatisticsRow("Timing_Search", "N/A", "N/A", "N/A", timing.searchTimes ?? new Statistics()));

        return rows;
    }

    private string FormatNumber(double value) {
        if (double.IsNaN(value) || double.IsInfinity(value)) {
            return string.Empty;
        }

        return value.ToString("0.######", CultureInfo.InvariantCulture);
    }

    private string MapPointType(AnalysisMode pointType) {
        return pointType == AnalysisMode.POINTTOTARGET ? "Search" : "Approach";
    }

    private string ToCsvRow(IReadOnlyList<string> columns) {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < columns.Count; i++) {
            if (i > 0) {
                builder.Append(',');
            }

            builder.Append(EscapeCsv(columns[i]));
        }

        return builder.ToString();
    }

    private string EscapeCsv(string value) {
        if (string.IsNullOrEmpty(value)) {
            return string.Empty;
        }

        bool shouldQuote = value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r");
        if (!shouldQuote) {
            return value;
        }

        string escaped = value.Replace("\"", "\"\"");
        return $"\"{escaped}\"";
    }

    private string SanitizeFileName(string input) {
        if (string.IsNullOrWhiteSpace(input)) {
            return "Export";
        }

        char[] invalidChars = Path.GetInvalidFileNameChars();
        StringBuilder builder = new StringBuilder(input.Length);
        foreach (char c in input) {
            builder.Append(Array.IndexOf(invalidChars, c) >= 0 ? '_' : c);
        }

        return builder.ToString();
    }

    private sealed class StatisticsRow {
        public string DataSet { get; }
        public string Hand { get; }
        public string Zone { get; }
        public string DeviationType { get; }
        public Statistics Stats { get; }

        public StatisticsRow(string dataSet, string hand, string zone, string deviationType, Statistics stats) {
            DataSet = dataSet;
            Hand = hand;
            Zone = zone;
            DeviationType = deviationType;
            Stats = stats ?? new Statistics();
        }
    }
}