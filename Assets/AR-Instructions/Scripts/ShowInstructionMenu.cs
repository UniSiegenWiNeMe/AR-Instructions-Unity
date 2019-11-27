using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShowInstructionMenu : MonoBehaviour
{
    /// <summary>
    /// The Instruction Menu (main menu)
    /// </summary>
    public GameObject InstructionMenu;
    
    /// <summary>
    /// Parents where holograms are store in
    /// </summary>
    public GameObject ContainerForSpawnedItems;

    /// <summary>
    /// Name of the instruction
    /// </summary>
    public string InstructionName;

    /// <summary>
    /// Are we in EditMode?
    /// </summary>
    public bool EditMode = false;


    /// <summary>
    /// Creates the instruction menu in replay mode
    /// </summary>
    public void OnShowInstructionMenuInReplayMode()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (ContainerForSpawnedItems == null)
            {
                ContainerForSpawnedItems = GameObject.Find("Container");
            }

            var menu = Instantiate(InstructionMenu);
            menu.transform.position = transform.position;
            if (!InstructionName.EndsWith(".save"))
            {
                InstructionName = Path.Combine(Application.persistentDataPath, InstructionName + ".save");
            }
            
            menu.GetComponent<MenuController>().Init(EditMode ? MenuMode.Edit : MenuMode.Replay, ContainerForSpawnedItems, InstructionName);
        });
    }

    /// <summary>
    /// Creates the instruction menu in record mode
    /// </summary>
    public void OnShowInstructionMenuInRecordMode()
    {
        if (string.IsNullOrEmpty(InstructionName))
        {
            InstructionName = OpenKeyboardOnStart.keyboardText;
        }

        if (ContainerForSpawnedItems == null)
        {
            ContainerForSpawnedItems = GameObject.Find("Container");
        }

        var menu = Instantiate(InstructionMenu);
        menu.transform.position = transform.position;

        menu.GetComponent<MenuController>().Init(MenuMode.Record, ContainerForSpawnedItems, InstructionName);
    }

    public void OnShowInstructionMenuInReplayModeEvent()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (ContainerForSpawnedItems == null)
            {
                ContainerForSpawnedItems = GameObject.Find("Container");
            }
            
            InstructionName = InstructionManagerSingleton.Instance.Instruction.Name;
            var menu = Instantiate(InstructionMenu);
            menu.transform.position = transform.position;
            if (!InstructionName.EndsWith(".save"))
            {
                InstructionName = Path.Combine(Application.persistentDataPath, InstructionName + ".save");
            }

            menu.GetComponent<MenuController>().Init(EditMode ? MenuMode.Edit : MenuMode.Replay, ContainerForSpawnedItems, InstructionName);
            gameObject.transform.parent.gameObject.SetActive(false);
        });
    }
}
