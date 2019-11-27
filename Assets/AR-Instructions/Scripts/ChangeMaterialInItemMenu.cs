using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialInItemMenu : MonoBehaviour
{
    public Material Green;
    public Material Red;
    public Material Yellow;

    public void SetGreen()
    {
        GetComponent<MeshRenderer>().material = Green;
    }
    public void SetRed()
    {
        GetComponent<MeshRenderer>().material = Red;
    }
    public void SetYellow()
    {
        GetComponent<MeshRenderer>().material = Yellow;
    }

}
