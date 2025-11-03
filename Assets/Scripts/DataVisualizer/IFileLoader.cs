using System.Collections.Generic;
using UnityEngine;

public abstract class IFileLoader
{
    public abstract string[] getFilePaths(string startDirectory);
    public abstract List<TrackingData> loadFile(string path);
}
