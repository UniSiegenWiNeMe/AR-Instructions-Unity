using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetSetterController : MonoBehaviour
{
    private Vector3 position;
    public void MovmentStarted()
    {
        position = transform.localPosition;
    }
    public void Moved()
    {

        InstructionManager.Instance.OffsetChanged(transform.localPosition - position);
    }
}
