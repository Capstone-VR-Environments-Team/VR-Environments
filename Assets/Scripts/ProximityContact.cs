using UnityEngine;

public class ProximityContact : MonoBehaviour
{
    private bool hasBeenTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController") && !hasBeenTriggered)
        {
            hasBeenTriggered = true;
            
            // Log the Proximity Hit
            // We use transform.parent.position because this script is on a child object
            // centered on the target.
            if (transform.parent != null)
            {
                EventBus.OnProximityHit?.Invoke(transform.parent.position);
            }
        }
    }

    public void ResetTrigger()
    {
        hasBeenTriggered = false;
    }
}