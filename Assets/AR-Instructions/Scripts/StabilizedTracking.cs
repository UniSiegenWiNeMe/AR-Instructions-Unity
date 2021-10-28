using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Vuforia;


public class StabilizedTracking : DefaultObserverEventHandler
{
    /// <summary>
    /// Number of Frames to capure the marker
    /// </summary>
    public int TargetCount = 100;
    private Transform mCentralAnchorPointTransform;
    private List<Transform> _rawTransformations;
    private bool _transformSet = false;
    private Status currentState;

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


    protected override void Start()
    {
        base.Start();

        IsMarkerScanned = false;
        MarkerPosition = new Vector3();
        MarkerRotation = new Quaternion();
        _rawTransformations = new List<Transform>();
        _transformSet = false;

        VuforiaApplication.Instance.OnVuforiaStarted += OnVuforiaStarted;
    }

    void OnVuforiaStarted()
    {
        mCentralAnchorPointTransform = VuforiaBehaviour.Instance.transform;
    }
    internal void Reset()
    {
        IsMarkerScanned = false;
        MarkerPosition = new Vector3();
        MarkerRotation = new Quaternion();
        _rawTransformations = new List<Transform>();
        _transformSet = false;

        VuforiaBehaviour.Instance.enabled = true;

        MarkerReset?.Invoke(this,null);
    }


    protected override void HandleTargetStatusChanged(Status previousStatus, Status newStatus)
    {
        currentState = newStatus;
    }

    public void Update()
    {
        if (currentState == Status.TRACKED && _rawTransformations.Count <= TargetCount)
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
}


public class MarkerScannedEventArgs : EventArgs
{
    public Vector3 Position;
    public Quaternion Rotation;

    public MarkerScannedEventArgs(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}