using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class CsvLogger : ILogger {
    private StringBuilder _buffer;           
    private string _filePath;
    private string _fileName;
    private long _time;
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
            "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23}",
            data.timeStamp - _time,
            data.leftHandPos.x, data.leftHandPos.y, data.leftHandPos.z,
            data.leftHandRotation.x, data.leftHandRotation.y, data.leftHandRotation.z, data.leftHandRotation.w,
            data.rightHandPos.x, data.rightHandPos.y, data.rightHandPos.z,
            data.rightHandRotation.x, data.rightHandRotation.y, data.rightHandRotation.z, data.rightHandRotation.w,
            data.gazeOrigin.x, data.gazeOrigin.y, data.gazeOrigin.z,
            data.gazeDirection.x, data.gazeDirection.y, data.gazeDirection.z,
            data.focusDistance,
            data.leftPupilDiameter,
            data.rightPupilDiameter
        ));
    }

    public override void ClearLog() {
        _buffer.Clear();
        WriteHeader();
    }

    public override void SaveLog(string directory) {
        if (!_initialized) {
            Debug.LogWarning("CsvLogger not initialized. Call InitLog() before saving.");
            return;
        }

        try {
            _filePath = Path.Combine(directory, _fileName);
            Debug.Log(_filePath);
            File.WriteAllText(_filePath, _buffer.ToString());
            Debug.Log($"CSV log saved to: {_filePath}");
        } catch (Exception e) {
            Debug.LogError($"Failed to save CSV log: {e.Message}");
        }
    }

    // Override the base InitLog to also create file path and header
    public override void InitLog(string trialName) {
        _time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _fileName = $"{trialName}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        _buffer.Clear();

        WriteHeader();
        _initialized = true;
    }

    private void WriteHeader() {
        _buffer.AppendLine("Timestamp," +
                           "Lx,Ly,Lz,LqX,LqY,LqZ,LqW," +
                           "Rx,Ry,Rz,RqX,RqY,RqZ,RqW," +
                           "Gx,Gy,Gz,Glx,Gly,Glz,Gf,Lpd,Rpd");
    }

}

