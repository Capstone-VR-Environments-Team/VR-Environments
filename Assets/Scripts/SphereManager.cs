using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    [Header("Events")]
    public HandDataRecorder recorder;
    public ExperimentController experimentController;

    private int spheresCollected = 0;
    private GameObject currentSphere;
    private GameObject prevSphere;
    private bool showHands;
    private bool showTargets;
    private float handVisibleTime;
    private float targetProximity;
    private bool started = false;
    private Vector3 offsetValues;

    public void BeginTrial(Vector3 headsetPosition)
    {
        List<Vector3> sphereVectors = LoadTrialSettings.Instance.GetLoadedTargets();
        showHands = LoadTrialSettings.Instance.GetShowHands();
        showTargets = LoadTrialSettings.Instance.GetShowTargets();
        handVisibleTime = LoadTrialSettings.Instance.GetHandVisibleTime();
        targetProximity = LoadTrialSettings.Instance.GetTargetProximity();
        offsetValues = LoadTrialSettings.Instance.GetOffsetValues();
        totalSpheres = sphereVectors.Count;
        spheres = new GameObject[totalSpheres];
        for (int i = 0; i < totalSpheres; i++) {
            spheres[i] = Instantiate(spherePrefab,
                                    sphereVectors[i] + headsetPosition,
                                    Quaternion.identity);

            SphereContact sc = spheres[i].GetComponent<SphereContact>();
            if (sc != null)
            {
                sc.targetId = i + 1;
            }

            SphereCollider c = spheres[i].GetComponent<SphereCollider>();
            if (c)
            {
                c.isTrigger = true;
            }
            
            if (i != 0) 
            {
                GameObject proximityObject = new();
                proximityObject.transform.SetParent(spheres[i].transform);
                proximityObject.transform.localPosition = new Vector3();
                SphereCollider triggerCollider = proximityObject.AddComponent<SphereCollider>();
                triggerCollider.isTrigger = true;
                triggerCollider.radius = c.radius + targetProximity;
                triggerCollider.center = c.center;
                ProximityAlertTrigger proximityAlert = proximityObject.AddComponent<ProximityAlertTrigger>();
                proximityAlert.Initialize(i + 1);
                proximityAlert.OnProximityEnter += OnProximityEnter;
            }
            spheres[i].SetActive(false);
        }
        
        SpawnNextSphere();
    }

    private void OnProximityEnter(int targetId)
    {
        LoggingManager.Instance.LogProximityHit(currentSphere.transform.position);
        Debug.Log("Proximity Hit");
    }

    public void HideAfterExit()
    {
        if (currentSphere && !showTargets)
        {
            currentSphere.GetComponent<Renderer>().enabled = false;
        }
        if (prevSphere)
        {
            prevSphere.GetComponent<Renderer>().enabled = false;
        }
    }

    public void ShowCurrentSphere()
    {
        if (currentSphere)
        {
            currentSphere.GetComponent<Renderer>().enabled = true;
        }
        if (leftHand)
        {
            leftHand.GetComponent<MeshRenderer>().enabled = true;
        }
        if (rightHand)
        {
            rightHand.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void OnSphereInteracted()
    {
        if (spheresCollected == 0) {
            if (!started) StartTrial();
            return;
        }
        HandleSphereInteract();
    }


    void SpawnNextSphere()
    {
        if (spheresCollected < this.spheres.Length && this.spheres[spheresCollected] != null) {
            if (currentSphere) {
                prevSphere = currentSphere;
            }
            currentSphere = this.spheres[spheresCollected];
            currentSphere.SetActive(true);
        }
    }

    public void ApplyVisibilitySettings()
    {
        //if (!showHands && handVisibleTime > 0)
        //{
        //    StartCoroutine(HideHandsAfterDelay(handVisibleTime));
        //} else
        //{
        if (leftHand != null)
        {
            leftHand.GetComponent<MeshRenderer>().enabled = showHands;
        }
        if (rightHand != null)
        {
            rightHand.GetComponent<MeshRenderer>().enabled = showHands;
        }
        //}
    }

    void ApplyOffsetSettings()
    {
        leftHand.transform.localPosition = offsetValues;
        rightHand.transform.localPosition = offsetValues;
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
        recorder.StopRecording();
        // - Load next trial
        ResetTrial();
    }

    public void ResetTrial()
    {
        spheresCollected = 0;
        if (spheres != null){
            foreach(GameObject s in spheres){
                Destroy(s);
            }
            spheres = null;
        }
        spheresCollected = 0;
        started = false;
        // SpawnNextSphere();
    }

    void StartTrial() {
        Debug.Log("Beginning Start Process");
        started = true;
        StartCoroutine(DelayStart(3.0f));
    }

    IEnumerator DelayStart(float waitTime) {
        yield return new WaitForSeconds(waitTime);

        experimentController.StartExperiment();
        ApplyOffsetSettings();
        //ApplyVisibilitySettings();
        HandleSphereInteract();
        Debug.Log("Trial Started");
    }

    void HandleSphereInteract() {
        spheresCollected++;
        LoggingManager.Instance.LogTargetHit(currentSphere.transform.position, spheresCollected);
        Debug.Log($"Sphere collected! {spheresCollected}/{totalSpheres}");

        //if (currentSphere) {
        //    currentSphere.SetActive(false);
        //}

        if (spheresCollected >= totalSpheres) {
            EndTrial();
        } else {
            leftHand.GetComponent<MeshRenderer>().enabled = true;
            rightHand.GetComponent<MeshRenderer>().enabled = true;
            HideAfterExit();
            SpawnNextSphere();
        }
    }

}