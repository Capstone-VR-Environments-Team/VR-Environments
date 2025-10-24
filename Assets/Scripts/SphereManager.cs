using UnityEngine;
using UnityEngine.Events;

public class SphereCollectionManager : MonoBehaviour
{
    [Header("Sphere Settings")]
    public GameObject spherePrefab; // Drag your sphere prefab here
    public int totalSpheres = 5;

    public GameObject[] spheres; // Array to hold sphere instances
    
    [Header("Spawn Locations")]
    public Transform[] spawnPoints; // Optional: predefined spawn locations
    public Vector3 spawnAreaMin = new Vector3(-2f, 1f, 2f);
    public Vector3 spawnAreaMax = new Vector3(2f, 3f, 5f);
    
    [Header("Events")]
    public UnityEvent onSphereCollected;
    public UnityEvent onAllSpheresCollected;
    
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
        
        // Invoke the collection event
        onSphereCollected.Invoke();
        
        // Destroy current sphere
        if (currentSphere != null)
        {
            Destroy(currentSphere);
        }
        
        // Check if all spheres collected
        if (spheresCollected >= totalSpheres)
        {
            EndTrial();
        }
        else
        {
            // Spawn next sphere
            SpawnNextSphere();
        }
    }
    
    void SpawnNextSphere()
    {
        Vector3 spawnPosition;
        
        // Use predefined spawn points if available
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            spawnPosition = spawnPoints[randomIndex].position;
        }
        else
        {
            // Random position within defined area
            spawnPosition = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y),
                Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            );
        }
    }
    
    void EndTrial()
    {
        Debug.Log("Trial complete! All spheres collected.");
        onAllSpheresCollected.Invoke();
        
        // - Save data
        // - Load next scene
        // - Show results UI
        // - Restart trial
    }
    
    public void ResetTrial()
    {
        spheresCollected = 0;
        SpawnNextSphere();
    }
}