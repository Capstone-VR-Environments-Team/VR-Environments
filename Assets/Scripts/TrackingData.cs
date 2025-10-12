using System.Net;
using UnityEngine;

public class TrackingData
{
    public double timeStamp;
    public Vector3 leftHandPos;
    public Vector3 rightHandPos;
    public Quaternion leftHandRotation;
    public Quaternion rightHandRotation;

    public TrackingData() { }
    public TrackingData(Vector3 leftHandPos, Vector3 rightHandPos, Quaternion leftHandRotation, Quaternion rightHandRotation) { 
        this.leftHandPos = leftHandPos;
        this.rightHandPos = rightHandPos;
        this.leftHandRotation = leftHandRotation;
        this.rightHandRotation = rightHandRotation;
    }

    public TrackingData(TrackingData original) {
        leftHandPos = original.leftHandPos;
        rightHandPos = original.rightHandPos;
        leftHandRotation = original.leftHandRotation;
        rightHandRotation = original.rightHandRotation;
    }
}
