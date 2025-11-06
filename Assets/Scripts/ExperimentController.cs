using UnityEngine;
using TMPro; // if using TextMeshPro
using UnityEngine.UI;

public class ExperimentController : MonoBehaviour
{
    public HandDataRecorder recorder; // optional: see section below
    public TMP_Text statusText; // use Text if not using TMP

    public void StartExperiment()
    {
        if (recorder != null) {
            recorder.StartRecording();
            LoggingManager.Instance.StartRecording("PLACEHOLDER_NAME");
            UpdateStatus("Recording...");
        }
    }

    public void StopExperiment()
    {
        if (recorder != null) {
            recorder.StopRecording();
            LoggingManager.Instance.StopRecording();
            UpdateStatus("Stopped. Data Saved.");
        }
    }

    void UpdateStatus(string s)
    {
        if (statusText != null) statusText.text = $"Status: {s}";
    }
}

