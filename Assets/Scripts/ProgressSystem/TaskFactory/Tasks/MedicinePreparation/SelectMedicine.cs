﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMedicine : TaskBase  {

    private string[] conditions = {"BottlePickup"};

    public SelectMedicine() : base(TaskType.SelectMedicine, true, false) {
        Subscribe();
        AddConditions(conditions);
    }

    #region Event Subscriptions
    public override void Subscribe() {
        base.SubscribeEvent(PickupObject, EventType.PickupObject);
    }
    private void PickupObject(CallbackData data) {
        GameObject g = data.DataObject as GameObject;
        GeneralItem item = g.GetComponent<GeneralItem>();
        if (item == null) {
            Logger.Print("was null");
            return;
        }
        ObjectType type = item.ObjectType;

        if (type == ObjectType.Bottle) {
            ToggleCondition("BottlePickup");
        }
        CheckClearConditions(true);
    }
    #endregion


    public override void FinishTask() {
        Logger.Print("All conditions fulfilled, task finished!");
        base.FinishTask();
    }

    public override string GetDescription() {
        return "Valitse sopiva lääkepullo.";
    }
 
    public override string GetHint() {
        return "Jääkaapissa on erikokoisia lääkepulloja. Valitse näistä yksi.";
    }
}
