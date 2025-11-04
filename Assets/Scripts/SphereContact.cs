using UnityEngine;

public class SphereContact : MonoBehaviour
{
    private bool hasBeenTriggered = false;
    private SphereManager sphereManager;

    void Start()
    {
        sphereManager = FindObjectOfType<SphereManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasBeenTriggered && other.CompareTag("GameController"))
        {
            hasBeenTriggered = true;
            Debug.Log("Sphere found!");
            GetComponent<Renderer>().material.color = Color.green;
            sphereManager.OnSphereInteracted();
        }
    }
    
    public void ResetTrigger()
    {
        hasBeenTriggered = false;
    }
}
