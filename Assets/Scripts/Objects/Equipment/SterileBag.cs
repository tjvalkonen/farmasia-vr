using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SterileBag : GeneralItem {

    #region fields

    public List<Syringe> Syringes { get; private set; }
    public bool IsClosed { get; private set; }
    public bool IsSterile { get; private set; }
    [SerializeField]
    private GameObject childCollider;

    [SerializeField]
    private DragAcceptable closeButton;

    private float ejectSpeed = 0.6f;
    private float ejectDistance = 0.47f;
    #endregion

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        NullCheck.CheckSafe(closeButton);

        Syringes = new List<Syringe>();
        
        ObjectType = ObjectType.SterileBag;

        IsClosed = false;
        IsSterile = true;

        Type.On(InteractableType.Interactable);

        CollisionSubscription.SubscribeToTrigger(childCollider, new TriggerListener().OnEnter(collider => OnBagEnter(collider)));
    }

    private void OnBagEnter(Collider other) {

        Syringe syringe = Interactable.GetInteractable(other.transform) as Syringe;

        if (syringe == null) {
            return;
        }

        if (syringe.IsAttached) {
            return;
        }

        if (Syringes.Count >= 6) {
            return;
        }

        if (syringe.State == InteractState.Grabbed) {
            Hand.GrabbingHand(syringe).Connector.Connection.Remove();
        }

        VRInput.Hands[0].Hand.HandCollider.RemoveInteractable(syringe);
        VRInput.Hands[0].Hand.ExtendedHandCollider.RemoveInteractable(syringe);
        VRInput.Hands[1].Hand.HandCollider.RemoveInteractable(syringe);
        VRInput.Hands[1].Hand.ExtendedHandCollider.RemoveInteractable(syringe);

        Logger.Print("Set syringe");

        SetSyringe(syringe);

        if (syringe.IsClean) {
            IsSterile = false;
        }

        Events.FireEvent(EventType.CloseSterileBag, CallbackData.Object(this));

        if (Syringes.Count == 6) {
            CloseSterileBag();
        }
    }

    public override void Interact(Hand hand) {
        base.Interact(hand);

        OpenSterileBag();

        float angle = Vector3.Angle(Vector3.down, transform.up);

        if (angle < 45) {
            return;
        }

        Logger.Print("Release syringes");

        foreach (Syringe s in Syringes) {
            ReleaseSyringe(s);
        }
        Syringes.Clear();
        IsClosed = false;
    }

    private void SetSyringe(Syringe syringe) {

        syringe.RigidbodyContainer.Disable();
        SetColliders(syringe.transform, false);

        syringe.transform.SetParent(transform);

        syringe.transform.localPosition = ObjectPosition(Syringes.Count);
        syringe.transform.localEulerAngles = new Vector3(180, 180, 0);
        Syringes.Add(syringe);
    }
    private void SetColliders(Transform t, bool enabled) {

        Collider coll = t.GetComponent<Collider>();

        if (coll != null) {
            coll.enabled = enabled;
        }

        foreach (Transform child in t) {
            SetColliders(child, enabled);
        }
    }

    private void ReleaseSyringe(Syringe syringe) {
        StartCoroutine(MoveSyringe(syringe));
    }

    private IEnumerator MoveSyringe(Syringe syringe) {
        float totalDistance = 0;

        while (totalDistance < ejectDistance) {
            float distance = Time.deltaTime * ejectSpeed;
            totalDistance += distance;
            syringe.transform.localPosition += Vector3.up * distance;
            yield return null;
        }

        syringe.transform.SetParent(null);
        SetColliders(syringe.transform, true);
        syringe.RigidbodyContainer.Enable();
    }

    private void OpenSterileBag() {

        if (closeButton.IsGrabbed) {
            Hand.GrabbingHand(closeButton).Uninteract();
        }

        closeButton.gameObject.SetActive(false);
    }
    private void CloseSterileBag() {
        IsClosed = true;
        
        closeButton.gameObject.SetActive(true);
        closeButton.OnAccept += CloseSterileBagFinal;
    }

    private void CloseSterileBagFinal() {
        Events.FireEvent(EventType.CloseSterileBag, CallbackData.Object(this));
    }

    private Vector3 ObjectPosition(int index) {

        Vector3 pos = new Vector3(0, 0.172f, 0);
        pos.x = (0.2f / 5) * index - 0.1f;

        return pos;
    }
}