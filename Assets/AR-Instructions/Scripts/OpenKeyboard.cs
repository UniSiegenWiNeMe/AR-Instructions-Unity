using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class OpenKeyboard : MonoBehaviour
{
    // For System Keyboard
    public TouchScreenKeyboard keyboard;
    public static string keyboardText = "";
    public TextMeshPro TextOutPut;

    public TextTypedEvent TextTyped;

#if UNITY_WSA
    private void Update()
    {
        if (keyboard != null)
        {
            keyboardText = keyboard.text;
            if (!TouchScreenKeyboard.visible)
            {
                TextTyped.Invoke(keyboardText);
                keyboard = null;
            }
            else
            {
                if (TextOutPut != null)
                {
                    TextOutPut.text = "Anweisung:" + Environment.NewLine + keyboardText;
                }
            }
        }
    }
#endif

    public void OpenSystemKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }
}

[System.Serializable]
public class TextTypedEvent : UnityEvent<string>
{
}