using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Shows the player's pack, toggled with I.
/// Only displays and forwards input, the rules live in Inventory and PlayerController.
/// </summary>
public class InventoryWindowScript : WindowController {
    [SerializeField] private GameController gameController;
    [SerializeField] private RectTransform grid;
    [SerializeField] private float cellSize = 120f;
    [SerializeField] private float cellGap = 8f;
    [SerializeField] private Sprite cellSprite; // Optional: flat colour until there is real art.
    [SerializeField] private Color cellColor = new Color(0.16f, 0.16f, 0.18f, 1f);
    [SerializeField] private Color itemBackgroundColor = new Color(0.36f, 0.31f, 0.2f, 1f);
    [SerializeField] private Color validDropColor = new Color(0.3f, 0.8f, 0.3f, 0.45f);
    [SerializeField] private Color invalidDropColor = new Color(0.9f, 0.25f, 0.25f, 0.45f);

    private Inventory inventory;
    private readonly List<GameObject> itemViews = new List<GameObject>();
    private Image dropPreview;
    private bool isOpen;

    private void Start() {
        inventory = gameController.playerController.GetInventory();
        inventory.Changed += Refresh;
        grid.sizeDelta = new Vector2(inventory.Width, inventory.Height) * cellSize;
        BuildCells();
        dropPreview = CreateImage("DropPreview", grid);
        dropPreview.raycastTarget = false;
        Refresh();
        HideWindow();
    }

    private void OnDestroy() {
        if (inventory != null) {
            inventory.Changed -= Refresh;
        }
    }

    private void Update() {
        if (Keyboard.current.iKey.wasPressedThisFrame) {
            isOpen = !isOpen;
            if (isOpen) {
                ShowWindow();
            } else {
                HideWindow();
            }
        }
    }

    public void UseEntry(InventoryEntry entry) {
        gameController.playerController.UseItem(entry);
    }

    public void DropEntry(InventoryEntry entry) {
        gameController.DropItem(entry);
    }

    /// <summary>
    /// Puts the dragged item above the others, with the preview just below it.
    /// </summary>
    public void BeginDrag(RectTransform itemRect) {
        dropPreview.rectTransform.SetAsLastSibling();
        itemRect.SetAsLastSibling();
    }

    /// <summary>
    /// Lets the item follow the pointer and shows on which cells it would land.
    /// </summary>
    public void ShowDrag(RectTransform itemRect, InventoryEntry entry, Vector2 screenPosition, Camera eventCamera, bool rotated) {
        Vector2 pointer = ToGridPoint(screenPosition, eventCamera);
        itemRect.anchoredPosition = pointer;
        itemRect.localRotation = RotationOf(rotated);

        Vector2Int size = Inventory.SizeOf(entry.Item, rotated);
        Vector2Int target = TargetCell(pointer, size);
        dropPreview.gameObject.SetActive(true);
        dropPreview.color = inventory.CanPlace(entry.Item, target.x, target.y, rotated, entry) ? validDropColor : invalidDropColor;
        dropPreview.rectTransform.anchoredPosition = CenterOf(target.x, target.y, size);
        dropPreview.rectTransform.sizeDelta = (Vector2)size * cellSize - Vector2.one * cellGap;
    }

    public void FinishDrag(InventoryEntry entry, Vector2 screenPosition, Camera eventCamera, bool rotated) {
        Vector2Int size = Inventory.SizeOf(entry.Item, rotated);
        Vector2Int target = TargetCell(ToGridPoint(screenPosition, eventCamera), size);
        // A successful move redraws through Changed. A failed one needs this to snap the item back.
        if (!inventory.TryMove(entry, target.x, target.y, rotated)) {
            Refresh();
        }
    }

    private void Refresh() {
        foreach (GameObject view in itemViews) {
            Destroy(view);
        }
        itemViews.Clear();
        dropPreview.gameObject.SetActive(false);
        foreach (InventoryEntry entry in inventory.Entries) {
            itemViews.Add(CreateItemView(entry));
        }
    }

