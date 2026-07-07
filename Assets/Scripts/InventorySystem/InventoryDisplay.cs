using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Verwaltet die visuelle Darstellung des Inventar-Grids und Drag-and-Drop-Funktionalität.
/// </summary>
public class InventoryDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private Transform gridContainer; // Parent für alle Slots
    [SerializeField] private GameObject slotPrefab; // Prefab für einen Slot
    [SerializeField] private float slotSize = 64f; // Größe eines Slots in Pixeln (für Pixel-Perfect)

    private InventorySlot[,] slotGrid = new InventorySlot[InventorySystem.GRID_WIDTH, InventorySystem.GRID_HEIGHT];
    private InventorySlot draggedSlot;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
        InitializeGrid();
    }

    private void InitializeGrid() {
        // Erstelle das 5x5 Slot-Grid
        for (int y = 0; y < InventorySystem.GRID_HEIGHT; y++) {
            for (int x = 0; x < InventorySystem.GRID_WIDTH; x++) {
                GameObject slotGO = Instantiate(slotPrefab, gridContainer);
                InventorySlot slot = slotGO.GetComponent<InventorySlot>();
                
                if (slot == null) {
                    slot = slotGO.AddComponent<InventorySlot>();
                }
                
                slot.Initialize(x, y);
                slotGrid[x, y] = slot;

                // Positioniere den Slot im Grid (für Pixel-Perfect)
                RectTransform slotRect = slotGO.GetComponent<RectTransform>();
                if (slotRect != null) {
                    slotRect.sizeDelta = new Vector2(slotSize, slotSize);
                    slotRect.anchoredPosition = new Vector2(x * slotSize, -y * slotSize);
                }
            }
        }

        // Setze die Größe des Grid-Containers
        RectTransform containerRect = gridContainer.GetComponent<RectTransform>();
        if (containerRect != null) {
            containerRect.sizeDelta = new Vector2(
                InventorySystem.GRID_WIDTH * slotSize,
                InventorySystem.GRID_HEIGHT * slotSize
            );
        }
    }

    /// <summary>
    /// Aktualisiert die Slot-Anzeige basierend auf dem Inventar-Status.
    /// </summary>
    public void RefreshDisplay() {
        // Alle Slots leeren
        for (int y = 0; y < InventorySystem.GRID_HEIGHT; y++) {
            for (int x = 0; x < InventorySystem.GRID_WIDTH; x++) {
                slotGrid[x, y].SetEmpty();
            }
        }

        // Items neu anzeigen
        var items = inventorySystem.GetAllItems();
        foreach (var item in items) {
            InventorySlot slot = slotGrid[item.gridX, item.gridY];
            if (slot != null) {
                slot.SetItem(item.item, item.quantity);
            }
        }
    }

    /// <summary>
    /// Findet den Slot an der gegebenen Maus-Position.
    /// </summary>
    private InventorySlot GetSlotAtMousePosition() {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        Vector2 mousePos = Mouse.current.position.ReadValue();
        pointerData.position = mousePos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            mousePos,
            canvas.worldCamera,
            out Vector2 localPoint
        );

        // Berechne die Grid-Position basierend auf der lokalen Position
        int gridX = Mathf.FloorToInt(localPoint.x / slotSize);
        int gridY = Mathf.FloorToInt(-localPoint.y / slotSize); // Y ist invertiert

        if (gridX >= 0 && gridX < InventorySystem.GRID_WIDTH &&
            gridY >= 0 && gridY < InventorySystem.GRID_HEIGHT) {
            return slotGrid[gridX, gridY];
        }

        return null;
    }

    // IPointerEnterHandler
    public void OnPointerEnter(PointerEventData eventData) {
        InventorySlot slot = GetSlotAtMousePosition();
        if (slot != null) {
            slot.SetHighlight(true);
        }
    }

    // IPointerExitHandler
    public void OnPointerExit(PointerEventData eventData) {
        InventorySlot slot = GetSlotAtMousePosition();
        if (slot != null) {
            slot.SetHighlight(false);
        }
    }

    // IBeginDragHandler
    public void OnBeginDrag(PointerEventData eventData) {
        draggedSlot = GetSlotAtMousePosition();
        if (draggedSlot != null && !draggedSlot.IsEmpty()) {
            draggedSlot.SetHighlight(true);
            if (canvasGroup != null) {
                canvasGroup.blocksRaycasts = false; // Erlaube Raycasts unter den gedrückten Elementen
            }
        }
    }

    // IDragHandler
    public void OnDrag(PointerEventData eventData) {
        if (draggedSlot != null) {
            // Optional: Visuelle Effekte beim Draggen (z.B. Icon folgt der Maus)
        }
    }

    // IEndDragHandler
    public void OnEndDrag(PointerEventData eventData) {
        if (draggedSlot != null && !draggedSlot.IsEmpty()) {
            InventorySlot targetSlot = GetSlotAtMousePosition();

            if (targetSlot != null && draggedSlot != targetSlot) {
                // Versuche, das Item zu verschieben
                if (inventorySystem.MoveItem(draggedSlot.GetGridX(), draggedSlot.GetGridY(),
                                              targetSlot.GetGridX(), targetSlot.GetGridY())) {
                    RefreshDisplay();
                    Debug.Log($"Item verschoben von ({draggedSlot.GetGridX()}, {draggedSlot.GetGridY()}) zu ({targetSlot.GetGridX()}, {targetSlot.GetGridY()})");
                } else {
                    Debug.Log("Ungültiger Drop - Item passt nicht an die Zielposition.");
                }
            }

            draggedSlot.SetHighlight(false);
            draggedSlot = null;

            if (canvasGroup != null) {
                canvasGroup.blocksRaycasts = true;
            }
        }
    }
}
