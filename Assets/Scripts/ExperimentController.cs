using UnityEngine;
using TMPro; // if using TextMeshPro

public class ExperimentController : MonoBehaviour
{
    public TMP_Text statusText; // use Text if not using TMP
    public GameObject headset;

    private Vector3 headsetPosition;

    public void PrimeExperiment()
    {
        headsetPosition = headset.transform.position;
        EventBus.PrimeExperiment?.Invoke(headsetPosition);
        UpdateStatus("Prepping Trial");
    }

    public void StopExperiment()
    {
        EventBus.StopExperiment?.Invoke();
        UpdateStatus("Stopped. Data Saved.");
    }

    public void StartExperiment() {
        EventBus.StartExperiment?.Invoke(headsetPosition);
        UpdateStatus("Recording...");
    }

    void UpdateStatus(string s)
    {
        if (statusText != null) statusText.text = $"Status: {s}";
    }
}

