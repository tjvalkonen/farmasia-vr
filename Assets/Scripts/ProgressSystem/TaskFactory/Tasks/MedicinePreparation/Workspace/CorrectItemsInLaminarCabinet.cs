using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Correct amount of items inserted into Fume Cupboard.
/// </summary>
public class CorrectItemsInLaminarCabinet : TaskBase {
    #region Fields
    public enum Conditions { BigSyringe, SmallSyringes, Needles, Luerlock, RightSizeBottle }
    private int smallSyringes, needles;
    private int objectCount;
    private int checkTimes;
    private CabinetBase laminarCabinet;
    #endregion

    #region Constructor
    ///  <summary>
    ///  Constructor for CorrectItemsInLaminarCabinet task.
    ///  Is removed when finished and doesn't require previous task completion.
    ///  </summary>
    public CorrectItemsInLaminarCabinet() : base(TaskType.CorrectItemsInLaminarCabinet, true, false) {
        Subscribe();
        AddConditions((int[]) Enum.GetValues(typeof(Conditions)));
        smallSyringes = 0;
        needles = 0;
        objectCount = 0;
        checkTimes = 0;
        points = 2;
    }
    #endregion

    #region Event Subscriptions
    /// <summary>
    /// Subscribes to required Events.
    /// </summary>
    public override void Subscribe() { 
        base.SubscribeEvent(CorrectItems, EventType.CorrectItemsInLaminarCabinet);
    }
    /// <summary>
    /// Once fired by an event, checks which item was picked and sets the corresponding condition to be true.
    /// </summary>
    /// <param name="data">"Refers to the data returned by the trigger."</param>
    private void CorrectItems(CallbackData data) {
        List<GameObject> objects = data.DataObject as List<GameObject>;
        if (objects.Count == 0) {
            return;
        }
        checkTimes++;
        objectCount = objects.Count;

        foreach(GameObject value in objects) {
            GeneralItem item = value.GetComponent<GeneralItem>();
            ObjectType type = item.ObjectType;
            switch (type) {
                case ObjectType.Syringe:
                    Syringe syringe = item as Syringe;
                    if (syringe.Container.Capacity == 20) {
                        EnableCondition(Conditions.BigSyringe); 
                    } else if (syringe.Container.Capacity == 1) {
                        smallSyringes++;
                        if (smallSyringes == 6) {
                            EnableCondition(Conditions.SmallSyringes);
                        }
                    }
                    break;
                case ObjectType.Needle:
                    needles++;
                    if (needles == 7) {
                        EnableCondition(Conditions.Needles); 
                    }
                    break;
                case ObjectType.Luerlock:
                    EnableCondition(Conditions.Luerlock);
                    break;
                case ObjectType.Bottle:
                    MedicineBottle bottle = item as MedicineBottle;
                    if (bottle.Container.Capacity == 100) {
                        EnableCondition(Conditions.RightSizeBottle);
                    }
                    break;
            }
        }
        
        bool check = CheckClearConditions(true);
        if (!check) {
            if (checkTimes == 1) {
                UISystem.Instance.CreatePopup(-1, "Wrong amount of items", MessageType.Mistake);
                G.Instance.Progress.Calculator.Subtract(TaskType.CorrectItemsInLaminarCabinet);
            }
            smallSyringes = 0;
            needles = 0;
            DisableConditions();
        }
    } 
    #endregion

    #region Public Methods
    /// <summary>
    /// Once all conditions are true, this method is called.
    /// </summary>
    public override void FinishTask() {
        if (checkTimes == 1) {
            if (objectCount == 16) {
                UISystem.Instance.CreatePopup(1, "Right amount of items", MessageType.Notify);
            } else {
                UISystem.Instance.CreatePopup(0, "Too many items", MessageType.Notify);
            }
        }
        base.FinishTask();
    }
    
    /// <summary>
    /// Used for getting the task's description.
    /// </summary>
    /// <returns>"Returns a String presentation of the description."</returns>
    public override string GetDescription() {
        return "Tarkista valitsemiesi välineiden määrä.";
    }

    /// <summary>
    /// Used for getting the hint for this task.
    /// </summary>
    /// <returns>"Returns a String presentation of the hint."</returns>
    public override string GetHint() {
        return "Tarkista välineitä kaappiin viedessäsi, että olet valinnut oikean määrän välineitä ensimmäisellä hakukerralla."; 
    }
    #endregion
}