using UnityEngine;

public class HandDataRecorder : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public bool isRecording = false; 
    private Vector3 _headsetStartPosition;

    private void OnEnable() {
        EventBus.StartExperiment += StartRecording;
        EventBus.StopExperiment += StopRecording;
    }

    private void OnDisable() {
        EventBus.StartExperiment -= StartRecording;
        EventBus.StopExperiment -= StopRecording;
    }

    void Update()
    {
        if (isRecording) {
            Vector3 leftPos = leftHand.position - _headsetStartPosition;
            Quaternion leftRot = leftHand.rotation;
            Vector3 rightPos = rightHand.position - _headsetStartPosition;
            Quaternion rightRot = rightHand.rotation;

            EventBus.OnLeftHandTracked?.Invoke(leftPos, leftRot);
            EventBus.OnRightHandTracked?.Invoke(rightPos, rightRot);
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
