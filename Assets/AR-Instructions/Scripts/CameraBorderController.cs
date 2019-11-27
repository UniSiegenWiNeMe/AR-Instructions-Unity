using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraBorderController : MonoBehaviour
{
    public Material Idle;
    public Material Recording;

    private bool _idle = true;

    public MeshRenderer Renderer;
    public TextMeshProUGUI Instruction;
    public TextMeshProUGUI Timer;

    public void ToggleMaterial()
    {
        
        if (Renderer)
        {
            _idle = !_idle;

            if (_idle)
            {
                Renderer.material = Idle;
            }
            else
            {
                Renderer.material = Recording;
            }
        }
    }

    public void SetIdleMaterial()
    {
       
        if (Renderer)
        {
            _idle = true;
            Renderer.material = Idle;
        }
    }


    public void SetRecordingMaterial()
    {
        if (Renderer)
        {
            _idle = false;
            Renderer.material = Recording;
        }
    }

    public void Show()
    {
        Renderer.enabled = true;
        Instruction.enabled = true;
        Timer.enabled = true;
    }
    public void Hide()
    {
        Renderer.enabled = false;
        Instruction.enabled = false;
        Timer.enabled = false;
    }

}
