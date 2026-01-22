using UnityEngine;
using TMPro; // if using TextMeshPro
using UnityEngine.UI;

public class ExperimentController : MonoBehaviour
{
    public HandDataRecorder recorder; // optional: see section below
    public TMP_Text statusText; // use Text if not using TMP
    public SphereManager sphereManager;
    public GameObject headset;

    public void StartExperiment()
    {
        Vector3 headsetPosition = headset.transform.position;
        if (recorder != null) {
            sphereManager.BeginTrial(headsetPosition);
            recorder.StartRecording(headsetPosition);
            LoggingManager.Instance.StartRecording("PLACEHOLDER_NAME", headsetPosition);
            UpdateStatus("Recording...");
        }
    }

    public void StopExperiment()
    {
        if (recorder != null) {
            recorder.StopRecording();
            LoggingManager.Instance.StopRecording();
            sphereManager.ResetTrial();
            UpdateStatus("Stopped. Data Saved.");
        }
    }

    void UpdateStatus(string s)
    {
        if (statusText != null) statusText.text = $"Status: {s}";
    }
}

