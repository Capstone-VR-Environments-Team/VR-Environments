using System;
using UnityEngine;

public class TrackingData
{
    public long timeStamp;
    public Vector3 leftHandPos;
    public Vector3 rightHandPos;
    public Quaternion leftHandRotation;
    public Quaternion rightHandRotation;

    public TrackingData() {
        this.timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
    public TrackingData(Vector3 leftHandPos, Vector3 rightHandPos, Quaternion leftHandRotation, Quaternion rightHandRotation) {
        this.timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); 
        this.leftHandPos = leftHandPos;
        this.rightHandPos = rightHandPos;
        this.leftHandRotation = leftHandRotation;
        this.rightHandRotation = rightHandRotation;
    }

    public TrackingData(TrackingData original) {
        this.timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        leftHandPos = original.leftHandPos;
        rightHandPos = original.rightHandPos;
        leftHandRotation = original.leftHandRotation;
        rightHandRotation = original.rightHandRotation;
    }
}
