using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteConfirmationController : MonoBehaviour
{
    public event EventHandler OnDelete;
    public event EventHandler OnCancle;

    public void Delete()
    {
        OnDelete?.Invoke(this,null);
    }
    public void Cancle()
    {
        OnCancle?.Invoke(this, null);
    }
}
