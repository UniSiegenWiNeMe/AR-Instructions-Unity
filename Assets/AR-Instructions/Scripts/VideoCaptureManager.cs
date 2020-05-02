using UnityEngine;
using System.Collections;
using System.Linq;

using System.IO;
using System;
using TMPro;
using UnityEngine.Events;

using UnityEngine.XR.WSA.WebCam;


public class VideoCaptureManager : MonoBehaviour
{
    public float MaxRecordingTime = 120.0f;
    public string MediaPath = "media";
    public GameObject CameraBorder;
    public string CameraInstructionTextStart = "Air tap to start recording.";
    public string CameraInstructionTextStop = "Air tap to stop recording.";

    public VideoFinishedEvent OnVideoFinished;
    public UnityEvent OnVideoRecordingStarted;


    private VideoCapture _videoCapture = null;
    private Resolution _cameraResolution;
    private float _cameraFramerate;
    private DateTime _startRecordingTime = DateTime.MaxValue;
    private float m_stopRecordingTimer = float.MaxValue;

    private string _mediaPath;
    private string _fileNameFullPath;
    private bool _takingVideo = false;

    private GameObject _cameraBorder;
    private CameraBorderController _cameraBorderController;

    private string _fileName;
    

    void Start()
    {
        //_cameraResolution = VideoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        //_cameraFramerate = VideoCapture.GetSupportedFrameRatesForResolution(_cameraResolution).OrderByDescending((fps) => fps).First();

        // full resolution is lagging
        _cameraResolution = new Resolution() { width = 896, height = 504 };
        try
        {
            _cameraFramerate = VideoCapture.GetSupportedFrameRatesForResolution(_cameraResolution).OrderByDescending((fps) => fps).First();
        }
        catch (Exception)
        {
            _cameraFramerate = 30f;
        }

        _mediaPath = Path.Combine(Application.persistentDataPath, MediaPath);


        var mainCameraGameObject = GameObject.Find("Main Camera");

        _cameraBorder = mainCameraGameObject.transform.Find(CameraBorder.name + "(Clone)")?.gameObject;
        if (_cameraBorder == null)
        {
            _cameraBorder = Instantiate(CameraBorder, mainCameraGameObject.transform);
        }

        _cameraBorderController = _cameraBorder.GetComponent<CameraBorderController>();
        _cameraBorderController.Hide();
    }

    void Update()
    {
        if (_videoCapture != null)
        {
            if (_videoCapture.IsRecording)
            {
                _cameraBorderController.Timer.text = (DateTime.Now - _startRecordingTime).ToString(@"m\:ss");

                if (Time.time > m_stopRecordingTimer)
                {
                    _videoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
                }
            }
        }
    }

    public void Prepare()
    {
        if (!_takingVideo)
        {
            _takingVideo = true;
            _cameraBorderController.Instruction.text = CameraInstructionTextStart;
            _cameraBorderController.Timer.text = string.Empty;
            _cameraBorderController.SetIdleMaterial();
            _cameraBorderController.Show();
        }
        else
        {
            Debug.Log("Video recording already running");
        }
    }

    public void OnTapped()
    {
        if (_videoCapture != null)
        {
            if (_videoCapture.IsRecording && (DateTime.Now - _startRecordingTime) > new TimeSpan(0,0,3))
            {
                StopRecording();
            }
        }
        else
        {
            StartVideoCapture();
        }
    }

    public void StopRecording()
    {
        if (_videoCapture != null)
        {
            try
            {
                _videoCapture.StopRecordingAsync(OnStoppedRecordingVideo);

            }
            catch (Exception)
            {
                Debug.Log("Error on stopping recording");
            }
        }
    }

    private void StartVideoCapture()
    {
        VideoCapture.CreateAsync(false, OnVideoCaptureCreated);
    }

    private void OnVideoCaptureCreated(VideoCapture videoCapture)
    {
        if (videoCapture != null)
        {
            _videoCapture = videoCapture;
            CameraParameters cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.frameRate = _cameraFramerate;
            cameraParameters.cameraResolutionWidth = _cameraResolution.width;
            cameraParameters.cameraResolutionHeight = _cameraResolution.height;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

            _cameraBorderController.SetRecordingMaterial();
            _cameraBorderController.Instruction.text = CameraInstructionTextStop;
            _startRecordingTime = DateTime.Now;

            if (!Directory.Exists(_mediaPath))
            {
                Directory.CreateDirectory(_mediaPath);
            }


            _videoCapture.StartVideoModeAsync(cameraParameters,
                                               VideoCapture.AudioState.ApplicationAndMicAudio,
                                               OnStartedVideoCaptureMode);
        }
        else
        {
            Debug.LogError("Failed to create VideoCapture Instance!");
            OnVideoFinished?.Invoke("Error");
        }
    }

    void OnStartedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
    {
        if (result.success)
        {
            _fileName = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".mp4";
            _fileNameFullPath = Path.Combine(_mediaPath, _fileName);
            _videoCapture.StartRecordingAsync(_fileNameFullPath, OnStartedRecordingVideo);
        }
        else
        {
            OnVideoFinished?.Invoke("Error");
        }
    }

    void OnStoppedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
    {
        _takingVideo = false;
        _videoCapture = null;
        _cameraBorderController.Hide();
        _startRecordingTime = DateTime.MaxValue;

        OnVideoFinished?.Invoke(_fileName);
    }

    void OnStartedRecordingVideo(VideoCapture.VideoCaptureResult result)
    {
        m_stopRecordingTimer = Time.time + MaxRecordingTime;
        OnVideoRecordingStarted?.Invoke();
    }

    void OnStoppedRecordingVideo(VideoCapture.VideoCaptureResult result)
    {
        _videoCapture.StopVideoModeAsync(OnStoppedVideoCaptureMode);
    }
}


[System.Serializable]
public class VideoFinishedEvent : UnityEvent<string>
{
}