using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Repräsentiert einen einzelnen Slot im Inventar-Grid.
/// Speichert die Position im Grid und kann ein Item anzeigen.
/// </summary>
public class InventorySlot : MonoBehaviour {
    [SerializeField] private int gridX;
    [SerializeField] private int gridY;
    
    private Image slotBackground;
    private Image itemDisplay;
    private TextMeshProUGUI quantityText;
    private ItemObject heldItem;
    private int quantity;
    
    private Color defaultColor = new Color(0.7f, 0.7f, 0.7f, 1f); // Grau für leere Slots
    private Color occupiedColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Dunkelgrau für besetzte Slots
    private Color highlightColor = new Color(1f, 1f, 0.8f, 1f); // Highlight bei Hover

    private void Awake() {
        slotBackground = GetComponent<Image>();
        
        // Suche nach itemDisplay Image (Kind-Objekt)
        Transform itemDisplayTransform = transform.Find("ItemDisplay");
        if (itemDisplayTransform != null) {
            itemDisplay = itemDisplayTransform.GetComponent<Image>();
        }
        
        // Suche nach QuantityText (Kind-Objekt)
        Transform quantityTransform = transform.Find("QuantityText");
        if (quantityTransform != null) {
            quantityText = quantityTransform.GetComponent<TextMeshProUGUI>();
        }
        
        SetEmpty();
    }

    public void Initialize(int x, int y) {
        gridX = x;
        gridY = y;
        SetEmpty();
    }

    public void SetItem(ItemObject item, int qty = 1) {
        heldItem = item;
        quantity = qty;
        
        if (item != null && itemDisplay != null) {
            itemDisplay.sprite = item.ItemSprite;
            itemDisplay.enabled = true;
            slotBackground.color = occupiedColor;
        }
        
        UpdateQuantityDisplay();
    }

    public void SetEmpty() {
        heldItem = null;
        quantity = 0;
        
        if (itemDisplay != null) {
            itemDisplay.sprite = null;
            itemDisplay.enabled = false;
        }
        
        if (slotBackground != null) {
            slotBackground.color = defaultColor;
        }
        
        UpdateQuantityDisplay();
    }

    private void UpdateQuantityDisplay() {
        if (quantityText != null) {
            if (quantity > 1) {
                quantityText.text = quantity.ToString();
                quantityText.enabled = true;
            } else {
                quantityText.enabled = false;
            }
        }
    }

    public void SetHighlight(bool highlight) {
        if (slotBackground != null) {
            slotBackground.color = highlight ? highlightColor : (heldItem != null ? occupiedColor : defaultColor);
        }
    }

    // Getter
    public int GetGridX() => gridX;
    public int GetGridY() => gridY;
    public ItemObject GetItem() => heldItem;
    public int GetQuantity() => quantity;
    public bool IsEmpty() => heldItem == null;

    public void SetQuantity(int newQuantity) {
        quantity = newQuantity;
        UpdateQuantityDisplay();
    }

    public void AddQuantity(int amount) {
        quantity += amount;
        UpdateQuantityDisplay();
    }
}
