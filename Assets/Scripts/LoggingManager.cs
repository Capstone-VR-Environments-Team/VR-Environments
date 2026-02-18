using System;
using System.IO;
using UnityEngine;

public class LoggingManager : Singleton<LoggingManager>
{
    ILogger logger;
    [SerializeField] bool logging = false;

    public TrackingData currentTrackingData = new TrackingData();

    public CollectedTimingData collectedTimingData = new CollectedTimingData();
    private long _startTime;
    private string _currentTrialName;
    private string _directory;
    private Vector3 _headsetPosition;
    private float _logTimer = 0f;
    private const float LogInterval = 0.025f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logger = new CsvLogger();
    }

    // Update is called once per frame
    void Update()
    {
        if (logging) {
            // Accumulate time passed since last frame
            _logTimer += Time.deltaTime;

            // Check if 25ms has passed
            if (_logTimer >= LogInterval) {
                logger.LogData(new TrackingData(currentTrackingData));
                _logTimer -= LogInterval;
            }
        }
    }

    public void StartRecording(string name, Vector3 headsetPosition) {
        _currentTrialName = name;
        _startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _headsetPosition = headsetPosition;
        // Reset data for new trial
        collectedTimingData = new CollectedTimingData();


        logger.InitLog(name);
        _logTimer = 0f;
        logging = true;
    }

    public void StopRecording()
    {
        if (logging == false) return;
        _directory = FileManager.Instance.SaveSessionInformation(collectedTimingData);
        logger.SaveLog(_directory); // Save CSV
        logging = false;
    }



    public double GetTrialTime()
    {
        if (!logging) return 0.0;
        long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return (currentTime - _startTime);
    }

    public void LogTargetHit(Vector3 targetLocation, int targetId)
    {
        if (!logging) return;

        double time = GetTrialTime();

        collectedTimingData.TargetHits.Add(new HitEvent(time, targetLocation - _headsetPosition));

        string noteContent = $"Target {targetId} hit";

        collectedTimingData.Notes.Add(new NoteEvent(time, noteContent));

        LiveTrialViewManager ui = FindFirstObjectByType<LiveTrialViewManager>();
        if (ui != null)
        {
            TimeSpan t = TimeSpan.FromSeconds(time / 1000);
            string timeString = string.Format("{0:D1}:{1:D2}", t.Minutes, t.Seconds);
            ui.AppendToLog($"{timeString} - {noteContent}");
        }
    }

    public void LogProximityHit(Vector3 targetLocation)
    {
        if (!logging) return;
        collectedTimingData.TargetProximityHits.Add(new HitEvent(GetTrialTime(), targetLocation - _headsetPosition));
    }

    public void LogNote(string content, double timestamp)
    {
        if (!logging) return;

        collectedTimingData.Notes.Add(new NoteEvent(timestamp, content));
    }

}