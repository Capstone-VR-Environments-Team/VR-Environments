using UnityEngine;

public class SphereContact : MonoBehaviour
{
    private SphereManager sphereManager;
    private Renderer sphereRenderer;
    private int targetId = -1;
    private bool hasBeenTriggered = false;
    private Vector3 location;

    void Initialize(int id, Vector3 loc)
    {
        targetId = id;
        location = loc;

    void Start()
    {
        sphereManager = FindFirstObjectByType<SphereManager>();
        sphereRenderer = GetComponent<Renderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            if (!hasBeenTriggered)
            {
                hasBeenTriggered = true;
                Debug.Log($"Sphere {targetId} found!");
                sphereRenderer.enabled = true;
                sphereRenderer.material.color = Color.green;
                EventBus.OnTargetHit?.Invoke(location, targetId);
                sphereManager.OnSphereInteracted();
            } else
            {
                EventBus.OnTargetReEntry?.Invoke(targetId);
                sphereManager.ShowCurrentSphere();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GameController") && hasBeenTriggered)
        {
            EventBus.OnTargetExit?.Invoke(targetId);
            sphereManager.ApplyVisibilitySettings();
            sphereManager.HideAfterExit();
        }

    }
    public void ResetTrigger()
    {
        hasBeenTriggered = false;
    }
}
