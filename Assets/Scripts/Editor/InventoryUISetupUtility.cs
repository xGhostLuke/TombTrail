using UnityEngine;
using UnityEngine.UI;

using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Editor-Utility zum automatischen Erstellen der Inventar-UI Struktur
/// Menü: Tools > Inventory > Setup Inventory UI
/// </summary>
public class InventoryUISetupUtility {
    [MenuItem("Tools/Inventory/Setup Inventory UI")]
    public static void SetupInventoryUI() {
        // Canvas erstellen
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) {
            GameObject canvasGO = new GameObject("InventoryCanvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
        }

        // InventoryPanel (Hintergrund)
        GameObject panelGO = new GameObject("InventoryPanel");
        panelGO.transform.SetParent(canvas.transform, false);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(330, 350);

        // GridContainer fuer 5x5 Slots
        GameObject gridGO = new GameObject("GridContainer");
        gridGO.transform.SetParent(panelGO.transform, false);
        Image gridImage = gridGO.AddComponent<Image>();
        gridImage.color = new Color(0.05f, 0.05f, 0.05f, 1f);
        RectTransform gridRect = gridGO.GetComponent<RectTransform>();
        gridRect.sizeDelta = new Vector2(320, 320);

        GridLayoutGroup gridLayout = gridGO.AddComponent<GridLayoutGroup>();
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 5;
        gridLayout.cellSize = new Vector2(64, 64);
        gridLayout.spacing = new Vector2(0, 0);

        // InventorySystem finden oder erstellen
        InventorySystem invSystem = Object.FindFirstObjectByType<InventorySystem>();
        if (invSystem == null) {
            GameObject systemGO = new GameObject("InventorySystem");
            invSystem = systemGO.AddComponent<InventorySystem>();
        }

        // InventoryUI Script an Canvas
        InventoryUI invUI = canvas.gameObject.GetComponent<InventoryUI>();
        if (invUI == null) {
            invUI = canvas.gameObject.AddComponent<InventoryUI>();
        }

        // InventoryDisplay Script an GridContainer
        InventoryDisplay invDisplay = gridGO.GetComponent<InventoryDisplay>();
        if (invDisplay == null) {
            invDisplay = gridGO.AddComponent<InventoryDisplay>();
        }

        // Lade SlotPrefab
        GameObject slotPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SlotPrefab.prefab");
        if (slotPrefab == null) {
            Debug.LogError("SlotPrefab.prefab nicht gefunden!");
            return;
        }

        // Setze die Referenzen mittels Reflection
        var displayField = typeof(InventoryDisplay).GetField("inventorySystem", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (displayField != null) displayField.SetValue(invDisplay, invSystem);
        
        displayField = typeof(InventoryDisplay).GetField("gridContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (displayField != null) displayField.SetValue(invDisplay, gridGO.transform);
        
        displayField = typeof(InventoryDisplay).GetField("slotPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (displayField != null) displayField.SetValue(invDisplay, slotPrefab);

        // Setze die Referenzen in InventoryUI
        var uiField = typeof(InventoryUI).GetField("inventoryCanvas", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (uiField != null) uiField.SetValue(invUI, canvas);
        
        uiField = typeof(InventoryUI).GetField("inventorySystem", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (uiField != null) uiField.SetValue(invUI, invSystem);
        
        uiField = typeof(InventoryUI).GetField("inventoryDisplay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (uiField != null) uiField.SetValue(invUI, invDisplay);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Inventar-UI erfolgreich erstellt!");
    }

    [MenuItem("Tools/Inventory/Create Slot Prefab")]
    public static void CreateSlotPrefab() {
        // SlotPrefab erstellen
        GameObject slotGO = new GameObject("SlotPrefab");
        Image slotImage = slotGO.AddComponent<Image>();
        slotImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        RectTransform slotRect = slotGO.GetComponent<RectTransform>();
        slotRect.sizeDelta = new Vector2(64, 64);

        // ItemDisplay (Kind)
        GameObject itemDisplayGO = new GameObject("ItemDisplay");
        itemDisplayGO.transform.SetParent(slotGO.transform, false);
        Image itemDisplayImage = itemDisplayGO.AddComponent<Image>();
        itemDisplayImage.color = new Color(1, 1, 1, 0.5f);
        RectTransform itemDisplayRect = itemDisplayGO.GetComponent<RectTransform>();
        itemDisplayRect.sizeDelta = new Vector2(62, 62);

        // QuantityText (Kind)
        GameObject quantityGO = new GameObject("QuantityText");
        quantityGO.transform.SetParent(slotGO.transform, false);
        Text quantityText = quantityGO.AddComponent<Text>();
        quantityText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        quantityText.text = "1";
        quantityText.alignment = TextAnchor.LowerRight;
        quantityText.fontSize = 12;
        RectTransform quantityRect = quantityGO.GetComponent<RectTransform>();
        quantityRect.anchoredPosition = new Vector2(-2, 2);

        // Prefab speichern
        string path = "Assets/Prefabs/SlotPrefab.prefab";
        PrefabUtility.SaveAsPrefabAsset(slotGO, path);
        Object.DestroyImmediate(slotGO);

        Debug.Log($"✅ SlotPrefab erstellt: {path}");
    }

    [MenuItem("Tools/Inventory/Create Pickable Item Prefab")]
    public static void CreatePickableItemPrefab() {
        // PickableItem Prefab
        GameObject itemGO = new GameObject("PickableItem");

        // Sprite Renderer
        SpriteRenderer spriteRenderer = itemGO.AddComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;

        // Circle Collider
        CircleCollider2D collider = itemGO.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.5f;

        // Rigidbody (no gravity)
        Rigidbody2D rb = itemGO.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;

        // PickableItem Script
        PickableItem pickableScript = itemGO.AddComponent<PickableItem>();

        // Prefab speichern
        string path = "Assets/Prefabs/PickableItem.prefab";
        PrefabUtility.SaveAsPrefabAsset(itemGO, path);
        Object.DestroyImmediate(itemGO);

        Debug.Log($"✅ PickableItem Prefab erstellt: {path}");
    }

    [MenuItem("Tools/Inventory/Quick Setup (All)")]
    public static void QuickSetup() {
        CreateSlotPrefab();
        CreatePickableItemPrefab();
        SetupInventoryUI();
        Debug.Log("Komplettes Inventar-System setup durchgefuehrt!");
    }
}
