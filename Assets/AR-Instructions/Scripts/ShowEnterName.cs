using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowEnterName : MonoBehaviour
{
    public GameObject EnterNamePrefab;


    public void OnShowEnterName()
    {
        var enterName = Instantiate(EnterNamePrefab);
        enterName.transform.position = transform.position;

        var keyboard = enterName.GetComponent<OpenKeyboardOnStart>();


#if UNITY_EDITOR
        keyboard.OutputTextMesh.text = "Name: EDITOR-" + DateTime.Now.ToString("yyyy.MM.dd hh-mm");
        OpenKeyboardOnStart.keyboardText = "EDITOR-" + DateTime.Now.ToString("yyyy.MM.dd hh-mm");
#endif

    }
}
