using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockAxis : MonoBehaviour
{
    public bool LockXAxis = false;
    public bool LockYAxis = false;
    public bool LockZAxis = false;


    private Vector3 initalPosition;
    public void Start()
    {
        initalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 tmp = transform.localPosition;
        if (LockXAxis)
        {
            tmp.x = initalPosition.x;
        }
        if (LockYAxis)
        {
            tmp.y = initalPosition.y;
        }
        if (LockZAxis)
        {
            tmp.z = initalPosition.z;
        }


        transform.localPosition = tmp;

    }
}
