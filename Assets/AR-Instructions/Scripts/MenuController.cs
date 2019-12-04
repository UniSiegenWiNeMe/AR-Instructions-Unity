using System;
using UnityEngine;

public enum MenuMode
{
    Record,
    Replay
    //Edit
}


public class MenuController : MonoBehaviour
{
    public GameObject ItemPanel;
    public GameObject PhotoPanel;
    public GameObject MainPanel;
    public GameObject MarkerPanel;
    public GameObject[] GameObjectsToHide;

    private MenuMode _mode;
    private InstructionManager _instructionManager;
    private GameObject _containerForSpawnedItems;
    private bool _visbility;

    public void Init(MenuMode mode, GameObject containerForSpawnedItems)
    {
        _mode = mode;
        _containerForSpawnedItems = containerForSpawnedItems;
        _instructionManager = InstructionManager.Instance;

        var vumark = GameObject.Find("VuMark");

#if UNITY_EDITOR
        SetMode(_mode);
        return;
#endif
        if (vumark)
        {
            var stabilizedTracking = vumark.GetComponentInChildren<StabilizedTracking>();

            if (stabilizedTracking)
            {
                if (stabilizedTracking.IsMarkerScanned)
                {
                    SetMode(_mode);
                }
                else
                {
                    stabilizedTracking.MarkerScanned += StabilizedTracking_MarkerScanned;
                }
            }
        }
    }

    public void Init(MenuMode mode, GameObject containerForSpawnedItems, string instructionName = null)
    {
        _mode = mode;
        _containerForSpawnedItems = containerForSpawnedItems;
        _instructionManager = InstructionManager.Instance;

        var vumark = GameObject.Find("VuMark");

#if UNITY_EDITOR
        SetMode(_mode);
        return;
#endif
        if (vumark)
        {
            var stabilizedTracking = vumark.GetComponentInChildren<StabilizedTracking>();
            
            if(stabilizedTracking)
            {
                if(stabilizedTracking.IsMarkerScanned)
                {
                    SetMode(_mode);
                }
                else
                {
                    stabilizedTracking.MarkerScanned += StabilizedTracking_MarkerScanned;
                }
            }
        }
    }

    private void StabilizedTracking_MarkerScanned(object sender, MarkerScannedEventArgs e)
    {
        SetMode(_mode);
    }

    public void SetMode(MenuMode mode)
    {
        MarkerPanel.SetActive(false);
        PhotoPanel.SetActive(true);
        PhotoPanel.GetComponent<PhotoVideoPanelController>().SetMode(mode);

        MainPanel.SetActive(true);
        MainPanel.GetComponent<MainPanelController>().Init(mode, _containerForSpawnedItems);

        if (mode == MenuMode.Record) //|| _mode == MenuMode.Edit
        {
            ItemPanel.SetActive(true);
            ItemPanel.GetComponent<ItemPanelController>().ContainerForSpawnedItems = _containerForSpawnedItems;
            PhotoPanel.GetComponent<PhotoVideoPanelController>().Reset(_instructionManager.GetCurrentMediaFiles());
        }
        else
        {
            PhotoPanel.GetComponent<PhotoVideoPanelController>().Reset(_instructionManager.GetCurrentMediaFiles());
        }

    }

    public void Reset()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => { 
            MainPanel.GetComponent<MainPanelController>().Reset();
            PhotoPanel.GetComponent<PhotoVideoPanelController>().Reset(_instructionManager.GetCurrentMediaFiles());
        });
    }

    public void ToggleVisibility()
    {
        _visbility = !_visbility;

        foreach (var _gameObject in GameObjectsToHide)
        {
            _gameObject.SetActive(_visbility);
        }
    }
    public void Hide()
    {
        _visbility = false;

        foreach (var _gameObject in GameObjectsToHide)
        {
            _gameObject.SetActive(_visbility);
        }
    }
    public void Show()
    {
        _visbility = true;

        foreach (var _gameObject in GameObjectsToHide)
        {
            _gameObject.SetActive(_visbility);
        }
    }
}
