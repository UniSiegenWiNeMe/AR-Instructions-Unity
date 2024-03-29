﻿using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{

    public GameObject SelectMenuPrefab;
    public GameObject HandMenuPrefab;
    public GameObject InstructionMenuPrefab;
    public GameObject EnterNamePrefab;
    public GameObject ParentForInstructionHolograms;
    public StabilizedTracking StabilizedTracking;
    public GameObject OffsetHandler;

    private GameObject _selectMenu;
    private GameObject _handMenu;
    private GameObject _instructionMenu;
    private GameObject _enterName;
    private MenuMode _mode = MenuMode.Replay;


    public void Start()
    {
        if(SelectMenuPrefab == null ||
            HandMenuPrefab == null ||
            InstructionMenuPrefab == null ||
            EnterNamePrefab == null)
        {
            throw new ArgumentException("Missing Prefab in GUIManager!");
        }

        if (ParentForInstructionHolograms == null)
        {
            ParentForInstructionHolograms = GameObject.Find("Offset");
        }

        InstructionManager.Instance.ImportCompleted += OnCompleted;


        _handMenu = Instantiate(HandMenuPrefab);
        _handMenu.GetComponent<HandMenuController>().HomeInteractable.OnClick.AddListener(HandMenu_OnHomeClick);
        _handMenu.GetComponent<HandMenuController>().MarkerScanInteractable.OnClick.AddListener(HandMenu_OnMarkerScanClick);
        _handMenu.GetComponent<HandMenuController>().OffsetInteractable.OnClick.AddListener(HandMenu_OnOffsetClick);

        CreateSelectMenu();
        

    }

    private void HandMenu_OnOffsetClick()
    {
        OffsetHandler.SetActive(true);
    }

    private void OnCompleted(object sender, EventArgs e)
    {
        CreateSelectMenu();
        //ShowInstructionMenu(MenuMode.Record);
    }

    private void CreateSelectMenu()
    {
        _selectMenu = Instantiate(SelectMenuPrefab);
        _selectMenu.GetComponent<SelectInstructionMenuController>().CreateNewInstructionInteractable.OnClick.AddListener(SelectInstructionMenu_OnCreateNewInstructionClick);
        _selectMenu.GetComponent<SelectInstructionMenuController>().ImportInstructionInteractable.OnClick.AddListener(SelectInstructionMenu_OnImportInstructionClick);
        _selectMenu.GetComponent<SelectInstructionMenuController>().InstructionSelected += SelectInstructionMenu_OnSelect;
        _selectMenu.GetComponent<SelectInstructionMenuController>().ModeChanged += SelectInstructionMenu_ModeChanged;
    }

    private void SelectInstructionMenu_ModeChanged(object sender, ModeChangedEventArgs e)
    {
        _mode = e.Mode;

        _handMenu.GetComponent<HandMenuController>().ChangeMode(_mode);
    }

    private void HandMenu_OnMarkerScanClick()
    {
        StabilizedTracking.Reset();

    }

    private void HandMenu_OnHomeClick()
    {
        Reset();
    }

    private void Reset()
    {
        InstructionManager.Instance.Reset();
        Destroy(_enterName);
        if (_instructionMenu != null)
        {
            Destroy(_instructionMenu);
        }
        DestroyAllSpawnedItems();
        DestroySelectMenu();
        CreateSelectMenu();
    }

    private void DestroyAllSpawnedItems()
    {
        while (ParentForInstructionHolograms.transform.childCount > 0)
        {
            DestroyImmediate(ParentForInstructionHolograms.transform.GetChild(0).gameObject);
        }
    }

    public void SelectInstructionMenu_OnCreateNewInstructionClick()
    {
        _enterName = Instantiate(EnterNamePrefab);
        _enterName.transform.position = transform.position;
        _enterName.GetComponent<EnterInstructionNameController>().Continue += EnterName_Continue;

        DestroySelectMenu();
    }

    private void EnterName_Continue(object sender, EventArgs e)
    {
        var tmp = _enterName.GetComponent<EnterInstructionNameController>().InstructionName;
        ShowInstructionMenu(MenuMode.Record);

        _enterName.GetComponent<EnterInstructionNameController>().Continue -= EnterName_Continue;
        Destroy(_enterName);
    }

    public void SelectInstructionMenu_OnImportInstructionClick()
    {
        DestroySelectMenu();
        InstructionManager.Instance.ImportInstruction();
    }
    private void SelectInstructionMenu_OnSelect(object sender, InstructionSelectionEventArgs e)
    {
        ShowInstructionMenu(_selectMenu.GetComponent<SelectInstructionMenuController>().Mode);
        ParentForInstructionHolograms.GetComponent<OffsetController>().SetTransform(InstructionManager.Instance.Instruction.OffsetForHolograms);
        
        DestroySelectMenu();
    }

    private void ShowInstructionMenu(MenuMode mode)
    {
        _instructionMenu = Instantiate(InstructionMenuPrefab);
        _instructionMenu.transform.position = transform.position;

        if (mode == MenuMode.Replay)
        {
            var menuController = _instructionMenu.GetComponent<MenuController>();
            if (menuController != null)
            {
                menuController.Init(MenuMode.Replay, ParentForInstructionHolograms);
            }
            else
            {
                Debug.Log("menuController is null");
            }

            var mainPanelController = _instructionMenu.gameObject.GetComponentInChildren<MainPanelController>(true);
            if (mainPanelController != null)
            {
                mainPanelController.HomeButtonClicked.AddListener(Reset);
            }
            else
            {
                Debug.Log("mainPanelController is null");
            }
        }
        else
        {
            _instructionMenu.GetComponent<MenuController>().Init(MenuMode.Record, ParentForInstructionHolograms);
        }
    }

    private void DestroySelectMenu()
    {
        if (_selectMenu != null)
        {
            _selectMenu.GetComponent<SelectInstructionMenuController>().CreateNewInstructionInteractable.OnClick.RemoveAllListeners();
            _selectMenu.GetComponent<SelectInstructionMenuController>().ImportInstructionInteractable.OnClick.RemoveAllListeners();
            _selectMenu.GetComponent<SelectInstructionMenuController>().InstructionSelected -= SelectInstructionMenu_OnSelect;

            Destroy(_selectMenu);
            _selectMenu = null;
        }
    }
}
