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
    public GameObject leftHandCollider;
    public GameObject rightHandCollider;

    [Header("Spawn Locations")]
    
    [Header("Events")]
    public HandDataRecorder recorder;
    public ExperimentController experimentController;

    private int spheresCollected = 0;
    private GameObject currentSphere;
    private GameObject prevSphere;
    private int handVisType;
    private int targetVisType;
    private float handFlickerOnDuration;
    private float handFlickerOffDuration;
    private float targetFlickerOnDuration;
    private float targetFlickerOffDuration;
    private float targetProximity;
    private float targetSize;
    private string leftHandColor;
    private string rightHandColor;
    private string targetColor;
    private int timeBeforeStart;
    public bool started = false;
    private Vector3 offsetValues;

    private bool showHandsInProximity;

    private Coroutine handsFlickerRoutine;
    private Coroutine targetsFlickerRoutine;
    private Coroutine startTimerRoutine;

    private bool inProximity = false;

    private void OnEnable() {
        EventBus.PrimeExperiment += BeginTrial;
        EventBus.StopExperiment += ResetTrial;
    }

    private void OnDisable() {
        EventBus.PrimeExperiment -= BeginTrial;
        EventBus.StopExperiment -= ResetTrial;
    }

    private void Update()
    {
        ApplyOffsetSettings();
    }

    public void Start()
    {
        leftHandColor = '#' + SessionManager.Instance.GetLeftHandColor();
        rightHandColor = '#' + SessionManager.Instance.GetRightHandColor();
        offsetValues = SessionManager.Instance.GetOffsetValues();
        targetColor = '#' + SessionManager.Instance.GetTargetColor();
        SetColors();

        timeBeforeStart = SessionManager.Instance.GetTimeBeforeStart();
    }

    public void BeginTrial(Vector3 headsetPosition)
    {
        List<Vector3> sphereVectors = SessionManager.Instance.GetLoadedTargets();
        
        targetProximity = SessionManager.Instance.GetTargetProximity();
        targetSize = SessionManager.Instance.GetTargetSize();
        handFlickerOnDuration = SessionManager.Instance.GetHandFlickerOnDuration();
        handFlickerOffDuration = SessionManager.Instance.GetHandFlickerOffDuration();
        targetFlickerOnDuration = SessionManager.Instance.GetTargetFlickerOnDuration();
        targetFlickerOffDuration = SessionManager.Instance.GetTargetFlickerOffDuration();
        handVisType = SessionManager.Instance.GetHandsVisibilityType();
        targetVisType = SessionManager.Instance.GetTargetVisibilityType();
        showHandsInProximity = SessionManager.Instance.GetShowHandsInProximity();

        totalSpheres = sphereVectors.Count;
        spheres = new GameObject[totalSpheres];
        for (int i = 0; i < totalSpheres; i++) {
            spheres[i] = Instantiate(spherePrefab,
                                    sphereVectors[i] + headsetPosition,
                                    Quaternion.identity);
            spheres[i].transform.localScale = Vector3.one * (targetSize * 2);
            SphereContact sc = spheres[i].GetComponent<SphereContact>();
            if (sc != null)
            {
                sc.Initialize(i + 1, sphereVectors[i]);
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
                float parentRadius = spheres[i].transform.localScale.x / 2f;
                float scaleRatio = (parentRadius + targetProximity) / parentRadius;
                proximityObject.transform.localScale = Vector3.one * scaleRatio;
                triggerCollider.center = c.center;
                ProximityAlertTrigger proximityAlert = proximityObject.AddComponent<ProximityAlertTrigger>();
                proximityAlert.Initialize(sphereVectors[i], i);
                EventBus.OnProximityHit += OnProximityEnter;
            }
            spheres[i].SetActive(false);
        }
        
        SpawnNextSphere();
    }

    private void OnProximityEnter(Vector3 location, int id)
    {
        inProximity = true;
        if (showHandsInProximity)
        {
            ShowHandsInProximity();
        }
        Debug.Log("Proximity Hit");
    }

    private void ShowHandsInProximity()
    {
        if (leftHand)
        {
            leftHand.GetComponent<MeshRenderer>().enabled = true;
        }
        if (rightHand)
        {
            rightHand.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void HideAfterExit()
    {
        inProximity = false;
        if (currentSphere && targetVisType == 0)
        {
            currentSphere.GetComponent<Renderer>().enabled = false;
        }
    }

    public void ShowCurrentSphere()
    {
        if (currentSphere)
        {
            currentSphere.GetComponent<MeshRenderer>().enabled = true;
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

    public void OnStartSphereEnter()
    {
        if (!started)
        {
            if (startTimerRoutine != null) StopCoroutine(startTimerRoutine);
            startTimerRoutine = StartCoroutine(DelayStart(timeBeforeStart));
        }
    }

    public void OnStartSphereExit()
    {
        if (!started)
        {
            if (startTimerRoutine != null)
            {
                StopCoroutine(startTimerRoutine);
                startTimerRoutine = null;
                Debug.Log("Hand removed from start sphere early. Timer reset.");
            }
        }
    }

    public void OnSphereInteracted()
    {
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
        if (leftHand != null)
        {
            leftHand.GetComponent<MeshRenderer>().enabled = handVisType != 0;
        }
        if (rightHand != null)
        {
            rightHand.GetComponent<MeshRenderer>().enabled = handVisType != 0;
        }
    }

    void ApplyOffsetSettings()
    {
        leftHand.transform.position = leftHandCollider.transform.position + offsetValues;
        rightHand.transform.position = rightHandCollider.transform.position + offsetValues;

        leftHand.transform.rotation = leftHandCollider.transform.rotation; 
        rightHand.transform.rotation = rightHandCollider.transform.rotation;
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
        StartCoroutine(DelayStart(timeBeforeStart));
    }

    IEnumerator DelayStart(int waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Debug.Log("Setting started to true");
        started = true;
        experimentController.StartExperiment();

        if (handVisType == 1)
        {
            handsFlickerRoutine = StartCoroutine(FlickerHands());
        }
        if (targetVisType == 1)
        {
            targetsFlickerRoutine = StartCoroutine(FlickerTargets());
        }

        if (spheres != null && spheres.Length > 0)
        {
            EventBus.OnTargetHit?.Invoke(spheres[0].transform.position, 1);
        }

        HandleSphereInteract();
        Debug.Log("Trial Started");
    }

    void HandleSphereInteract() {
        spheresCollected++;
        Debug.Log($"Sphere collected! {spheresCollected}/{totalSpheres}");

        if (spheresCollected >= totalSpheres) {
            EndTrial();
        } else {
            leftHand.GetComponent<MeshRenderer>().enabled = true;
            rightHand.GetComponent<MeshRenderer>().enabled = true;
            if (prevSphere)
            {
                prevSphere.gameObject.SetActive(false);
            }
            SpawnNextSphere();
        }
    }

    private IEnumerator FlickerHands()
    {
        while (started)
        {
            if (!(showHandsInProximity && inProximity))
            {
                if (leftHand)
                {
                    MeshRenderer lr = leftHand.GetComponent<MeshRenderer>();
                    if (lr) lr.enabled = true;
                }

                if (rightHand)
                {
                    MeshRenderer rr = rightHand.GetComponent<MeshRenderer>();
                    if (rr) rr.enabled = true;
                }
            }

            yield return new WaitForSeconds(handFlickerOnDuration);


            if (!(showHandsInProximity && inProximity))
            {
                // Turn hands off
                if (leftHand)
                {
                    MeshRenderer lr = leftHand.GetComponent<MeshRenderer>();
                    if (lr) lr.enabled = false;
                }

                if (rightHand)
                {
                    MeshRenderer rr = rightHand.GetComponent<MeshRenderer>();
                    if (rr) rr.enabled = false;
                }
            }

            yield return new WaitForSeconds(handFlickerOffDuration);
            
        }
    }

    private IEnumerator FlickerTargets()
    {
        while (started)
        {
            if (currentSphere)
            {
                Renderer tr = currentSphere.GetComponent<Renderer>();
                if (tr) tr.enabled = true;
            }

            yield return new WaitForSeconds(targetFlickerOnDuration);

            if (currentSphere)
            {
                Renderer tr = currentSphere.GetComponent<Renderer>();
                if (tr) tr.enabled = false;
            }

            yield return new WaitForSeconds(targetFlickerOffDuration);
        }
    }

    private void SetColors()
    {
        leftHand.GetComponent<MeshRenderer>().material.color = ColorUtility.TryParseHtmlString(leftHandColor, out Color lhColor) ? lhColor : Color.blue;
        rightHand.GetComponent<MeshRenderer>().material.color = ColorUtility.TryParseHtmlString(rightHandColor, out Color rhColor) ? rhColor : Color.red;
        spherePrefab.GetComponent<MeshRenderer>().sharedMaterial.color = ColorUtility.TryParseHtmlString(targetColor, out Color tColor) ? tColor : Color.gray;
    }

    public Color GetTargetColor()
    {
        return ColorUtility.TryParseHtmlString(targetColor, out Color tColor) ? tColor : Color.gray;
    }
}