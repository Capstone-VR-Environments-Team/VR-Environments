using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XCharts.Runtime;

public class StatisticalViewManager : MonoBehaviour
{
    [Header("Deviation Statistics")]
    [SerializeField] private TMP_Dropdown deviationDropdown;
    public TMP_Dropdown DeviationDropdown => deviationDropdown;

    [SerializeField] private StatisticsManager deviationStatisticsManager;
    public StatisticsManager DeviationStatisticsManager => deviationStatisticsManager;

    [Header("Target Statistics")]

    [SerializeField] private StatisticsManager targetStatisticsManager;
    public StatisticsManager TargetStatisticsManager => targetStatisticsManager;

    [Header("Graphs")]
    [SerializeField] private BaseChart xAxisDeviationGraph;
    public BaseChart XAxisDeviationGraph => xAxisDeviationGraph;

    [SerializeField] private BaseChart yAxisDeviationGraph;
    public BaseChart YAxisDeviationGraph => yAxisDeviationGraph;

    [SerializeField] private BaseChart deviationMagnitudeGraph;
    public BaseChart DeviationMagnitudeGraph => deviationMagnitudeGraph;

    [SerializeField] private BaseChart eegSignalsGraph;
    public BaseChart EEGSignalsGraph => eegSignalsGraph;

    [Header("Control Panel")]
    [SerializeField] private TMP_Dropdown pathDropdown;
    public TMP_Dropdown PathDropdown => pathDropdown;

    [SerializeField] private Button endAnalysisButton;
    public Button EndAnalysisButton => endAnalysisButton;

    private HandResultPackage leftResults;
    private HandResultPackage rightResults;
    private List<double> allTimes;

    private HandResultPackage currentResults;

    public void SetResults(HandResultPackage leftResults, HandResultPackage rightResults, List<double> allTimes)
    {
        this.leftResults = leftResults;
        this.rightResults = rightResults;
        this.allTimes = allTimes;

        UpdatePath();
        RefreshData();
    }

    public void SetTargets(Statistics targetData)
    {
        targetStatisticsManager.SetStatistics(targetData);
    }

    private void Start()
    {
        deviationDropdown.onValueChanged.AddListener(UpdateDeviationStatistics);
        pathDropdown.onValueChanged.AddListener(UpdatePath);
    }

    private void UpdatePath()
    {
        UpdatePath(pathDropdown.value);
    }

    private void UpdatePath(int index)
    {
        string path = pathDropdown.options[index].text;

        if (path == "Left Hand")
        {
            currentResults = leftResults;
        }
        else if (path == "Right Hand")
        {
            currentResults = rightResults;
        }

        RefreshData();
    }

    private void UpdateDeviationStatistics()
    {
        UpdateDeviationStatistics(deviationDropdown.value);
    }

    private void UpdateDeviationStatistics(int index)
    {
        string selectedDeviation = deviationDropdown.options[index].text;

        if (selectedDeviation == "Up/Down")
        {
            deviationStatisticsManager.SetStatistics(currentResults.statsV);
        }
        else if (selectedDeviation == "Left/Right")
        {
            deviationStatisticsManager.SetStatistics(currentResults.statsH);
        }
        else if (selectedDeviation == "Total")
        {
            deviationStatisticsManager.SetStatistics(currentResults.statsDist);
        }
    }

    private void UpdateGraphs()
    {
        UpdateGraph(deviationMagnitudeGraph, currentResults.distVals, currentResults.pointTypes);
        UpdateGraph(xAxisDeviationGraph, currentResults.hVals, currentResults.pointTypes);
        UpdateGraph(yAxisDeviationGraph, currentResults.vVals, currentResults.pointTypes);
    }

    private void UpdateGraph(BaseChart graph, List<double> values, List<AnalysisMode> pointTypes)
    {
        graph.ClearData();
        graph.RemoveAllSerie();

        int count = Mathf.Min(values.Count, allTimes.Count);
        if (count == 0) { return; }
        AnalysisMode previousType = AnalysisMode.LineToTarget;

        Color approachColor = new Color{ r = 106.0f / 255.0f, g = 153.0f / 255.0f, b = 77.0f / 255.0f, a = 1 }; 
        Color homingColor = Color.purple;

        int currentSerieIndex = -1;

        for (int i = 0; i < count; i++) {
            double timestamp = allTimes[i];
            double val = values[i];
            AnalysisMode currentType = pointTypes[i];

            // Handle Switch between mode
            if (i == 0 || currentType != previousType) {
                currentSerieIndex++;

                var newSerie = graph.AddSerie<Line>($"Segment_{currentSerieIndex}");

                // Configure Style
                newSerie.lineStyle.width = 2.0f;

                // Set Color based on Mode
                if (currentType == AnalysisMode.PointToTarget)
                {
                    newSerie.lineStyle.color = homingColor;
                    newSerie.itemStyle.color = homingColor;
                } 
                else
                {
                    newSerie.lineStyle.color = approachColor;
                    newSerie.itemStyle.color = approachColor;
                }

                if (i > 0) {
                    graph.AddData(currentSerieIndex - 1, timestamp, val);
                }
            }

            // Add the current data to the current series
            graph.AddData(currentSerieIndex, timestamp, val);

            previousType = currentType;
        }

        Debug.Log($"Times = {allTimes.Count}, vals = {values.Count}");
        graph.RefreshChart();
    }

    private void RefreshData()
    {
        UpdateDeviationStatistics();
        UpdateGraphs();
    }

    private void OnDestroy()
    {
        deviationDropdown.onValueChanged.RemoveAllListeners();
        pathDropdown.onValueChanged.RemoveAllListeners();
    }
}
