using Assets.AR_Instructions.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlaybackController : MonoBehaviour
{
    public VideoPlayer VideoPlayer;
    public VideoPlayerState State = VideoPlayerState.Unkown;
    public TextMeshPro VideoTimerText;

    public void Start()
    {
        VideoPlayer.loopPointReached += VideoPlayer_loopPointReached;
    }

    private void VideoPlayer_loopPointReached(VideoPlayer source)
    {
        State = VideoPlayerState.Stopped;
    }

    public void OnTapped()
    {
        switch (State)
        {
            case VideoPlayerState.Playing:
                //InstructionManager.Instance.ActionLog.Add(new ActionLogEntry(InstructionManager.Instance.CurrentStepNumber, ActionType.VideoPaused, DateTime.Now));
                VideoPlayer.Pause();
                State = VideoPlayerState.Paused;
                break;
            case VideoPlayerState.Paused:
            case VideoPlayerState.Stopped:
            case VideoPlayerState.NotStarted:
                //    if (State == VideoPlayerState.Paused) InstructionManager.Instance.ActionLog.Add(new ActionLogEntry(InstructionManager.Instance.CurrentStepNumber, ActionType.VideoUnpaused, DateTime.Now));
                //    if (State == VideoPlayerState.Stopped) InstructionManager.Instance.ActionLog.Add(new ActionLogEntry(InstructionManager.Instance.CurrentStepNumber, ActionType.VideoRestarted, DateTime.Now));
                //    if (State == VideoPlayerState.NotStarted) InstructionManager.Instance.ActionLog.Add(new ActionLogEntry(InstructionManager.Instance.CurrentStepNumber, ActionType.VideoStarted, DateTime.Now));

                VideoPlayer.Play();
                State = VideoPlayerState.Playing;
                break;
            case VideoPlayerState.Hidden:
                break;
            case VideoPlayerState.Unkown:
                break;
            default:
                break;
        }
    }


    public void Update()
    {
        if (State == VideoPlayerState.Playing)
        {
            //Get the video duration by frameCount and frameRate
            double time = VideoPlayer.frameCount / VideoPlayer.frameRate;
            if (time > 0)
            {
                var VideoUrlLength = TimeSpan.FromSeconds(time);
                var totalMinutes = VideoUrlLength.Minutes.ToString("00");
                var totalSeconds = VideoUrlLength.Seconds.ToString("00");

                var currentTime = TimeSpan.FromSeconds(VideoPlayer.time);
                var currentMinutes = currentTime.Minutes.ToString("00");
                var currentSeconds = currentTime.Seconds.ToString("00");

                VideoTimerText.text = currentMinutes + ":" + currentSeconds + "/" + totalMinutes + ":" + totalSeconds;
            }
        }
    }
}


public enum VideoPlayerState
{
    Playing = 0,
    Paused,
    Stopped,
    NotStarted,
    Hidden,
    Unkown
}
