﻿using UnityEngine;
using Valve.VR;

public class MenuInterface : MonoBehaviour {

    private SteamVR_Action_Boolean menuAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Menu");
    private GameObject menuContainer;
    private Hand leftHand;
    private Transform cam;

    public Vector3 cameraCenter { get => cam.position + cam.forward * centerOffset; }
    public Vector3 offset;
    public Vector3 localPosOffset;
    public float lerpAmount = 0.15f;
    public float centerOffset = 0.2f;
    public bool visible => menuContainer.activeSelf;

    private void Start() {
        menuContainer = gameObject.transform.GetChild(0).gameObject;
        leftHand = GameObject.FindGameObjectWithTag("Controller (Left)").GetComponent<Hand>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        localPosOffset = transform.localPosition;
    }

    private void Update() {
        if (visible) {
            // keeps the pause menu in front of the player when moving
            Vector3 cameraPosition = cam.transform.position;
            cameraPosition += localPosOffset;
            transform.LookAt(cameraPosition, Vector3.up);
            transform.position = Vector3.Lerp(transform.position, GetTransformPosition() + localPosOffset, Time.deltaTime / lerpAmount);
        }
        if (menuAction != null && menuAction.GetStateDown(leftHand.HandType)) {
            Close();
        }
    }

    private Vector3 GetTransformPosition() {
        Vector3 forward = new Vector3(cam.forward.x, 0, cam.forward.z).normalized;
        Vector3 right = new Vector3(cam.right.x, 0, cam.right.z).normalized;
        Vector3 targetPosition = cameraCenter + forward * offset.z + right * offset.x;
        targetPosition = new Vector3(targetPosition.x, cameraCenter.y + offset.y, targetPosition.z);
        return targetPosition;
    }
    
    public void Close() {
        menuContainer.SetActive(!visible);
    }
}
