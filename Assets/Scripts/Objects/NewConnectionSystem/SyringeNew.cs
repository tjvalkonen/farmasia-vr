﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyringeNew : ReceiverItem {

    public LiquidContainer Container;

    // How much liquid is moved per click
    public int LiquidTransferStep = 50;

    public float defaultPosition, maxPosition;

    public Transform handle;

    // The LiquidContainer this Pipette is interacting with
    public LiquidContainer BottleContainer { get; set; }

    public bool hasBeenInBottle;

    [SerializeField]
    private ItemDisplay display;
    protected override void Start() {
        base.Start();

        Container = LiquidContainer.FindLiquidContainer(transform);

        Type.On(InteractableType.Interactable);

        Container.OnAmountChange += SetSyringeHandlePosition;
        SetSyringeHandlePosition();

        AfterRelease = (interactable) => {
            Logger.Print("Syringe disassembled!");
            Events.FireEvent(EventType.SyringeDisassembled, CallbackData.Object((this, interactable)));
        };
    }

    public override void OnGrabStart(Hand hand) {
        base.OnGrabStart(hand);

        display.EnableDisplay();
    }

    public override void OnGrabEnd(Hand hand) {
        base.OnGrabEnd(hand);

        if (State != InteractState.LuerlockAttached && State != InteractState.Grabbed) {
            display.DisableDisplay();
        }
    }

    public override void OnGrab(Hand hand) {
        base.OnGrab(hand);

        bool takeMedicine = VRInput.GetControlDown(hand.HandType, Controls.TakeMedicine);
        bool sendMedicine = VRInput.GetControlDown(hand.HandType, Controls.EjectMedicine);

        int liquidAmount = 0;

        if (takeMedicine) liquidAmount -= LiquidTransferStep;
        if (sendMedicine) liquidAmount += LiquidTransferStep;
        if (liquidAmount == 0) return;

        if (SlotOccupied) {
            Logger.Print("Cap is on");
            return;
        }

        if (takeMedicine) {
            TakeMedicine(liquidAmount);
        } else if (sendMedicine) {
            SendMedicine(liquidAmount);
        }

    }

    public void TakeMedicine(int amount) {
        Logger.Print("INTERACT STATE: " + State);
        if (State == InteractState.InBottle) {
            TransferToBottle(amount);
            Events.FireEvent(EventType.TakingMedicineFromBottle, CallbackData.Object(this));
        } else {
            Logger.Print("PipetteContainer not in bottle");
        }
    }

    public void SendMedicine(int amount) {
        Logger.Print("INTERACT STATE: " + State);
        if (State == InteractState.InBottle) {
            TransferToBottle(amount);
            Events.FireEvent(EventType.TakingMedicineFromBottle, CallbackData.Object(this));
        } else {
            Eject();
        }
    }

    private void Eject() {
        Container.SetAmount(0);
    }

    private void TransferToBottle(int amount) {
        if (BottleContainer == null) return;
        //if (Vector3.Angle(-BottleContainer.transform.up, transform.up) > 25) return;

        Container.TransferTo(BottleContainer, amount);
    }

    public void SetSyringeHandlePosition() {
        Vector3 pos = handle.localPosition;
        pos.y = SyringePos();
        handle.localPosition = pos;
    }

    private float SyringePos() {
        return Factor * (maxPosition - defaultPosition);
    }

    private float Factor {
        get {
            return 1.0f * Container.Amount / Container.Capacity;
        }
    }
}