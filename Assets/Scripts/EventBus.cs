using System;
using UnityEngine;

public static class EventBus {
    // High-frequency continuous data
    public static Action<Vector3, Quaternion> OnLeftHandTracked;
    public static Action<Vector3, Quaternion> OnRightHandTracked;
    public static Action<Vector3, Vector3, float, float, float> OnEyesTracked;

    // Discrete game events
    public static Action<Vector3, int> OnTargetHit;
    public static Action<Vector3> OnProximityHit;
    public static Action<string, double> OnNoteEnter;


    public static Action<Vector3> StartExperiment;
    public static Action StopExperiment;
    public static Action<Vector3> PrimeExperiment;
}