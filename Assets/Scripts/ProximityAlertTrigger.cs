using System;
using UnityEngine;

public class ProximityAlertTrigger : MonoBehaviour
{
    private bool isHandInProximity = false;
    public Vector3 location;
    public void Initialize(Vector3 loc)
    {
        location = loc;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController") && !isHandInProximity)
        {
            isHandInProximity = true;
            EventBus.OnProximityHit.Invoke(location);
        }
    }

    public void ResetTrigger()
    {
        isHandInProximity = false;
    }
}