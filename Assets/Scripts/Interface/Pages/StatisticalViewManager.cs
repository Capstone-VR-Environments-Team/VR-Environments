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

    [SerializeField] private TMP_Dropdown componentDropdown;
    public TMP_Dropdown ComponentDropdown => componentDropdown;

    [SerializeField] private Button endAnalysisButton;
    public Button EndAnalysisButton => endAnalysisButton;

    private List<double> allTimes;
    HandResultPackage leftResults;
    HandResultPackage rightResults;

    Dictionary<PathType, Dictionary<ComponentType, DeviationData>> resultsMap;
    TargetAnalysisResults targetData;
    HandResultPackage currentResults;
    DeviationData currentDeviationData;

    PathType currentPath = PathType.RightHand;
    ComponentType currentComponent = ComponentType.Overall;

    public void SetResults(HandResultPackage leftResults, HandResultPackage rightResults,
        TargetAnalysisResults targetData, List<double> allTimes)
    {
        this.allTimes = allTimes;
        this.targetData = targetData;
        this.leftResults = leftResults;
        this.rightResults = rightResults;
        resultsMap = new Dictionary<PathType, Dictionary<ComponentType, DeviationData>>()
        {
            {
                PathType.LeftHand, new Dictionary<ComponentType, DeviationData>()
                {
                    { ComponentType.Overall, leftResults.total },
                    { ComponentType.Approach, leftResults.approach },
                    { ComponentType.Search, leftResults.search }
                }
            },
            {
                PathType.RightHand, new Dictionary<ComponentType, DeviationData>()
                {
                    { ComponentType.Overall, rightResults.total },
                    { ComponentType.Approach, rightResults.approach },
                    { ComponentType.Search, rightResults.search }
                }
            }
        };

        UpdatePath();
        UpdateComponent();
        RefreshData();
    }

    private void Start()
    {
        deviationDropdown.onValueChanged.AddListener(UpdateDeviationStatistics);
        pathDropdown.onValueChanged.AddListener(UpdatePath);
        componentDropdown.onValueChanged.AddListener(UpdateComponent);
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
            currentPath = PathType.LeftHand;
        }
        else if (path == "Right Hand")
        {
            currentResults = rightResults;
            currentPath = PathType.RightHand;
        }

        RefreshData();
    }

    private void UpdateComponent()
    {
        UpdateComponent(componentDropdown.value);
    }

    private void UpdateComponent(int index)
    {
        string component = componentDropdown.options[index].text;

        if (component == "Overall")
        {
            targetStatisticsManager.SetStatistics(targetData.targetToTargetTimes);
            currentComponent = ComponentType.Overall;
        }
        else if (component == "Approach")
        {
            targetStatisticsManager.SetStatistics(targetData.preSearchTimes);
            currentComponent = ComponentType.Approach;
        }
        else if (component == "Search")
        {
            targetStatisticsManager.SetStatistics(targetData.searchTimes);
            currentComponent = ComponentType.Search;
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
            deviationStatisticsManager.SetStatistics(currentDeviationData.statsV);
        }
        else if (selectedDeviation == "Left/Right")
        {
            deviationStatisticsManager.SetStatistics(currentDeviationData.statsH);
        }
        else if (selectedDeviation == "Total")
        {
            deviationStatisticsManager.SetStatistics(currentDeviationData.statsDist);
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
        currentDeviationData = resultsMap[currentPath][currentComponent];

        UpdateDeviationStatistics();
        UpdateGraphs();
    }

    private void OnDestroy()
    {
        deviationDropdown.onValueChanged.RemoveAllListeners();
        pathDropdown.onValueChanged.RemoveAllListeners();
        componentDropdown.onValueChanged.RemoveAllListeners();
    }

    private enum PathType
    {
        LeftHand,
        RightHand
    }

    private enum ComponentType
    {
        Overall,
        Approach,
        Search
    }
}
