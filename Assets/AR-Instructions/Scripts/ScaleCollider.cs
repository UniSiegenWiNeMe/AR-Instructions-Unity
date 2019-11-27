using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleCollider : MonoBehaviour
{
    public float threshold;
    public Vector3 DefaultSize;
    public Vector3 ExtendedSize;


    private BoxCollider _collider;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<BoxCollider>();
    }


    public void OnStarScale()
    {
    }

    public void OnEndScale()
    {
        if(transform.localScale.x < threshold|| transform.localScale.y < threshold || transform.localScale.y < threshold)
        {
            _collider.size = ExtendedSize;
        }
        else
        {
            _collider.size = DefaultSize;
        }
    }

    
}
