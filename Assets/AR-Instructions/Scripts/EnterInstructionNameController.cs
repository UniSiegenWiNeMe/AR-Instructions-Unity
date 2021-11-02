using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using TMPro;
using UnityEngine;

public class EnterInstructionNameController : MonoBehaviour
{
    public event EventHandler Continue;

    
    public string InstructionName;
    public TextMeshPro OutputTextMesh;

    private Interactable _continueButton;


    void Start()
    {
        _continueButton = GetComponentInChildren<Interactable>();


#if UNITY_EDITOR
        OutputTextMesh.text = "Name: EDITOR-" + DateTime.Now.ToString("yyyy.MM.dd hh-mm");
        InstructionName = "EDITOR-" + DateTime.Now.ToString("yyyy.MM.dd hh-mm");
        _continueButton.IsEnabled = true;
#endif
    }


    public void SetInstructionName(string name)
    {
        InstructionName = name;
    }

    //public void OpenKeyboard()
    //{
    //    keyboard = new MixedRealityKeyboard();
    //    keyboard.ShowKeyboard();
    //    keyboard.OnCommitText.AddListener(() => { _continueButton.IsEnabled = true; });
    //    keyboard.OnHideKeyboard.AddListener(() => { _continueButton.IsEnabled = true; keyboard = null; });
    //}

    public void OnContinue()
    {
        InstructionManager.Instance.CreateNewInstruction(InstructionName, DateTime.Now);

        Continue?.Invoke(this, null);
    }

}