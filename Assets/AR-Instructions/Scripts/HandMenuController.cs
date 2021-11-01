using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMenuController : MonoBehaviour
{
    public Interactable MarkerScanInteractable;
    public Interactable HomeInteractable;
    public Interactable OffsetInteractable;

    public void Start()
    {
        if (MarkerScanInteractable == null || HomeInteractable == null || OffsetInteractable == null)
        {
            throw new NullReferenceException("Missing Reference in FootMenuController.");
        }
    }

    public void ChangeMode(MenuMode mode)
    {
        OffsetInteractable.gameObject.SetActive(mode == MenuMode.Record ? true : false);
    }
}
