using UnityEngine;
using UnityEngine.Video;
using System.IO;

[RequireComponent(typeof(VideoPlayer))]
public class GameOverVideoPlayer : MonoBehaviour
{
    [Header("WebGL Settings")]
    [SerializeField, Tooltip("Exact name of the file in StreamingAssets (e.g. GameOver.mp4)")]
    private string _webGLVideoFileName = "GameOver.mp4";

    [Header("Desktop Settings")]
    [SerializeField, Tooltip("The video clip asset for PC/Mac builds")]
    private VideoClip _desktopVideoClip;

    private VideoPlayer _videoPlayer;

    private void Awake()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
    }

    private void Start()
    {
        PlayVideo();
    }

    public void PlayVideo()
    {

#if UNITY_WEBGL && !UNITY_EDITOR
            // --- WEBGL EXECUTION ---
            // Streams the raw file from the server to save RAM
            
            string videoPath = Path.Combine(Application.streamingAssetsPath, _webGLVideoFileName);
            _videoPlayer.source = VideoSource.Url;
            _videoPlayer.url = videoPath;
            _videoPlayer.Prepare();
            
            _videoPlayer.prepareCompleted += (source) => 
            {
                _videoPlayer.Play();
            };
            
#else
        // --- DESKTOP & EDITOR EXECUTION ---

        if (_desktopVideoClip != null)
        {
            _videoPlayer.source = VideoSource.VideoClip;
            _videoPlayer.clip = _desktopVideoClip;
            _videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("[CrossPlatformVideoPlayer] Desktop Video Clip is missing in the Inspector!");
        }
#endif
    }
}