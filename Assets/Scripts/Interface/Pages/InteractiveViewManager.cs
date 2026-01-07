using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveViewManager : MonoBehaviour
{
    [Header("Control Panel")]
    [SerializeField] private Button endReviewButton;
    public Button EndReviewButton => endReviewButton;

    [SerializeField] private Toggle showLeftPathsToggle;
    public Toggle ShowLeftPathsToggle => showLeftPathsToggle;

    [SerializeField] private Toggle showRightPathsToggle;
    public Toggle ShowRightPathsToggle => showRightPathsToggle;

    [SerializeField] private Toggle showOptimalPathsToggle;
    public Toggle ShowOptimalPathsToggle => showOptimalPathsToggle;

    [Header("Statistics")]
    [SerializeField] private TMP_Dropdown pathDropdown;
    public TMP_Dropdown PathDropdown => pathDropdown;

    [SerializeField] private StatisticsManager statisticsManager;
    public StatisticsManager StatisticsManager => statisticsManager;

    private void Start()
    {
        pathDropdown.onValueChanged.AddListener(UpdatePath);
    }

    Statistics leftStatistics;
    Statistics rightStatistics;

    Statistics currentStatistics;

    public void SetStatistics(Statistics leftStatistics, Statistics rightStatistics)
    {
        this.leftStatistics = leftStatistics;
        this.rightStatistics = rightStatistics;

        UpdatePath();
        UpdateStatistics();
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
            currentStatistics = leftStatistics;
        }
        else if (path == "Right Hand")
        {
            currentStatistics = rightStatistics;
        }

        UpdateStatistics();
    }

    public void UpdateStatistics()
    {
        statisticsManager.SetStatistics(currentStatistics);
    }

    private void OnDestroy()
    {
        pathDropdown.onValueChanged.RemoveAllListeners();
    }
}
