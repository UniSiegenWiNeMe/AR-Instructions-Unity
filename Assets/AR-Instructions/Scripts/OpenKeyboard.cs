using Microsoft.MixedReality.Toolkit.Experimental.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class OpenKeyboard : MonoBehaviour
{
    // For System Keyboard
    public MixedRealityKeyboard keyboard;
    public static string keyboardText = "";
    public TextMeshPro TextOutPut;

    public TextTypedEvent TextTyped;

#if UNITY_WSA
    private void Update()
    {
        if (keyboard != null)
        {
            keyboardText = keyboard.Text;
        }

        //if (keyboard != null)
        //{
        //    keyboardText = keyboard.Text;
        //    if (!keyboard.Visible)
        //    {
        //        TextTyped.Invoke(keyboardText);
        //        keyboard = null;
        //    }
        //    else
        //    {
        //        if (TextOutPut != null)
        //        {
        //            TextOutPut.text = "Anweisung:" + Environment.NewLine + keyboardText;
        //        }
        //    }
        //}
    }
#endif

    public void OpenSystemKeyboard()
    {
        keyboard = new MixedRealityKeyboard();
        keyboard.OnCommitText.AddListener(() => { TextTyped.Invoke(keyboardText); });
        keyboard.OnHideKeyboard.AddListener(() => { TextOutPut.text = "Anweisung:" + Environment.NewLine + keyboardText; });
    }
}

[System.Serializable]
public class TextTypedEvent : UnityEvent<string>
{
}