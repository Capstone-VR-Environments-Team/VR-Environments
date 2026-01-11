using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {
    [Header("Movement Settings")]
    public float moveSpeed = 8f;

    [Header("Look Settings")]
    public float lookSensitivity = 25f;
    public float minPitch = -90f;
    public float maxPitch = 90f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 20f;

    private float yaw;
    private float pitch;

    [SerializeField]
    private bool _on;

    void Start() {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        _on = false;
    }

    void LateUpdate() {
        if (_on) {
            var mouse = Mouse.current;
            var keyboard = Keyboard.current;

            // Look
            if (mouse.rightButton.isPressed) {
                Vector2 delta = mouse.delta.ReadValue();
                yaw += delta.x * lookSensitivity * Time.deltaTime;
                pitch -= delta.y * lookSensitivity * Time.deltaTime;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

                transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
            }

            // Zoom
            float scroll = mouse.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.01f) {
                Vector3 direction = transform.forward;
                transform.position += scroll * Time.deltaTime * zoomSpeed * direction;
            }

            // Movement
            Vector3 inputDir = Vector3.zero;

            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) inputDir += transform.forward;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) inputDir -= transform.forward;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) inputDir -= transform.right;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) inputDir += transform.right;
            if (keyboard.spaceKey.isPressed || keyboard.leftShiftKey.isPressed) inputDir += transform.up;
            if (keyboard.leftCtrlKey.isPressed) inputDir -= transform.up;

            transform.position += moveSpeed * Time.deltaTime * inputDir.normalized;
        }
    }

    public void TurnOn() {
        _on = true;
    }

    public void TurnOff() {
        _on = false;
    }
}
