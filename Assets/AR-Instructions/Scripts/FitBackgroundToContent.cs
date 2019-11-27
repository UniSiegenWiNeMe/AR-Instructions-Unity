using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FitBackgroundToContent : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Show the opaque background of tooltip.")]
    private bool showBackground = true;

    /// <summary>
    /// Show the opaque background of tooltip.
    /// </summary>
    public bool ShowBackground
    {
        get { return showBackground; }
        set { showBackground = value; }
    }

    [SerializeField]
    [Tooltip("GameObject text that is displayed on the tooltip.")]
    private GameObject label;

    [SerializeField]
    [Tooltip("Parent of the Text and Background")]
    private GameObject contentParent;


    [TextArea]
    [SerializeField]
    [Tooltip("Text for the ToolTip to display")]
    private string toolTipText;

    /// <summary>
    /// Text for the ToolTip to display
    /// </summary>
    public string ToolTipText
    {
        set
        {
            if (value != toolTipText)
            {
                toolTipText = value;
                RefreshLocalContent();
            }
        }
        get { return toolTipText; }
    }


    [SerializeField]
    [Tooltip("The padding around the content (height / width)")]
    private Vector2 backgroundPadding = Vector2.zero;

    [SerializeField]
    [Tooltip("The offset of the background (x / y / z)")]
    private Vector3 backgroundOffset = Vector3.zero;


    [SerializeField]
    [Range(0.01f, 3f)]
    [Tooltip("The scale of all the content (label, backgrounds, etc.)")]
    private float contentScale = 1f;

    /// <summary>
    /// The scale of all the content (label, backgrounds, etc.)
    /// </summary>
    public float ContentScale
    {
        get { return contentScale; }
        set
        {
            contentScale = value;
            RefreshLocalContent();
        }
    }

    
    [SerializeField]
    [Range(10, 60)]
    [Tooltip("The font size of the tooltip.")]
    private int fontSize = 30;
    private int prevTextLength;
    private int prevTextHash;
    private TextMeshPro cachedLabelText;
    private Vector2 localContentSize;
    private List<IToolTipBackground> backgrounds = new List<IToolTipBackground>();
    

    /// <summary>
    /// getter/setter for size of tooltip.
    /// </summary>
    public Vector2 LocalContentSize => localContentSize;
    
    /// <summary>
    /// The offset of the background (x / y / z)
    /// </summary>
    public Vector3 LocalContentOffset => backgroundOffset;


    protected virtual void RefreshLocalContent()
    {
        // Set the scale of the pivot
        contentParent.transform.localScale = Vector3.one * contentScale;
        label.transform.localScale = Vector3.one * 0.005f;
        // Set the content using a text mesh by default
        // This function can be overridden for tooltips that use Unity UI

        // Has content changed?
        int currentTextLength = toolTipText.Length;
        int currentTextHash = toolTipText.GetHashCode();

        // If it has, update the content
        if (currentTextLength != prevTextLength || currentTextHash != prevTextHash)
        {
            prevTextHash = currentTextHash;
            prevTextLength = currentTextLength;

            if (cachedLabelText == null)
                cachedLabelText = label.GetComponent<TextMeshPro>();

            if (cachedLabelText != null && !string.IsNullOrEmpty(toolTipText))
            {
                cachedLabelText.fontSize = fontSize;
                cachedLabelText.text = toolTipText.Trim();
                // Force text mesh to use center alignment
                cachedLabelText.alignment = TextAlignmentOptions.CenterGeoAligned;
                // Update text so we get an accurate scale
                cachedLabelText.ForceMeshUpdate();
                // Get the world scale of the text
                // Convert that to local scale using the content parent
                Vector3 localScale = Vector3.Scale(cachedLabelText.transform.lossyScale / contentScale, cachedLabelText.textBounds.size);
                localContentSize.x = localScale.x + backgroundPadding.x;
                localContentSize.y = localScale.y + backgroundPadding.y;
            }

            foreach (IToolTipBackground background in backgrounds)
            {
                background.OnContentChange(localContentSize, LocalContentOffset, contentParent.transform);
            }
        }

        foreach (IToolTipBackground background in backgrounds)
        {
            background.IsVisible = showBackground;
        }

        
    }


    public void Update()
    {
        RefreshLocalContent();
    }


    /// <summary>
    /// virtual functions
    /// </summary>
    protected virtual void OnEnable()
    {
        // Make sure the tool tip text isn't empty
        if (string.IsNullOrEmpty(toolTipText))
            toolTipText = " ";

        backgrounds.Clear();
        foreach (IToolTipBackground background in GetComponents(typeof(IToolTipBackground)))
        {
            backgrounds.Add(background);
        }

        
        RefreshLocalContent();

        ShowBackground = showBackground;
    }

}
