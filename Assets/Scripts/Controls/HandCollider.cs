﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCollider : MonoBehaviour {

    private static string iTag = "Interactable";
    public List<GameObject> GrabObjects { get; private set; }

    private void Start() {
        GrabObjects = new List<GameObject>();
    }

    public void OnTriggerEnter(Collider coll) {

        if (coll.gameObject.tag != iTag) {
            return;
        }

        GrabObjects.Add(coll.gameObject);

        ObjectHighlight hObject = coll.gameObject.GetComponent<ObjectHighlight>();
        hObject.StartCoroutine(hObject.InsideCheck(this));
    }
    public void OnTriggerExit(Collider coll) {

        if (coll.gameObject.tag != iTag) {
            return;
        }

        GrabObjects.Remove(coll.gameObject);
    }

    public bool Contains(GameObject obj) {
        return GrabObjects.Contains(obj);
    }

    public Interactable GetGrab() {

        GameObject o = GetGrabObject();

        if (o == null) {
            return null;
        }

        return GetGrabObject().GetComponent<Interactable>();
    }

    public GameObject GetGrabObject() {

        float closestDistance = float.MaxValue;
        GameObject closest = null;

        foreach (GameObject rb in GrabObjects) {

            float distance = Vector3.Distance(transform.position, rb.transform.position);

            if (distance < closestDistance) {
                closestDistance = distance;
                closest = rb;
            }
        }

        return closest;
    }
}
