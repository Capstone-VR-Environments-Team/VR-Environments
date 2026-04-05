using UnityEngine;

public class SphereContact : MonoBehaviour
{
    private SphereManager sphereManager;
    private Renderer sphereRenderer;
    public int targetId = -1;
    private bool hasBeenTriggered = false;

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
                sphereManager.OnSphereInteracted();
            } else
            {
                EventBus.OnPreviousSphereReentered?.Invoke(transform.position);
                sphereManager.ShowCurrentSphere();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GameController") && hasBeenTriggered)
        {
            EventBus.OnPreviousSphereLeft?.Invoke(transform.position);
            sphereManager.ApplyVisibilitySettings();
            sphereManager.HideAfterExit();
        }

    }
    public void ResetTrigger()
    {
        hasBeenTriggered = false;
    }
}
