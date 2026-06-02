using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemId;
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
}
