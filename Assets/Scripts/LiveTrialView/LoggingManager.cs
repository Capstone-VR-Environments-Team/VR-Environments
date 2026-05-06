using System;
using System.Collections;
using UnityEngine;

public class LoggingManager : MonoBehaviour
{
    ILogger logger;
    [SerializeField] bool logging = false;

    public TrackingData currentTrackingData = new TrackingData();

    public CollectedTimingData collectedTimingData = new CollectedTimingData();
    private string _directory;
    private Vector3 _headsetPosition;
    private float _logTimer = 0f;
    private const float LogInterval = 0.025f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logger = new CsvLogger();
    }

    private void OnEnable()
    {
        EventBus.OnLeftHandTracked += UpdateLeftHand;
        EventBus.OnRightHandTracked += UpdateRightHand;
        EventBus.StartExperiment += StartRecording;
        EventBus.StopExperiment += StopRecording;
        EventBus.OnTargetHit += LogTargetHit;
        EventBus.OnProximityHit += LogProximityHit;
        EventBus.OnNoteEnter += LogNote;
        EventBus.OnEyesTracked += UpdateEyes;
        EventBus.OnTargetReEntry += LogTargetReEntry;
        EventBus.OnTargetExit += LogTargetExit;
        EventBus.LastSphere += HitLast;
    }

    private void OnDisable()
    {
        EventBus.OnLeftHandTracked -= UpdateLeftHand;
        EventBus.OnRightHandTracked -= UpdateRightHand;
        EventBus.StartExperiment -= StartRecording;
        EventBus.StopExperiment -= StopRecording;
        EventBus.OnTargetHit -= LogTargetHit;
        EventBus.OnProximityHit -= LogProximityHit;
        EventBus.OnNoteEnter -= LogNote;
        EventBus.OnEyesTracked -= UpdateEyes;
        EventBus.OnTargetReEntry -= LogTargetReEntry;
        EventBus.OnTargetExit -= LogTargetExit;
        EventBus.LastSphere -= HitLast;


    }

    // Update is called once per frame
    void Update()
    {
        if (logging)
        {
            // Accumulate time passed since last frame
            _logTimer += Time.deltaTime;

            // Check if 25ms has passed
            if (_logTimer >= LogInterval)
            {
                logger.LogData(new TrackingData(currentTrackingData));
                _logTimer -= LogInterval;
            }
        }
    }

    public void UpdateEyes(Vector3 gazeOrigin, Vector3 gazeDir, float focusDist, float leftEyeDiameter,
        float rightEyeDiameter)
    {
        currentTrackingData.gazeOrigin = gazeOrigin;
        currentTrackingData.gazeDirection = gazeDir;
        currentTrackingData.focusDistance = focusDist;
        currentTrackingData.leftPupilDiameter = leftEyeDiameter;
        currentTrackingData.rightPupilDiameter = rightEyeDiameter;
    }

    public void UpdateLeftHand(Vector3 leftPos, Quaternion leftRot)
    {
        currentTrackingData.leftHandPos = leftPos;
        currentTrackingData.leftHandRotation = leftRot;
    }

    public void UpdateRightHand(Vector3 rightPos, Quaternion rightRot)
    {
        currentTrackingData.rightHandPos = rightPos;
        currentTrackingData.rightHandRotation = rightRot;
    }

    public void StartRecording(Vector3 headsetPosition)
    {
        _headsetPosition = headsetPosition;
        // Reset data for new trial
        collectedTimingData = new CollectedTimingData();


        logger.InitLog(SessionManager.Instance.GetTrialName());
        _logTimer = 0f;
        logging = true;
    }

    public void HitLast()
    {
        StartCoroutine(HitLastCoroutine());
    }

    private IEnumerator HitLastCoroutine()
    {
        yield return new WaitForSeconds(.125f);
        logging = false;
    }

    public void StopRecording()
    {
        _directory = SessionManager.Instance.SaveSessionInformation(collectedTimingData);
        logger.SaveLog(_directory); // Save CSV
        logging = false;
    }

    public void LogTargetHit(Vector3 targetLocation, int targetId)
    {
        if (!logging) return;

        double time = SessionManager.Instance.GetTrialTime();
        collectedTimingData.TargetHits.Add(new HitEvent(time, targetLocation, targetId));
        collectedTimingData.TargetEvents.Add(new TargetEventRecord(time, TargetEventType.TargetHit, targetLocation, targetId));
        LogHitEvent(time, $"Target {targetId} hit");
    }

    public void LogProximityHit(Vector3 targetLocation, int targetId)
    {
        if (!logging) return;

        double time = SessionManager.Instance.GetTrialTime();
        collectedTimingData.TargetProximityHits.Add(new HitEvent(time, targetLocation, targetId));
        collectedTimingData.TargetEvents.Add(new TargetEventRecord(time, TargetEventType.ProximityHit, targetLocation, targetId));
    }

    public void LogNote(string content, double timestamp)
    {
        if (!logging) return;

        collectedTimingData.Notes.Add(new NoteEvent(timestamp, content));
    }

    public void LogTargetReEntry(Vector3 targetLocation, int targetId)
    {
        if (!logging) return;

        double time = SessionManager.Instance.GetTrialTime();
        collectedTimingData.ReEnterTargetHits.Add(new HitEvent(time, targetLocation, targetId));
        collectedTimingData.TargetEvents.Add(new TargetEventRecord(time, TargetEventType.TargetReEntry, targetLocation, targetId));
        LogHitEvent(time, $"Target {targetId} re-entered");
    }

    public void LogTargetExit(Vector3 targetLocation, int targetId)
    {
        if (!logging) return;

        double time = SessionManager.Instance.GetTrialTime();
        collectedTimingData.LeaveTargetHits.Add(new HitEvent(time, targetLocation, targetId));
        collectedTimingData.TargetEvents.Add(new TargetEventRecord(time, TargetEventType.TargetExit, targetLocation, targetId));
        LogHitEvent(time, $"Target {targetId} exited");
    }

    private void LogHitEvent(double time, string noteContent)
    {
        collectedTimingData.Notes.Add(new NoteEvent(time, noteContent));
        LiveTrialViewManager ui = FindFirstObjectByType<LiveTrialViewManager>();
        if (ui != null)
        {
            TimeSpan t = TimeSpan.FromSeconds(time / 1000);
            string timeString = string.Format("{0:D1}:{1:D2}", t.Minutes, t.Seconds);
            ui.AppendToLog($"{timeString} - {noteContent}");
        }
    }
}