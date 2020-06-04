using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetController : MonoBehaviour
{
    public Transform Offset;

    private Vector3 origin;

    public void Start()
    {
        origin = transform.localPosition;    
    }
    public void Update()
    {
        transform.localPosition = origin+Offset.localPosition;
    }

    internal void SetTransform(SerializableTransform offsetForHolograms)
    {
        var tmp = transform;
        offsetForHolograms.ToTransform(ref tmp);
        origin = transform.localPosition;
    }
}
