using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Hauptmanager für das Inventar-UI.
/// Handhabt Tastatur-Input ("I"), Canvas-Sichtbarkeit und koordiniert alle Komponenten.
/// </summary>
public class InventoryUI : MonoBehaviour {
    [SerializeField] private Canvas inventoryCanvas;
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private InventoryDisplay inventoryDisplay;

    private bool isInventoryOpen = false;
    private InputAction inventoryAction;

    private void OnEnable() {
        if (inventoryAction == null) {
            inventoryAction = new InputAction("Inventory", InputActionType.Button, "<Keyboard>/i");
            inventoryAction.Enable();
        }
        inventoryAction.performed += OnInventoryInput;
    }

    private void OnDisable() {
        if (inventoryAction != null) {
            inventoryAction.performed -= OnInventoryInput;
        }
    }

    private void Awake() {
        // Auto-Suche, falls nicht manuell zugewiesen
        if (inventoryCanvas == null) {
            inventoryCanvas = GetComponentInParent<Canvas>();
        }
        if (inventorySystem == null) {
            inventorySystem = GetComponentInParent<InventorySystem>();
        }
        if (inventoryDisplay == null) {
            inventoryDisplay = GetComponent<InventoryDisplay>();
        }

        // Inventar zu Beginn schließen
        CloseInventory();
    }

    private void OnInventoryInput(InputAction.CallbackContext context) {
        if (context.performed) {
            ToggleInventory();
        }
    }

    /// <summary>
    /// Öffnet das Inventar.
    /// </summary>
    public void OpenInventory() {
        if (isInventoryOpen) {
            return;
        }

        isInventoryOpen = true;

        if (inventoryCanvas != null) {
            inventoryCanvas.enabled = true;
        }

        if (inventoryDisplay != null) {
            inventoryDisplay.RefreshDisplay();
        }

        Debug.Log("Inventar geöffnet");
    }

    /// <summary>
    /// Schließt das Inventar.
    /// </summary>
    public void CloseInventory() {
        if (!isInventoryOpen) {
            return;
        }

        isInventoryOpen = false;

        if (inventoryCanvas != null) {
            inventoryCanvas.enabled = false;
        }

        Debug.Log("Inventar geschlossen");
    }

    /// <summary>
    /// Schaltet die Inventar-Sichtbarkeit um.
    /// </summary>
    public void ToggleInventory() {
        if (isInventoryOpen) {
            CloseInventory();
        } else {
            OpenInventory();
        }
    }

    /// <summary>
    /// Gibt an, ob das Inventar aktuell offen ist.
    /// </summary>
    public bool IsInventoryOpen() {
        return isInventoryOpen;
    }

    /// <summary>
    /// Fügt ein Item zum Inventar hinzu (an der ersten verfügbaren Position).
    /// </summary>
    public bool AddItemAtFirstAvailable(ItemObject item, int width = 1, int height = 1, int quantity = 1) {
        for (int y = 0; y < InventorySystem.GRID_HEIGHT; y++) {
            for (int x = 0; x < InventorySystem.GRID_WIDTH; x++) {
                if (inventorySystem.CanPlaceItem(x, y, width, height)) {
                    bool success = inventorySystem.AddItem(item, x, y, width, height, quantity);
                    if (success && inventoryDisplay != null) {
                        inventoryDisplay.RefreshDisplay();
                        return true;
                    }
                }
            }
        }

        Debug.LogWarning($"Konnte Item nicht platzieren - Inventar ist voll oder Item passt nicht.");
        return false;
    }

    /// <summary>
    /// Fügt ein Item an einer spezifischen Position hinzu.
    /// </summary>
    public bool AddItemAtPosition(ItemObject item, int x, int y, int width = 1, int height = 1, int quantity = 1) {
        bool success = inventorySystem.AddItem(item, x, y, width, height, quantity);
        if (success && inventoryDisplay != null) {
            inventoryDisplay.RefreshDisplay();
        }
        return success;
    }

    /// <summary>
    /// Entfernt ein Item aus dem Inventar.
    /// </summary>
    public bool RemoveItemAt(int x, int y) {
        bool success = inventorySystem.RemoveItem(x, y);
        if (success && inventoryDisplay != null) {
            inventoryDisplay.RefreshDisplay();
        }
        return success;
    }

    /// <summary>
    /// Leert das Inventar.
    /// </summary>
    public void ClearInventory() {
        inventorySystem.Clear();
        if (inventoryDisplay != null) {
            inventoryDisplay.RefreshDisplay();
        }
    }
}
