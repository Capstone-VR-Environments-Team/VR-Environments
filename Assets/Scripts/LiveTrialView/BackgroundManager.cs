using UnityEngine;
using UnityEngine.Video;
using System.IO;

public class BackgroundManager : MonoBehaviour
{
    [Header("References")]
    public Material skyboxMaterial;
    public Material defaultMaterial;
    private VideoPlayer videoPlayer;
    [SerializeField] private Canvas participantCanvas;

    private void Start()
    {
        UpdateBackground();
        participantCanvas.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventBus.LastSphere += ClearBackground;
    }

    private void OnDisable()
    {
        EventBus.LastSphere -= ClearBackground;
    }

    public void UpdateBackground()
    {
        Debug.Log("Background Updating");
        string backgroundType = SessionManager.Instance.GetBackgroundType();
        if (backgroundType == "Image") {
            UpdateBackgroundFromImage(SessionManager.Instance.GetBackgroundFilePath());
        } else if (backgroundType == "Video") {
            UpdateBackgroundFromVideo(SessionManager.Instance.GetBackgroundFilePath());
        } else {
            RenderSettings.skybox = defaultMaterial;
        }
    }

    private void ClearBackground()
    {
        RenderSettings.skybox = defaultMaterial;
        participantCanvas.gameObject.SetActive(true);
    }

    private void UpdateBackgroundFromImage(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("Image file not found at: " + filePath);
            return;
        }

        ResetSkybox();
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D newTexture = new Texture2D(2, 2);

        if (newTexture.LoadImage(fileData))
        {
            skyboxMaterial.SetTexture("_MainTex", newTexture);
            RenderSettings.skybox = skyboxMaterial;
        }
    }

    private void UpdateBackgroundFromVideo(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("Video file not found at: " + filePath);
            return;
        }

        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null) videoPlayer = gameObject.AddComponent<VideoPlayer>();

        ResetSkybox();

        string formattedPath = filePath.Replace("\\", "/");

        videoPlayer.url = formattedPath;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.isLooping = true;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;

        RenderTexture videoRenderTexture = new RenderTexture(4096, 2048, 0);
        videoRenderTexture.format = RenderTextureFormat.ARGB32;

        videoPlayer.targetTexture = videoRenderTexture;
        skyboxMaterial.mainTexture = videoRenderTexture;

        videoPlayer.Play();
        RenderSettings.skybox = skyboxMaterial;
    }

    private void ResetSkybox()
    {
        if (videoPlayer != null && videoPlayer.isPlaying) videoPlayer.Stop();
        if (skyboxMaterial != null) skyboxMaterial.SetTexture("_MainTex", null);
    }
}