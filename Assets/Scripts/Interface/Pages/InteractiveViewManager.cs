using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveViewManager : MonoBehaviour
{
    [Header("Visualization Settings")]
    public Material lineMaterial;
    public float lineWidth = 0.3f;

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

    private GameObject _leftLine;
    private GameObject _rightLine;
    private GameObject _targetLine;
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
        endReviewButton.onClick.AddListener(LeaveScreen);
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

    public void SetPaths(List<TrackingData> rawData, List<KeyPoint> targetData) {
        List<Vector3> leftPoints = new();
        List<Vector3> rightPoints = new();
        List<Vector3> targetPoints = new();

        // Split the data
        foreach (TrackingData data in rawData) {
            leftPoints.Add(data.leftHandPos);
            rightPoints.Add(data.rightHandPos);
        }

        foreach (KeyPoint point in targetData) {
            targetPoints.Add(point.Position);
        }

        if (leftPoints.Count > 1)
            _leftLine = CreateLine("LeftHand", leftPoints, Color.red);

        if (rightPoints.Count > 1)
            _rightLine = CreateLine("RightHand", rightPoints, Color.blue);

        if (rightPoints.Count > 1)
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

    private void LeaveScreen() {
        if (_leftLine)
            _leftLine.SetActive(false);
        if (_rightLine)
            _rightLine.SetActive(false);
        if (_targetLine)
            _targetLine.SetActive(false);
        _controller.TurnOff();
    }

    public void EnterScreen() {
        if (_leftLine)
            _leftLine.SetActive(true);
        if (_rightLine)
            _rightLine.SetActive(true);
        if (_targetLine)
            _targetLine.SetActive(true);
        _controller.TurnOn();
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
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.startColor = color;
        lr.endColor = color;
        lr.useWorldSpace = true;
        lineObj.SetActive(false);
        return lineObj;
    }
}
