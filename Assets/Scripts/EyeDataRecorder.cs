using UnityEngine;
using static Varjo.XR.VarjoEyeTracking;

public class EyeDataRecorder : MonoBehaviour {

    public Transform headset;
    private Transform camera;
    public Vector3 gazeOrigin = Vector3.zero;
    public Vector3 gazeDirection = Vector3.zero;
    public float focusDistance = 0.0f;
    public float leftPupilDiameter = 0.0f;
    public float rightPupilDiameter = 0.0f;
    public bool isGazeValid = false;
    public bool isRecording = false;
    private Vector3 _headsetStartPosition;

    void Start() {
        // Check if the feature is actually enabled in settings
        if (IsGazeAllowed()) {
            Debug.Log("Varjo: Gaze is allowed.");

            // This forces the headset to look for eyes and run the 
            // "follow the dot" sequence if needed.
            if (!IsGazeCalibrated()) {
                Debug.Log("Requesting calibration...");
                RequestGazeCalibration(GazeCalibrationMode.Fast);
            } else {
                Debug.Log("Already Calibrated!");
            }
        } else {
            Debug.LogError("Varjo: Gaze is NOT allowed.");
        }

        camera = Camera.main.transform;
    }

    void Update() {
        if (isRecording) {
            GazeData data = GetGaze();
            EyeMeasurements measurements = GetEyeMeasurements();
            //Debug.Log("Eye Status: " + data.status); // expensive
            if (data.status == GazeStatus.Valid) {
                isGazeValid = true;
                EventBus.OnEyesTracked?.Invoke(camera.TransformPoint(data.gaze.origin) - _headsetStartPosition,
                    camera.transform.TransformDirection(data.gaze.forward),
                    data.focusDistance,
                    measurements.leftPupilDiameterInMM,
                    measurements.rightPupilDiameterInMM);
            }
        }
    }

    private void OnEnable() {
        EventBus.StartExperiment += StartRecording;
        EventBus.StopExperiment += StopRecording;
    }

    private void OnDisable() {
        EventBus.StartExperiment -= StartRecording;
        EventBus.StopExperiment -= StopRecording;
    }

    public void StartRecording(Vector3 headsetPosition) {
        isRecording = true;
        _headsetStartPosition = headsetPosition;

    }

    public void StopRecording() {
        isRecording = false;
    }
}