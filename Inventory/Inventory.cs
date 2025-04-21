using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public List<Item> items = new List<Item>();
    public int space = 20;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool Add(Item item)
    {
        if (items.Count >= space)
        {
            Debug.Log("背包已满");
            return false;
        }
        items.Add(item);
        GameEvents.Instance.OnInventoryChanged?.Invoke();
        return true;
    }

    public void Remove(Item item)
    {
        items.Remove(item);

        GameEvents.Instance.OnInventoryChanged?.Invoke();
    }
}