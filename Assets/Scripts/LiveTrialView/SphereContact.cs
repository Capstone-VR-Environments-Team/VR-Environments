using UnityEngine;

public class SphereContact : MonoBehaviour
{
    private SphereManager sphereManager;
    private Renderer sphereRenderer;
    private int targetId = -1;
    private bool hasBeenTriggered = false;
    private Vector3 location;

    public void Initialize(int id, Vector3 loc)
    {
        targetId = id;
        location = loc;
    }

    void Start()
    {
        sphereManager = FindFirstObjectByType<SphereManager>();
        sphereRenderer = GetComponent<Renderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            if (targetId == 1 && !sphereManager.started)
            {
                Debug.Log("Holding start sphere. Timer started.");
                sphereRenderer.enabled = true;
                hasBeenTriggered = true;
                sphereRenderer.material.color = Color.green; 
                sphereManager.OnStartSphereEnter();
                return; 
            }
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
                EventBus.OnTargetReEntry?.Invoke(location, targetId);
                sphereManager.ShowCurrentSphere();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GameController") && hasBeenTriggered)
        {
            if (targetId == 1 && !sphereManager.started)
            {
                Debug.Log("Exited start sphere too early. Timer canceled.");
                hasBeenTriggered = false;
                sphereRenderer.material.color = sphereManager.GetTargetColor(); 
                sphereManager.OnStartSphereExit();
                return;
            }
            if (hasBeenTriggered)
            {
                EventBus.OnTargetExit?.Invoke(location, targetId);
                sphereManager.ApplyVisibilitySettings();
                sphereManager.HideAfterExit();
            }
        }

    }
    public void ResetTrigger()
    {
        hasBeenTriggered = false;
    }
}
