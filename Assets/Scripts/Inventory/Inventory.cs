using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    public List<Item> Items { get; private set; } = new List<Item>();

    public System.Action OnInventoryChanged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatic() => Instance = null;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool AddItem(Item item)
    {
        if (item == null) return false;
        Items.Add(item);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool RemoveItem(Item item)
    {
        if (!Items.Remove(item)) return false;
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool RemoveItemById(string id)
    {
        Item item = Items.Find(i => i.itemId == id);
        if (item == null) return false;
        return RemoveItem(item);
    }

    public bool HasItem(string id) => Items.Exists(i => i.itemId == id);

    public Item FindById(string id) => Items.Find(i => i.itemId == id);
}
