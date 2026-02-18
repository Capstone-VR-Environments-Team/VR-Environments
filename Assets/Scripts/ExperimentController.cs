using UnityEngine;
using TMPro; // if using TextMeshPro
using UnityEngine.UI;

public class ExperimentController : MonoBehaviour
{
    public HandDataRecorder recorder; // optional: see section below
    public EyeDataRecorder eyeRecorder;
    public TMP_Text statusText; // use Text if not using TMP
    public SphereManager sphereManager;
    public GameObject headset;
    public LiveTrialViewManager trialViewManager;

    private Vector3 headsetPosition;

    public void PrimeExperiment()
    {
        headsetPosition = headset.transform.position;
        if (recorder != null) {
            sphereManager.BeginTrial(headsetPosition);
            UpdateStatus("Prepping Trial");
        }
    }

    public void StopExperiment()
    {
        if (recorder != null) {
            trialViewManager.StopTimer();
            LoggingManager.Instance.StopRecording();
            sphereManager.ResetTrial();
            recorder.StopRecording();
            eyeRecorder.StopRecording();
            UpdateStatus("Stopped. Data Saved.");
            FileManager.Instance.ResetFileManager();
        }
    }

    public void StartExperiment() {
        LoggingManager.Instance.StartRecording("PLACEHOLDER_NAME", headsetPosition);
        trialViewManager.StartTimer();
        recorder.StartRecording(headsetPosition);
        eyeRecorder.StartRecording(headsetPosition);
        UpdateStatus("Recording...");
    }

    void UpdateStatus(string s)
    {
        if (statusText != null) statusText.text = $"Status: {s}";
    }
}

