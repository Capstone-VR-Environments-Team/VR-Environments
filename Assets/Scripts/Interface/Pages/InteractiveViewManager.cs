using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InteractiveViewManager : MonoBehaviour
{
    [Header("Visualization Settings")]
    public Material lineMaterial;
    public float lineWidth = 2f;

    [Header("Control Panel")]
    [SerializeField] private Button endReviewButton;
    [SerializeField] private Toggle showLeftPathsToggle;
    [SerializeField] private Toggle showRightPathsToggle;
    [SerializeField] private Toggle showOptimalPathsToggle;
    [SerializeField] private Toggle showTargetsToggle;

    [Header("Statistics")]
    [SerializeField] private TMP_Dropdown pathDropdown;
    [SerializeField] private StatisticsManager statisticsManager;

    private GameObject _leftLine;
    private GameObject _rightLine;
    private GameObject _targetLine;
    private GameObject _targets;

    private CameraController _controller;

    private void Awake() {
        _controller = FindFirstObjectByType<CameraController>();
    }

    private void Start()
    {
        pathDropdown.onValueChanged.AddListener(UpdatePath);
        showLeftPathsToggle.onValueChanged.AddListener(ToggleLeft);
        showRightPathsToggle.onValueChanged.AddListener(ToggleRight);
        showOptimalPathsToggle.onValueChanged.AddListener(ToggleOptimal);
        showTargetsToggle.onValueChanged.AddListener(ToggleTargets);
        endReviewButton.onClick.AddListener(LeaveScreen);
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

    private void LoadFromStore()
    {
        AnalysisResultsStore store = AnalysisResultsStore.Instance;
        if (!store.HasAnalysisData || store.ProcessedData == null || store.TrialInfo == null)
        {
            return;
        }

        SetStatistics(
            store.ProcessedData.AnalyzedData.GetStatistics(Hand.LEFT, MovementZone.OVERALL, DeviationType.TOTAL),
            store.ProcessedData.AnalyzedData.GetStatistics(Hand.RIGHT, MovementZone.OVERALL, DeviationType.TOTAL));

        SetPaths(store.RawData, store.TrialInfo.CollectedTimingData.TargetHits);
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

    public void AddEndReviewOnClick(UnityEngine.Events.UnityAction action) {
        endReviewButton.onClick.AddListener(action);
    }

    public void SetPaths(List<TrackingData> rawData, List<HitEvent> targetData) {
        ClearPathObjects();

        List<Vector3> leftPoints = new();
        List<Vector3> rightPoints = new();
        List<Vector3> targetPoints = new();

        _targets = new GameObject("Targets");
        _targets.transform.position = Vector3.zero;

        // Split the data
        foreach (TrackingData data in rawData) {
            leftPoints.Add(data.leftHandPos);
            rightPoints.Add(data.rightHandPos);
        }

        foreach (HitEvent point in targetData) {
            targetPoints.Add(point.location);
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.SetParent(_targets.transform);
            sphere.transform.localPosition = point.location;
            sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        }

        if (leftPoints.Count > 1)
            _leftLine = CreateLine("LeftHand", leftPoints, Color.red);

        if (rightPoints.Count > 1)
            _rightLine = CreateLine("RightHand", rightPoints, Color.blue);

        if (targetPoints.Count > 1)
            _targetLine = CreateLine("Targets", targetPoints, Color.green);

        Debug.Log($"Added lines with {leftPoints.Count} left, {rightPoints.Count} right, and {targetPoints.Count} points");
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
        showLeftPathsToggle.onValueChanged.RemoveAllListeners();
        showRightPathsToggle.onValueChanged.RemoveAllListeners();
        showOptimalPathsToggle.onValueChanged.RemoveAllListeners();
        showTargetsToggle.onValueChanged.RemoveAllListeners();
        endReviewButton.onClick.RemoveAllListeners();

        ClearPathObjects();
    }

    private void ClearPathObjects()
    {
        if (_leftLine)
        {
            Destroy(_leftLine);
            _leftLine = null;
        }

        if (_rightLine)
        {
            Destroy(_rightLine);
            _rightLine = null;
        }

        if (_targetLine)
        {
            Destroy(_targetLine);
            _targetLine = null;
        }

        if (_targets)
        {
            Destroy(_targets);
            _targets = null;
        }
    }

    private void ToggleLeft(bool isOn) {
        if (_leftLine)
            _leftLine.SetActive(isOn);
    }

    private void ToggleRight(bool isOn) {
        if (_rightLine)
            _rightLine.SetActive(isOn);
    }
    private void ToggleOptimal(bool isOn) {
        if (_targetLine)
            _targetLine.SetActive(isOn);
    }
    private void ToggleTargets(bool isOn) {
        if (_targets)
            _targets.SetActive(isOn);
    }

    private void LeaveScreen() {
        SceneManager.LoadScene("ReviewPastSession");
    }

    /** 
     Create the line object from the given points
    */
    GameObject CreateLine(string handLabel, List<Vector3> points, Color color) {
        GameObject lineObj = new GameObject($"{handLabel}");
        var lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
        lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.widthCurve = AnimationCurve.Constant(0,1,1);
        lr.widthMultiplier = 0.01f;
        lr.startColor = color;
        lr.endColor = color;
        lr.useWorldSpace = true;
        lineObj.SetActive(true);
        return lineObj;
    }
}
