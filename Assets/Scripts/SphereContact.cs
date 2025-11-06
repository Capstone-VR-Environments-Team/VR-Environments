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
        if (other.CompareTag("GameController"))
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
