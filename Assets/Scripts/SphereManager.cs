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
    private bool showHands;
    private bool showTargets;
    private float targetVisibleTime;
    private float handVisibleTime;
    private float targetProximity;
    private bool started = false;
    private Vector3 offsetValues;

    public void BeginTrial(Vector3 headsetPosition)
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
            GameObject proximityObject = new GameObject();
            proximityObject.transform.SetParent(spheres[i].transform);
            SphereCollider triggerCollider = proximityObject.AddComponent<SphereCollider>();
            triggerCollider.isTrigger = true;
            triggerCollider.radius = c.radius + targetProximity;
            triggerCollider.center = c.center;
            ProximityAlertTrigger proximityAlert = proximityObject.AddComponent<ProximityAlertTrigger>();
            proximityAlert.Initialize(i + 1);
            proximityAlert.OnProximityEnter += OnProximityEnter;
            spheres[i].SetActive(false);
        }
        
        SpawnNextSphere();
    }

    private void OnProximityEnter(int targetId)
    {
        LoggingManager.Instance.LogProximityHit(currentSphere.transform.position);
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
            currentSphere = this.spheres[spheresCollected];
            currentSphere.SetActive(true);
            
            if (!showTargets && targetVisibleTime > 0)
            {
                StartCoroutine(HideSphereAfterDelay(targetVisibleTime));
            }
        }
    }

    void ApplyVisibilitySettings()
    {
        if (!showHands && handVisibleTime > 0)
        {
            StartCoroutine(HideHandsAfterDelay(handVisibleTime));
        } else
        {
            if (leftHand != null)
            {
                leftHand.GetComponent<MeshRenderer>().enabled = showHands;
            }
            if (rightHand != null)
            {
                rightHand.GetComponent<MeshRenderer>().enabled = showHands;
            }
        }
    }

    void ApplyOffsetSettings()
    {
        leftHand.transform.localPosition = offsetValues;
        rightHand.transform.localPosition = offsetValues;
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
        ApplyVisibilitySettings();
        HandleSphereInteract();
        Debug.Log("Trial Started");
    }

    void HandleSphereInteract() {
        spheresCollected++;
        LoggingManager.Instance.LogTargetHit(currentSphere.transform.position, spheresCollected);
        Debug.Log($"Sphere collected! {spheresCollected}/{totalSpheres}");

        if (currentSphere) {
            currentSphere.SetActive(false);
        }

        if (spheresCollected >= totalSpheres) {
            EndTrial();
        } else {
            SpawnNextSphere();
        }
    }

}