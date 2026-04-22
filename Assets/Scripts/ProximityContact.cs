using UnityEngine;

public class ProximityContact : MonoBehaviour
{
    private bool hasBeenTriggered = false;
    public int targetId;
    public void Initialize(int id)
    {
        targetId = id;
    }

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
                EventBus.OnProximityHit?.Invoke(transform.parent.position, targetId);
            }
        }
    }

    public void ResetTrigger()
    {
        hasBeenTriggered = false;
    }
}