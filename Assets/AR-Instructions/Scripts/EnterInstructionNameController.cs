using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using TMPro;
using UnityEngine;

public class EnterInstructionNameController : MonoBehaviour
{
    public event EventHandler Continue;

    
    public MixedRealityKeyboard keyboard;
    public string keyboardText = "";
    public TextMeshPro OutputTextMesh;

    private Interactable _continueButton;


    void Start()
    {
        _continueButton = GetComponentInChildren<Interactable>();

        OpenKeyboard();

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
            
        //    if(keyboard.status == TouchScreenKeyboard.Status.Done || keyboard.status == TouchScreenKeyboard.Status.Canceled)
        //    {
        //        _continueButton.IsEnabled = true;
        //        keyboard = null;
        //    }
        }
    }
#endif

    public void OpenKeyboard()
    {
        keyboard = new MixedRealityKeyboard();
        keyboard.ShowKeyboard();
        keyboard.OnCommitText.AddListener(() => { _continueButton.IsEnabled = true; });
        keyboard.OnHideKeyboard.AddListener(() => { _continueButton.IsEnabled = true; });
    }

    public void OnContinue()
    {
        InstructionManager.Instance.CreateNewInstruction(keyboardText, DateTime.Now);

        Continue?.Invoke(this, null);
    }

}