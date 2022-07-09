﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandStateManager : MonoBehaviour {

    private HandEffectSpawner[] spawners;
    public Material material;

    public void Start() {
        spawners = FindObjectsOfType<HandEffectSpawner>();
        Subscribe();
    }

    public void Subscribe() {
        Events.SubscribeToEvent(WashingHands, EventType.WashingHands);
        Events.SubscribeToEvent(OpenedDoor, EventType.RoomDoor);
    }

    private void WashingHands(CallbackData data) {
        var liquid = (data.DataObject as HandWashingLiquid);
        if (liquid.type.Equals("Soap")) SetDirty();
        if (liquid.type.Equals("Water")) SetClean();
        if (liquid.type.Equals("HandSanitizer")) SetShiny();
    }

    private void OpenedDoor(CallbackData data) {
        SetDefault();
    }

    public void SetDirty() {
        material.SetFloat("_StepEdge", 0.05f);
    }

    public void SetClean() {
        spawners[0].StartSpawning("SoapBubble", 0.01f);
        spawners[1].StartSpawning("SoapBubble", 0.01f);
        StartCoroutine(Lerp(0.05f, 0.6f, 6.0f, "_StepEdge"));
    }

    public void SetShiny() {
        spawners[0].StartSpawning("LensFlare", 1.2f);
        spawners[1].StartSpawning("LensFlare", 1.2f);
        material.SetInt("_Shiny", 1);
        StartCoroutine(Lerp(10.0f, 2.0f, 1.0f, "_FresnelEffectPower"));
    }

    public void SetDefault() {
        material.SetFloat("_StepEdge", 0.6f);
        material.SetInt("_Shiny", 0);
        material.SetFloat("_FresnelEffectPower", 10.0f);
    }

    private IEnumerator Lerp(float a, float b, float duration, string property) {
        float elapsed = 0.0f;
        while (elapsed < duration) {
            material.SetFloat(property, Mathf.Lerp(a, b, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
