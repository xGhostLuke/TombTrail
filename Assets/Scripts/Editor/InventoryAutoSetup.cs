using UnityEngine;
using UnityEngine.UI;

using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Manuelles Setup der Inventar-UI
/// Führe das aus in der Console: InventoryAutoSetup.RunSetup()
/// </summary>
public class InventoryAutoSetup {

    public static void RunSetup() {
        Debug.Log("🔧 Starte Inventar-Setup...");

        // Canvas suchen oder erstellen
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) {
            GameObject canvasGO = new GameObject("InventoryCanvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            Debug.Log("✅ Canvas erstellt");
        }

        // InventoryPanel
        Transform existingPanel = canvas.transform.Find("InventoryPanel");
        if (existingPanel != null) {
            Object.DestroyImmediate(existingPanel.gameObject);
        }

        GameObject panelGO = new GameObject("InventoryPanel");
        panelGO.transform.SetParent(canvas.transform, false);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(330, 350);
        Debug.Log("✅ InventoryPanel erstellt");

        // GridContainer
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
        Debug.Log("✅ GridContainer (5x5) erstellt");

        // InventoryUI Script
        InventoryUI invUI = canvas.gameObject.AddComponent<InventoryUI>();
        Debug.Log("✅ InventoryUI Script hinzugefügt");

        // InventoryDisplay Script
        InventoryDisplay invDisplay = gridGO.AddComponent<InventoryDisplay>();
        Debug.Log("✅ InventoryDisplay Script hinzugefügt");

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("✅✅✅ Inventar-UI komplett erstellt! Du kannst jetzt spielen!");
    }
}
