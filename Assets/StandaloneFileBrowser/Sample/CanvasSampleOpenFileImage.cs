using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;
using UnityEngine.Networking;

[RequireComponent(typeof(Button))]
public class CanvasSampleOpenFileImage : MonoBehaviour, IPointerDownHandler {
    public RawImage output;

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnPointerDown(PointerEventData eventData) {
        UploadFile(gameObject.name, "OnFileUpload", ".png, .jpg", false);
    }

    // Called from browser
    public void OnFileUpload(string url) {
        StartCoroutine(OutputRoutine(url));
    }
#else
    //
    // Standalone platforms & editor
    //
    public void OnPointerDown(PointerEventData eventData) { }

    void Start() {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick() {
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", ".png", false);
        if (paths.Length > 0) {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
    }
#endif

    private IEnumerator OutputRoutine(string path) {
        // Local files require the file:/// prefix
        string url = "file:///" + path;

        // Using statement ensures the web request is properly disposed of to prevent memory leaks
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url)) {
            // Wait for the file to load
            yield return uwr.SendWebRequest();

            // Check for errors
            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError($"Error loading image: {uwr.error}");
            } else {
                // Extract the texture from the downloaded data
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

                // Assign it to your UI RawImage (check the exact variable name in your script, it's usually 'output' or 'image')
                output.texture = texture;
            }
        }
    }
}