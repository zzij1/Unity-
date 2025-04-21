using UnityEngine;

public class ItemPickUp : Interactable
{
    public Item item;
    public override void Interact()
    {
        base.Interact();
        PickUp();
    }

    private void PickUp()
    {
        Debug.Log("Picked up"+item.name);
        bool wasPickedUp=Inventory.instance.Add(item);
        if (wasPickedUp)
        {
            GameEvents.Instance.TriggerItemPickedUp(item.name);
            Destroy(gameObject);
        }
    }
}
