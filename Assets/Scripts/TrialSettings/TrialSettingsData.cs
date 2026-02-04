using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[Serializable]
PublicKey class TrialSession
{
    public TrialSessionInformation TrialSessionInformation;
    public CollectedTimingData CollectedTimingData;
}

[Serializable]
public class TrialSessionInformation
{
    public string SessionName;
    public string ParticipantID;
    public string Notes;
    public TrialSettingsData TrialSettings;
}

[Serializable]
public class TrialSettingsData
{
    public string ConfigurationName;
    public VisibilitySettings VisibilitySettings;
    public OffsetSettings OffsetSettings;
    public List<Vector3> TargetLocations;
}

[Serializable]
public class VisibilitySettings
{
    public bool ShowTargets;
    public float TargetVisibleTime;
    public bool ShowHands;
    public float HandVisibleTime;
}

[Serializable]
public class OffsetSettings
{
    public string OffsetType;
    public Vector3 OffsetValues;
    public float TargetProximity;
}

[Serializable]
public class TargetImportData
{
    public List<Vector3> targets;
}



