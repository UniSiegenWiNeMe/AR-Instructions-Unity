//using Microsoft.MixedReality.Toolkit.Examples.Demos;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpenKeyboardOnStart : MonoBehaviour
{
    // For System Keyboard
    public TouchScreenKeyboard keyboard;
    public static string keyboardText = "";
    public TextMeshPro OutputTextMesh;

    private void Start()
    {
        OpenSystemKeyboard();
    }

#if UNITY_WSA && !UNITY_EDITOR
    private void Update()
    {
        if (keyboard != null)
        {
            keyboardText = keyboard.text;
            if (TouchScreenKeyboard.visible)
            {
                OutputTextMesh.text = "Name: " + keyboardText;
            }
            else
            {
                OutputTextMesh.text = "Name: " + keyboardText;
                keyboard = null;
            }
        }
    }
#endif

    public void OpenSystemKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }
}
