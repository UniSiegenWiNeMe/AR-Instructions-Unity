using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class MenuItemController : MonoBehaviour
{
    public TextMeshPro label;
    public int NumberOfCharsToShow = 27;
    public GameObject RemoveButton;
    public TextMeshPro Label;
    public bool EditMode = false;
    public GameObject InstructionMenuPrefab;

    public event EventHandler<InstructionSelectionEventArgs> InstructionSelected;
    public event EventHandler<InstructionSelectionEventArgs> InstructionRemoved;


    private Instruction _instruction;
    private GameObject ContainerForSpawnedItems;


    public void SetInstruction(Instruction instruction)
    {
        _instruction = instruction;
        string itemText = _instruction.Name;
        if (name.Length > NumberOfCharsToShow)
        {
            itemText = itemText.Substring(0, NumberOfCharsToShow).TrimEnd('.') + "...";
        }

        if (label)
        {
            label.text = itemText;
        }
    }
    public void SetMode(bool editMode)
    {
        RemoveButton.SetActive(editMode);
        Label.text = editMode ? "Bearbeiten" : "Auswählen";
        EditMode = editMode;
    }


    public void Selected()
    {
        InstructionManager.Instance.Instruction = _instruction;

        InstructionSelected?.Invoke(this, new InstructionSelectionEventArgs(_instruction));
    }

    public void Removed()
    {
        InstructionManager.Instance.Remove(_instruction.Name);

        InstructionRemoved?.Invoke(this, new InstructionSelectionEventArgs(_instruction));
    }
}

public class InstructionSelectionEventArgs:EventArgs
{
    public Instruction SelectedInstruction { get; set; }

    public InstructionSelectionEventArgs(Instruction selectedInstruction)
    {
        SelectedInstruction = selectedInstruction;
    }
}
