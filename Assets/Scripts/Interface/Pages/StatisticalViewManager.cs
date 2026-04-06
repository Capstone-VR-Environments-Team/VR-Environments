using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XCharts.Runtime;

public class StatisticalViewManager : MonoBehaviour
{
    [Header("Deviation Statistics")]
    [SerializeField] private TMP_Dropdown deviationDropdown;
    [SerializeField] private StatisticsManager deviationStatisticsManager;

    [Header("Target Statistics")]
    [SerializeField] private StatisticsManager targetStatisticsManager;

    [Header("Graphs")]
    [SerializeField] private BaseChart xAxisDeviationGraph;
    [SerializeField] private BaseChart yAxisDeviationGraph;
    [SerializeField] private BaseChart zAxisDeviationGraph;
    [SerializeField] private BaseChart deviationMagnitudeGraph;

    [Header("Control Panel")]
    [SerializeField] private TMP_Dropdown pathDropdown;
    [SerializeField] private TMP_Dropdown componentDropdown;
    [SerializeField] private Button exportButton;
    [SerializeField] private Button endAnalysisButton;

    private IReadOnlyList<double> allTimes = System.Array.Empty<double>();
    private IReadOnlyList<double> currentTimes = System.Array.Empty<double>();
    private AnalyzedData analyzedData;
    TargetAnalysisResults targetData;
    private IReadOnlyList<AnalysisMode> currentPointTypes = System.Array.Empty<AnalysisMode>();
    private IReadOnlyList<double> currentDistValues = System.Array.Empty<double>();
    private IReadOnlyList<double> currentXValues = System.Array.Empty<double>();
    private IReadOnlyList<double> currentYValues = System.Array.Empty<double>();
    private IReadOnlyList<double> currentZValues = System.Array.Empty<double>();
    private readonly Dictionary<Hand, IReadOnlyList<AnalysisMode>> pointTypesByHand = new Dictionary<Hand, IReadOnlyList<AnalysisMode>>();

    PathType currentPath = PathType.RightHand;
    ComponentType currentComponent = ComponentType.Overall;

    public void SetResults(AnalyzedData analyzedData, IReadOnlyList<AnalysisMode> leftPointTypes, IReadOnlyList<AnalysisMode> rightPointTypes,
        TargetAnalysisResults targetData, List<double> allTimes)
    {
        this.allTimes = allTimes ?? (IReadOnlyList<double>)System.Array.Empty<double>();
        this.targetData = targetData;
        this.analyzedData = analyzedData;

        pointTypesByHand[Hand.LEFT] = leftPointTypes ?? System.Array.Empty<AnalysisMode>();
        pointTypesByHand[Hand.RIGHT] = rightPointTypes ?? System.Array.Empty<AnalysisMode>();

        UpdatePath();
        UpdateComponent();
        RefreshData();
    }

    private void Start()
    {
        exportButton.onClick.AddListener(AnalysisResultsStore.Instance.ExportAnalysisResults);
        deviationDropdown.onValueChanged.AddListener(UpdateDeviationStatistics);
        pathDropdown.onValueChanged.AddListener(UpdatePath);
        componentDropdown.onValueChanged.AddListener(UpdateComponent);
        endAnalysisButton.onClick.AddListener(OnCancel);
        LoadFromStore();
    }

    private void OnEnable()
    {
        EventBus.DataChanged += LoadFromStore;
        LoadFromStore();
    }

    private void OnDisable()
    {
        EventBus.DataChanged -= LoadFromStore;
    }

    private void OnCancel() {
        SceneManager.LoadScene("ReviewPastSession");
    }

    private void LoadFromStore()
    {
        AnalysisResultsStore store = AnalysisResultsStore.Instance;
        if (!store.HasAnalysisData || store.ProcessedData == null)
        {
            return;
        }

        SetResults(
            store.ProcessedData.AnalyzedData,
            store.ProcessedData.LeftPointTypes,
            store.ProcessedData.RightPointTypes,
            store.ProcessedData.TargetAnalysisResults,
            store.ProcessedData.AllTimes);
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
            currentPath = PathType.LeftHand;
        }
        else if (path == "Right Hand")
        {
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
        Hand selectedHand = GetSelectedHand();
        MovementZone selectedZone = GetSelectedZone();
        DeviationType selectedDeviation = GetSelectedDeviationType(index);

        Statistics selectedStats = analyzedData?.GetStatistics(selectedHand, selectedZone, selectedDeviation) ?? new Statistics();
        deviationStatisticsManager.SetStatistics(selectedStats);
    }

    private void UpdateGraphs()
    {
        UpdateGraph(deviationMagnitudeGraph, currentTimes, currentDistValues, currentPointTypes);
        UpdateGraph(xAxisDeviationGraph, currentTimes, currentXValues, currentPointTypes);
        UpdateGraph(yAxisDeviationGraph, currentTimes, currentYValues, currentPointTypes);
        UpdateGraph(zAxisDeviationGraph, currentTimes, currentZValues, currentPointTypes);
    }

