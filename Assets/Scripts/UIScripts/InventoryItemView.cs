using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// One item in the inventory window: left click uses it, right click drops it,
/// dragging moves it and R turns it while dragging.
/// </summary>
public class InventoryItemView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
    private InventoryWindowScript window;
    private InventoryEntry entry;
    private RectTransform rect;
    private bool rotated;
    private bool isDragging;
    private Vector2 pointerPosition;
    private Camera pointerCamera;

    public void Setup(InventoryWindowScript owner, InventoryEntry shownEntry) {
        window = owner;
        entry = shownEntry;
        rect = (RectTransform)transform;
        rotated = entry.Rotated;
    }

    private void Update() {
        if (isDragging && Keyboard.current.rKey.wasPressedThisFrame) {
            rotated = !rotated;
            window.ShowDrag(rect, entry, pointerPosition, pointerCamera, rotated);
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        // Releasing a drag over the item also arrives here, and a click can cost a turn.
        if (eventData.dragging) {
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Left) {
            window.UseEntry(entry);
        } else if (eventData.button == PointerEventData.InputButton.Right) {
            window.DropEntry(entry);
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            eventData.pointerDrag = null;
            return;
        }
        isDragging = true;
        window.BeginDrag(rect);
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData) {
        if (!isDragging) {
            return;
        }
        pointerPosition = eventData.position;
        pointerCamera = eventData.pressEventCamera;
        window.ShowDrag(rect, entry, pointerPosition, pointerCamera, rotated);
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!isDragging) {
            return;
        }
        isDragging = false;
        window.FinishDrag(entry, eventData.position, eventData.pressEventCamera, rotated);
    }
}
