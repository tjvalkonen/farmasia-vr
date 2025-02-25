﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class OpenScalpelCover : Task {
    public enum Conditions { OpenedScalpelCover }
    private CabinetBase laminarCabinet;

    public OpenScalpelCover() : base(TaskType.OpenScalpelCover, false) {
        SetCheckAll(true);
        
        AddConditions((int[])Enum.GetValues(typeof(Conditions)));
    }

    public override void Subscribe() {
        base.SubscribeEvent(ScalpelCoverOpened, EventType.ScalpelCoverOpened);
        base.SubscribeEvent(WrongSpotOpened, EventType.WrongSpotOpened);
        base.SubscribeEvent(SetCabinetReference, EventType.ItemPlacedForReference);
    }

    private void SetCabinetReference(CallbackData data) {
        CabinetBase cabinet = (CabinetBase)data.DataObject;
        if (cabinet.type == CabinetBase.CabinetType.Laminar) {
            laminarCabinet = cabinet;
            base.UnsubscribeEvent(SetCabinetReference, EventType.ItemPlacedForReference);
        }
    }
    private void ScalpelCoverOpened(CallbackData data) {
        var scalpel = (data.DataObject as Scalpel);
        CheckIfInsideLaminarCabinet(scalpel);
        EnableCondition(Conditions.OpenedScalpelCover);
        CompleteTask();
    }

    private void CheckIfInsideLaminarCabinet(Interactable interactable) {
        if (laminarCabinet.GetContainedItems().Contains(interactable)) {
            return;
        } else {
            CreateTaskMistake("Avasit suojamuovin laminaarikaapin ulkopuolella!!!", 1);
        }

        if (laminarCabinet.GetContainedItems() == null) {
            CreateTaskMistake("Avasit suojamuovin laminaarikaapin ulkopuolella!!!", 1);
        }
    }

    public void WrongSpotOpened(CallbackData data) {
        CreateTaskMistake("Avasit suojamuovin väärästä päästä! Muista aina avata aseptiset ja terävät työvälineet oikeasta päästä!", 1);
    }
}
