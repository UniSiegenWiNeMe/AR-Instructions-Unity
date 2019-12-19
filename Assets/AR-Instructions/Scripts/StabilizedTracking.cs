using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class StabilizedTracking : MonoBehaviour, ITrackableEventHandler
{
    /// <summary>
    /// Number of Frames to capure the marker
    /// </summary>
    public int TargetCount = 100;

    public bool IsMarkerScanned
    {
        get;
        private set;
    }
    public Vector3 MarkerPosition
    {
        get;
        private set;
    }
    public Quaternion MarkerRotation
    {
        get;
        private set;
    }

    public event EventHandler<MarkerScannedEventArgs> MarkerScanned;
    public event EventHandler MarkerReset;

    protected TrackableBehaviour TrackableBehaviour;
    protected TrackableBehaviour.Status PreviousStatus;
    protected TrackableBehaviour.Status NewStatus;

    private List<Transform> _rawTransformations;
    private bool _transformSet = false;

    protected virtual void Start()
    {
        TrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (TrackableBehaviour)
            TrackableBehaviour.RegisterTrackableEventHandler(this);

        _rawTransformations = new List<Transform>();
    }

    protected virtual void OnDestroy()
    {
        if (TrackableBehaviour)
            TrackableBehaviour.UnregisterTrackableEventHandler(this);
    }


    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged( TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        PreviousStatus = previousStatus;
        NewStatus = newStatus;

        Debug.Log("Trackable " + TrackableBehaviour.TrackableName +
                  " " + TrackableBehaviour.CurrentStatus);

        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NO_POSE)
        {
            OnTrackingLost();
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }
    }

    internal void Reset()
    {
        IsMarkerScanned = false;
        MarkerPosition = new Vector3();
        MarkerRotation = new Quaternion();
        _rawTransformations = new List<Transform>();
        _transformSet = false;

        VuforiaBehaviour.Instance.enabled = true;

        MarkerReset?.Invoke(this, null);
    }

    public void Update()
    {
        if(NewStatus == TrackableBehaviour.Status.TRACKED && _rawTransformations.Count <= TargetCount)
        {
            _rawTransformations.Add(this.transform);

            if (_rawTransformations.Count == TargetCount && !_transformSet)
            {
                CalculateMedian(_rawTransformations);
            }
        }

    }

    private void CalculateMedian(List<Transform> rawTransformations)
    {
        _transformSet = true;

        List<float> positionX = new List<float>();
        List<float> positionY = new List<float>();
        List<float> positionZ = new List<float>();
        List<float> rotationX = new List<float>();
        List<float> rotationY = new List<float>();
        List<float> rotationZ = new List<float>();
        List<float> rotationW = new List<float>();

        foreach (var transformation in rawTransformations)
        {
            positionX.Add(transformation.position.x);
            positionY.Add(transformation.position.y);
            positionZ.Add(transformation.position.z);
            rotationX.Add(transformation.rotation.x);
            rotationY.Add(transformation.rotation.y);
            rotationZ.Add(transformation.rotation.z);
            rotationW.Add(transformation.rotation.w);
        }

        List<List<float>> allLists = new List<List<float>>() { positionX, positionX, positionX, positionX, positionX, positionX, positionX, };

        foreach (var list in allLists)
        {
            list.Sort();
        }

        Vector3 position = new Vector3(positionX[TargetCount / 2], positionY[TargetCount / 2], positionZ[TargetCount / 2]);
        Quaternion rotation = new Quaternion(rotationX[TargetCount / 2], rotationY[TargetCount / 2], rotationZ[TargetCount / 2], rotationW[TargetCount / 2]);

        IsMarkerScanned = true;
        MarkerPosition = position;
        MarkerRotation = rotation;
        VuforiaBehaviour.Instance.enabled = false;

        MarkerScanned?.Invoke(this, new MarkerScannedEventArgs(position, rotation));

        
    }

    protected virtual void OnTrackingFound()
    {
        
    }


    protected virtual void OnTrackingLost()
    {
        
    }

}

public class MarkerScannedEventArgs:EventArgs
{
    public Vector3 Position;
    public Quaternion Rotation;

    public MarkerScannedEventArgs(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}