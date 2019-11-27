using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class LoadVideoToQuad : MonoBehaviour
{
    public Material PlayButtonImage;
    public TextMeshPro VideoTimerText;
    private VideoPlayer _videoPlayer;
    public void LoadVideoToQuadByFileName(string fileName = null)
    {
        if (fileName != null)
        {
            if (_videoPlayer == null)
            {
                _videoPlayer = GetComponent<VideoPlayer>();
            }
            _videoPlayer.enabled = true;
            _videoPlayer.url = Path.Combine(Application.persistentDataPath, "media", fileName);

            GetComponent<LoadImageToQuad>().LoadDefaultVideoImage(PlayButtonImage);
            GetComponent<Interactable>().enabled = true;
            GetComponent<VideoPlaybackController>().State = VideoPlayerState.NotStarted;

            VideoTimerText.text = "00:00/" + "mm:ss";
        }
        else
        {
        }
    }
}
