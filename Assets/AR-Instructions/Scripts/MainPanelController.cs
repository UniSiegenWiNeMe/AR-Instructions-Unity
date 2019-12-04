using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MainPanelController : MonoBehaviour
{
    /// <summary>
    /// Controller of the photo and video panel
    /// </summary>
    public PhotoVideoPanelController PhotoVideoPanelController;

    /// <summary>
    /// Text label where the instruction text is displayed
    /// </summary>
    public TextMeshPro InstructionText;

    /// <summary>
    /// Gameobject to the next step button
    /// </summary>
    public GameObject NextStepButton;

    /// <summary>
    /// Gameobject to the previous step button
    /// </summary>
    public GameObject PreviousStepButton;

    /// <summary>
    /// Text label where the stepcounter is displayed
    /// </summary>
    public TextMeshPro StepCounterText;

    /// <summary>
    /// Parent where the holograms are store in
    /// </summary>
    public GameObject ContainerForSpawnedItems;

    public GameObject ArrowPrefab;
    public GameObject BoxPrefab;
    public GameObject CircleArrowPrefab;

    public OpenKeyboard Keyboard;

    public GameObject InsertTextButton;
    public GameObject FinishButton;

    /// <summary>
    /// this event is triggerd every time some data of the instruction has changed
    /// </summary>
    public UnityEvent OnNewData;

    /// <summary>
    /// Mode of the menu
    /// </summary>
    private MenuMode _mode;
    private InstructionManager _instructionManager;

    public void Init(MenuMode mode, GameObject ContainerForSpawnedItems)
    {
        _mode = mode;
        _instructionManager = InstructionManager.Instance;
        this.ContainerForSpawnedItems = ContainerForSpawnedItems;

        if (_mode == MenuMode.Record /*|| _mode == MenuMode.Edit*/)
        {
            LoadStep(_instructionManager.GetCurrentStep());
            //if (_mode == MenuMode.Edit)
            //{
                
            //}
            //else
            //{
            //    //_instructionManager.CreateNewInstruction(InstructionName, DateTime.Now);
            //}

            NextStepButton.SetActive(true);

            Keyboard.TextTyped.AddListener(NewText);
        }
        else
        {
            InsertTextButton.SetActive(false);
            FinishButton.SetActive(false);

            LoadStep(_instructionManager.GetCurrentStep());
            if (!_instructionManager.NextStepAvailabe())
            {
                NextStepButton.SetActive(false);
            }
        }
        PreviousStepButton.SetActive(false);
    }

    //public void Init(MenuMode mode, GameObject ContainerForSpawnedItems, string InstructionName = null)
    //{
    //    _mode = mode;
    //    _instructionManager = InstructionManager.Instance;
    //    this.ContainerForSpawnedItems = ContainerForSpawnedItems;

    //    if (_mode == MenuMode.Record || _mode == MenuMode.Edit)
    //    {
    //        if(_mode == MenuMode.Edit)
    //        {
    //            _instructionManager.LoadInstruction(InstructionName);
    //            LoadStep(_instructionManager.GetCurrentStep());
    //        }
    //        else
    //        {
    //            _instructionManager.CreateNewInstruction(InstructionName, DateTime.Now);
    //        }

    //        NextStepButton.SetActive(true);

    //        Keyboard.TextTyped.AddListener(NewText);
    //    }
    //    else
    //    {
    //        _instructionManager.LoadInstruction(InstructionName);

    //        InsertTextButton.SetActive(false);
    //        FinishButton.SetActive(false);

    //        LoadStep(_instructionManager.GetCurrentStep());
    //        if (!_instructionManager.NextStepAvailabe())
    //        {
    //            NextStepButton.SetActive(false);
    //        }
    //    }
    //    PreviousStepButton.SetActive(false);

    //}

    /// <summary>
    /// goes into the next step of the instruction or adds a new steps if current steps is the last one
    /// </summary>
    public void NextStep()
    {
        // is this alway necessary?
        _instructionManager.Save();

        ClearItems();

        if (!_instructionManager.NextStepAvailabe() && (_mode == MenuMode.Record /*||_mode == MenuMode.Edit*/))
        {
            _instructionManager.AddStep();

            PhotoVideoPanelController.Reset(_instructionManager.GetCurrentMediaFiles());
            InstructionText.text = "Anweisung:" + Environment.NewLine + "Kein Anweisungstext vorhanden.";
        }
        else
        {
            _instructionManager.StepForward();
            LoadStep(_instructionManager.GetCurrentStep());

            if (!_instructionManager.NextStepAvailabe() && _mode == MenuMode.Replay)
            {
                NextStepButton.SetActive(false);
            }
        }

        // this should be alway true
        if (_instructionManager.PreviousStepAvailabe())
        {
            PreviousStepButton.SetActive(true);
        }

        SetStepCounterText();
    }

    internal void Reset()
    {
        SetInstructionText(_instructionManager.CurrentInstructionText);
        SetStepCounterText();

        NextStepButton.SetActive(true);
        PreviousStepButton.SetActive(false);
        Keyboard.TextTyped.AddListener(NewText);
    }

    public void PreviousStep()
    {
        _instructionManager.StepBack(); // return value is not that important since the preivous step button is only avaible when there is a preivous step

        ClearItems();

        LoadStep(_instructionManager.GetCurrentStep());

        PreviousStepButton.SetActive(_instructionManager.PreviousStepAvailabe());
        NextStepButton.SetActive(true);

        SetStepCounterText();
    }

    /// <summary>
    /// clears all items of the current step. the instruction is not influenced by this
    /// </summary>
    private void ClearItems()
    {
        foreach (Transform child in ContainerForSpawnedItems.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void LoadStep(Step step)
    {
        if (step != null)
        {
            PhotoVideoPanelController.Reset(step.MediaFiles);

            SetInstructionText(step.Text);
            SetStepCounterText();
            foreach (var item in step.Items)
            {
                LoadItem(item);
            }
        }
    }

    private void SetInstructionText(string instruction = null)
    {
        
        if (!String.IsNullOrEmpty(instruction))
        {
            InstructionText.text = "Anweisung:" + Environment.NewLine + instruction;
        }
        else
        {
            InstructionText.text = "Anweisung: " + Environment.NewLine + "Kein Anweisungstext vorhanden";
        }
    }

    private void LoadItem(Item item)
    {
        if (item.IsActive) // items which were deleted are only inactive (soft delete)
        {
            GameObject element = InstantiateElement(item);

            if(item.HasText == false)
            {
                element.GetComponentInChildren<ToolTip>().gameObject.SetActive(false);
            }

            if (_mode == MenuMode.Record /*|| _mode == MenuMode.Edit*/)
            {
                var boundingBox = element.GetComponentInChildren<BoundingBox>();
                boundingBox.RotateStopped.AddListener(OnItemManipulated);
                boundingBox.ScaleStopped.AddListener(OnItemManipulated);
                var manipulationManager = element.GetComponentInChildren<ManipulationHandler>();
                manipulationManager.OnManipulationEnded.AddListener(OnItemManipulated);
            }
            else
            {
                foreach (Transform child in element.transform)
                {
                    if (child.gameObject.name != "Visual")
                    {
                        child.gameObject.SetActive(false);
                    }
                    else
                    {
                        var manipulationHandlers = child.gameObject.GetComponentsInChildren<ManipulationHandler>();

                        foreach (var manipulationHandler in manipulationHandlers)
                        {
                            manipulationHandler.enabled = false;
                        }
                    }
                }
            }

            item._gameObject = element;
        }
    }

    private void OnItemManipulated(ManipulationEventData arg0)
    {
        OnItemManipulated();
    }

    private void OnItemManipulated()
    {
        if (_mode == MenuMode.Record /*|| _mode == MenuMode.Edit*/)
        {
            OnNewData?.Invoke();
            _instructionManager.Save();
        }
    }

    private GameObject InstantiateElement(Item item)
    {
        GameObject newGameObject;
        GameObject prefab;
        switch (item.Type)
        {
            case ItemType.Arrow:
                prefab = ArrowPrefab;
                break;
            case ItemType.Box:
                prefab = BoxPrefab;
                break;
            case ItemType.CircleArrow:
                prefab = CircleArrowPrefab;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        newGameObject = Instantiate(prefab, ContainerForSpawnedItems.transform);

        Transform visualTransform = newGameObject.transform.Find("Visual");

        visualTransform.localPosition = new Vector3(item.Transform.Position[0], item.Transform.Position[1], item.Transform.Position[2]);
        visualTransform.localRotation = new Quaternion(item.Transform.Rotation[0], item.Transform.Rotation[1], item.Transform.Rotation[2], item.Transform.Rotation[3]);
        visualTransform.localScale = new Vector3(item.Transform.Scale[0], item.Transform.Scale[1], item.Transform.Scale[2]);

        if (item.HasText)
        {
            Transform toolTipTransform = visualTransform.Find("Simple Line ToolTip");
            Transform pivotTransform = toolTipTransform.Find("Pivot");
            pivotTransform.localPosition = new Vector3(item.TextPosition[0], item.TextPosition[1], item.TextPosition[2]);
            toolTipTransform.gameObject.GetComponent<ToolTip>().ToolTipText = item.Text;
        }

        var tmpColor = new Color(item.Color[0], item.Color[1], item.Color[2], item.Color[3]);
        visualTransform.gameObject.GetComponentInChildren<MeshRenderer>().material.color = tmpColor;

        return newGameObject;
    }

    private void SetStepCounterText()
    {
        StepCounterText.text = $"Schritt: {_instructionManager.CurrentStepNumber + 1}/{_instructionManager.StepsCount}";
    }

    private void NewText(string text)
    {
        _instructionManager.CurrentInstructionText = text;
        OnNewData?.Invoke();
    }
}

[System.Serializable]
public class StepChangedEvent : UnityEvent<List<MediaFile>>
{
}
