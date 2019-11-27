using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSelectionMenu : MonoBehaviour
{
    public GameObject SelectionMenu;

    /// <summary>
    /// Creates the SelectionMenu for replay mode
    /// </summary>
    public void OnShowSelectionMenuInReplayMode()
    {
        var menu = Instantiate(SelectionMenu);
        menu.transform.position = transform.position;
        menu.GetComponent<SelectInstructionMenuController>().Init(MenuMode.Replay);
    }

    /// <summary>
    /// Creates the SelectionMenu for recording mode
    /// </summary>
    public void OnShowSelectionMenuInRecordEditMode()
    {
        var menu = Instantiate(SelectionMenu);
        menu.transform.position = transform.position;
        menu.GetComponent<SelectInstructionMenuController>().Init(MenuMode.Edit);
    }
}
