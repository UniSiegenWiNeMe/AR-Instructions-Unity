using Microsoft.MixedReality.Toolkit.UI;
using System;
using TMPro;
using UnityEngine;

public class EnterInstructionNameController : MonoBehaviour
{
    public event EventHandler Continue;

    // For System Keyboard
    public TouchScreenKeyboard keyboard;
    public string keyboardText = "";
    public TextMeshPro OutputTextMesh;

    private Interactable _continueButton;


    void Start()
    {
        _continueButton = GetComponentInChildren<Interactable>();

        OpenSystemKeyboard();

#if UNITY_EDITOR
        OutputTextMesh.text = "Name: EDITOR-" + DateTime.Now.ToString("yyyy.MM.dd hh-mm");
        keyboardText = "EDITOR-" + DateTime.Now.ToString("yyyy.MM.dd hh-mm");
        _continueButton.IsEnabled = true;
#endif
    }

#if UNITY_WSA && !UNITY_EDITOR
    private void Update()
    {
        if (keyboard != null)
        {
            keyboardText = keyboard.text;
            OutputTextMesh.text = "Name: " + keyboardText;
            
            if(keyboard.status == TouchScreenKeyboard.Status.Done || keyboard.status == TouchScreenKeyboard.Status.Canceled)
            {
                _continueButton.IsEnabled = true;
                keyboard = null;
            }
        }
    }
#endif

    public void OpenSystemKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }

    public void OnContinue()
    {
        InstructionManager.Instance.CreateNewInstruction(keyboardText, DateTime.Now);

        Continue?.Invoke(this, null);
    }

}
