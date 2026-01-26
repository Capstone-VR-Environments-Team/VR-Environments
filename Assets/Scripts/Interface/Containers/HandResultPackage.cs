using System.Collections.Generic;

public struct HandResultPackage
{
    public Statistics statsDist;
    public Statistics statsH;
    public Statistics statsV;
    public Statistics statsSearchTime;
    public List<AnalysisMode> pointTypes;
    public List<double> distVals;
    public List<double> hVals;
    public List<double> vVals;
}