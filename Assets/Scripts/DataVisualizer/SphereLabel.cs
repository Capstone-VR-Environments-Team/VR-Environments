using UnityEngine;
using TMPro;

public class SphereLabel : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Transform _textPivot; // The Empty GameObject at (0,0,0)
    [SerializeField] private TextMeshPro _textLabel;

    [Header("Label Position")]
    [Min(0f)]
    [SerializeField] private float _outsideOffset = 0.01f;

    private MeshRenderer _renderer;

    void Awake() {
        _renderer = GetComponent<MeshRenderer>();
        ForceLabelCenter();
    }

    public void Initialize(int number) {
        if (_textLabel == null) return;

        ForceTextRectCenter();
        _textLabel.text = number.ToString();
        _textLabel.alignment = TextAlignmentOptions.Center;

        if (_renderer.material.HasProperty("_Color")) {
            _textLabel.color = GetContrastColor(_renderer.material.color);
        }
    }

    void LateUpdate() {
        Camera activeCamera = Camera.main;
        if (activeCamera == null || _textPivot == null) return;

        Vector3 center = transform.position;
        Vector3 toCamera = activeCamera.transform.position - center;
        if (toCamera.sqrMagnitude < 0.0001f) return;

        float sphereRadius = GetSphereRadius();
        float outsideOffset = Mathf.Min(_outsideOffset, sphereRadius * 0.1f);
        _textPivot.position = center + toCamera.normalized * (sphereRadius + outsideOffset);

        // TMP text reads correctly when the pivot forward points away from the camera.
        Vector3 fromCamera = _textPivot.position - activeCamera.transform.position;
        if (fromCamera.sqrMagnitude < 0.0001f) return;

        _textPivot.rotation = Quaternion.LookRotation(fromCamera, activeCamera.transform.up);

        if (_textLabel != null && _textLabel.transform.parent == _textPivot) {
            ForceTextRectCenter();
            _textLabel.transform.localPosition = Vector3.zero;
        }
    }

    private float GetSphereRadius() {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider != null) {
            Vector3 lossy = transform.lossyScale;
            float maxScale = Mathf.Max(lossy.x, Mathf.Max(lossy.y, lossy.z));
            return sphereCollider.radius * maxScale;
        }

        if (_renderer != null) {
            Vector3 extents = _renderer.bounds.extents;
            return Mathf.Max(extents.x, Mathf.Max(extents.y, extents.z));
        }

        return 0.5f * Mathf.Max(transform.lossyScale.x, Mathf.Max(transform.lossyScale.y, transform.lossyScale.z));
    }

    private void ForceLabelCenter() {
        if (_textPivot != null) {
            if (_textPivot.parent == transform) {
                _textPivot.localPosition = Vector3.zero;
            } else {
                _textPivot.position = transform.position;
            }
        }

        if (_textLabel != null) {
            ForceTextRectCenter();
            _textLabel.alignment = TextAlignmentOptions.Center;

            if (_textPivot != null && _textLabel.transform.parent == _textPivot) {
                _textLabel.transform.localPosition = Vector3.zero;
                _textLabel.transform.localRotation = Quaternion.identity;
            } else {
                _textLabel.transform.position = transform.position;
            }
        }
    }

    private void ForceTextRectCenter() {
        RectTransform textRect = _textLabel != null ? _textLabel.rectTransform : null;
        if (textRect == null) return;

        // Ensure the visual center of the text object sits at the sphere center.
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = Vector2.zero;
        textRect.anchoredPosition3D = Vector3.zero;
    }

    private Color GetContrastColor(Color color) {
        // Perceived brightness formula
        float brightness = (0.2126f * color.r) + (0.7152f * color.g) + (0.0722f * color.b);
        return brightness > 0.5f ? Color.black : Color.white;
    }
}