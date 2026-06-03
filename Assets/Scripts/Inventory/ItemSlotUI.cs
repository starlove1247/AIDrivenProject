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
    Outline _outline;

    void Awake()
    {
        _outline = gameObject.AddComponent<Outline>();
        _outline.effectColor = new Color(1f, 0.85f, 0f);
        _outline.effectDistance = new Vector2(3, -3);
        _outline.enabled = false;
    }

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

    public void SetSelected(bool selected)
    {
        if (_outline != null) _outline.enabled = selected;
    }
}
