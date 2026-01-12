using UnityEngine;

public class SphereContact : MonoBehaviour
{
    private bool hasBeenTriggered = false;
    private SphereCollectionManager sphereManager;

    void Start()
    {
        sphereManager = FindFirstObjectByType<SphereCollectionManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController") && !hasBeenTriggered)
        {
            hasBeenTriggered = true;
            Debug.Log("Sphere found!");
            GetComponent<Renderer>().material.color = Color.green;

            // --- NEW: Log the Hit ---
            // Pass the Sphere's position (the target location), not the hand's position
            LoggingManager.Instance.LogTargetHit(transform.position);

            sphereManager.OnSphereInteracted();
        }
    }
    
    public void ResetTrigger()
    {
        hasBeenTriggered = false;
        // Reset color if needed here, though SphereManager usually handles disabling
        GetComponent<Renderer>().material.color = Color.white; // Or original color
        
        // Ensure the proximity sensor (if child) is also reset
        ProximityContact prox = GetComponentInChildren<ProximityContact>();
        if (prox != null) prox.ResetTrigger();
    }
}