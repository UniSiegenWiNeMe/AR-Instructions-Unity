using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetMarkerMessage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var stabilizedTracking = GetComponentInParent<StabilizedTracking>();
        stabilizedTracking.MarkerScanned += StabilizedTracking_MarkerScanned;
    }

    private void StabilizedTracking_MarkerScanned(object sender, MarkerScannedEventArgs e)
    {
        var text = GetComponent<TextMeshPro>();
        if (text != null)
        {
            text.text = "Marker erfolgreich gescannt.";
        }
    }

    
}
