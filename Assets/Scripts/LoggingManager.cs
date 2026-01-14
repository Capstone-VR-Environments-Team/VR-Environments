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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logger = new CsvLogger();
    }

    // Update is called once per frame
    void Update()
    {
        if (logging) {
            logger.LogData(new TrackingData(currentTrackingData));
        }
    }

    public void StartRecording(string name) {
        _currentTrialName = name;
        _startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        
        // Reset data for new trial
        collectedTimingData = new CollectedTimingData();

        _directory = CreateSaveDirectory();

        logger.InitLog(name, _directory);
        logging = true;
    } 

    public void StopRecording() { 

        logger.SaveLog(); // Save CSV
        SaveTimingData(); // Save JSON
        logging = false;
    }

    public string CreateSaveDirectory()
    {
        string folderName = $"{_currentTrialName}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

        string rootPath = Application.persistentDataPath;

        string directoryPath = Path.Combine(rootPath, folderName);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Debug.Log($"Directory created at: {directoryPath}");
        }

        return directoryPath;
    }

    public double GetTrialTime()
    {
        if (!logging) return 0.0;
        long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return (currentTime - _startTime) / 1000.0; // Convert to Seconds
    }

    public void LogTargetHit(Vector3 targetLocation)
    {
        if (!logging) return;
        collectedTimingData.TargetHits.Add(new HitEvent(GetTrialTime(), targetLocation));
    }

    public void LogProximityHit(Vector3 targetLocation)
    {
        if (!logging) return;
        collectedTimingData.TargetProximityHits.Add(new HitEvent(GetTrialTime(), targetLocation));
    }

    public void LogNote(string content, double timestamp)
    {
        if (!logging) return;
        collectedTimingData.Notes.Add(new NoteEvent(timestamp, content));
    }

    private void SaveTimingData()
    {
        string json = JsonUtility.ToJson(collectedTimingData, true);
        string fileName = $"{_currentTrialName}_{DateTime.Now:yyyyMMdd_HHmmss}_Timing.json";
        string filePath = Path.Combine(_directory, fileName);

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log($"Timing JSON saved to: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save Timing JSON: {e.Message}");
        }
    }

}
