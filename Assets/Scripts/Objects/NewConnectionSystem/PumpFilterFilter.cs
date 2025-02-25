﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpFilterFilter : FilterPart {
    public bool CanBeCut { 
        get {
            // Logger.Print(Attached + " " + ConnectedItem);
            return !isCut; 
        } 
    }
    private bool isCut = false;

    public void Cut(Transform bladeTransform) {

        RotateToBlade(bladeTransform);

        transform.GetChild(0).gameObject.SetActive(false); // Uncut filter
        var leftHalf = transform.GetChild(2).gameObject;
        var rightHalf = transform.GetChild(3).gameObject;
        leftHalf.SetActive(true);
        rightHalf.SetActive(true);
        transform.DetachChildren();

        isCut = false;
        Events.FireEvent(EventType.FilterCutted, CallbackData.Object(this));
    }

    // This does not really work, yet!
    private void RotateToBlade(Transform bladeTransform) {
        var bladeDown = bladeTransform.forward;
        var relativeBladeDirection = Vector3.ProjectOnPlane(bladeDown, transform.up);
        var angleDifference = Vector3.Angle(relativeBladeDirection, transform.right);
        transform.Rotate(0f, angleDifference, 0f, Space.Self);
    }
}
