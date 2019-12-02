using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class SelectInstructionMenuController : MonoBehaviour
{
    public MenuMode Mode { get; private set; }

    public GameObject ItemParent;
    public GameObject ItemPrefab;
    public int NumberOfItemsToShow = 6;

    public TextMeshPro PageCounterText;
    public GameObject NextPageButton;
    public GameObject PreviousPageButton;

    public Interactable CreateNewInstructionInteractable;
    public Interactable ImportInstructionInteractable;

    public event EventHandler<InstructionSelectionEventArgs> InstructionSelected;

    private int _currentPage = 1;
    private int _maxPageNumber;



    // Start is called before the first frame update
    void Start()
    {
        Mode = MenuMode.Replay;

        
        var items = InstructionManager.Instance.GetInstructionNames(0, NumberOfItemsToShow);
        LoadItemsToMenu(items);

        if (PageCounterText != null)
        {
            _maxPageNumber = (InstructionManager.Instance.Count / NumberOfItemsToShow);
            if(InstructionManager.Instance.Count % NumberOfItemsToShow != 0)
            {
                _maxPageNumber++;
            }

            PageCounterText.text = _currentPage + "/" + _maxPageNumber;
        }

        NextPageButton.SetActive(InstructionManager.Instance.Count > NumberOfItemsToShow);
        PreviousPageButton.SetActive(false);
    }

    public void SetEditMode(bool mode)
    {
        Mode = mode ? MenuMode.Edit : MenuMode.Replay;

        foreach (Transform item in ItemParent.transform)
        {
            item.gameObject.GetComponent<MenuItemController>().SetMode(mode);
        }
    }

    private void LoadItemsToMenu(IEnumerable<string> items, bool clearBeforeLoad = false)
    {
        if(clearBeforeLoad)
        {
            while (ItemParent.transform.childCount > 0)
            {                
                DestroyImmediate(ItemParent.transform.GetChild(0).gameObject);
            }
        }

        foreach (var item in items)
        {
            var instruction = SaveLoadManager.Instance.Load(item);
            var itemGameObject = Instantiate(ItemPrefab, ItemParent.transform);
            itemGameObject.gameObject.GetComponent<MenuItemController>().SetMode(Mode == MenuMode.Replay ? false: true );
            itemGameObject.GetComponent<MenuItemController>().SetInstruction(instruction);
            itemGameObject.GetComponent<MenuItemController>().InstructionSelected += SelectInstructionMenuController_InstructionSelected;
            itemGameObject.GetComponent<MenuItemController>().InstructionRemoved += SelectInstructionMenuController_InstructionRemoved;

        }
        ItemParent.GetComponent<GridObjectCollection>()?.UpdateCollection();
    }

    private void SelectInstructionMenuController_InstructionRemoved(object sender, InstructionSelectionEventArgs e)
    {
        RefreshPage();
    }

    public void RefreshPage()
    {
        var items = InstructionManager.Instance.GetInstructionNames((_currentPage-1) * NumberOfItemsToShow, NumberOfItemsToShow);
        LoadItemsToMenu(items, true);
        
        //Update page counter
    }

    private void SelectInstructionMenuController_InstructionSelected(object sender, InstructionSelectionEventArgs e)
    {
        InstructionSelected?.Invoke(sender, e);
    }

    public void OnNextPage()
    {
        var items = InstructionManager.Instance.GetInstructionNames(_currentPage * NumberOfItemsToShow, NumberOfItemsToShow);
        LoadItemsToMenu(items,true);

        PreviousPageButton.SetActive(true);
        _currentPage++;
        if (_currentPage * NumberOfItemsToShow >= InstructionManager.Instance.Count)
        {
            NextPageButton.SetActive(false);
        }
        PageCounterText.text = _currentPage + "/" + _maxPageNumber;
    }

    public void OnPreviousPage()
    {
        _currentPage--;
        var items = InstructionManager.Instance.GetInstructionNames(_currentPage-1 * NumberOfItemsToShow, NumberOfItemsToShow);
        LoadItemsToMenu(items, true);

        NextPageButton.SetActive(true);

        if (_currentPage > 1)
        {
            PreviousPageButton.SetActive(true);
        }
        else
        {
            PreviousPageButton.SetActive(false);
        }
        PageCounterText.text = _currentPage + "/" + _maxPageNumber;
    }
    
}


