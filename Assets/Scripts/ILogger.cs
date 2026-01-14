
public abstract class ILogger
{
    abstract public void InitLog(string trialName, string directory);
    abstract public void LogData(TrackingData data);
    abstract public void ClearLog();
    abstract public void SaveLog();

}
