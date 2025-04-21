using UnityEngine;
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Medicine")]
public class Medicine : Item
{
    public int returnBlood;
    public override void Use()
    {
        base.Use();
        int newHealth = Mathf.Min(CharacterManager.instance.playerState.currentHealth + returnBlood, CharacterManager.instance.playerState.maxHealth);
        CharacterManager.instance.playerState.currentHealth = newHealth;
        Inventory.instance.Remove(this);
    }
}
