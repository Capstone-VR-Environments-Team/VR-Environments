using UnityEngine;
using UnityEngine.Video;

public class SkyboxContentManager : MonoBehaviour
{
    [Header("Components")]
    public VideoPlayer videoPlayer;
    public Material skyboxMaterial;
    public RenderTexture videoRenderTexture;
    public Canvas infoCanvas;

    [Header("Settings")]
    public string texturePropertyName = "_MainTex";
    public VideoClip testVideo;
    public Texture2D testImage;
    public Texture2D loadingImage;

    private void Start()
    {
        RenderSettings.skybox = skyboxMaterial;
        Load();
    }

    public void Load()
    {
        ShowImage(loadingImage);
        infoCanvas.enabled = true;
    }

    public void Image()
    {
        ShowImage(testImage);
    }

    public void Video()
    {
        ShowVideo(testVideo);
    }

    public void ShowVideo(VideoClip newClip)
    {
        infoCanvas.enabled = false; ;
        skyboxMaterial.SetTexture(texturePropertyName, videoRenderTexture);

        videoPlayer.clip = newClip;
        videoPlayer.Stop();
        videoPlayer.Prepare();
        videoPlayer.Play();
    }

    public void ShowImage(Texture2D newImage)
    {
        infoCanvas.enabled = false;
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }

        skyboxMaterial.SetTexture(texturePropertyName, newImage);
    }
}