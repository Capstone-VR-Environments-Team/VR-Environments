using System;
using UnityEngine;

public class ProximityAlertTrigger : MonoBehaviour
{
    private bool isHandInProximity = false;
    public Vector3 location;
    private int _targetId = -1;

    public void Initialize(int targetId, Vector3 loc)
    {
        _targetId = targetId;
        location = loc;
        targetId = id;
    }

    private void OnEnable()
    {
        // Subscribe to target interaction events to know when we should reset
        EventBus.OnTargetHit += HandleTargetInteraction;
        EventBus.OnTargetReEntry += HandleTargetInteraction;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks when the object is destroyed/disabled
        EventBus.OnTargetHit -= HandleTargetInteraction;
        EventBus.OnTargetReEntry -= HandleTargetInteraction;
    }

    private void HandleTargetInteraction(Vector3 targetLocation, int targetId)
    {
        // If a target was hit/re-entered, and its location is different from THIS proximity trigger's location,
        // it means the user went to a different target (e.g., Target A). 
        // We reset this trigger (Target B) so it can fire again.
        // We use a small distance tolerance (0.001f) to account for microscopic float precision differences.
        if (Vector3.Distance(targetLocation, location) > 0.001f)
        {
            isHandInProximity = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController") && !isHandInProximity)
        {
            isHandInProximity = true;
            // Using the null-conditional operator (?.) is a slightly safer way to invoke events
            EventBus.OnProximityHit?.Invoke(location, _targetId); 
        }
    }

    public void ResetTrigger()
    {
        isHandInProximity = false;
    }
}