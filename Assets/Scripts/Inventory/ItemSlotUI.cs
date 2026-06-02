using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Button slotButton;

    Item _item;
    System.Action<Item> _onSelected;

    public void Setup(Item item, System.Action<Item> onSelected)
    {
        _item = item;
        _onSelected = onSelected;

        nameText.text = item.itemName;
        iconImage.sprite = item.icon;
        iconImage.enabled = item.icon != null;

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(() => _onSelected?.Invoke(_item));
    }
}
