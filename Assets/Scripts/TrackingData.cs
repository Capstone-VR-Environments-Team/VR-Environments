using System;
using UnityEngine;

[Serializable]
public class TrackingData
{
    public long timeStamp;
    public Vector3 leftHandPos;
    public Vector3 rightHandPos;
    public Quaternion leftHandRotation;
    public Quaternion rightHandRotation;
    public Vector3 gazeOrigin = Vector3.zero;
    public Vector3 gazeDirection = Vector3.zero;
    public float focusDistance = 0.0f;
    public float leftPupilDiameter = 0.0f;
    public float rightPupilDiameter = 0.0f;

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

    public TrackingData(Vector3 leftHandPos, Vector3 rightHandPos, Quaternion leftHandRotation, Quaternion rightHandRotation, 
        Vector3 gazeOrigin, Vector3 gazeDirection, float focusDistance, float leftPupilDiameter, float rightPupilDiameter) {
        this.timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        this.leftHandPos = leftHandPos;
        this.rightHandPos = rightHandPos;
        this.leftHandRotation = leftHandRotation;
        this.rightHandRotation = rightHandRotation;
        this.gazeOrigin = gazeOrigin;
        this.gazeDirection = gazeDirection;
        this.focusDistance = focusDistance;
        this.leftPupilDiameter = leftPupilDiameter;
        this.rightPupilDiameter = rightPupilDiameter;

    }

    public TrackingData(TrackingData original) {
        this.timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        leftHandPos = original.leftHandPos;
        rightHandPos = original.rightHandPos;
        leftHandRotation = original.leftHandRotation;
        rightHandRotation = original.rightHandRotation;
        gazeOrigin = original.gazeOrigin;
        gazeDirection = original.gazeDirection;
        focusDistance = original.focusDistance;
        leftPupilDiameter = original.leftPupilDiameter;
        rightPupilDiameter = original.rightPupilDiameter;
    }
}