    private void BuildCells() {
        for (int y = 0; y < inventory.Height; y++) {
            for (int x = 0; x < inventory.Width; x++) {
                Image cell = CreateImage("Cell", grid);
                cell.sprite = cellSprite;
                cell.color = cellColor;
                cell.raycastTarget = false;
                cell.rectTransform.anchoredPosition = CenterOf(x, y, Vector2Int.one);
                cell.rectTransform.sizeDelta = Vector2.one * ( cellSize - cellGap );
            }
        }
    }

    private GameObject CreateItemView(InventoryEntry entry) {
        Image background = CreateImage(entry.Item.name, grid);
        background.color = itemBackgroundColor;
        // Sized upright and turned afterwards, so the icon turns with it and stays undistorted.
        background.rectTransform.sizeDelta = (Vector2)Inventory.SizeOf(entry.Item, false) * cellSize - Vector2.one * cellGap;
        background.rectTransform.anchoredPosition = CenterOf(entry.X, entry.Y, entry.Size);
        background.rectTransform.localRotation = RotationOf(entry.Rotated);

        Image icon = CreateImage("Icon", background.rectTransform);
        icon.sprite = entry.Item.ItemSprite;
        icon.preserveAspect = true;
        icon.raycastTarget = false;
        Stretch(icon.rectTransform);
        if (entry.Count > 1) {
            AddCountLabel(background.rectTransform, entry);
        }

        background.gameObject.AddComponent<InventoryItemView>().Setup(this, entry);
        return background.gameObject;
    }

    private void AddCountLabel(RectTransform parent, InventoryEntry entry) {
        // A dark patch keeps the number readable on any sprite.
        Image backdrop = CreateImage("Count", parent);
        backdrop.color = new Color(0f, 0f, 0f, 0.8f);
        backdrop.raycastTarget = false;
        backdrop.rectTransform.anchorMin = new Vector2(1f, 0f);
        backdrop.rectTransform.anchorMax = new Vector2(1f, 0f);
        backdrop.rectTransform.pivot = new Vector2(1f, 0f);
        backdrop.rectTransform.anchoredPosition = Vector2.zero;
        backdrop.rectTransform.sizeDelta = Vector2.one * cellSize * 0.42f;
        // Children turn with a turned item, but the number should stay upright.
        backdrop.rectTransform.localRotation = Quaternion.Inverse(RotationOf(entry.Rotated));

        var labelObject = new GameObject("Number", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        labelObject.layer = parent.gameObject.layer;
        labelObject.transform.SetParent(backdrop.rectTransform, false);
        TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
        label.text = entry.Count.ToString();
        label.fontSize = cellSize * 0.36f;
        label.fontStyle = FontStyles.Bold;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;
        Stretch(label.rectTransform);
    }

    private static void Stretch(RectTransform rect) {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private Image CreateImage(string objectName, Transform parent) {
        var imageObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.layer = parent.gameObject.layer;
        imageObject.transform.SetParent(parent, false);
        Image image = imageObject.GetComponent<Image>();
        // Everything is placed from the grid's top-left corner, which is where cell (0,0) starts.
        image.rectTransform.anchorMin = new Vector2(0f, 1f);
        image.rectTransform.anchorMax = new Vector2(0f, 1f);
        return image;
    }

    private Vector2 ToGridPoint(Vector2 screenPosition, Camera eventCamera) {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(grid, screenPosition, eventCamera, out Vector2 point);
        return point;
    }

    /// <summary>
    /// The pointer holds an item at its middle, so the item's top-left cell lies half its size away.
    /// </summary>
    private Vector2Int TargetCell(Vector2 gridPoint, Vector2Int size) {
        return new Vector2Int(
            Mathf.RoundToInt(gridPoint.x / cellSize - size.x / 2f),
            Mathf.RoundToInt(-gridPoint.y / cellSize - size.y / 2f));
    }

    /// <summary>
    /// Centre of an area in grid units. Items are placed by their centre because they turn around it.
    /// </summary>
    private Vector2 CenterOf(int x, int y, Vector2Int size) {
        return new Vector2(( x + size.x / 2f ) * cellSize, -( y + size.y / 2f ) * cellSize);
    }

    private static Quaternion RotationOf(bool rotated) {
        return Quaternion.Euler(0f, 0f, rotated ? -90f : 0f);
    }
}
