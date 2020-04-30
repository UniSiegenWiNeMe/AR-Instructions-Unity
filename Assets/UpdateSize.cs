using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSize : MonoBehaviour
{

    public Collider HostCollider;
    public bool UpdateXAxis;
    public bool UpdateYAxis;
    public bool UpdateZAxis;
    public float ScalingFactor = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newScale = transform.localScale;

        if (UpdateXAxis)
        {
            newScale.x =  ((HostCollider.bounds.extents.x ) / HostCollider.transform.localScale.x )* ScalingFactor;
        }
        if (UpdateYAxis)
        {
            newScale.y = ((HostCollider.bounds.extents.y ) / HostCollider.transform.localScale.y) * ScalingFactor;
        }
        if (UpdateZAxis)
        {
            newScale.z = ((HostCollider.bounds.extents.z ) / HostCollider.transform.localScale.z) * ScalingFactor;
        }

        transform.localScale = newScale;
    }
}
