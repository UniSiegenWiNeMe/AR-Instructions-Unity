using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.XR.WSA.WebCam;
using System.IO;
using System;
using System.Threading.Tasks;
using UnityEngine.Events;
using TMPro;

public class PhotoCaptureManager: MonoBehaviour
{
    public string MediaPath = "media";
    public GameObject CameraBorder;
    public string CameraText = "Air tap to take a photo.";
    public PhotoFinishedEvent OnPhotoFinished;

    private PhotoCapture _photoCaptureObject = null;
    private Resolution _cameraResolution;
    private bool _takingPicture = false;
    private string _fullPathFileName;
    private string _mediaPath;
    private GameObject _cameraBorder;
    private TextMeshProUGUI _cameraBorderText;
    private CameraBorderController _cameraBorderController;
    private string _fileName;

    

    void Start()
    {
        _cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        _mediaPath = Path.Combine(Application.persistentDataPath, MediaPath);

        var mainCameraGameObject = GameObject.Find("Main Camera");

        _cameraBorder = mainCameraGameObject.transform.Find(CameraBorder.name + "(Clone)")?.gameObject;
        if (_cameraBorder == null)
        {
            _cameraBorder = Instantiate(CameraBorder, mainCameraGameObject.transform);
        }

        _cameraBorderText = _cameraBorder.GetComponentInChildren<TextMeshProUGUI>();
        _cameraBorderController = _cameraBorder.GetComponent<CameraBorderController>();
        _cameraBorderController.Hide();

    }
    public void Prepare()
    {
        if(!_takingPicture)
        {
            _takingPicture = true;
            _cameraBorderText.text = CameraText;
            _cameraBorderController.SetIdleMaterial();
            _cameraBorderController.Timer.text = string.Empty;
            _cameraBorderController.Show();

        }

    }

    public void TakePicture()
    {
        // Create a PhotoCapture object
        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject)
        {
            if (captureObject != null)
            {
                _photoCaptureObject = captureObject;
                CameraParameters cameraParameters = new CameraParameters();
                cameraParameters.hologramOpacity = 0.0f;
                cameraParameters.cameraResolutionWidth = _cameraResolution.width;
                cameraParameters.cameraResolutionHeight = _cameraResolution.height;
                cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

                if (!Directory.Exists(_mediaPath))
                {
                    Directory.CreateDirectory(_mediaPath);
                }

                _fileName = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".jpg";
                _fullPathFileName = Path.Combine(_mediaPath, _fileName);

                _cameraBorderController.SetRecordingMaterial();
                // Activate the camera
                _photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result)
                {
                // Take a picture
                _photoCaptureObject.TakePhotoAsync(_fullPathFileName, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
                });
            }
            else
            {
                Debug.LogError("Failed to create PhotoCapture Instance!");
                OnPhotoFinished?.Invoke("Error");
            }
        });
    }

    void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            _photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
        }
        else
        {
            Debug.Log("Failed to save Photo to disk " + result.hResult + " " + result.resultType.ToString());
            OnPhotoFinished?.Invoke("Error");
        }
    }

    
    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Shutdown the photo capture resource
        _photoCaptureObject.Dispose();
        _photoCaptureObject = null;

        _takingPicture = false;
        _cameraBorderController.Hide();

        OnPhotoFinished?.Invoke(_fileName);
    }

}


[System.Serializable]
public class PhotoFinishedEvent : UnityEvent<string>
{
}