    private void UpdateGraph(BaseChart graph, IReadOnlyList<double> times, IReadOnlyList<double> values, IReadOnlyList<AnalysisMode> pointTypes)
    {
        if (graph == null) {
            return;
        }

        Tooltip tooltip = graph.EnsureChartComponent<Tooltip>();
        if (tooltip != null) {
            tooltip.show = true;
            tooltip.trigger = Tooltip.Trigger.Item;
            tooltip.titleFormatter = "{i}";
            tooltip.itemFormatter = "Time: {c0:f0}\nDeviation: {c1:f4}";
        }

        graph.ClearData();
        graph.RemoveAllSerie();

        int count = Mathf.Min(values.Count, Mathf.Min(times.Count, pointTypes.Count));
        if (count == 0) { return; }
        AnalysisMode previousType = AnalysisMode.LINETOTARGET;

        Color previousSphereColor = new Color { r = 188f / 255f, g = 71f / 255.0f, b = 73f / 255.0f, a = 1 };
        Color approachColor = new Color{ r = 106.0f / 255.0f, g = 153.0f / 255.0f, b = 77.0f / 255.0f, a = 1 }; 
        Color searchColor = Color.purple;

        int currentSerieIndex = -1;

        for (int i = 0; i < count; i++) {
            double timestamp = times[i];
            double val = values[i];
            AnalysisMode currentType = pointTypes[i];

            // Handle Switch between mode
            if (i == 0 || currentType != previousType) {
                currentSerieIndex++;

                var newSerie = graph.AddSerie<Line>($"Segment_{currentSerieIndex}");

                // Configure Style
                newSerie.lineStyle.width = 2.0f;
                newSerie.symbol.show = false;

                // Set Color based on Mode
                if (currentType == AnalysisMode.PREVIOUSSPHERE)
                {
                    newSerie.lineStyle.color = previousSphereColor;
                    newSerie.itemStyle.color = previousSphereColor;
                }
                else if (currentType == AnalysisMode.POINTTOTARGET)
                {
                    newSerie.lineStyle.color = searchColor;
                    newSerie.itemStyle.color = searchColor;
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
        Hand selectedHand = GetSelectedHand();

        // Graphs intentionally always use OVERALL values regardless of selected component.
        SingleDataSet overallTotalData = analyzedData?.GetDataOrEmpty(selectedHand, MovementZone.OVERALL, DeviationType.TOTAL) ?? SingleDataSet.Empty;
        SingleDataSet overallXData = analyzedData?.GetDataOrEmpty(selectedHand, MovementZone.OVERALL, DeviationType.X_DEV) ?? SingleDataSet.Empty;
        SingleDataSet overallYData = analyzedData?.GetDataOrEmpty(selectedHand, MovementZone.OVERALL, DeviationType.Y_DEV) ?? SingleDataSet.Empty;
        SingleDataSet overallZData = analyzedData?.GetDataOrEmpty(selectedHand, MovementZone.OVERALL, DeviationType.Z_DEV) ?? SingleDataSet.Empty;

        currentDistValues = overallTotalData.Points ?? System.Array.Empty<double>();
        currentXValues = overallXData.Points ?? System.Array.Empty<double>();
        currentYValues = overallYData.Points ?? System.Array.Empty<double>();
        currentZValues = overallZData.Points ?? System.Array.Empty<double>();
        currentTimes = overallTotalData.Times != null && overallTotalData.Times.Count > 0
            ? overallTotalData.Times
            : allTimes;

        if (!pointTypesByHand.TryGetValue(selectedHand, out currentPointTypes))
        {
            currentPointTypes = System.Array.Empty<AnalysisMode>();
        }

        UpdateDeviationStatistics();
        UpdateGraphs();
    }

    private Hand GetSelectedHand()
    {
        return currentPath == PathType.LeftHand ? Hand.LEFT : Hand.RIGHT;
    }

    private MovementZone GetSelectedZone()
    {
        return currentComponent switch
        {
            ComponentType.Approach => MovementZone.APPROACH,
            ComponentType.Search => MovementZone.SEARCH,
            _ => MovementZone.OVERALL
        };
    }

    private DeviationType GetSelectedDeviationType(int index)
    {
        string selectedDeviation = deviationDropdown.options[index].text;
        return selectedDeviation switch
        {
            "X" => DeviationType.X_DEV,
            "Y" => DeviationType.Y_DEV,
            "Z" => DeviationType.Z_DEV,
            _ => DeviationType.TOTAL
        };
    }

    private void OnDestroy()
    {
        deviationDropdown.onValueChanged.RemoveAllListeners();
        pathDropdown.onValueChanged.RemoveAllListeners();
        componentDropdown.onValueChanged.RemoveAllListeners();
        endAnalysisButton.onClick.RemoveAllListeners();
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

    public void AddEndAnalysisOnClick(UnityEngine.Events.UnityAction action)
    {
        endAnalysisButton.onClick.AddListener(action);
    }
}
