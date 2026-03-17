using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class SaveTrialSettings : MonoBehaviour
{
    [Header("Configuration Name")]
    public TMP_InputField configurationNameInput;
    public TMP_Text fileUploaded;

    [Header("Visibility Settings")]

    public Toggle showTargetsToggle;
    public TMP_InputField targetVisibleTimeInput;
    public Toggle showHandsToggle;
    public TMP_InputField handVisibleTimeInput;

    [Header("Offset Settings")]
    public TMP_Dropdown offsetTypeDropdown;
    public TMP_InputField offsetXInput;
    public TMP_InputField offsetYInput;
    public TMP_InputField offsetZInput;
    public TMP_InputField targetRangeInput;

    [Header("Background Settings")]
    public TMP_Dropdown backgroundTypeDropdown;
    public TMP_Dropdown directionTypeDropdown;
    public TMP_InputField speedInput;
    public TMP_InputField numberOfObjectsInput;
    public Material skyboxMaterial;

    private List<Vector3> _tempTargetLocations = new List<Vector3>();
    private VideoPlayer videoPlayer;

    public void OnUploadLocationsClicked()
    {
        string filePath = FileSelector.getFilePath(SessionManager.BaseDataPath, new string[] { "json", "csv" });
        var (importedData, fileName) = FileManager.LoadFromFile<TargetImportData>(filePath);
        if (importedData != null && importedData.targets != null)
        {
            Debug.Log("filename: " + fileName);
            fileUploaded.SetText(fileName);
            _tempTargetLocations = importedData.targets;
        }
        else
        {
            fileUploaded.SetText("File Upload Failed");
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
        if (!File.Exists(filePath))
        {
            Debug.LogError("Image file not found at: " + filePath);
            return;
        }

        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D newTexture = new Texture2D(2, 2);

        if (newTexture.LoadImage(fileData))
        {
            skyboxMaterial.SetTexture("_MainTex", newTexture);
            RenderSettings.skybox = skyboxMaterial;
        }
    }

    public void onVideoUpload()
    {
        string filePath = FileSelector.getFilePath(SessionManager.BaseDataPath, new string[] { "mp4" });
        if (!File.Exists(filePath))
        {
            Debug.LogError("Video file not found at: " + filePath);
            return;
        }

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = filePath;
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        videoPlayer.isLooping = true;
        videoPlayer.Play();

        RenderSettings.skybox = skyboxMaterial;
    }


    public void OnSaveButtonClicked()
    {
        TrialSettingsData trial = new TrialSettingsData
        {
            ConfigurationName = configurationNameInput.text,
            VisibilitySettings = new VisibilitySettings
            {
                ShowTargets = showTargetsToggle.isOn,
                TargetVisibleTime = SafeParse(targetVisibleTimeInput.text, 0),
                ShowHands = showHandsToggle.isOn,
                HandVisibleTime = SafeParse(handVisibleTimeInput.text, 0)
            },
            OffsetSettings = new OffsetSettings
            {
                OffsetType = offsetTypeDropdown.options[offsetTypeDropdown.value].text,
                OffsetValues = new Vector3(
                    SafeParse(offsetXInput.text, 0),
                    SafeParse(offsetYInput.text, 0),
                    SafeParse(offsetZInput.text, 0)
                ),
                TargetProximity = SafeParse(targetRangeInput.text, 0)
            },
            BackgroundSettings = new BackgroundSettings
            {
                BackgroundType = backgroundTypeDropdown.options[backgroundTypeDropdown.value].text,
                ImageBackground = null,
                VideoBackground = null,
                Direction = directionTypeDropdown.options[directionTypeDropdown.value].text,
                Speed = SafeParse(speedInput.text, 0),
                NumberOfObjects = (int)SafeParse(numberOfObjectsInput.text, 0)
            },
            TargetLocations = _tempTargetLocations
        };
        SessionManager.Instance.SaveSettingsFile(trial, trial.ConfigurationName);
    }
}

