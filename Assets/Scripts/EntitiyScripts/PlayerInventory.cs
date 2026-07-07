using UnityEngine;

using System.Collections.Generic;

/// <summary>
/// Inventory-Komponente für den Player.
/// Interagiert mit InventorySystem für Grid-basierte Item-Platzierung.
/// Bietet automatische Platzierung und Feedback-System.
/// </summary>
public class PlayerInventory : MonoBehaviour {
    private InventorySystem inventorySystem;
    private PlayerController playerController;

    // Events für UI-Feedback
    public delegate void OnItemAdded(ItemObject item, bool success);
    public event OnItemAdded ItemAddedCallback;

    public delegate void OnInventoryFull();
    public event OnInventoryFull InventoryFullCallback;

    private void Awake() {
        // Inventar-System kann als Komponente oder im Canvas sein
        inventorySystem = GetComponent<InventorySystem>();
        if (inventorySystem == null) {
            inventorySystem = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<InventorySystem>();
        }

        playerController = GetComponent<PlayerController>();
    }

    private void Start() {
        if (inventorySystem == null) {
            Debug.LogError("PlayerInventory: InventorySystem nicht gefunden!");
        }
    }

    /// <summary>
    /// Versucht Item automatisch im Grid zu platzieren.
    /// Sucht erste verfügbare Position.
    /// </summary>
    public bool AddItem(ItemObject item) {
        if (item == null) {
            Debug.LogWarning("PlayerInventory: Item ist null!");
            return false;
        }

        if (inventorySystem == null) {
            Debug.LogError("PlayerInventory: InventorySystem nicht verfügbar!");
            ItemAddedCallback?.Invoke(item, false);
            return false;
        }

        int gridWidth = item.gridWidth;
        int gridHeight = item.gridHeight;

        // Versuche Item an erster verfügbarer Position zu platzieren
        for (int y = 0; y < InventorySystem.GRID_HEIGHT; y++) {
            for (int x = 0; x < InventorySystem.GRID_WIDTH; x++) {
                if (inventorySystem.CanPlaceItem(x, y, gridWidth, gridHeight)) {
                    bool success = inventorySystem.AddItem(item, x, y, gridWidth, gridHeight);

                    if (success) {
                        // TODO: Sound-Feedback (wird später integriert)
                        // AudioManager.PlaySound("itemPickup", transform.position);

                        ItemAddedCallback?.Invoke(item, true);
                        return true;
                    }
                }
            }
        }

        // Kein Platz gefunden
        Debug.LogWarning($"PlayerInventory: Kein Platz für Item {item.name} ({gridWidth}x{gridHeight})!");
        ItemAddedCallback?.Invoke(item, false);
        InventoryFullCallback?.Invoke();
        return false;
    }

    /// <summary>
    /// Entfernt Item aus Inventar an Position
    /// </summary>
    public bool RemoveItem(int x, int y) {
        if (inventorySystem == null) {
            return false;
        }
        return inventorySystem.RemoveItem(x, y);
    }

    /// <summary>
    /// Gibt alle Items im Inventar zurück
    /// </summary>
    public List<InventorySystem.InventoryItem> GetAllItems() {
        if (inventorySystem == null) {
            return new List<InventorySystem.InventoryItem>();
        }
        return inventorySystem.GetAllItems();
    }

    /// <summary>
    /// Prüft ob Inventar voll ist (kein Platz mehr)
    /// </summary>
    public bool IsInventoryFull() {
        if (inventorySystem == null) {
            return true;
        }

        // Prüfe ob noch ein 1x1 Item Platz hat
        for (int y = 0; y < InventorySystem.GRID_HEIGHT; y++) {
            for (int x = 0; x < InventorySystem.GRID_WIDTH; x++) {
                if (inventorySystem.CanPlaceItem(x, y, 1, 1)) {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Gibt Inventar-Referenz zurück (für direkten Zugriff wenn nötig)
    /// </summary>
    public InventorySystem GetInventorySystem() {
        return inventorySystem;
    }

    /// <summary>
    /// Network-ready: Wird später für Multiplayer erweitert
    /// </summary>
    public void SyncInventoryToNetwork() {
        // TODO: Implementierung für Netzwerk-Synchronisation
        // photonView.RPC("RPC_SyncInventory", RpcTarget.All, GetInventoryData());
    }
}
