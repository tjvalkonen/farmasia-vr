﻿using System;
using UnityEngine;

public class HandConnector : ItemConnector {

    #region Fields
    public Hand Hand { get; private set; }

    public bool IsGrabbed { get => GrabbedInteractable != null; }

    public Interactable GrabbedInteractable { get; private set; }

    public override ItemConnection Connection { get; set; }
    #endregion

    public HandConnector(Hand hand) : base(hand.transform) {
        Hand = hand;
    }

    #region Attaching
    public override void ConnectItem(Interactable interactable) {

        Logger.Print("Hand Connector connect item: " + interactable.name);

        if (interactable.Rigidbody == null) {
            Logger.Warning("Interactable has no rigidbody");
        }

        // release item from other hand
        bool isGrabbingSameObject = interactable == Hand.Other.Connector.GrabbedInteractable;
        if (isGrabbingSameObject) {
            Hand.GrabbingHand(interactable).Connector.Connection.Remove();
        }

        Logger.PrintVariables("set " + Hand.HandType + " interactable", interactable.name);
        GrabbedInteractable = interactable;
        GrabbedInteractable.State.On(InteractState.Grabbed);
        GrabbedInteractable.Interactors.SetHand(Hand);

        Events.FireEvent(EventType.PickupObject, CallbackData.Object(GrabbedInteractable.gameObject));
        AttachGrabbedItem(GrabbedInteractable);

        Logger.Print("After attach: " + Connection + ", type: " + Connection.GetType());
    }

    private bool AllowSmoothAttach(Interactable interactable) {
        if (interactable.Type != InteractableType.SmallObject) {
            return false;
        }

        bool isAttachedToLuerlock = interactable.State == InteractState.LuerlockAttached;
        LuerlockAdapter luerlock = isAttachedToLuerlock
                                    ? interactable.Interactors.LuerlockPair.Value
                                    : interactable as LuerlockAdapter;
        return luerlock == null || !luerlock.HasAttachedObjects;
    }

    private void AttachGrabbedItem(Interactable interactable) {

        // #if UNITY_NONVRCOMPUTER
        // Connection = ItemConnection.AddRigidConnection(this, Hand.Offset, interactable.gameObject);
        // #else

        Logger.Print("StartIf---------------------------------------------");

        if (interactable.IsAttached) {
            Logger.Print("Interactable is attached");

            if (interactable.State == InteractState.LuerlockAttached) {
                Logger.Print("Interactable is Luerlock attached");

                LuerlockAdapter luerlock = interactable.Interactors.LuerlockPair.Value;


                if (luerlock.State == InteractState.Grabbed) {
                    Logger.Print("Luerlock is grabbed");

                    ConnectionHandler.GrabLuerlockAttachedItemWhenLuerlockIsGrabbed(this, Hand.transform, interactable);
                } else {
                    Logger.Print("Luerlock not grabbed");

                    if (luerlock.GrabbedObjectCount == 2) {
                        Logger.PrintVariables("Grabbing both items, grabbedCount", luerlock.GrabbedObjectCount);
                        ConnectionHandler.GrabLuerlockAttachedItemWhenOtherLuerlockAttachedItemIsGrabbed(this, Hand.transform, interactable);
                    } else {
                        Logger.Print("Only grabbing Luerlock item");
                        ConnectionHandler.GrabLuerlockAttachedItem(this, Hand.transform, interactable);
                    }
                }
            } else if (interactable.State == InteractState.NeedleAttached) {
                Logger.Print("Interactable is Needle attached");

                Needle needle = interactable.Interactors.Needle;

                if (needle.State == InteractState.Grabbed) {
                    Logger.Print("Needle is grabbed");
                    ConnectionHandler.GrabLuerlockAttachedItemWhenLuerlockIsGrabbed(this, Hand.transform, interactable);
                } else {
                    Logger.Print("Needle not grabbed");
                    ConnectionHandler.GrabLuerlockAttachedItem(this, Hand.transform, interactable);
                }
            } else {
                throw new Exception("Interactrable State is attached but not attached");
            }


        } else if (interactable as LuerlockAdapter is var luerlock && luerlock != null) {
            Logger.Print("Interactable is Luerlock");
            if (luerlock.GrabbedObjectCount > 0) {
                Logger.Print("Luerlock has items");
                ConnectionHandler.GrabLuerlockWhenAttachedItemsAreGrabbed(this, Hand.transform, luerlock);
            } else {
                Logger.Print("Luerlock does not have items");
                ConnectionHandler.GrabItem(this, Hand.Offset, luerlock);
            }
        } else {
            Logger.Print("Regular grab item");
            ConnectionHandler.GrabItem(this, Hand.Offset, interactable);
        }

        Logger.Print("EndIf---------------------------------------------");
        // #endif
    }
    #endregion

    #region Releasing
    public override void OnReleaseItem() {
        if (!IsGrabbed) {
            Logger.Error("ReleaseItem(): Invalid state (is not grabbíng)");
            return;
        }

        if (GrabbedInteractable.State != InteractState.Grabbed) {
            Logger.Error("ReleaseItem(): Invalid state (item is not grabbed)");
            return;
        }


        if (GrabbedInteractable.Rigidbody) {
            GrabbedInteractable.Rigidbody.velocity = VRInput.Skeleton(Hand.HandType).velocity;
            GrabbedInteractable.Rigidbody.angularVelocity = VRInput.Skeleton(Hand.HandType).angularVelocity;
        }

        Logger.PrintVariables("unset " + Hand.HandType + " interactable", GrabbedInteractable.name);
        GrabbedInteractable.State.Off(InteractState.Grabbed);
        Logger.Print("AKA. IsGrabbed = false");

        SafeRelease();

        GrabbedInteractable = null;
    }
    private void SafeRelease() {

        if (GrabbedInteractable.State == InteractState.LuerlockAttached) {

            Logger.Print("Releasing luerlock attached item");

            LuerlockAdapter l = GrabbedInteractable.Interactors.LuerlockPair.Value;

            Logger.PrintVariables("Grabbed count", l.GrabbedObjectCount);

            if (l.GrabbedObjectCount > 0) {
                ConnectionHandler.ReleaseLuerlockAttachedItemWhenOtherLuerlockAttachedItemIsGrabbed(GrabbedInteractable, l);
                return;
            }
        }

        if (GrabbedInteractable as LuerlockAdapter is var luerlock && luerlock != null) {
            Logger.Print("Releasing luerlock");
            Logger.PrintVariables("grabbed Object count", luerlock.GrabbedObjectCount);
            if (luerlock.GrabbedObjectCount > 0) {
                Logger.Print("object count was 1");
                ConnectionHandler.ReleaseLuerlockWhenLuerlockAttachedItemIsGrabbed(luerlock);
            }
        }
    }
    #endregion
}
