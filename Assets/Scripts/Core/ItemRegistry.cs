using System.Collections.Generic;
using UnityEngine;

public class ItemRegistry : MonoBehaviour
{
    public static ItemRegistry Instance { get; private set; }

    [SerializeField] Item[] allItems;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatic() => Instance = null;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public IReadOnlyList<Item> AllItems => allItems;

    public static Item Find(string id)
    {
        if (Instance == null) return null;
        foreach (Item item in Instance.allItems)
            if (item.itemId.ToLower() == id.ToLower()) return item;
        return null;
    }
}
