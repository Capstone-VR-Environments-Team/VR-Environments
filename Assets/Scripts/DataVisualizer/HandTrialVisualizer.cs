using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HandTrailVisualizer : MonoBehaviour {
    [Header("Visualization Settings")]
    public Material lineMaterial;
    public float lineWidth = 0.3f;
    public GameObject trialTogglePrefab;
    public Transform toggleContainer;
    public Button toggleAllButton;
    public Button toggleLeftButton;
    public Button toggleRightButton;

    private readonly List<GameObject> activeTrails = new();
    private readonly Dictionary<string, GameObject> trailLookup = new();
    private readonly List<Toggle> trailToggles = new();
    private IFileLoader fileLoader = null;

    private readonly Color[] colorPalette = {
        Color.red, Color.green, Color.blue, Color.yellow,
        Color.cyan, Color.magenta, Color.white, new Color(1f,0.5f,0f)
    };

    private void Start() {
        fileLoader = new CSVFileLoader();

        // Add in the different filter options
        toggleAllButton.onClick.AddListener(() => ToggleGroup("All"));
        toggleLeftButton.onClick.AddListener(() => ToggleGroup("LeftHand"));
        toggleRightButton.onClick.AddListener(() => ToggleGroup("RightHand"));
    }

    void Update() {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Press 'L' to open file dialog
        if (keyboard.lKey.wasPressedThisFrame) {
            LoadFiles();
        }
    }
    void LoadFiles() {
        // Get multiple selected files
        string[] filePaths = FileSelector.getFilePaths(Application.persistentDataPath, "csv");
        
        // Load all the selected files and create their selected trails
        foreach (string file in filePaths) {
            List<TrackingData> data = fileLoader.loadFile(file);
            CreateTrails(data, Path.GetFileNameWithoutExtension(file));
        }
    }

    void CreateTrails(List<TrackingData> trackingData, string fileName) {
        List<Vector3> leftPoints = new();
        List<Vector3> rightPoints = new();

        // Split the data
        foreach (TrackingData data in trackingData) {
            leftPoints.Add(data.leftHandPos);
            rightPoints.Add(data.rightHandPos);
        }

        // Grab next color
        int colorIndex = activeTrails.Count % colorPalette.Length;

        // Make left trail
        if (leftPoints.Count > 1)
            CreateLine(fileName, "LeftHand", leftPoints, colorPalette[colorIndex]);

        // Make right trail
        if (rightPoints.Count > 1)
            CreateLine(fileName, "RightHand", rightPoints, colorPalette[(colorIndex + 1) % colorPalette.Length]);

        Debug.Log($"Loaded {fileName} with {leftPoints.Count} left and {rightPoints.Count} right points");
    }

    /** 
     Create the line object from the given points
    */
    void CreateLine(string fileName, string handLabel, List<Vector3> points, Color color) {
        GameObject lineObj = new GameObject($"{fileName}_{handLabel}");
        var lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
        lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.startColor = color;
        lr.endColor = color;
        lr.useWorldSpace = true;
        activeTrails.Add(lineObj);
        trailLookup[lineObj.name] = lineObj;
        AddTrailToggle(lineObj, color);
    }

    /** 
     Add a toggle to the toggling menu for the given game object
    */
    void AddTrailToggle(GameObject trailObj, Color color) {
        GameObject toggleGO = Instantiate(trialTogglePrefab, toggleContainer);
        Toggle toggle = toggleGO.GetComponent<Toggle>();
        Text label = toggleGO.GetComponentInChildren<Text>();
        Debug.Log(trailObj.name);
        label.text = trailObj.name;
        toggle.name = trailObj.name;
        Debug.Log(label.text);
        label.color = color;
        Debug.Log(label.color);
        toggle.isOn = true;

        toggle.onValueChanged.AddListener(isOn => trailObj.SetActive(isOn));

        trailToggles.Add(toggle);
    }

    /** 
     Handle toggling a group based on descriptor in the name or all items
    */
    void ToggleGroup(string group) {
        bool newState;

        if (group == "All") {
            bool anyOff = trailToggles.Exists(t => !t.isOn);
            newState = anyOff;
            foreach (var t in trailToggles)
                t.isOn = newState;
            return;
        }

        bool anyOffGroup = trailToggles.Exists(t => t.name.Contains(group) && !t.isOn);
        newState = anyOffGroup;

        foreach (var t in trailToggles)
            if (t.name.Contains(group))
                t.isOn = newState;
    }

}
