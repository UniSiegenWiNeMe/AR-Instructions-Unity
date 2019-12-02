using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PhotoVideoPanelController : MonoBehaviour
{

    public GameObject PreviousMediaFileButton;
    public GameObject NextMediaFileButton;

    public LoadImageToQuad ImageLoader;
    public LoadVideoToQuad VideoLoader;

    public TextMeshPro MediaFileCounterText;
    public TextMeshPro VideoTimerText;
    
    public UnityEvent OnNewData;
    public GameObject TakePhotoButton;
    public GameObject TakeVideoButton;
    public GameObject DeleteButton;

    private int _currentMediaIndex;
    private List<MediaFile> _mediaFiles;

    public void Reset(List<MediaFile> newMediaFiles)
    {
        _mediaFiles = newMediaFiles;
        PreviousMediaFileButton.gameObject.SetActive(false);
        if (_mediaFiles.Count > 0)
        {
            _currentMediaIndex = 0;
            var mediaFile = _mediaFiles[0];

            LoadMediaFile(mediaFile);

            //turn next button only on when there is more than 1 mediafile
            NextMediaFileButton.gameObject.SetActive(_mediaFiles.Count > 1 ? true : false);
            SetMediaFileCounter(_currentMediaIndex + 1, _mediaFiles.Count);
        }
        else
        {
            _currentMediaIndex = 0;
            ImageLoader.LoadImageToQuadByFileName();
            SetMediaFileCounter(0,0);
            VideoTimerText.text = string.Empty;
            PreviousMediaFileButton.gameObject.SetActive(false);
            NextMediaFileButton.gameObject.SetActive(false);
        }
        DeleteButton.SetActive(false);
    }

    
    public void SetMode(MenuMode mode)
    {
        if (mode == MenuMode.Replay)
        {
            TakePhotoButton.SetActive(false);
            TakeVideoButton.SetActive(false);
            
        }
    }

    public void LoadNextMediaFile()
    {
        if (_mediaFiles != null)
        {
            if (_mediaFiles.Count > 1 && _currentMediaIndex + 1 < _mediaFiles.Count)
            {
                _currentMediaIndex++;

                var mediaFile = _mediaFiles[_currentMediaIndex];
                LoadMediaFile(mediaFile);

                PreviousMediaFileButton.gameObject.SetActive(true);

                if (_currentMediaIndex + 1 >= _mediaFiles.Count)
                {
                    NextMediaFileButton.gameObject.SetActive(false);
                }

                SetMediaFileCounter(_currentMediaIndex + 1, _mediaFiles.Count);
                
            }
        }
    }


    private void LoadMediaFile(MediaFile mediaFile)
    {
        if (mediaFile.Type == MediaType.Image)
        {
            ImageLoader.LoadImageToQuadByFileName(mediaFile.FileName);
            VideoTimerText.text = string.Empty;
        }
        else
        {
            VideoLoader.LoadVideoToQuadByFileName(mediaFile.FileName);
        }
    }

    public void LoadPreivousMediaFile()
    {
        if (_currentMediaIndex - 1 >= 0)
        {
            _currentMediaIndex--;
            var mediaFile = _mediaFiles[_currentMediaIndex];
            LoadMediaFile(mediaFile);

            if (_currentMediaIndex <= 0)
            {
                PreviousMediaFileButton.gameObject.SetActive(false);
            }

            if (_currentMediaIndex + 1 < _mediaFiles.Count)
            {
                NextMediaFileButton.gameObject.SetActive(true);
            }
            SetMediaFileCounter(_currentMediaIndex + 1,  _mediaFiles.Count);
        }
    }

    public void TookPhoto(string fileName)
    {
        if (fileName != "Error")
        {
            _mediaFiles.Add(new MediaFile(fileName, MediaType.Image));
            SetUIForLatestMediaFile();
            DeleteButton.SetActive(true);
            NewData();
        }
    }

    public void TookVideo(string fileName)
    {
        if (fileName != "Error")
        {
            _mediaFiles.Add(new MediaFile(fileName, MediaType.Video));
            SetUIForLatestMediaFile();
            DeleteButton.SetActive(true);
            NewData();
        }
    }

    private void SetUIForLatestMediaFile()
    {
        _currentMediaIndex = _mediaFiles.Count - 1;
        SetMediaFileCounter(_mediaFiles.Count, _mediaFiles.Count);
        if (_currentMediaIndex >= 1)
        {
            PreviousMediaFileButton.gameObject.SetActive(true);
        }
        NextMediaFileButton.gameObject.SetActive(false);
    }

    private void NewData()
    {
        OnNewData?.Invoke();
        InstructionManager.Instance.Save();
    }

    private void SetMediaFileCounter(int currentMediaIndex, int count)
    {
        MediaFileCounterText.text = currentMediaIndex + "/" + count;
    }

    public void OnDeleteCurrentFile()
    {
        _mediaFiles.RemoveAt(_currentMediaIndex);

        if(_currentMediaIndex == 0)
        {
            if(_mediaFiles.Count > 0)
            {

                LoadMediaFile(_mediaFiles[_currentMediaIndex]);

                if (_mediaFiles.Count > 1)
                {
                    NextMediaFileButton.gameObject.SetActive(true);
                }
                else
                {
                    NextMediaFileButton.gameObject.SetActive(false);
                }
                SetMediaFileCounter(_currentMediaIndex+1, _mediaFiles.Count);
            }
            else
            {
                ImageLoader.LoadImageToQuadByFileName();
                PreviousMediaFileButton.gameObject.SetActive(false);
                NextMediaFileButton.gameObject.SetActive(false);
                DeleteButton.SetActive(false);
                SetMediaFileCounter(_currentMediaIndex, _mediaFiles.Count);
            }
        }
        else
        {
            LoadPreivousMediaFile();
        }
        OnNewData?.Invoke();
        InstructionManager.Instance.Save();

        //if (_currentMediaIndex > 0)
        //{
        //    _currentMediaIndex--;
        //}
    }
}
