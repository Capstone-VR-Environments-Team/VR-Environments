using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management; // Required for XR control

public class XRManager : MonoBehaviour {
    // Call this when loading into your VR scene
    public void TurnVROn() {
        StartCoroutine(StartXRCoroutine());
    }

    // Call this before going back to a 2D desktop scene
    public void TurnVROff() {
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete) {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            Debug.Log("XR subsystems stopped. Headset display is off.");
        }
    }

    private IEnumerator StartXRCoroutine() {
        Debug.Log("Initializing XR...");
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader == null) {
            Debug.LogError("Initializing XR Failed. Is the headset plugged in?");
        } else {
            Debug.Log("Starting XR...");
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            Debug.Log("VR successfully active.");
        }
    }
}