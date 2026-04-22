using System;
using UnityEngine;

public class ProximityAlertTrigger : MonoBehaviour
{
    private bool isHandInProximity = false;
    public Vector3 location;
    public int targetId;
    public void Initialize(Vector3 loc, int id)
    {
        location = loc;
        targetId = id;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController") && !isHandInProximity)
        {
            isHandInProximity = true;
            EventBus.OnProximityHit.Invoke(location, targetId);
        }
    }

    public void ResetTrigger()
    {
        isHandInProximity = false;
    }
}