﻿using System;

public class CloseBottles : Task {

    public enum Conditions { BottlesClosed }
    private int openedBottles = 0;

    public CloseBottles() : base(TaskType.CloseBottles, true) {
        SetCheckAll(true);
        AddConditions((int[])Enum.GetValues(typeof(Conditions)));
    }

    public override void Subscribe() {
        base.SubscribeEvent(TrackClosedBottles, EventType.BottleClosed);
        base.SubscribeEvent(TrackOpenedBottles, EventType.BottleOpened);
    }

    private void TrackClosedBottles(CallbackData data) {
        openedBottles--;
        if (Started) {
            if (openedBottles == 0) {
                EnableCondition(Conditions.BottlesClosed);
                CompleteTask();
            }
        }
    }

    private void TrackOpenedBottles(CallbackData data) {
        openedBottles++;
    }
}
