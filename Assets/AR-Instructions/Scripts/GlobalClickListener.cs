// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Script used to handle input action events. Invokes Unity events when the configured input action starts or ends. 
/// </summary>
public class GlobalClickListener : MonoBehaviour, IMixedRealityInputActionHandler
{
    private bool _listening;

    public bool listening
    {
        get { return _listening; }
        set { _listening = value; }
    }

    [SerializeField]
    [Tooltip("Input Action to handle")]
    private MixedRealityInputAction InputAction = MixedRealityInputAction.None;

    [SerializeField]
    [Tooltip("Whether input events should be marked as used after handling so other handlers in the same game object ignore them")]
    private bool MarkEventsAsUsed = false;

    /// <summary>
    /// Unity event raised on action start, e.g. button pressed or gesture started. 
    /// Includes the input event that triggered the action.
    /// </summary>
    public InputActionUnityEvent OnInputActionStarted;

    /// <summary>
    /// Unity event raised on action end, e.g. button released or gesture completed.
    /// Includes the input event that triggered the action.
    /// </summary>
    public InputActionUnityEvent OnInputActionEnded;

    void IMixedRealityInputActionHandler.OnActionStarted(BaseInputEventData eventData)
    {
        if (eventData.MixedRealityInputAction == InputAction && !eventData.used && _listening)
        {
            OnInputActionStarted.Invoke(eventData);
            if (MarkEventsAsUsed)
            {
                eventData.Use();
            }
        }
    }
    void IMixedRealityInputActionHandler.OnActionEnded(BaseInputEventData eventData)
    {
        if (eventData.MixedRealityInputAction == InputAction && !eventData.used && _listening)
        {
            OnInputActionEnded.Invoke(eventData);
            if (MarkEventsAsUsed)
            {
                eventData.Use();
            }
        }
    }

    public void SetListening(bool value)
    {
        listening = value;
    }

    public void OnEnable()
    {
        MixedRealityToolkit.InputSystem.RegisterHandler<IMixedRealityInputActionHandler>(this);
    }

    public void OnDisable()
    {
        MixedRealityToolkit.InputSystem?.UnregisterHandler<IMixedRealityInputActionHandler>(this);
    }
}
