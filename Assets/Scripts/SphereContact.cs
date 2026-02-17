using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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
            if (!hasBeenTriggered)
            {
                hasBeenTriggered = true;
                Debug.Log($"Sphere {targetId} found!");
                sphereRenderer.enabled = true;
                sphereRenderer.material.color = Color.green;
                sphereManager.OnSphereInteracted();
            } else
            {
                sphereManager.ShowCurrentSphere();
            }
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("GameController") && hasBeenTriggered)
    //    {
    //        sphereRenderer.enabled = false;
    //        sphereManager.HideAfterExit();
    //    }

    //}
    public void ResetTrigger()
    {
        hasBeenTriggered = false;
    }
}
