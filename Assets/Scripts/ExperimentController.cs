using System;
using UnityEngine;
using TMPro; // if using TextMeshPro

public class ExperimentController : MonoBehaviour
{
    public GameObject headset;

    private Vector3 headsetPosition;

    [SerializeField] LiveTrialViewManager liveTrialViewManager;

    private void Start()
    {
        liveTrialViewManager.AddBeginTrialOnClick(PrimeExperiment);
        liveTrialViewManager.AddEndTrialOnClick(StopExperiment);
    }

    public void PrimeExperiment()
    {
        headsetPosition = headset.transform.position;
        EventBus.PrimeExperiment?.Invoke(headsetPosition);
        Debug.Log("Prepping Trial");
    }

    public void StopExperiment()
    {
        EventBus.StopExperiment?.Invoke();
        Debug.Log("Stopped. Data Saved.");
    }

    public void StartExperiment() {
        EventBus.StartExperiment?.Invoke(headsetPosition);
        Debug.Log("Recording...");
    }
}

