﻿using System.Collections.Generic;
using UnityEngine;

public class NeedleConnector : AttachmentConnector {


    #region Fields
    public override ItemConnection Connection { get; set; }

    protected override InteractState AttachState => InteractState.NeedleAttached;
    #endregion

    public NeedleConnector(Needle needle, GameObject collider) : base(needle.transform) {
        GeneralItem = needle;
        attached = new AttachedObject();
        this.Collider = collider;
    }

    #region Attaching
    public override void ConnectItem(Interactable interactable) {

        if ((interactable as Syringe) is var syringe && syringe != null && syringe.HasSyringeCap) {
            Logger.Warning("Trying to attach needle to syringe with a cap");
            return;
        }
        Logger.Print("Connect item to needle: " + interactable.name);

        bool itemGrabbed = interactable.State == InteractState.Grabbed;
        Hand itemHand = itemGrabbed ? Hand.GrabbingHand(interactable) : null;

        if (itemGrabbed) {
            interactable.GetComponent<ItemConnection>().Remove();
        }

        ReplaceObject(interactable?.gameObject);

        if (itemGrabbed) {
            itemHand.InteractWith(interactable, false);
        }
    }

    protected override void SetInteractors() {
        attached.Interactable.Interactors.SetNeedle(GeneralItem as Needle);
    }

    protected override void AttachEvents(GameObject intObject) {
        AudioManager.Play(AudioClipType.LockedItem);
    }

    protected override void SnapObjectPosition() {
        Transform obj = attached.GameObject.transform;
        Transform coll = Collider.transform;
        Transform luerlockPos = LuerlockAdapter.LuerlockPosition(obj);

        Vector3 offset = coll.position - luerlockPos.position;
        GeneralItem.transform.position -= offset;
    }
    #endregion

    #region Releasing
    public override void OnReleaseItem() {
        attached.Interactable.Interactors.ResetNeedle();

        // Attach state might need to change
        attached.Interactable.State.Off(AttachState);
        ReplaceObject(null);
    }
    #endregion
}
