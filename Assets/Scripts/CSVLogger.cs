using System;
using System.IO;
using System.Text;
using System.Globalization;
using UnityEngine;

public class CsvLogger : ILogger {
    private StringBuilder _buffer;           
    private string _filePath;                
    private bool _initialized = false;

    public CsvLogger() {
        _buffer = new StringBuilder();
    }

    public override void LogData(TrackingData data) {
        if (!_initialized) {
            Debug.LogWarning("CsvLogger not initialized. Call InitLog() before logging.");
            return;
        }

        _buffer.AppendLine(string.Format(CultureInfo.InvariantCulture,
            "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
            data.timeStamp,
            data.leftHandPos.x, data.leftHandPos.y, data.leftHandPos.z,
            data.leftHandRotation.x, data.leftHandRotation.y, data.leftHandRotation.z, data.leftHandRotation.w,
            data.rightHandPos.x, data.rightHandPos.y, data.rightHandPos.z,
            data.rightHandRotation.x, data.rightHandRotation.y, data.rightHandRotation.z, data.rightHandRotation.w
        ));
    }

    public override void ClearLog() {
        _buffer.Clear();
        WriteHeader();
    }

    public override void SaveLog() {
        if (!_initialized) {
            Debug.LogWarning("CsvLogger not initialized. Call InitLog() before saving.");
            return;
        }

        try {
            File.WriteAllText(_filePath, _buffer.ToString());
            Debug.Log($"CSV log saved to: {_filePath}");
        } catch (Exception e) {
            Debug.LogError($"Failed to save CSV log: {e.Message}");
        }
    }

    // Override the base InitLog to also create file path and header
    public new void InitLog(string trialName) {
        base.InitLog(trialName);

        string fileName = $"{trialName}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        _filePath = Path.Combine(Application.persistentDataPath, fileName);
        _buffer.Clear();

        WriteHeader();
        _initialized = true;
    }

    private void WriteHeader() {
        _buffer.AppendLine("Timestamp," +
                           "Lx,Ly,Lz,LqX,LqY,LqZ,LqW," +
                           "Rx,Ry,Rz,RqX,RqY,RqZ,RqW");
    }
}

