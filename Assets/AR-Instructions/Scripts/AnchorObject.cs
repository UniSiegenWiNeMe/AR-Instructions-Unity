using UnityEngine;
using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Experimental.Utilities;

// Script from https://github.com/Microsoft/MixedRealityToolkit-Unity/issues/937 to manage World Anchors
// 
public class AnchorObject : MonoBehaviour
{
    public string AnchorName = "123456789";
    public bool Undo = false;
    public bool SetAnchorOnStart;

    private Vector3 OldPosition;
    private Quaternion OldRotation;

    public WorldAnchorManager worldAnchorManager;

    private void Awake()
    {
        if (!worldAnchorManager)
        {
            worldAnchorManager = FindObjectOfType<WorldAnchorManager>();
        }
    }

    void Start()
    {
        OldPosition =  this.gameObject.transform.position;
        OldRotation =  this.gameObject.transform.rotation;

        if (SetAnchorOnStart)
        {
            worldAnchorManager.AttachAnchor(this.gameObject, AnchorName);
            Debug.Log("Anchor attached for: " + this.gameObject.name + " - AnchorID: " + AnchorName);
        }
    }

    void Update()
    {
        if (Undo)
        {
            worldAnchorManager.RemoveAnchor(this.gameObject);
            RestoreBackupAnchor();
            Undo = false;
        }
    }

    public void RestoreBackupAnchor()
    {
        this.gameObject.transform.position = OldPosition;
        this.gameObject.transform.rotation = OldRotation;
        worldAnchorManager.AttachAnchor(this.gameObject, AnchorName);
    }

    public void AttachAnchor()
    {
        if (this.enabled)
        {
            OldPosition =  this.gameObject.transform.position;
            OldRotation =  this.gameObject.transform.rotation;

            worldAnchorManager.AttachAnchor(this.gameObject, AnchorName);
            Debug.Log("Anchor attached for: " + this.gameObject.name + " - AnchorID: " + AnchorName);
        }
    }

    

    public void RemoveAnchor()
    {
        if (this.enabled)
        {
            OldPosition =  this.gameObject.transform.position;
            OldRotation =  this.gameObject.transform.rotation;
            worldAnchorManager.RemoveAnchor(this.gameObject);
        }
    }
/*
    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        //Debug.LogFormat("OnManipulationStarted\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
        //    eventData.InputSource,
        //    eventData.SourceId,
        //    eventData.CumulativeDelta.x,
        //    eventData.CumulativeDelta.y, 
        //    eventData.CumulativeDelta.z);

        worldAnchorManager.RemoveAnchor(this.gameObject);
        Debug.Log("OnManipulationStarted - Anchor Removed");
        OldPosition = SavedPosition;
        OldRotation = SavedRotation;
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        //if (LogGesturesUpdateEvents)
        //{
        //    Debug.LogFormat("OnManipulationUpdated\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
        //        eventData.InputSource,
        //        eventData.SourceId,
        //        eventData.CumulativeDelta.x,
        //        eventData.CumulativeDelta.y,
        //        eventData.CumulativeDelta.z);
        //}
    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        //Debug.LogFormat("OnManipulationCompleted\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
        //    eventData.InputSource,
        //    eventData.SourceId,
        //    eventData.CumulativeDelta.x,
        //    eventData.CumulativeDelta.y,
        //    eventData.CumulativeDelta.z);

        worldAnchorManager.AttachAnchor(this.gameObject, AnchorName);
        Debug.Log("OnManipulationCompleted - Anchor Attached");
    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
        //    Debug.LogFormat("OnManipulationCanceled\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
        //        eventData.InputSource,
        //        eventData.SourceId,
        //        eventData.CumulativeDelta.x,
        //        eventData.CumulativeDelta.y,
        //        eventData.CumulativeDelta.z);
    }

    */
}

