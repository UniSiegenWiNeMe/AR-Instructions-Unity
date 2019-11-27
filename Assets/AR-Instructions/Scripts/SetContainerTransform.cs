using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetContainerTransform : MonoBehaviour
{
    public StabilizedTracking StabilizedTracking;

    public void Start()
    {
        StabilizedTracking.MarkerScanned += StabilizedTracking_MarkerScanned;
    }

    private void StabilizedTracking_MarkerScanned(object sender, MarkerScannedEventArgs args)
    {
        transform.SetPositionAndRotation(args.Position, args.Rotation);
    }
}
