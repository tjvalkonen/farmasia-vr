﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WritingPen : GeneralItem {

    private bool isWriting;

    protected override void Start() {
        base.Start();
        objectType = ObjectType.Pen;
        Type.On(InteractableType.Interactable);
    }

    protected override void OnCollisionEnter(Collision other) {
        base.OnCollisionEnter(other);

        // Get the Writable component of the collided item
        GameObject foundObject = GetInteractableObject(other.transform);
        Writable writable = foundObject?.GetComponent<WritingTarget>()?.GetWritable();
        if (writable == null) {
            return;
        }
        if (!base.IsGrabbed) { // prevent accidental writing when pen not grabbed
            return;
        }
        if (isWriting) { // must submit or cancel before selecting another item
            return;
        }

        Write(writable, foundObject);
    }

    private void Write(Writable writable, GameObject foundObject) {
        
        // Find the writing options object, make it visible, set a callback for when user submits text from the options
        
        var writingGameObject = GameObject.Find("WritingOptions");
        if (writingGameObject == null) {
            Logger.Print("Writing options not found");
            return;
        }
        Logger.Print("Opened writing options");
        var writingOptions = writingGameObject.GetComponent<WritingOptions>();
        if (writingOptions == null) {
            Logger.Print("Did not found gameObject writing options");
            return;
        }
        isWriting = true;

        // Now show it
        writingOptions.SetVisible(true);
        // Give it the writable's info (text and maximum number of lines)
        writingOptions.SetWritable(writable);

        // Set the callback so that it writes to the writable when it is submitted
        writingOptions.onSubmit = (selectedOptions) => {
            SubmitWriting(writable, foundObject, selectedOptions);
            isWriting = false;
        };
        writingOptions.onCancel = () => {
            isWriting = false;
        };
    }

    public void SubmitWriting(Writable writable, GameObject foundObject, Dictionary<WritingType, string> selectedOptions) {
        writable.AddWrittenLines(selectedOptions);
        Events.FireEvent(EventType.WriteToObject, CallbackData.Object(foundObject));
    }
}
