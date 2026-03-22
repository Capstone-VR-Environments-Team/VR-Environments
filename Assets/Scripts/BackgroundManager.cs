using UnityEngine;
using UnityEngine.Video;
using System.IO;

public class BackgroundManager : MonoBehaviour
{
    [Header("References")]
    public Material skyboxMaterial;

    private VideoPlayer videoPlayer;

    public void UpdateBackground()
    {
        string backgroundType = SessionManager.Instance.GetBackgroundType();
        if (backgroundType == "Image")
        {
            UpdateBackgroundFromImage(SessionManager.Instance.GetBackgroundImagePath());
        } else if (backgroundType == "Video")
        {
            videoPlayer = new VideoPlayer();
            UpdateBackgroundFromVideo(SessionManager.Instance.GetBackgroundVideoPath());
        }
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
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = filePath;
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        videoPlayer.isLooping = true;
        videoPlayer.Play();

        RenderSettings.skybox = skyboxMaterial;
    }

    private void ResetSkybox()
    {
        if (videoPlayer != null && videoPlayer.isPlaying) videoPlayer.Stop();
        if (skyboxMaterial != null) skyboxMaterial.SetTexture("_MainTex", null);
    }
}