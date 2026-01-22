using UnityEngine;

public class SphereContact : MonoBehaviour
{
    private SphereManager sphereManager;

    void Start()
    {
        sphereManager = FindFirstObjectByType<SphereManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            Debug.Log("Sphere found!");

            LoggingManager.Instance.LogTargetHit(transform.position);

            sphereManager.OnSphereInteracted();
        }
    }
}
