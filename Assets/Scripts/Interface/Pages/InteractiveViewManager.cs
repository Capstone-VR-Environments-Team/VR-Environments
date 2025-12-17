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
}
