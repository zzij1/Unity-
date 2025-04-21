using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string name="New Item";
    public Sprite icon=null;
    public bool isWeaponItem=false;
    [TextArea]
    public string itemDescription;
    public virtual void Use()
    {
        Debug.Log("use"+name);
        //在子类中重写以实现具体物品逻辑
        RemoveFromInventory();
    }

    public void RemoveFromInventory()
    {
        Inventory.instance.Remove(this);
    }
}
