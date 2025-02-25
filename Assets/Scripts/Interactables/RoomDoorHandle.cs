public class RoomDoorHandle : AnimatedDoorHandle {

    public DoorGoTo destination;

    protected override void Start() {
        base.Start();
        Type.Set(InteractableType.Interactable);
    }

    public override void Interact(Hand hand) {
        base.Interact(hand);
        if (destination == DoorGoTo.None) {
            return;
        }
        Events.FireEvent(EventType.RoomDoor, CallbackData.Object(destination));
    }
}
