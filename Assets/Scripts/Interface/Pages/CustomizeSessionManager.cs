using System.Collections.Generic;
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
    [SerializeField] private Button uploadBackgroundFile; // TODO: needs to be able to handle both image and video
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
    private string imageBackgroundFilePath = "";
    private string videoBackgroundFilePath = "";

    private float _diffY;

    private void Start()
    {
        _diffY = handFlickerShow.gameObject.transform.localPosition.y -
                 targetFlickerShow.gameObject.transform.localPosition.y;
        ;
        handVisibility.onValueChanged.AddListener(delegate { UpdateFlickerInputFields(); });
        targetVisibility.onValueChanged.AddListener(delegate { UpdateFlickerInputFields(); });
        movingObjectsToggle.onValueChanged.AddListener(delegate { UpdateBackgroundInputFields(); });
        
        UpdateFlickerInputFields();
        UpdateBackgroundInputFields();
    }

    private void UpdateFlickerInputFields()
    {
        string handFlickerState = handVisibility.options[handVisibility.value].text;
        string targetFlickerState = targetVisibility.options[targetVisibility.value].text;

        if (handFlickerState == "Flicker" && targetFlickerState == "Flicker")
        {
            targetFlickerShow.gameObject.transform.localPosition =
                handFlickerShow.gameObject.transform.localPosition - new Vector3(0, _diffY, 0);
            targetFlickerHide.gameObject.transform.localPosition =
                handFlickerHide.gameObject.transform.localPosition - new Vector3(0, _diffY, 0);
            UpdateFlickerInputFields(true, true);
        }
        else if (handFlickerState == "Flicker")
        {
            UpdateFlickerInputFields(true, false);
        }
        else if (targetFlickerState == "Flicker")
        {
            targetFlickerShow.gameObject.transform.localPosition =
                handFlickerShow.gameObject.transform.localPosition;
            targetFlickerHide.gameObject.transform.localPosition =
                handFlickerHide.gameObject.transform.localPosition;
            UpdateFlickerInputFields(false, true);
        }
        else
        {
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

    public void OnUploadLocationsClicked()
    {
        string filePath = FileSelector.getFilePath(SessionManager.BaseDataPath, new string[] { "json", "csv" });
        var (importedData, fileName) = FileManager.LoadFromFile<TargetImportData>(filePath);
        if (importedData != null && importedData.targets != null)
        {
            Debug.Log("filename: " + fileName);
            uploadedFileNameText.SetText(fileName);
            _tempTargetLocations = importedData.targets;
        }
        else
        {
            uploadedFileNameText.SetText("File Upload Failed");
            Debug.LogError("Failed to load target locations from file.");
        }
    }

    public float SafeParse(string input, float defaultValue)
    {
        if (float.TryParse(input, out float result))
        {
            return result;
        }
        return defaultValue;
    }

    public void onImageUpload()
    {
        string filePath = FileSelector.getFilePath(SessionManager.BaseDataPath, new string[] { "jpg", "png" });
        if (!string.IsNullOrEmpty(filePath))
        {
            imageBackgroundFilePath = filePath;
        }
    }

    public void onVideoUpload()
    {
        string filePath = FileSelector.getFilePath(SessionManager.BaseDataPath, new string[] { "mp4" });
        if (!string.IsNullOrEmpty(filePath))
        {
            videoBackgroundFilePath = filePath;
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
                HandsFlickerOnDuration = SafeParse(handFlickerShow.text, 1),
                HandsFlickerOffDuration = SafeParse(handFlickerHide.text, 1),
                TargetFlickerOnDuration = SafeParse(handFlickerShow.text, 1),
                TargetFlickerOffDuration = SafeParse(targetFlickerHide.text, 1),
            },
            OffsetSettings = new OffsetSettings
            {
                OffsetType = offsetTypeDropdown.options[offsetTypeDropdown.value].text,
                OffsetValues = new Vector3(
                    SafeParse(offsetXInput.text, 0),
                    SafeParse(offsetYInput.text, 0),
                    SafeParse(offsetZInput.text, 0)
                ),
                TargetProximity = SafeParse(targetRangeInput.text, 10),
                ShowHandsInProximity = showHandInProximityToggle.isOn
            },
            BackgroundSettings = new BackgroundSettings
            {
                ImageBackground = imageBackgroundFilePath,
                VideoBackground = videoBackgroundFilePath,
                Direction = directionTypeDropdown.options[directionTypeDropdown.value].text,
                Speed = SafeParse(speedInput.text, 10),
                NumberOfObjects = (int)SafeParse(numberOfObjectsInput.text, 100),
                ObjectSize = new Vector3(
                    SafeParse(objectSizeXInput.text, 25),
                    SafeParse(objectSizeYInput.text, 25),
                    SafeParse(objectSizeZInput.text, 100)
                    )
            },
            TargetSettings = new TargetSettings
            {
                TimeBeforeStart = (int)SafeParse(timeBeforeStart.text, 3),
                TargetSize = SafeParse(targetSize.text, 5)
            },
            ColorSettings = new ColorSettings
            {
                BackgroundObjectColor = string.IsNullOrEmpty(objectColorInput.text) ? "000000" : objectColorInput.text,
                LeftHandColor = string.IsNullOrEmpty(leftHandColor.text) ? "0000FF" : leftHandColor.text,
                RightHandColor = string.IsNullOrEmpty(rightHandColor.text) ? "FF0000" : rightHandColor.text,
                TargetColor = string.IsNullOrEmpty(targetColor.text) ? "C0C0C0" : targetColor.text
            },
            TargetLocations = _tempTargetLocations
        };
        SessionManager.Instance.SaveSettingsFile(trial, trial.ConfigurationName);
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
