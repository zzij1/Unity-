using System;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlot : MonoBehaviour
{
    public Equipment item;
    public Image icon;
    public Button removeButton;
    public GameObject weaponModel;
    public Transform weaponTransform;
    private Animator animator; // 引用角色的 Animator
    private PlayerAttack playerAttack; // 引用 PlayerAttack 脚本

    private void Start()
    {
        animator = CharacterManager.instance.player.GetComponent<Animator>();
        playerAttack = CharacterManager.instance.player.GetComponent<PlayerAttack>();
        animator.SetBool("IsArmed", false);
    }

    public void AddItem(Equipment newItem)
    {
        // 如果已经有武器，先卸下
        if (item != null)
        {
            ClearSlot();
        }

        // 装备新武器
        item = newItem;
        icon.enabled = true;
        icon.sprite = item.icon;
        removeButton.interactable = true;
        removeButton.gameObject.SetActive(true);

        // 加载武器模型
        if (item.weaponPrefab != null)
        {
            Debug.Log("实例化武器");
            weaponModel = Instantiate(item.weaponPrefab, weaponTransform); // 实例化武器模型并放置在 WeaponSocket 上
            weaponModel.transform.localPosition = Vector3.zero; // 重置位置
            weaponModel.transform.localRotation = Quaternion.identity; // 重置旋转
        }

        // 更新角色攻击力
        CharacterManager.instance.UpdatePlayerDamage();
        if (animator != null)
        {
            animator.SetBool("IsArmed", true); // 设置为持刀状态
        }
        if (playerAttack != null)
        {
            playerAttack.SetArmed(true); // 设置为持刀攻击
        }
    }

    public void ClearSlot()
    {
        if (item != null)
        {
            // 销毁武器模型
            if (weaponModel != null)
            {
                Destroy(weaponModel);
                weaponModel = null;
            }

            // 卸下武器
            item.isEquipped = false;
            item = null;
            icon.sprite = null;
            icon.enabled = false;
            removeButton.interactable = false;
            removeButton.gameObject.SetActive(false);

            // 更新角色攻击力
            CharacterManager.instance.UpdatePlayerDamage();
            if (animator != null)
            {
                animator.SetBool("IsArmed", false); // 设置为空手状态
            }
            if (playerAttack != null)
            {
                playerAttack.SetArmed(false); // 设置为拳头攻击
            }
        }
    }

    public void OnRemoveButton()
    {
        if (item != null)
        {
            // 尝试将武器放回背包
            bool canClear = Inventory.instance.Add(item);
            if (canClear)
            {
                ClearSlot();
            }
        }
    }
    
}
