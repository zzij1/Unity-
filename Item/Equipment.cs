using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public int damage;
    public bool isEquipped = false;
    public GameObject weaponPrefab; // 武器的预制体
    
    void OnEnable()
    {
        isEquipped = false;
        isWeaponItem = true;
    }
    public override void Use()
    {
        base.Use();
        if (!isEquipped)
        {
            Equip();
            RemoveFromInventory();
        }
    }

    private void Equip()
    {
        isEquipped = true;
        CharacterManager.instance.weaponSlot.AddItem(this);
    }
    
}