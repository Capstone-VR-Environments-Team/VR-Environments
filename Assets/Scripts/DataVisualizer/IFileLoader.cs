using System.Collections.Generic;

public abstract class IFileLoader
{
    public abstract List<TrackingData> loadFile(string path);
}
