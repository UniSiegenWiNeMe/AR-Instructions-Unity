using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootMenuController : MonoBehaviour
{
    public Interactable MarkerScanInteractable;
    public Interactable HomeInteractable;

    public void Start()
    {
        if(MarkerScanInteractable == null || HomeInteractable == null)
        {
            throw new NullReferenceException("Missing Reference in FootMenuController.");
        }
    }
}
