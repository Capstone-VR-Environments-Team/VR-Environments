using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CollectedTimingData
{
    public List<HitEvent> TargetHits = new();
    public List<HitEvent> TargetProximityHits = new();
    public List<HitEvent> LeaveTargetHits = new();
    public List<HitEvent> ReEnterTargetHits = new();
    public List<NoteEvent> Notes = new();
}

[Serializable]
public class HitEvent
{
    public double time;
    public Vector3 location;

    public HitEvent(double time, Vector3 location)
    {
        this.time = time;
        this.location = location;
    }
}

[Serializable]
public class NoteEvent
{
    public double time;
    public string content;

    public NoteEvent(double time, string content)
    {
        this.time = time;
        this.content = content;
    }
}