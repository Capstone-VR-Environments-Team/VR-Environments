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
    [SerializeField] private GameObject targetSpherePrefab;
    [SerializeField] private Vector3 defaultTargetSphereScale = new Vector3(0.05f, 0.05f, 0.05f);

    [Header("Default Colors")]
    [SerializeField] private Color defaultLeftLineColor = Color.blue;
    [SerializeField] private Color defaultRightLineColor = Color.red;
    [SerializeField] private Color defaultTargetColor = Color.gray;

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
    private Color _leftLineColor;
    private Color _rightLineColor;
    private Color _targetColor;
    private Color _optimalLineColor = Color.green;
    private Vector3 _targetSphereScale;

    private void Awake() {
        _controller = FindFirstObjectByType<CameraController>();
    }

    private void Start()
    {
        _leftLineColor = defaultLeftLineColor;
        _rightLineColor = defaultRightLineColor;
        _targetColor = defaultTargetColor;
        _targetSphereScale = defaultTargetSphereScale;

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

        ApplyColorsFromTrialSettings(store.TrialInfo);
        ApplySphereScaleFromTrialSettings(store.TrialInfo);

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

        // Dictionary to keep track of spawned spheres and their labels
        Dictionary<Vector3, SphereLabel> spawnedSpheres = new Dictionary<Vector3, SphereLabel>();

        foreach (HitEvent point in targetData) {
            targetPoints.Add(point.location);
            int currentTargetNumber = targetPoints.Count;

            // Check if a sphere already exists at this location (using a small tolerance for float precision)
            SphereLabel existingLabel = null;
            foreach (var kvp in spawnedSpheres) {
                if (Vector3.Distance(kvp.Key, point.location) < 0.001f) {
                    existingLabel = kvp.Value;
                    break;
                }
            }

            if (existingLabel != null) {
                // A sphere already exists here; just update the label
                existingLabel.AppendNumber(currentTargetNumber);
            } 
            else {
                // No sphere exists here; create a new one
                GameObject sphere = targetSpherePrefab != null
                    ? Instantiate(targetSpherePrefab)
                    : GameObject.CreatePrimitive(PrimitiveType.Sphere);

                sphere.transform.SetParent(_targets.transform);
                sphere.transform.localPosition = point.location;
                sphere.transform.localScale = _targetSphereScale;

                MeshRenderer sphereRenderer = sphere.GetComponent<MeshRenderer>();
                if (sphereRenderer != null) {
                    sphereRenderer.material.color = _targetColor;
                }

                SphereLabel label = sphere.GetComponent<SphereLabel>();
                if (label != null) {
                    label.Initialize(currentTargetNumber);
                    // Track this new sphere for future overlap checks
                    spawnedSpheres.Add(point.location, label);
                }
            }
        }

        if (leftPoints.Count > 1)
            _leftLine = CreateLine("LeftHand", leftPoints, _leftLineColor);

        if (rightPoints.Count > 1)
            _rightLine = CreateLine("RightHand", rightPoints, _rightLineColor);

        if (targetPoints.Count > 1)
            _targetLine = CreateLine("Targets", targetPoints, _optimalLineColor);

        Debug.Log($"Added lines with {leftPoints.Count} left, {rightPoints.Count} right, and {targetPoints.Count} points");
    }

    private void ApplyColorsFromTrialSettings(JsonWrapper trialInfo) {
        _leftLineColor = defaultLeftLineColor;
        _rightLineColor = defaultRightLineColor;
        _targetColor = defaultTargetColor;

        ColorSettings visibilitySettings = trialInfo?.TrialSessionInformation?.TrialSettings?.ColorSettings;
        if (visibilitySettings == null) {
            return;
        }

        if (TryParseHexColor(visibilitySettings.LeftHandColor, out Color leftColor)) {
            _leftLineColor = leftColor;
        }

        if (TryParseHexColor(visibilitySettings.RightHandColor, out Color rightColor)) {
            _rightLineColor = rightColor;
        }

        if (TryParseHexColor(visibilitySettings.TargetColor, out Color targetColor)) {
            _targetColor = targetColor;
        }
    }

    private void ApplySphereScaleFromTrialSettings(JsonWrapper trialInfo) {
        _targetSphereScale = defaultTargetSphereScale;

        TargetSettings targetSettings = trialInfo?.TrialSessionInformation?.TrialSettings?.TargetSettings;
        if (targetSettings == null) {
            return;
        }

        _targetSphereScale = Vector3.one * (targetSettings.TargetSize / 100.0f * 2f);
    }

    private bool TryParseHexColor(string colorString, out Color color) {
        color = Color.white;
        if (string.IsNullOrWhiteSpace(colorString)) {
            return false;
        }

        string normalized = colorString.Trim();
        if (!normalized.StartsWith("#")) {
            normalized = "#" + normalized;
        }

        return ColorUtility.TryParseHtmlString(normalized, out color);
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
        lr.widthMultiplier = Mathf.Max(0.0001f, lineWidth * 0.005f);
        lr.startColor = color;
        lr.endColor = color;
        lr.useWorldSpace = true;
        lineObj.SetActive(true);
        return lineObj;
    }
}
