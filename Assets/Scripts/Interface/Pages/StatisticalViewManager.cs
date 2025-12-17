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
    [SerializeField] private LineChart xAxisDeviationGraph;
    public LineChart XAxisDeviationGraph => xAxisDeviationGraph;

    [SerializeField] private LineChart yAxisDeviationGraph;
    public LineChart YAxisDeviationGraph => yAxisDeviationGraph;

    [SerializeField] private LineChart deviationMagnitudeGraph;
    public LineChart DeviationMagnitudeGraph => deviationMagnitudeGraph;

    [SerializeField] private LineChart eegSignalsGraph;
    public LineChart EEGSignalsGraph => eegSignalsGraph;

    [Header("Control Panel")]
    [SerializeField] private TMP_Dropdown pathDropdown;
    public TMP_Dropdown PathDropdown => pathDropdown;

    [SerializeField] private Button endAnalysisButton;
    public Button EndAnalysisButton => endAnalysisButton;


}
