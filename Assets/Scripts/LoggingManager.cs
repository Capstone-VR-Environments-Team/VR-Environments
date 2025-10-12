using UnityEngine;

public class LoggingManager : Singleton<LoggingManager>
{
    [SerializeField] ILogger logger;

    public TrackingData currentTrackingData = new TrackingData();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logger.InitLog("PLACEHOLDER_NAME");
    }

    // Update is called once per frame
    void Update()
    {
        logger.LogData(new TrackingData(currentTrackingData));
    }

}
