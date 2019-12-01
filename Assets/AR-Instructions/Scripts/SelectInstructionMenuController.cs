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
    private MenuMode _mode;
    List<string> allInstructionFiles;
    public int NumberOfItemsToShow = 5;
    public int NumberOfCharsToShow = 15;
    public GameObject ItemParent;
    public GameObject ItemPrefab;

    public TextMeshPro PageCounterText;
    public GameObject NextPageButton;
    public GameObject PreviousPageButton;
    public GameObject CreateNewInstructionButton;
    public GameObject ImportButton;

    private int currentPage = 1;
    private int _maxPageNumber;


    // Start is called before the first frame update
    void Start()
    {
        allInstructionFiles = GetAllInstructionFiles();

        var items = allInstructionFiles.Take(NumberOfItemsToShow);
        LoadItemsToMenu(items);

        if (PageCounterText != null)
        {
            _maxPageNumber = (allInstructionFiles.Count / NumberOfItemsToShow);
            if(allInstructionFiles.Count%NumberOfItemsToShow != 0)
            {
                _maxPageNumber++;
            }

            PageCounterText.text = currentPage + "/" + _maxPageNumber;
        }

        NextPageButton.SetActive(allInstructionFiles.Count > NumberOfItemsToShow);
        PreviousPageButton.SetActive(false);
    }

    public void SetEditMode(bool mode)
    {
        foreach (Transform item in ItemParent.transform)
        {
            item.gameObject.GetComponent<MenuItemController>().SetMode(mode);
        }
    }
    //public void Init(MenuMode mode)
    //{
    //    _mode = mode;

    //    CreateNewInstructionButton.SetActive((_mode == MenuMode.Record ||_mode == MenuMode.Edit)? true : false);
    //    ImportButton.SetActive((_mode == MenuMode.Record || _mode == MenuMode.Edit) ? true : false);
    //}

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
            //itemGameObject.GetComponentInChildren<ShowInstructionMenu>().InstructionName = item;
            //itemGameObject.GetComponentInChildren<ShowInstructionMenu>().EditMode = _mode == MenuMode.Edit ? true : false;

            
            itemGameObject.GetComponent<MenuItemController>().SetInstruction(instruction);
            itemGameObject.GetComponent<MenuItemController>().SelectionMenu = gameObject;

            //itemGameObject.GetComponent<TextMeshPro>().text = itemText;
            //itemGameObject.GetComponentInChildren<Interactable>().OnClick.AddListener(OnSelect);
        }
        ItemParent.GetComponent<GridObjectCollection>()?.UpdateCollection();
    }

    public void OnNextPage()
    {
        var items = allInstructionFiles.Skip(currentPage * NumberOfItemsToShow).Take(NumberOfItemsToShow);
        LoadItemsToMenu(items,true);

        PreviousPageButton.SetActive(true);
        currentPage++;
        if (currentPage * NumberOfItemsToShow >= allInstructionFiles.Count)
        {
            NextPageButton.SetActive(false);
        }
        PageCounterText.text = currentPage + "/" + _maxPageNumber;
    }

    public void OnPreviousPage()
    {
        currentPage--;
        var items = allInstructionFiles.Skip((currentPage-1) * NumberOfItemsToShow).Take(NumberOfItemsToShow);
        LoadItemsToMenu(items, true);

        NextPageButton.SetActive(true);

        if (currentPage > 1)
        {
            PreviousPageButton.SetActive(true);
        }
        else
        {
            PreviousPageButton.SetActive(false);
        }
        PageCounterText.text = currentPage + "/" + _maxPageNumber;
    }

    private void OnSelect()
    {
        gameObject.SetActive(false);
    }

    private List<string> GetAllInstructionFiles()
    {
        var files = Directory.GetFiles(Application.persistentDataPath, "*save").ToList();

        files.Sort();
        return files;
    }
}


