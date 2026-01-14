using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SphereManager : MonoBehaviour
{
    [Header("Sphere Settings")]
    public GameObject spherePrefab;
    public static int totalSpheres;

    public GameObject[] spheres;

    [Header("Hand References")]
    public GameObject leftHand;
    public GameObject rightHand;

    [Header("Spawn Locations")]
    public GameObject headset;
    
    private int spheresCollected = 0;
    private GameObject currentSphere;
    private bool showHands;
    private bool showTargets;
    private float targetVisibleTime;
    private float handVisibleTime;
    private float targetProximity;
    private Vector3 offsetValues;

    public void BeginTrial()
    {
        List<Vector3> sphereVectors = LoadTrialSettings.Instance.GetLoadedTargets();
        showHands = LoadTrialSettings.Instance.GetShowHands();
        showTargets = LoadTrialSettings.Instance.GetShowTargets();
        targetVisibleTime = LoadTrialSettings.Instance.GetTargetVisibleTime();
        handVisibleTime = LoadTrialSettings.Instance.GetHandVisibleTime();
        targetProximity = LoadTrialSettings.Instance.GetTargetProximity();
        offsetValues = LoadTrialSettings.Instance.GetOffsetValues();
        totalSpheres = sphereVectors.Count;
        spheres = new GameObject[totalSpheres];
        Vector3 headsetPosition = headset.transform.position;
        for (int i = 0; i < totalSpheres; i++) {
            spheres[i] = Instantiate(spherePrefab,
                                    sphereVectors[i] + headsetPosition,
                                    Quaternion.identity);
            Collider c = spheres[i].GetComponent<Collider>();
            if (c) {
                c.isTrigger = true;
            }
            spheres[i].SetActive(false);
        }
        leftHand.transform.localPosition = offsetValues;
        rightHand.transform.localPosition = offsetValues;
        ApplyVisibilitySettings();
        SpawnNextSphere();
    }
    
    public void OnSphereInteracted()
    {
        spheresCollected++;
        Debug.Log($"Sphere collected! {spheresCollected}/{totalSpheres}");
        
        if (currentSphere)
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
            this.spheres[spheresCollected].GetComponent<MeshRenderer>().enabled = showTargets;
            currentSphere = this.spheres[spheresCollected];
            
            if (!showTargets && targetVisibleTime > 0)
            {
                StartCoroutine(HideSphereAfterDelay(targetVisibleTime));
            }
        }
    }

    void ApplyVisibilitySettings()
    {
        if (leftHand != null)
        {
            leftHand.GetComponent<MeshRenderer>().enabled = showHands;
        }
        if (rightHand != null)
        {
            rightHand.GetComponent<MeshRenderer>().enabled = showHands;
        }
        if (!showHands && handVisibleTime > 0)
        {
            StartCoroutine(HideHandsAfterDelay(handVisibleTime));
        }
    }

    IEnumerator HideSphereAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (currentSphere != null)
        {
            currentSphere.GetComponent<MeshRenderer>().enabled = false;
            Debug.Log("Target hidden after visibility time expired");
        }
    }

    IEnumerator HideHandsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (leftHand != null)
        {
            leftHand.GetComponent<MeshRenderer>().enabled = false;
        }
        if (rightHand != null)
        {
            rightHand.GetComponent<MeshRenderer>().enabled = false;
        }

        Debug.Log("Hands hidden after visibility time expired");
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