using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public enum ActionType
{
    InstructionStarted,
    InstructionCompleted,
    StepForward,
    StepBackward,
    VideoPaused,
    VideoRestarted,
    VideoUnpaused,
    VideoStarted
}

public class ActionLogEntry
{
    public int StepNumber;
    public ActionType ActionType;
    public DateTime Timestamp;

    public ActionLogEntry(int stepNumber, ActionType actionType, DateTime timestamp)
    {
        StepNumber = stepNumber;
        ActionType = actionType;
        Timestamp = timestamp;
    }
}

public class InstructionManager : Singleton<InstructionManager>
{
    public List<ActionLogEntry> ActionLog = new List<ActionLogEntry>();

    public event EventHandler ImportCompleted;


    private List<string> _allFileFullNames;

    public List<string> AllFullFileNames
    {
        get
        {
            if (_allFileFullNames == null)
            {
                _allFileFullNames = GetAllInstructionFiles();
            }
            return _allFileFullNames;
        }
    }
    public int Count
    {
        get
        {
            return AllFullFileNames.Count;
        }
    }

    /// <summary>
    /// Object to store all information about the instruction
    /// </summary>
    public Instruction Instruction { get; set; }

    /// <summary>
    /// Instruction text of the current selected step
    /// </summary>
    public string CurrentInstructionText
    {
        get { return Instruction.Steps[CurrentStepNumber].Text; }
        set { Instruction.Steps[CurrentStepNumber].Text = value; }
    }

    public void SelectInstruction(Instruction instruction)
    {
        this.Instruction = instruction;
        ActionLog.Add(new ActionLogEntry(-1, ActionType.InstructionStarted, DateTime.Now));
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

    internal void Remove(string name)
    {
        foreach (var fileName in _allFileFullNames)
        {
            string tmp = ExtractFileNameFromPath(fileName);
            if (tmp == name)
            {
                MoveFileToDeleted(fileName);
                _allFileFullNames.Remove(fileName);
                return;
            }
        }
    }

    /// <summary>
    /// filename is without extension.
    /// </summary>
    /// <param name="path">full path to file. with extension</param>
    /// <returns></returns>
    private static string ExtractFileNameFromPath(string path)
    {
        return path.Substring(path.LastIndexOf("\\") + 1, path.LastIndexOf(".") - path.LastIndexOf("\\") - 1);
    }

    private void MoveFileToDeleted(string fileName)
    {
        var binPath = Path.Combine(Application.persistentDataPath, "bin");

        if (!Directory.Exists(binPath))
        {
            Directory.CreateDirectory(binPath);
        }
        try
        {

            File.Copy(fileName, Path.Combine(binPath, ExtractFileNameFromPath(fileName) + ".save"));
            File.Delete(fileName);
        }
        catch (Exception ex)
        {

            throw;
        }

    }

    /// <summary>
    /// Creates an new instruction object
    /// </summary>
    /// <param name="name">Name of the instruction</param>
    /// <param name="dateCreated">Date when instruction was created</param>
    internal void CreateNewInstruction(string name, DateTime dateCreated)
    {
        int i = 1;
        var tmpName = name;
        while (Exist(tmpName))
        {
            tmpName = name + " (" + i + ")";
            i++;
        }
        name = tmpName;

        Debug.Log(name);
        Instruction = null;
        Instruction = new Instruction(name, dateCreated);
        Instruction.Steps.Add(new Step(CurrentStepNumber));
        Save(true, Application.persistentDataPath);
        UpdateFileNames();

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private bool Exist(string name)
    {
        foreach (var fullFileName in AllFullFileNames)
        {
            if(ExtractFileNameFromPath(fullFileName) == name)
            {
                return true;
            }
        }

        return false;
    }

    internal void ImportInstruction()
    {
        var importer = new Import();
        importer.ImportCompleted += Importer_ImportCompleted;
        importer.DoImport();

    }

    private void Importer_ImportCompleted(object sender, string e)
    {
        //InstructionManager.Instance.LoadInstruction(e);
        UpdateFileNames();
        ImportCompleted?.Invoke(this, null);
    }

    //private void InstructionImported(object sender, EventArgs e)
    //{
    //    ImportCompleted?.Invoke(this, null);
    //}

    private void UpdateFileNames()
    {
        _allFileFullNames = GetAllInstructionFiles();
    }

    /// <summary>
    /// Loads an instruction from file system
    /// </summary>
    /// <param name="name"></param>
    internal bool LoadInstruction(string name)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            Instruction = SaveLoadManager.Instance.Load(name);
            if (Instruction == null)
            {
                Debug.Log("instruction = null");
            }
            CurrentStepNumber = 0;
        });

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
        ActionLog.Add(new ActionLogEntry(CurrentStepNumber, ActionType.StepForward, DateTime.Now));
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
        ActionLog.Add(new ActionLogEntry(CurrentStepNumber, ActionType.StepBackward, DateTime.Now));
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

