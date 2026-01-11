using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XCharts.Runtime;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

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
        UpdateGraph(deviationMagnitudeGraph, currentResults.distVals);
        UpdateGraph(xAxisDeviationGraph, currentResults.hVals);
        UpdateGraph(yAxisDeviationGraph, currentResults.vVals);
    }

    private void UpdateGraph(BaseChart graph, List<double> values)
    {
        graph.ClearData();
        for (int i = 0; i < 10; i++)
        {
            graph.AddData(0, allTimes[i], values[i]);
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
