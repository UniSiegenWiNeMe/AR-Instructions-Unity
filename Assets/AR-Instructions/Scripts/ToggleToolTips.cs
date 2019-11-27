using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleToolTips : MonoBehaviour
{
    private bool _isActive = false;

    private List<GameObject> _toolTips = new List<GameObject>();
    private List<SpawnNewItem> _spawnNewItemns = new List<SpawnNewItem>();

    public void Start()
    {
        var tooltips =  GetComponentsInChildren<ToolTip>();
        foreach (var tooltip in tooltips)
        {
            _toolTips.Add(tooltip.gameObject);
            tooltip.gameObject.SetActive(false);
        }


        var spawnNewItems = GetComponentsInChildren<SpawnNewItem>();

        foreach (var spawnNewItem in spawnNewItems)
        {
            _spawnNewItemns.Add(spawnNewItem);
        }
    }

    public void SetActive()
    {
        _isActive = true;
        foreach (var tooltip in _toolTips)
        {
            tooltip.SetActive(_isActive);
        }

        foreach (var spawnNewItem in _spawnNewItemns)
        {
            spawnNewItem.isToolTipActive = _isActive;
        }
    }

    public void SetInactive()
    {
        _isActive = false;
        foreach (var tooltip in _toolTips)
        {
            tooltip.SetActive(_isActive);
        }

        foreach (var spawnNewItem in _spawnNewItemns)
        {
            spawnNewItem.isToolTipActive = _isActive;
        }
    }

    public void ToogleActive()
    {
        _isActive = !_isActive;
        foreach (var tooltip in _toolTips)
        {
            tooltip.SetActive(_isActive);
        }

        foreach (var spawnNewItem in _spawnNewItemns)
        {
            spawnNewItem.isToolTipActive = _isActive;
        }
    }
}
