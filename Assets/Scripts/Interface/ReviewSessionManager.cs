using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReviewSessionManager : MonoBehaviour
{
    [Header("Control Panel")]
    [SerializeField] private Button endReviewButton;
    public Button EndReviewButton => endReviewButton;

    [SerializeField] private Toggle showLeftPathsToggle;
    public Toggle ShowLeftPathsToggle => showLeftPathsToggle;

    [SerializeField] private Toggle showRightPathsToggle;
    public Toggle ShowRightPathsToggle => showRightPathsToggle;

    [SerializeField] private GameObject scrollView;
    public GameObject ScrollView => scrollView;

}
