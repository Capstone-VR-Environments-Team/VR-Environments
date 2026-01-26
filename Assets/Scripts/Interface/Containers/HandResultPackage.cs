using System.Collections.Generic;

public struct HandResultPackage
{
    public DeviationData total;
    public DeviationData search;
    public DeviationData approach;
    public List<AnalysisMode> pointTypes;
    public List<double> distVals;
    public List<double> hVals;
    public List<double> vVals;
}