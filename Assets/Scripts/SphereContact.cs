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
        if (other.CompareTag("GameController") && !hasBeenTriggered)
        {
            hasBeenTriggered = true;
            Debug.Log($"Sphere {targetId} found!");

            sphereRenderer.enabled = true;
            sphereRenderer.material.color = Color.green;
            sphereManager.OnSphereInteracted();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GameController") && hasBeenTriggered)
        {
            sphereRenderer.enabled = false;
            sphereManager.HideAfterExit();
        }

    }
    public void ResetTrigger()
    {
        hasBeenTriggered = false;
    }
}