    public void Reset()
    {
        ActionLog.Add(new ActionLogEntry(CurrentStepNumber, ActionType.InstructionCompleted, DateTime.Now));
        Instruction = null;
        ExportActionLog(ActionLog);
        ActionLog = new List<ActionLogEntry>();
        CurrentStepNumber = 0;
        _toolTipTextCounter = 1;
    }

    private void ExportActionLog(List<ActionLogEntry> actionLog)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var action in actionLog)
        {
            var s = String.Format("{0};{1};{2}",
                          action.StepNumber,
                          action.ActionType,
                          action.Timestamp);
            s += Environment.NewLine;
            sb.Append(s);
        }

        

        var filteredActionLog = actionLog.Where(x => x.ActionType == ActionType.StepBackward ||
        x.ActionType == ActionType.StepForward ||
        x.ActionType == ActionType.InstructionCompleted ||
        x.ActionType == ActionType.InstructionStarted).OrderBy(x => x.Timestamp).ToList();

        Dictionary<int, TimeSpan> tmp = new Dictionary<int, TimeSpan>();
        for (int i = 0; i < filteredActionLog.Count; i++)
        {
            if(i > 0)
            {
                if (!tmp.ContainsKey(filteredActionLog[i].StepNumber))
                {
                    var timespan = filteredActionLog[i].Timestamp - filteredActionLog[i - 1].Timestamp;
                    tmp.Add(filteredActionLog[i].StepNumber, timespan);
                }
                else
                {
                    var timespan = filteredActionLog[i].Timestamp - filteredActionLog[i - 1].Timestamp;
                    tmp[filteredActionLog[i].StepNumber] = tmp[filteredActionLog[i].StepNumber].Add(timespan);
                }
            }
        }

        

        foreach (var keyValuePair in tmp)
        {
            var s = String.Format("{0};{1}",
                          keyValuePair.Key,
                          keyValuePair.Value);
            s += Environment.NewLine;
            sb.Append(s);
        }

        var totalTime = actionLog.Last().Timestamp - actionLog[0].Timestamp;
        sb.Append("Totaltime:" + totalTime);

        System.IO.File.WriteAllText(Path.Combine(Application.persistentDataPath,"ActionLog-"+DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss") +".txt"), sb.ToString());

    }

    public List<string> GetAllInstructionFiles()
    {
        var files = Directory.GetFiles(Application.persistentDataPath, "*save").ToList();

        files.Sort();

        
        return files;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageNumber">Zero based counting</param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public IEnumerable<string> GetInstructionNamesForPage(int pageNumber, int pageSize)
    {
        var skip = pageNumber * pageSize;
        return AllFullFileNames.Skip(skip).Take(pageSize);
    }

    internal void OffsetChanged(Vector3 localPosition)
    {
        Instruction.OffsetForHolograms.AddNewOffset(localPosition);
        Save();
    }


    //public IEnumerable<string> GetInstructionNames(int skip, int take)
    //{
    //    return AllFileNames.Skip(skip).Take(take);
    //}

}
