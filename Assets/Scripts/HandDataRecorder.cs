using UnityEngine;

public class HandDataRecorder : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public bool isRecording = false; 
    private Vector3 _headsetStartPosition;

    void Update()
    {
        if (isRecording) {
            Vector3 leftPos = leftHand.position - _headsetStartPosition;
            Quaternion leftRot = leftHand.rotation;
            Vector3 rightPos = rightHand.position - _headsetStartPosition;
            Quaternion rightRot = rightHand.rotation;

            LoggingManager.Instance.currentTrackingData.leftHandPos = leftPos;
            LoggingManager.Instance.currentTrackingData.rightHandPos = rightPos;
            LoggingManager.Instance.currentTrackingData.leftHandRotation = leftRot;
            LoggingManager.Instance.currentTrackingData.rightHandRotation = rightRot;
        }
    }

    public void StartRecording(Vector3 headsetPosition)
    {
        isRecording = true;
        _headsetStartPosition = headsetPosition;   
    }

    public void StopRecording()
    {
        isRecording = false;
    }
}
