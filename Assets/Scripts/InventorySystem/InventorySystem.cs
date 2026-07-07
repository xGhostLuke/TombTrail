using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Core-Logik des Inventar-Systems.
/// Verwaltet Item-Platzierung im 5x5 Grid mit Validierung für 1x1 und 2x1 Items.
/// </summary>
public class InventorySystem : MonoBehaviour {
    public const int GRID_WIDTH = 5;
    public const int GRID_HEIGHT = 5;

    [System.Serializable]
    public class InventoryItem {
        public ItemObject item;
        public int quantity;
        public int gridX;
        public int gridY;
        public int width;  // 1 für 1x1, 2 für 2x1
        public int height; // 1 für 1x1, 1 für 2x1

        public InventoryItem(ItemObject item, int x, int y, int width = 1, int height = 1, int qty = 1) {
            this.item = item;
            this.gridX = x;
            this.gridY = y;
            this.width = width;
            this.height = height;
            this.quantity = qty;
        }
    }

    private List<InventoryItem> inventoryItems = new List<InventoryItem>();
    private bool[] gridOccupancy = new bool[GRID_WIDTH * GRID_HEIGHT]; // Speichert, welche Slots besetzt sind

    private void Awake() {
        // Grid initialisieren (alle Slots leer)
        System.Array.Clear(gridOccupancy, 0, gridOccupancy.Length);
    }

    /// <summary>
    /// Gibt den Index im 1D-Array basierend auf Grid-Koordinaten zurück.
    /// </summary>
    private int GetGridIndex(int x, int y) {
        if (x < 0 || x >= GRID_WIDTH || y < 0 || y >= GRID_HEIGHT) {
            return -1;
        }
        return y * GRID_WIDTH + x;
    }

    /// <summary>
    /// Prüft, ob ein Item an der gegebenen Position Platz hat.
    /// </summary>
    public bool CanPlaceItem(int x, int y, int width = 1, int height = 1) {
        // Grenzen prüfen
        if (x + width > GRID_WIDTH || y + height > GRID_HEIGHT) {
            return false;
        }

        // Alle betroffenen Slots prüfen
        for (int dy = 0; dy < height; dy++) {
            for (int dx = 0; dx < width; dx++) {
                int idx = GetGridIndex(x + dx, y + dy);
                if (idx == -1 || gridOccupancy[idx]) {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Markiert Slot-Bereich als belegt/frei.
    /// </summary>
    private void SetGridOccupancy(int x, int y, int width, int height, bool occupied) {
        for (int dy = 0; dy < height; dy++) {
            for (int dx = 0; dx < width; dx++) {
                int idx = GetGridIndex(x + dx, y + dy);
                if (idx != -1) {
                    gridOccupancy[idx] = occupied;
                }
            }
        }
    }

    /// <summary>
    /// Fügt ein Item zum Inventar hinzu.
    /// </summary>
    public bool AddItem(ItemObject item, int x, int y, int width = 1, int height = 1, int quantity = 1) {
        if (!CanPlaceItem(x, y, width, height)) {
            Debug.LogWarning($"Kann Item nicht platzieren: Position ({x}, {y}) mit Größe ({width}x{height}) ist nicht verfügbar.");
            return false;
        }

        SetGridOccupancy(x, y, width, height, true);
        inventoryItems.Add(new InventoryItem(item, x, y, width, height, quantity));
        return true;
    }

    /// <summary>
    /// Entfernt ein Item aus dem Inventar.
    /// </summary>
    public bool RemoveItem(int x, int y) {
        InventoryItem item = GetItemAt(x, y);
        if (item != null) {
            SetGridOccupancy(item.gridX, item.gridY, item.width, item.height, false);
            inventoryItems.Remove(item);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Verschiebt ein Item zu einer neuen Position.
    /// </summary>
    public bool MoveItem(int fromX, int fromY, int toX, int toY) {
        InventoryItem item = GetItemAt(fromX, fromY);
        if (item == null) return false;

        if (!CanPlaceItem(toX, toY, item.width, item.height)) {
            return false;
        }

        // Altes Gebiet freigeben
        SetGridOccupancy(item.gridX, item.gridY, item.width, item.height, false);

        // Neue Position setzen
        item.gridX = toX;
        item.gridY = toY;

        // Neues Gebiet belegen
        SetGridOccupancy(toX, toY, item.width, item.height, true);

        return true;
    }

    /// <summary>
    /// Gibt das Item an der gegebenen Position zurück (sucht nach Items, die diese Position abdecken).
    /// </summary>
    public InventoryItem GetItemAt(int x, int y) {
        foreach (var item in inventoryItems) {
            if (x >= item.gridX && x < item.gridX + item.width &&
                y >= item.gridY && y < item.gridY + item.height) {
                return item;
            }
        }
        return null;
    }

    /// <summary>
    /// Gibt alle Items im Inventar zurück.
    /// </summary>
    public List<InventoryItem> GetAllItems() {
        return new List<InventoryItem>(inventoryItems);
    }

    /// <summary>
    /// Leert das Inventar.
    /// </summary>
    public void Clear() {
        inventoryItems.Clear();
        System.Array.Clear(gridOccupancy, 0, gridOccupancy.Length);
    }

    /// <summary>
    /// Prüft, ob das Inventar leer ist.
    /// </summary>
    public bool IsEmpty() {
        return inventoryItems.Count == 0;
    }

    /// <summary>
    /// Gibt die Anzahl der Items im Inventar zurück.
    /// </summary>
    public int GetItemCount() {
        return inventoryItems.Count;
    }
}
