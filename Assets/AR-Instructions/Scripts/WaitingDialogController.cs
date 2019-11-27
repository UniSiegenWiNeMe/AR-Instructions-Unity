using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingDialogController : MonoBehaviour
{
    public GameObject ObjectToReactivate;
    public GameObject Button;
    public void Close()
    {
        ObjectToReactivate.SetActive(true);

        Destroy(gameObject);
    }

    public void ExportCompleted()
    {
        Button.SetActive(true);
        GetComponentInChildren<FitBackgroundToContent>().ToolTipText = "Export abgeschlossen.";
    }
}
