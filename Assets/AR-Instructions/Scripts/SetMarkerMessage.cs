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
        stabilizedTracking.MarkerReset += StabilizedTracking_MarkerReset;
    }

    private void StabilizedTracking_MarkerReset(object sender, System.EventArgs e)
    {
        var text = GetComponent<TextMeshPro>();
        if (text != null)
        {
            text.text = "Marker wird gescannt."+System.Environment.NewLine+" Bitte warten.";
        }
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
