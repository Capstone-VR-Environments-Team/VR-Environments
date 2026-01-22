using UnityEngine;

public class SphereContact : MonoBehaviour
{
    private SphereManager sphereManager;
    public int targetId = -1;
    private bool hasBeenTriggered = false;

    void Start()
    {
        sphereManager = FindFirstObjectByType<SphereManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController") && !hasBeenTriggered)
        {
            hasBeenTriggered = true;
            Debug.Log($"Sphere {targetId} found!");

            LoggingManager.Instance.LogTargetHit(transform.position, targetId);

            sphereManager.OnSphereInteracted();
        }
    }

    public void ResetTrigger()
    {
        hasBeenTriggered = false;
    }
}
