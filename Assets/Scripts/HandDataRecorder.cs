using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class HandDataRecorder : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public bool isRecording = false;

    private List<string> dataBuffer = new List<string>();
    private float startTime;

    void Update()
    {
        if (!isRecording) return;

        float timestamp = Time.time - startTime;
        Vector3 leftPos = leftHand.position;
        Vector3 leftRot = leftHand.eulerAngles;
        Vector3 rightPos = rightHand.position;
        Vector3 rightRot = rightHand.eulerAngles;

        string line = $"{timestamp:F3},{leftPos.x:F4},{leftPos.y:F4},{leftPos.z:F4},{leftRot.x:F2},{leftRot.y:F2},{leftRot.z:F2}," +
                      $"{rightPos.x:F4},{rightPos.y:F4},{rightPos.z:F4},{rightRot.x:F2},{rightRot.y:F2},{rightRot.z:F2}";
        dataBuffer.Add(line);
    }

    public void StartRecording()
    {
        dataBuffer.Clear();
        startTime = Time.time;
        isRecording = true;
    }

    public void StopRecording()
    {
        isRecording = false;
        string path = Path.Combine(Application.persistentDataPath, $"hand_data_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv");
        File.WriteAllLines(path, dataBuffer);
        Debug.Log($"Saved hand motion data to: {path}");
    }
}
