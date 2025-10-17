using UnityEngine;

public class HandDataRecorder : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public bool isRecording = false; 

    void Update()
    {
        if (isRecording) {
            Vector3 leftPos = leftHand.position;
            Quaternion leftRot = leftHand.rotation;
            Vector3 rightPos = rightHand.position;
            Quaternion rightRot = rightHand.rotation;

            LoggingManager.Instance.currentTrackingData.leftHandPos = leftPos;
            LoggingManager.Instance.currentTrackingData.rightHandPos = rightPos;
            LoggingManager.Instance.currentTrackingData.leftHandRotation = leftRot;
            LoggingManager.Instance.currentTrackingData.rightHandRotation = rightRot;
        }
    }

    public void StartRecording()
    {
        isRecording = true;
    }

    public void StopRecording()
    {
        isRecording = false;
    }
}
