using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TrialSettings
{
    public float Close;
    public bool HandsHidden;
    public float HandVisibleTime;
    public bool TargetsHidden;
    public float TargetVisibleTime;
    public string HandOffsetType;
    public Vector3 HandOffsetValue;
    public string SelectedBackground;
}

[Serializable]
public class TrialData
{
    public string TrialName;
    public string Description;
    public TrialSettings Settings;
    public List<Vector3> Targets;
}

