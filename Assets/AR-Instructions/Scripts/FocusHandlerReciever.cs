using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusHandlerReciever : MonoBehaviour
{
    public float ScaleFactor = 1.5f;
    public Material FocusMaterial;

    private Material defaultMaterial;
    private Renderer _renderer;

    public void Start()
    {
        _renderer = GetComponent<Renderer>();
        defaultMaterial = _renderer.material;
    }

    public void OnFocusEntered()
    {
        Vector3 newLocalScale = new Vector3(transform.localScale.x * ScaleFactor, transform.localScale.y * ScaleFactor, transform.localScale.z * ScaleFactor);
        transform.localScale = newLocalScale;
        _renderer.material = FocusMaterial;
    }

    public void OnFocusLost()
    {
        Vector3 newLocalScale = new Vector3(transform.localScale.x / ScaleFactor, transform.localScale.y / ScaleFactor, transform.localScale.z / ScaleFactor);
        transform.localScale = newLocalScale;
        _renderer.material = defaultMaterial;
    }

}
