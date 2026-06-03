using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
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

    void Awake() { Instance = this; }

    void Start()
    {
        Inventory.Instance.OnInventoryChanged += Refresh;
        discardButton.onClick.AddListener(DiscardSelected);
        detailPanel.SetActive(false);
        panel.SetActive(false);
        Refresh();
    }

    void Update()
    {
        if (CLIUI.Instance != null && CLIUI.Instance.IsOpen) return;
        if (Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame)
            panel.SetActive(!panel.activeSelf);
    }

    void Refresh()
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        foreach (Item item in Inventory.Instance.Items)
        {
            GameObject go = Instantiate(itemSlotPrefab, slotContainer);
            go.GetComponent<ItemSlotUI>().Setup(item, SelectItem);
        }
    }

    void SelectItem(Item item)
    {
        _selectedItem = item;
        detailName.text = item.itemName;
        detailDescription.text = item.description;
        detailPanel.SetActive(true);
    }

    void DiscardSelected()
    {
        if (_selectedItem == null) return;
        Inventory.Instance.RemoveItem(_selectedItem);
        _selectedItem = null;
        detailPanel.SetActive(false);
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
