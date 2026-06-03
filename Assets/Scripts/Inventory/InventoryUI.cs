using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    public bool IsOpen => panel != null && panel.activeSelf;

    [Header("Slot List")]
    [SerializeField] Transform slotContainer;
    [SerializeField] GameObject itemSlotPrefab;

    [Header("Detail Panel")]
    [SerializeField] GameObject detailPanel;
    [SerializeField] TextMeshProUGUI detailName;
    [SerializeField] TextMeshProUGUI detailDescription;
    [SerializeField] Button discardButton;

    [Header("Toggle")]
    [SerializeField] Key toggleKey = Key.I;
    [SerializeField] GameObject panel;

    Item _selectedItem;
    ItemSlotUI _selectedSlotUI;
    Image _detailIcon;

    void Awake() { Instance = this; }

    void Start()
    {
        Inventory.Instance.OnInventoryChanged += Refresh;
        discardButton.onClick.AddListener(DiscardSelected);
        detailPanel.SetActive(false);
        panel.SetActive(false);

        // 執行期建立細節圖示 Image（3x = 240×240）
        var iconGO = new GameObject("DetailIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        iconGO.transform.SetParent(detailPanel.transform, false);
        iconGO.transform.SetAsFirstSibling();
        var rt = iconGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = new Vector2(160, 160);
        rt.anchoredPosition = new Vector2(0, -15f);
        _detailIcon = iconGO.GetComponent<Image>();
        _detailIcon.preserveAspect = true;
        _detailIcon.enabled = false;

        // 文字 2x 放大 + 重新定位（頂部錨，避免與圖示重疊）
        detailName.fontSize = 33;
        var nameRT = detailName.rectTransform;
        nameRT.anchorMin = nameRT.anchorMax = new Vector2(0.5f, 1f);
        nameRT.pivot = new Vector2(0.5f, 1f);
        nameRT.sizeDelta = new Vector2(300, 44);
        nameRT.anchoredPosition = new Vector2(0, -185f);

        detailDescription.fontSize = 24;
        var descRT = detailDescription.rectTransform;
        descRT.anchorMin = descRT.anchorMax = new Vector2(0.5f, 1f);
        descRT.pivot = new Vector2(0.5f, 1f);
        descRT.sizeDelta = new Vector2(300, 90);
        descRT.anchoredPosition = new Vector2(0, -234f);

        // 點擊 panel 空白處關閉細節
        var trigger = panel.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener(_ => CloseDetail());
        trigger.triggers.Add(entry);

        Refresh();
    }

    void Update()
    {
        if (CLIUI.Instance != null && CLIUI.Instance.IsOpen) return;
        if (Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame)
            panel.SetActive(!panel.activeSelf);

        // 點擊 InventoryPanel 視窗外關閉細節
        if (_selectedItem != null && Mouse.current != null
            && Mouse.current.leftButton.wasPressedThisFrame
            && !IsPointerOverPanel())
        {
            CloseDetail();
        }
    }

    bool IsPointerOverPanel()
    {
        var pointer = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);
        foreach (var r in results)
            if (r.gameObject == panel || r.gameObject.transform.IsChildOf(panel.transform))
                return true;
        return false;
    }

    void Refresh()
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        foreach (Item item in Inventory.Instance.Items)
        {
            GameObject go = Instantiate(itemSlotPrefab, slotContainer);
            var slotUI = go.GetComponent<ItemSlotUI>();
            slotUI.Setup(item, selectedItem => SelectItem(selectedItem, slotUI));
        }

        if (_selectedItem != null && !Inventory.Instance.HasItem(_selectedItem.itemId))
            CloseDetail();
    }

    void SelectItem(Item item, ItemSlotUI slot)
    {
        _selectedSlotUI?.SetSelected(false);
        _selectedItem = item;
        _selectedSlotUI = slot;
        slot.SetSelected(true);
        detailName.text = item.itemName;
        detailDescription.text = item.description;
        if (_detailIcon != null)
        {
            _detailIcon.sprite = item.icon;
            _detailIcon.enabled = item.icon != null;
        }
        detailPanel.SetActive(true);
    }

    void CloseDetail()
    {
        _selectedSlotUI?.SetSelected(false);
        _selectedSlotUI = null;
        _selectedItem = null;
        detailPanel.SetActive(false);
    }

    void DiscardSelected()
    {
        if (_selectedItem == null) return;
        Inventory.Instance.RemoveItem(_selectedItem);
        CloseDetail();
    }

    public void Show() => panel.SetActive(true);
    public void Hide() => panel.SetActive(false);

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        if (Inventory.Instance != null)
            Inventory.Instance.OnInventoryChanged -= Refresh;
    }
}
