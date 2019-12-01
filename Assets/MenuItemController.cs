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
    public GameObject InstructionMenu;
    public GameObject SelectionMenu;

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


    public void ShowInstrcutionMenu()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (ContainerForSpawnedItems == null)
            {
                ContainerForSpawnedItems = GameObject.Find("Container");
            }

            var menu = Instantiate(InstructionMenu);
            menu.transform.position = transform.position;
            if (!_instruction.Name.EndsWith(".save"))
            {
                _instruction.Name = Path.Combine(Application.persistentDataPath, _instruction.Name + ".save");
            }

            menu.GetComponent<MenuController>().Init(EditMode ? MenuMode.Edit : MenuMode.Replay, ContainerForSpawnedItems, _instruction);

            SelectionMenu.SetActive(false);
        });
    }
}
