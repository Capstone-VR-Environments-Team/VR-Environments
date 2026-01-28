using System;
using System.Diagnostics;
using UnityEngine;

public class ProximityAlertTrigger : MonoBehaviour
{
    private int targetId;
    private bool isHandInProximity = false;

    public event Action<int> OnProximityEnter;
    public void Initialize(int id)
    {
        targetId = id;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController") && !isHandInProximity)
        {
            isHandInProximity = true;
            OnProximityEnter.Invoke(targetId);
        }
    }
    
    public void ResetTrigger()
    {
        isHandInProximity = false;
    }
}