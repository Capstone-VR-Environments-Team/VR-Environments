using UnityEngine;
using UnityEngine.Events;

public class SphereCollectionManager : MonoBehaviour
{
    [Header("Sphere Settings")]
    public GameObject spherePrefab;
    public static int totalSpheres = 3;

    public GameObject[] spheres;

    [Header("Spawn Locations")]
    
    public Vector3 spawnAreaMin = new Vector3(0, 0, 0);
    public Vector3 spawnAreaMax = new Vector3(0, 0, 0);
    
    private int spheresCollected = 0;
    private GameObject currentSphere;
    
    void Start()
    {
        spheres = new GameObject[totalSpheres];
        for (int i = 0; i < spheres.Length; i++) {
            this.spheres[i] = Instantiate(spherePrefab,
                                    new Vector3(0, i, -7.5f),
                                        Quaternion.identity);
            Collider c = spheres[i].GetComponent<Collider>();
            if (c) {
                c.isTrigger = true;
            }
            this.spheres[i].SetActive(false);
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
        if (spheresCollected < this.spheres.Length && this.spheres[spheresCollected] != null) {
            this.spheres[spheresCollected].SetActive(true);
            currentSphere = this.spheres[spheresCollected];
        }
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
        // SpawnNextSphere();
    }
}