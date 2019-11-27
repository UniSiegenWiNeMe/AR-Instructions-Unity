using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionManagerSingleton : Singleton<InstructionManagerSingleton>
{

    /// <summary>
    /// Object to store all information about the instruction
    /// </summary>
    public Instruction Instruction { get; private set; }

    /// <summary>
    /// Instruction text of the current selected step
    /// </summary>
    public string CurrentInstructionText
    {
        get { return Instruction.Steps[CurrentStepNumber].Text; }
        set { Instruction.Steps[CurrentStepNumber].Text = value; }
    }
    
    /// <summary>
    /// Current step number. Zero based counting
    /// </summary>
    public int CurrentStepNumber { get; private set; }

    /// <summary>
    /// Count of steps in instruction
    /// </summary>
    public int StepsCount { get { return Instruction.StepsCount; } }

    /// <summary>
    /// Number for the "tool tips" at the holograms
    /// </summary>
    private int _toolTipTextCounter = 1;

    /// <summary>
    /// Creates an new instruction object
    /// </summary>
    /// <param name="name">Name of the instruction</param>
    /// <param name="dateCreated">Date when instruction was created</param>
    internal void CreateNewInstruction(string name, DateTime dateCreated)
    {
        Instruction = null;
        Instruction = new Instruction(name, dateCreated);
        Instruction.Steps.Add(new Step(CurrentStepNumber));
    }

    /// <summary>
    /// Loads an instruction from file system
    /// </summary>
    /// <param name="name"></param>
    internal bool LoadInstruction(string name)
    {
        Instruction = SaveLoadManager.Instance.Load(name);
        if(Instruction == null)
        {
            Debug.Log("instruction = null");
            return false;
        }
        CurrentStepNumber = 0;
        return true;
    }
    /// <summary>
    /// Gets the current step data
    /// </summary>
    /// <returns></returns>
    internal Step GetCurrentStep()
    {
        return Instruction.Steps[CurrentStepNumber];
    }

    /// <summary>
    /// Adds an item to the current step
    /// </summary>
    /// <param name="item"></param>
    internal void AddItem(Item item)
    {
        if(item.HasText)
        {
            item.Text = _toolTipTextCounter.ToString();
            item._gameObject.GetComponentInChildren<ToolTip>().ToolTipText = item.Text;
            _toolTipTextCounter++;
        }
        Instruction.Steps[CurrentStepNumber].Items.Add(item);
    }

    /// <summary>
    /// checks if there is a next step available
    /// </summary>
    /// <returns></returns>
    internal bool NextStepAvailabe()
    {
        return CurrentStepNumber + 1 == Instruction.Steps.Count ? false : true;
    }

    /// <summary>
    /// Adds a new step (empty) to the instruction
    /// </summary>
    internal void AddStep()
    {
        CurrentStepNumber++;
        _toolTipTextCounter = 1;
        Instruction.Steps.Add(new Step(CurrentStepNumber));
    }

    /// <summary>
    /// Creates a new instruction based on a template (list of string)
    /// </summary>
    /// <param name="lines">Each item in this list is a new step. The string is the text for the step</param>
    /// <param name="name">Name of the instruction</param>
    internal void LoadFromTemplate(List<string> lines, string name)
    {

        Instruction = new Instruction();
        Instruction.Name = name;

        for (int i = 0; i < lines.Count; i++)
        {
            Instruction.Steps.Add(new Step(i, lines[i]));
        }
        Save();
    }

    /// <summary>
    /// Get all media files of the current step
    /// </summary>
    /// <returns></returns>
    internal List<MediaFile> GetCurrentMediaFiles()
    {
        return Instruction.Steps[CurrentStepNumber].MediaFiles;
    }

    /// <summary>
    /// goes into the next step of the instruction
    /// </summary>
    /// <returns>false if there is no next step</returns>
    internal bool StepForward()
    {
        if (CurrentStepNumber + 1 < StepsCount)
        {
            CurrentStepNumber++;
            _toolTipTextCounter = 1;
            foreach (var item in Instruction.Steps[CurrentStepNumber].Items)
            {
                if(item.HasText)
                {
                    _toolTipTextCounter++;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// is there a previous step in related to the current step
    /// </summary>
    /// <returns></returns>
    internal bool PreviousStepAvailabe()
    {
        return CurrentStepNumber > 0 ? true : false;
    }

    /// <summary>
    /// goes into the previous step of the instruction
    /// </summary>
    /// <returns></returns>
    internal bool StepBack()
    {
        if (CurrentStepNumber - 1 >= 0)
        {
            CurrentStepNumber--;

            _toolTipTextCounter = 1;
            foreach (var item in Instruction.Steps[CurrentStepNumber].Items)
            {
                if (item.HasText)
                {
                    _toolTipTextCounter++;
                }
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// save the instruction to the filesystem
    /// </summary>
    /// <param name="sync">do it in a blocking way?</param>
    /// <param name="path">`Path to the location on the filesystem</param>
    public void Save(bool sync = false, string path = null)
    {
        SaveLoadManager.Instance.Save(Instruction, sync, path);
    }
    
}
