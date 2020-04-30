using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveParent : MonoBehaviour
{
    public Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var tmp = parent.localRotation * transform.localPosition;
        parent.localPosition += tmp;

        transform.localPosition = Vector3.zero;
    }
}
