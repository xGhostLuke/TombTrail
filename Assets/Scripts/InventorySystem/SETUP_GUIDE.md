# Inventar-UI-System Setup-Anleitung

## Überblick
Das System besteht aus 4 C#-Scripts:
- **InventorySystem.cs** - Core-Logik (Grid-Verwaltung, Item-Platzierung)
- **InventorySlot.cs** - Einzelner Slot (Visual + Data)
- **InventoryDisplay.cs** - Rendering & Drag-and-Drop
- **InventoryUI.cs** - Manager (Tastatur-Input, Toggle)

## Unity-Hierarchie einrichten

```
Canvas (Inventory UI)
├── GridContainer
│   ├── Slot (0,0) [Prefab-Instance]
│   ├── Slot (1,0)
│   ├── Slot (2,0)
│   └── ... (25 Slots total für 5x5 Grid)
└── (optional) HUDLayer
    └── CloseButton
```

## Schritt-für-Schritt Setup

### 1. Canvas erstellen
- **Hierarchy**: Right-Click → UI → Canvas
- **Name**: "InventoryUI"
- **Canvas Properties**:
  - Render Mode: Screen Space - Overlay
  - Canvas Scaler: Scale Factor = 1 (für Pixel-Perfect)
  - Preferred Width: 320, Preferred Height: 320

### 2. GridContainer erstellen
- **Hierarchy**: Add Image als Child von Canvas
- **Name**: "GridContainer"
- **RectTransform**:
  - Size: 320x320 (5 Slots * 64 Pixel pro Slot)
  - Anchor Preset: Top-Left
  - Position: (160, -160)
- **Layout Group**: Add "GridLayoutGroup"
  - Child Force Expand: Width=False, Height=False
  - Preferred Width: 64, Preferred Height: 64
  - Spacing: 0

### 3. Slot-Prefab erstellen
- **Create new Prefab**: Assets/Prefabs/InventorySlot.prefab
- **Hierarchy des Prefabs**:
  ```
  InventorySlot (Image)
  ├── ItemDisplay (Image) - zeigt Item-Sprite
  └── QuantityText (TextMeshPro) - zeigt Menge
  ```

**InventorySlot (Image)**:
- Size: 64x64
- Color: Medium Gray (180, 180, 180)
- Source Image: Weiße Quadrat-Sprite (oder einfache Placeholder)

**ItemDisplay (Image)**:
- Parent: InventorySlot
- Size: 60x60
- RectTransform: Anchor = Stretch
- Source Image: (wird durch Code gesetzt)

**QuantityText (TextMeshPro)**:
- Parent: InventorySlot
- Text: (wird durch Code gesetzt)
- Font Size: 3
- Alignment: Bottom-Right
- Color: Weiß

### 4. Scripts zur Canvas zuweisen
- **Canvas**:
  - Add Component: InventoryUI
  - Add Component: InventorySystem
  - Add Component: InventoryDisplay
  - Assign "GridContainer" to InventoryDisplay.gridContainer
  - Assign Slot-Prefab zu InventoryDisplay.slotPrefab
  - slotSize: 64

## Verwendung in Code

```csharp
// Inventar öffnen/schließen (auch per "I"-Taste automatisch)
inventoryUI.ToggleInventory();

// Item hinzufügen (1x1)
inventoryUI.AddItemAtFirstAvailable(weaponObject, width: 1, height: 1, quantity: 1);

// Item hinzufügen (2x1 - z.B. großes Schwert)
inventoryUI.AddItemAtFirstAvailable(bigSwordObject, width: 2, height: 1, quantity: 1);

// Item an spezifischer Position hinzufügen
inventoryUI.AddItemAtPosition(healingPotion, x: 0, y: 0, width: 1, height: 1, quantity: 5);

// Item entfernen
inventoryUI.RemoveItemAt(x: 0, y: 0);

// Inventar leeren
inventoryUI.ClearInventory();
```

## Kompatibilität mit existierenden Item-Klassen

Das System funktioniert mit:
- **ItemObject** (Basis-Klasse)
- **HealingObject** (erbt von ItemObject)
- **WeoponObject** (erbt von ItemObject)

Alle Items müssen ein `ItemSprite` haben (wird automatisch angezeigt).

## Performance-Optimierungen

1. **Canvas-Splitting**: Statische UI (Slots) ist auf der Inventory-Canvas, dynamische Elemente optional auf separater Canvas
2. **Slot-Pooling**: Bei Bedarf können Slots gepoolt werden (bei sehr häufigen Änderungen)
3. **Raycast-Optimierung**: InventoryDisplay hat CanvasGroup für effizientes Raycasting bei Drag

## Anmerkungen zur Pixel-Art-Ästhetik

- **slotSize = 64**: Ganzzahlige Pixel für saubere Darstellung
- **Canvas Scaler**: Scale mit Faktor 1 halten für konsistente Pixel-Größen
- **Sprite-Import**: Filter Mode = Point (no filter) für knackscharfe Sprites
- **Layout**: GridLayoutGroup nutzt ganzzahlige Abstände (kein Anti-Aliasing)

## Tastatur-Steuerung

- **I** (Taste): Inventar öffnen/schließen
- **Mouse Drag**: Items verschieben
- **Validierung**: Items können nur auf verfügbare Slots gezogen werden

## Erweiterte Features (Optional)

Für zukünftige Implementierungen:
- Tooltip bei Hover über Items
- Sound-Effekte beim Drag/Drop
- Animationen beim Öffnen/Schließen
- Mehrere Inventar-Tabs
