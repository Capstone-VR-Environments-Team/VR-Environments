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

    [SerializeField] private TMP_Text averageText;
    public TMP_Text AverageText => averageText;

    [SerializeField] private TMP_Text maximumText;
    public TMP_Text MaximumText => maximumText;

    [SerializeField] private TMP_Text minimumText;
    public TMP_Text MinimumText => minimumText;

    [SerializeField] private TMP_Text stDevText;
    public TMP_Text StDevText => stDevText;

    public void SetStatistics(Statistics statistics)
    {
        averageText.SetText(statistics.Average.ToString());
        maximumText.SetText(statistics.Maximum.ToString());
        minimumText.SetText(statistics.Minimum.ToString());
        stDevText.SetText(statistics.StDev.ToString());
    }
}
