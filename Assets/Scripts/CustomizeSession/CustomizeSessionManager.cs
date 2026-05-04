using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomizeSessionManager : MonoBehaviour
{
    [Header("Configuration Name")]
    [SerializeField] private TMP_InputField configurationNameInput;

    [Header("Visibility Settings")]
    [SerializeField] private TMP_Dropdown handVisibility;
    [SerializeField] private TMP_Dropdown targetVisibility;
    [SerializeField] private TMP_InputField handFlickerShow;
    [SerializeField] private TMP_InputField handFlickerHide;
    [SerializeField] private TMP_InputField targetFlickerShow;
    [SerializeField] private TMP_InputField targetFlickerHide;

    [Header("Offset Settings")]
    [SerializeField] private TMP_Dropdown offsetTypeDropdown;
    [SerializeField] private TMP_InputField offsetXInput;
    [SerializeField] private TMP_InputField offsetYInput;
    [SerializeField] private TMP_InputField offsetZInput;
    [SerializeField] private TMP_InputField targetRangeInput;
    [SerializeField] private Toggle showHandInProximityToggle;

    [Header("Background Settings")]
    [SerializeField] private Button uploadBackgroundFile;
    [SerializeField] private TMP_Text uploadedBackgroundFileNameText;
    [SerializeField] private Toggle movingObjectsToggle;
    [SerializeField] private TMP_Dropdown directionTypeDropdown;
    [SerializeField] private TMP_InputField speedInput;
    [SerializeField] private TMP_InputField numberOfObjectsInput;
    [SerializeField] private TMP_InputField objectColorInput;
    [SerializeField] private TMP_Text objectSizeInput;
    [SerializeField] private TMP_InputField objectSizeXInput;
    [SerializeField] private TMP_InputField objectSizeYInput;
    [SerializeField] private TMP_InputField objectSizeZInput;

    [Header("Target Settings")]
    [SerializeField] private TMP_InputField timeBeforeStart;
    [SerializeField] private TMP_InputField targetSize;

    [Header("Color Settings")]
    [SerializeField] private TMP_InputField leftHandColor;
    [SerializeField] private TMP_InputField rightHandColor;
    [SerializeField] private TMP_InputField targetColor;

    [Header("Buttons")]
    [SerializeField] private Button saveConfigurationButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button uploadTargetLocationsButton;
    [SerializeField] private TMP_Text uploadedFileNameText;

    private List<Vector3> _tempTargetLocations = new List<Vector3>();
    private string backgroundFilePath = "";

    private float _diffY;

    private void Start()
    {
        _diffY = handFlickerShow.gameObject.transform.localPosition.y -
                 targetFlickerShow.gameObject.transform.localPosition.y;
        ;
        handVisibility.onValueChanged.AddListener(delegate { UpdateFlickerInputFields(); });
        targetVisibility.onValueChanged.AddListener(delegate { UpdateFlickerInputFields(); });
        movingObjectsToggle.onValueChanged.AddListener(delegate { UpdateBackgroundInputFields(); });
        configurationNameInput.onValueChanged.AddListener(delegate { EnableButtons(); });
        
        UpdateFlickerInputFields();
        UpdateBackgroundInputFields();
        EnableButtons();
    }

    private void UpdateFlickerInputFields()
    {
        Debug.Log("here");
        string handFlickerState = handVisibility.options[handVisibility.value].text;
        string targetFlickerState = targetVisibility.options[targetVisibility.value].text;

        if (handFlickerState == "Flicker" && targetFlickerState == "Flicker")
        {
            targetVisibility.gameObject.transform.localPosition =
                handVisibility.gameObject.transform.localPosition - new Vector3(0, _diffY, 0);
            targetFlickerShow.gameObject.transform.localPosition =
                handFlickerShow.gameObject.transform.localPosition - new Vector3(0, _diffY, 0);
            targetFlickerHide.gameObject.transform.localPosition =
                handFlickerHide.gameObject.transform.localPosition - new Vector3(0, _diffY, 0);
            UpdateFlickerInputFields(true, true);
        }
        else if (handFlickerState == "Flicker")
        {
            targetVisibility.gameObject.transform.localPosition =
                handVisibility.gameObject.transform.localPosition - new Vector3(0, _diffY, 0);
            UpdateFlickerInputFields(true, false);
        }
        else if (targetFlickerState == "Flicker")
        {
            targetVisibility.gameObject.transform.localPosition =
                handFlickerShow.gameObject.transform.localPosition;
            targetFlickerShow.gameObject.transform.localPosition =
                handFlickerHide.gameObject.transform.localPosition;
            targetFlickerHide.gameObject.transform.localPosition =
                handVisibility.gameObject.transform.localPosition - new Vector3(0, _diffY, 0);
            UpdateFlickerInputFields(false, true);
        }
        else
        {
            targetVisibility.gameObject.transform.localPosition =
                handFlickerShow.gameObject.transform.localPosition;
            UpdateFlickerInputFields(false, false);
        }

    }

    private void UpdateFlickerInputFields(bool handFields, bool targetFields)
    {
        handFlickerShow.gameObject.SetActive(handFields);
        handFlickerHide.gameObject.SetActive(handFields);
        targetFlickerShow.gameObject.SetActive(targetFields);
        targetFlickerHide.gameObject.SetActive(targetFields);
    }

    private void UpdateBackgroundInputFields()
    {
        bool movingObjectsState = movingObjectsToggle.isOn;
        
        directionTypeDropdown.gameObject.SetActive(movingObjectsState);
        speedInput.gameObject.SetActive(movingObjectsState);
        numberOfObjectsInput.gameObject.SetActive(movingObjectsState);
        objectColorInput.gameObject.SetActive(movingObjectsState);
        objectSizeInput.gameObject.SetActive(movingObjectsState);
        objectSizeXInput.gameObject.SetActive(movingObjectsState);
        objectSizeYInput.gameObject.SetActive(movingObjectsState);
        objectSizeZInput.gameObject.SetActive(movingObjectsState);
    }

    private void EnableButtons()
    {
        saveConfigurationButton.interactable = !string.IsNullOrEmpty(configurationNameInput.text) && _tempTargetLocations.Count > 0;
    }

    public void OnUploadLocationsClicked()
    {
        string filePath = FileSelector.getFilePath(SessionManager.BaseDataPath, new string[] { "json", "csv" });
        if (string.IsNullOrEmpty(filePath)) return;

        try
        {
            var (importedData, fileName) = FileManager.LoadFromFile<TargetImportData>(filePath);

            if (importedData != null && importedData.targets != null && importedData.targets.Count > 0)
            {
                Debug.Log("filename: " + fileName);
                uploadedFileNameText.SetText(fileName);
                uploadedFileNameText.color = new Color32(56, 56, 56, 255);
                _tempTargetLocations = RoundTargetLocations(importedData.targets);
            }
            else
            {
                // Graceful failure for bad data
                uploadedFileNameText.SetText("Invalid file format");
                uploadedFileNameText.color = Color.red;
                _tempTargetLocations.Clear();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load target locations from file: {ex.Message}");
            uploadedFileNameText.SetText("File Read Error. Please try uploading again.");
            uploadedFileNameText.color = Color.red;
            _tempTargetLocations.Clear();
        }

        EnableButtons();
    }

    public float SafeParse(string input, float defaultValue, bool allowNegative = false)
    {
        if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
        {
            float val = (float)Math.Round(result, 5, MidpointRounding.AwayFromZero);
            return allowNegative ? val : Mathf.Abs(val);
        }
        return defaultValue;
    }

    public int SafeParseInt(string input, int defaultValue, bool allowNegative = false)
    {
        if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
        {
            return allowNegative ? result : Mathf.Abs(result);
        }
        return defaultValue;
    }

    private string ValidateHexColor(string input, string defaultColor)
    {
        if (string.IsNullOrWhiteSpace(input))
            return defaultColor;

        string cleanInput = input.Trim().TrimStart('#');

        if (cleanInput.Length != 6)
            return defaultColor;

        foreach (char c in cleanInput)
        {
            bool isHex = (c >= '0' && c <= '9') ||
                         (c >= 'A' && c <= 'F') ||
                         (c >= 'a' && c <= 'f');
            if (!isHex)
            {
                return defaultColor;
            }
        }
        return cleanInput.ToUpper();
    }

    public void onFileUpload()
    {
        string filePath = FileSelector.getFilePath(SessionManager.BaseDataPath, new string[] { "jpg", "png", "mp4" });
        if (!string.IsNullOrEmpty(filePath))
        {
            backgroundFilePath = filePath;
        }
    }


    public void OnSaveButtonClicked()
    {
        TrialSettingsData trial = new TrialSettingsData
        {
            ConfigurationName = configurationNameInput.text,
            VisibilitySettings = new VisibilitySettings
            {
                HandsVisibilityType = handVisibility.options[handVisibility.value].text,
                TargetVisibilityType = targetVisibility.options[targetVisibility.value].text,
                HandsFlickerOnDuration = SafeParse(handFlickerShow.text, 1f),
                HandsFlickerOffDuration = SafeParse(handFlickerHide.text, 1f),
                TargetFlickerOnDuration = SafeParse(targetFlickerShow.text, 1f), 
                TargetFlickerOffDuration = SafeParse(targetFlickerHide.text, 1f),
            },
            OffsetSettings = new OffsetSettings
            {
                OffsetType = offsetTypeDropdown.options[offsetTypeDropdown.value].text,
                OffsetValues = new Vector3(
                    SafeParse(offsetXInput.text, 0f, true), 
                    SafeParse(offsetYInput.text, 0f, true), 
                    SafeParse(offsetZInput.text, 0f, true)  
                ),
                TargetProximity = SafeParse(targetRangeInput.text, 10f),
                ShowHandsInProximity = showHandInProximityToggle.isOn
            },
            BackgroundSettings = new BackgroundSettings
            {
                MovingBackground = movingObjectsToggle.isOn,
                BackgroundFile = backgroundFilePath,
                Direction = directionTypeDropdown.options[directionTypeDropdown.value].text,
                Speed = SafeParse(speedInput.text, 10f),
                NumberOfObjects = SafeParseInt(numberOfObjectsInput.text, 100),

                Color = ValidateHexColor(objectColorInput.text, "000000"),

                ObjectSize = new Vector3(
                SafeParse(objectSizeXInput.text, 25f),
                SafeParse(objectSizeYInput.text, 25f),
                SafeParse(objectSizeZInput.text, 100f)
                )
            },
            TargetSettings = new TargetSettings
            {
                TimeBeforeStart = (int)SafeParse(timeBeforeStart.text, 3f),
                TargetSize = SafeParse(targetSize.text, 5f)
            },
            ColorSettings = new ColorSettings
            {
                LeftHandColor = ValidateHexColor(leftHandColor.text, "0000FF"),
                RightHandColor = ValidateHexColor(rightHandColor.text, "FF0000"),
                TargetColor = ValidateHexColor(targetColor.text, "C0C0C0")
            },
            TargetLocations = RoundTargetLocations(_tempTargetLocations)
        };
        SessionManager.Instance.SaveSettingsFile(trial, trial.ConfigurationName);
    }

    private static List<Vector3> RoundTargetLocations(IEnumerable<Vector3> targetLocations) {
        List<Vector3> roundedTargets = new List<Vector3>();
        if (targetLocations == null) {
            return roundedTargets;
        }

        foreach (Vector3 target in targetLocations) {
            roundedTargets.Add(new Vector3(
                (float)Math.Round(target.x, 5, MidpointRounding.AwayFromZero),
                (float)Math.Round(target.y, 5, MidpointRounding.AwayFromZero),
                (float)Math.Round(target.z, 5, MidpointRounding.AwayFromZero)));
        }

        return roundedTargets;
    }

    public void ResetInputs()
    {
        configurationNameInput.text = "";
        handVisibility.value = 0;
        targetVisibility.value = 0;
        handFlickerShow.text = "1";
        handFlickerHide.text = "1";
        targetFlickerShow.text = "1";
        targetFlickerHide.text = "1";

        offsetTypeDropdown.value = 0;
        offsetXInput.text = "0";
        offsetYInput.text = "0";
        offsetZInput.text = "0";
        targetRangeInput.text = "10";
        showHandInProximityToggle.isOn = false;

        movingObjectsToggle.isOn = false;
        directionTypeDropdown.value = 0;
        speedInput.text = "10";
        numberOfObjectsInput.text = "100";
        objectSizeXInput.text = "25";
        objectSizeYInput.text = "25";
        objectSizeZInput.text = "100";
        objectColorInput.text = "000000";

        timeBeforeStart.text = "3";
        targetSize.text = "5";

        leftHandColor.text = "0000FF";
        rightHandColor.text = "FF0000";
        targetColor.text = "C0C0C0";

        uploadedBackgroundFileNameText.text = "No file uploaded";
        uploadedFileNameText.text = "No file uploaded";
    }

    public void OnGoHomeClicked() {
        ResetInputs();
        SceneManager.LoadScene("HomeScreen");
    }

}
