using UnityEngine;

/// <summary>
/// Test-Script zum Validieren des Inventar-Systems.
/// Kann an einen beliebigen GameObject in der Scene angehängt werden.
/// </summary>
public class InventorySystemTest : MonoBehaviour {
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private ItemObject testItem1x1;  // Ein 1x1 Item
    [SerializeField] private ItemObject testItem2x1;  // Ein 2x1 Item

    private void Start() {
        if (inventoryUI == null) {
            inventoryUI = FindObjectOfType<InventoryUI>();
        }

        Debug.Log("=== Inventar-System Test ===");
        Debug.Log("Drücke 'I' zum Öffnen/Schließen des Inventars");
        Debug.Log("Drücke 'T' um Test-Items hinzuzufügen");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.T)) {
            AddTestItems();
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            ListInventoryContents();
        }
    }

    private void AddTestItems() {
        if (inventoryUI == null) {
            Debug.LogError("InventoryUI nicht zugewiesen!");
            return;
        }

        // Füge einige Test-Items hinzu
        Debug.Log("Füge Test-Items hinzu...");

        if (testItem1x1 != null) {
            for (int i = 0; i < 3; i++) {
                bool success = inventoryUI.AddItemAtFirstAvailable(testItem1x1, width: 1, height: 1, quantity: 1);
                if (success) {
                    Debug.Log($"✓ Item 1x1 #{i + 1} hinzugefügt");
                } else {
                    Debug.LogWarning($"✗ Konnte Item 1x1 #{i + 1} nicht hinzufügen");
                }
            }
        }

        if (testItem2x1 != null) {
            bool success = inventoryUI.AddItemAtFirstAvailable(testItem2x1, width: 2, height: 1, quantity: 1);
            if (success) {
                Debug.Log($"✓ Item 2x1 hinzugefügt");
            } else {
                Debug.LogWarning($"✗ Konnte Item 2x1 nicht hinzufügen");
            }
        }

        inventoryUI.OpenInventory();
    }

    private void ListInventoryContents() {
        InventorySystem inventory = inventoryUI.GetComponent<InventorySystem>();
        if (inventory == null) {
            Debug.LogError("InventorySystem nicht gefunden!");
            return;
        }

        var items = inventory.GetAllItems();
        Debug.Log($"=== Inventar-Inhalt ({items.Count} Items) ===");

        foreach (var item in items) {
            Debug.Log($"  - Item bei ({item.gridX}, {item.gridY}), Größe: {item.width}x{item.height}, Qty: {item.quantity}");
        }
    }
}
