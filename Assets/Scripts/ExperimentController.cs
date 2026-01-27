using UnityEngine;
using TMPro; // if using TextMeshPro
using UnityEngine.UI;

public class ExperimentController : MonoBehaviour
{
    public HandDataRecorder recorder; // optional: see section below
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
            LoggingManager.Instance.StopRecording();
            sphereManager.ResetTrial();
            UpdateStatus("Stopped. Data Saved.");
            trialViewManager.StopTimer();
        }
    }

    public void StartExperiment() {
        LoggingManager.Instance.StartRecording("PLACEHOLDER_NAME", headsetPosition);
        trialViewManager.StartTimer();
        UpdateStatus("Recording...");
    }

    void UpdateStatus(string s)
    {
        if (statusText != null) statusText.text = $"Status: {s}";
    }
}

