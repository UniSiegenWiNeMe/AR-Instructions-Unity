using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleVisibilityScript : MonoBehaviour
{
    bool visbility = true;

    public GameObject[] GameObjectsToHide;

    public void ToggleVisibility()
    {
        visbility = !visbility;

        foreach (var _gameObject in GameObjectsToHide)
        {
            _gameObject.SetActive(visbility);
        }
    }
}
