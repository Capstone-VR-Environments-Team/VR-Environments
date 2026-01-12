using UnityEngine;

public class SphereContact : MonoBehaviour
{
    //private bool hasBeenTriggered = false;
    private SphereManager sphereManager;

    void Start()
    {
        sphereManager = FindFirstObjectByType<SphereManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            // hasBeenTriggered = true;
            Debug.Log("Sphere found!");
            GetComponent<Renderer>().material.color = Color.green;

            LoggingManager.Instance.LogTargetHit(transform.position);

            sphereManager.OnSphereInteracted();
        }
    }
}
