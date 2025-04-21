using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasUI : MonoBehaviour
{
    //InventoryUI
    public Transform itemsParent;
    public InventorySlot[] slots;
    public GameObject inventoryUI;

    Inventory inventory;

    //StateUI
    public GameObject stateUI;
    public Text currentHealth;
    public Text damage;
    public Text currentLevel;
    public WeaponSlot weaponSlot;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        UpdateStateUI();
        UpdateInventoryUI();
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
        if (weaponSlot.item==null)
        {
            weaponSlot.icon.sprite = null;
            weaponSlot.icon.enabled = false;
            weaponSlot.removeButton.interactable = false;
            weaponSlot.removeButton.gameObject.SetActive(false);
        }

        ExperienceManager.Instance.OnLevelUp += UpdateLevel;
    }
    
    private void OnEnable()
    {
        inventory=Inventory.instance;
        GameEvents.Instance.OnInventoryChanged += UpdateInventoryUI;
        GameEvents.Instance.OnPlayerStateChanged += UpdateStateUI;
    }

    private void OnDisable()
    {
        GameEvents.Instance.OnInventoryChanged -= UpdateInventoryUI;
        GameEvents.Instance.OnPlayerStateChanged -= UpdateStateUI;
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
            stateUI.SetActive(!stateUI.activeSelf);
        }
    }

    private void UpdateInventoryUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    private void UpdateStateUI()
    {
        currentHealth.text = CharacterManager.instance.playerState.currentHealth.ToString() + "/" + CharacterManager.instance.playerState.maxHealth.ToString();
        damage.text = CharacterManager.instance.playerState.damage.ToString();
        
    }

    private void UpdateLevel(int Level)
    {
        currentLevel.text =Level.ToString();
    }
    
}