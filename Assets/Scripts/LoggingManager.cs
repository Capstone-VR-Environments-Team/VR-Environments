using UnityEngine;

public class LoggingManager : Singleton<LoggingManager>
{
    ILogger logger;
    [SerializeField] bool logging = false;

    public TrackingData currentTrackingData = new TrackingData();

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
        logger.InitLog(name);
        logging = true;
    } 

    public void StopRecording() { 
        logger.SaveLog();
        logging = false;
    }

}
