using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNewItem : MonoBehaviour
{
    public GameObject ItemToSpawn;
    public Transform Parent;
    public bool isToolTipActive = false;

    public event EventHandler<ObjectSpawnedEventArgs> ObjectSpawned;


    public void Spawn()
    {
        var PositionToSpawn =  transform.position- (transform.forward*0.3f);
        var RotationToSpawn = transform.rotation;
        var item = Instantiate(ItemToSpawn, Parent);

        var visualTransform = item.transform.Find("Visual");
        visualTransform.position = PositionToSpawn;
        visualTransform.rotation = RotationToSpawn;

        visualTransform.gameObject.GetComponentInChildren<MeshRenderer>().material = GetComponentInChildren<MeshRenderer>().material;

        item.GetComponentInChildren<BoundingBox>().Active = true;

        if(!isToolTipActive)
        {
            item.GetComponentInChildren<ToolTip>().gameObject.SetActive(false);
        }

        ObjectSpawned?.Invoke(this, new ObjectSpawnedEventArgs(item));
    }
}

public class ObjectSpawnedEventArgs:EventArgs
{
    public GameObject SpawnedObject;

    public ObjectSpawnedEventArgs(GameObject spawnedObject)
    {
        this.SpawnedObject = spawnedObject;
    }
}