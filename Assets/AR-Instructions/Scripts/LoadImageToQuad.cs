using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class LoadImageToQuad : MonoBehaviour
{
    public Material DefaultMaterial;

    public void LoadImageToQuadByFileName(string fileName = null)
    {
        var videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.enabled = false;
        GetComponent<Interactable>().enabled = false;

        if (fileName != null)
        {
            //Find the Standard Shader
            Material myNewMaterial = new Material(Shader.Find("Mixed Reality Toolkit/Standard"));
            //Set Texture on the material
            myNewMaterial.SetTexture("_MainTex", IMG2Sprite.instance.LoadTexture(Path.Combine(Application.persistentDataPath, "media" , fileName)));
            //Apply to GameObject
            GetComponent<MeshRenderer>().material = myNewMaterial;
        }
        else
        {
            GetComponent<MeshRenderer>().material = DefaultMaterial;
        }
    }

    public void LoadDefaultVideoImage(Material DefaultVideoMaterial)
    {
        GetComponent<MeshRenderer>().material = DefaultVideoMaterial;
    }
}
