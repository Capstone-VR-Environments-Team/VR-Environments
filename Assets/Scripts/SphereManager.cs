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
    private bool flickeringHands = true;
    private bool flickeringTargets = true;
    private float flickerInterval = 0.1f;
    private float handVisibleTime;
    private float targetProximity;
    private bool started = false;
    private Vector3 offsetValues;

    private Coroutine handsFlickerRoutine;
    private Coroutine targetsFlickerRoutine;

    private void OnEnable() {
        EventBus.PrimeExperiment += BeginTrial;
        EventBus.StopExperiment += ResetTrial;
    }

    private void OnDisable() {
        EventBus.PrimeExperiment -= BeginTrial;
        EventBus.StopExperiment -= ResetTrial;
    }

    public void BeginTrial(Vector3 headsetPosition)
    {
        List<Vector3> sphereVectors = SessionManager.Instance.GetLoadedTargets();
        showHands = SessionManager.Instance.GetShowHands();
        showTargets = SessionManager.Instance.GetShowTargets();
        handVisibleTime = SessionManager.Instance.GetHandVisibleTime();
        targetProximity = SessionManager.Instance.GetTargetProximity();
        offsetValues = SessionManager.Instance.GetOffsetValues();
        //flickerInterval = SessionManager.Instance.GetFlickerInterval();
        //flickeringHands = SessionManager.Instance.GetFlickeringHands();
        //flickeringTargets = SessionManager.Instance.GetFlickeringTargets();
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
                proximityObject.transform.localScale = new Vector3(1 + targetProximity / spheres[i].transform.localScale.x, 1 + targetProximity / spheres[i].transform.localScale.x, 1 + targetProximity / spheres[i].transform.localScale.x);
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
        EventBus.OnProximityHit?.Invoke(currentSphere.transform.position);
        Debug.Log("Proximity Hit");
    }

    public void HideAfterExit()
    {
        if (currentSphere && !showTargets)
        {
            currentSphere.GetComponent<Renderer>().enabled = false;
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
        if (handsFlickerRoutine != null) StopCoroutine(handsFlickerRoutine);
        if (targetsFlickerRoutine != null) StopCoroutine(targetsFlickerRoutine);

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
        if (flickeringHands && showHands)
        {
            handsFlickerRoutine = StartCoroutine(FlickerHands());
        }
        if (flickeringTargets && showTargets)
        {
            targetsFlickerRoutine = StartCoroutine(FlickerTargets());
        }
        HandleSphereInteract();
        Debug.Log("Trial Started");
    }

    void HandleSphereInteract() {
        spheresCollected++;
        EventBus.OnTargetHit?.Invoke(currentSphere.transform.position, spheresCollected);
        Debug.Log($"Sphere collected! {spheresCollected}/{totalSpheres}");

        //if (currentSphere) {
        //    currentSphere.SetActive(false);
        //}

        if (spheresCollected >= totalSpheres) {
            EndTrial();
        } else {
            leftHand.GetComponent<MeshRenderer>().enabled = true;
            rightHand.GetComponent<MeshRenderer>().enabled = true;
            if (prevSphere)
            {
                prevSphere.GetComponent<Renderer>().enabled = false;
            }
            SpawnNextSphere();
        }
    }
    private IEnumerator FlickerHands()
    {
        while (started)
        {
            yield return new WaitForSeconds(flickerInterval);

            if (showHands)
            {
                if (leftHand)
                {
                    MeshRenderer lr = leftHand.GetComponent<MeshRenderer>();
                    lr.enabled = !lr.enabled;
                }
                if (rightHand)
                {
                    MeshRenderer rr = rightHand.GetComponent<MeshRenderer>();
                    rr.enabled = !rr.enabled;
                }
            }
        }
    }

    private IEnumerator FlickerTargets()
    {
        while (started)
        {
            yield return new WaitForSeconds(flickerInterval);

            if (showTargets && currentSphere)
            {
                Renderer tr = currentSphere.GetComponent<Renderer>();
                if (tr)
                {
                    tr.enabled = !tr.enabled;
                }
            }
        }
    }
}