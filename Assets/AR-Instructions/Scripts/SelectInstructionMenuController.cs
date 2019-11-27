using Microsoft.MixedReality.Toolkit.UI;
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
    public GameObject LoadFromTemplate;

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

    public void Init(MenuMode mode)
    {
        _mode = mode;

        CreateNewInstructionButton.SetActive((_mode == MenuMode.Record ||_mode == MenuMode.Edit)? true : false);
        LoadFromTemplate.SetActive((_mode == MenuMode.Record || _mode == MenuMode.Edit) ? true : false);
    }

    private void LoadItemsToMenu(IEnumerable<string> items, bool clearBeforeLoad = false)
    {
        if(clearBeforeLoad)
        {
            foreach (Transform child in ItemParent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (var item in items)
        {
            var instruction = SaveLoadManager.Instance.Load(item);

            var itemGameObject = Instantiate(ItemPrefab, ItemParent.transform);
            itemGameObject.GetComponentInChildren<ShowInstructionMenu>().InstructionName = item;
            itemGameObject.GetComponentInChildren<ShowInstructionMenu>().EditMode = _mode == MenuMode.Edit ? true : false;

            string itemText = instruction.Name; 
            if (instruction.Name.Length >  NumberOfCharsToShow)
            {
                itemText = itemText.Substring(0, NumberOfCharsToShow).TrimEnd('.') + "...";
            }

            itemGameObject.GetComponent<TextMeshPro>().text = itemText;
            itemGameObject.GetComponentInChildren<Interactable>().OnClick.AddListener(OnSelect);
        }
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


