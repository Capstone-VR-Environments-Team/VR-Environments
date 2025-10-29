
public abstract class ILogger
{
    abstract public void InitLog(string trialName);
    abstract public void LogData(TrackingData data);
    abstract public void ClearLog();
    abstract public void SaveLog();

}
