
public abstract class ILogger
{
    private string _name;
    public void InitLog(string trialName) {
        _name = trialName;
    }
    abstract public void LogData(TrackingData data);
    abstract public void ClearLog();
    abstract public void SaveLog();

}
