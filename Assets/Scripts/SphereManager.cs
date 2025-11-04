using UnityEngine;
using UnityEngine.Events;

public class SphereCollectionManager : MonoBehaviour
{
    [Header("Sphere Settings")]
    public GameObject spherePrefab;
    public int totalSpheres = 5;

    public GameObject[] spheres;

    [Header("Spawn Locations")]
    
    public Vector3 spawnAreaMin = new Vector3(0, 0, 0);
    public Vector3 spawnAreaMax = new Vector3(5, 5, 5);
    
    private int spheresCollected = 0;
    private GameObject currentSphere;
    
    void Start()
    {
        for (int i = 0; i < totalSpheres; i++) {
            spheres[i] = Instantiate(spherePrefab,
                                    new Vector3(
                                        Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                                        Random.Range(spawnAreaMin.y, spawnAreaMax.y),
                                        Random.Range(spawnAreaMin.z, spawnAreaMax.z)),
                                        Quaternion.identity);
        }
        SpawnNextSphere();
    }
    
    public void OnSphereInteracted()
    {
        spheresCollected++;
        Debug.Log($"Sphere collected! {spheresCollected}/{totalSpheres}");
        
        if (currentSphere != null)
        {
            currentSphere.SetActive(false);
        }
        
        if (spheresCollected >= totalSpheres)
        {
            EndTrial();
        }
        else
        {
            SpawnNextSphere();
        }
    }
    
    void SpawnNextSphere()
    {
        spheres[spheresCollected].SetActive(true);
        currentSphere = spheres[spheresCollected];
    }
    
    void EndTrial()
    {
        Debug.Log("Trial complete! All spheres collected.");
        // - Save data
        // - Load next trial
        ResetTrial();
    }
    
    public void ResetTrial()
    {
        spheresCollected = 0;
        SpawnNextSphere();
    }
}