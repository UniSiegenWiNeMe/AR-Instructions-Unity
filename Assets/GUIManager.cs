using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{

    public GameObject SelectMenuPrefab;
    public GameObject FootMenuPrefab;
    public GameObject InstructionMenuPrefab;
    public GameObject EnterNamePrefab;
    public GameObject ContainerForSpawnedItems;
    public StabilizedTracking StabilizedTracking;


    private GameObject _selectMenu;
    private GameObject _footMenu;
    private GameObject _instructionMenu;
    private GameObject _enterName;


    public void Start()
    {
        if(SelectMenuPrefab == null ||
            FootMenuPrefab == null ||
            InstructionMenuPrefab == null ||
            EnterNamePrefab == null)
        {
            throw new ArgumentException("Missing Prefab in GUIManager!");
        }

        if (ContainerForSpawnedItems == null)
        {
            ContainerForSpawnedItems = GameObject.Find("Container");
        }

        InstructionManager.Instance.ImportCompleted += OnCompleted;


        _footMenu = Instantiate(FootMenuPrefab);
        _footMenu.GetComponent<FootMenuController>().HomeInteractable.OnClick.AddListener(FootMenu_OnHomeClick);
        _footMenu.GetComponent<FootMenuController>().MarkerScanInteractable.OnClick.AddListener(FootMenu_OnMarkerScanClick);

        CreateSelectMenu();
        

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
    }

    private void FootMenu_OnMarkerScanClick()
    {
        StabilizedTracking.Reset();

    }

    private void FootMenu_OnHomeClick()
    {
        InstructionManager.Instance.Reset();
        Destroy(_enterName);
        Destroy(_instructionMenu);
        DestroyAllSpawnedItems();
        DestroySelectMenu();
        CreateSelectMenu();
    }

    private void DestroyAllSpawnedItems()
    {
        while (ContainerForSpawnedItems.transform.childCount > 0)
        {
            DestroyImmediate(ContainerForSpawnedItems.transform.GetChild(0).gameObject);
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
        var tmp = _enterName.GetComponent<EnterInstructionNameController>().keyboardText;
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

        DestroySelectMenu();

    }

    private void ShowInstructionMenu(MenuMode mode)
    {
        _instructionMenu = Instantiate(InstructionMenuPrefab);
        _instructionMenu.transform.position = transform.position;

        if (mode == MenuMode.Replay)
        {
            _instructionMenu.GetComponent<MenuController>().Init(MenuMode.Replay, ContainerForSpawnedItems);
        }
        else
        {
            _instructionMenu.GetComponent<MenuController>().Init(MenuMode.Record, ContainerForSpawnedItems);
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
