# 🎮 Inventar-System - Integration & Setup

## ✅ Erstellte Komponenten

### 1. **Item-System**
- ✅ 3 Items erstellt:
  - **Health Potion** (1x1 Grid, Heilwert 20)
  - **Very Broken Sword** (2x1 Grid, Schaden 3)
  - **Rusty Sword** (2x1 Grid, Schaden 5)

### 2. **Inventar-UI (5x5 Grid)**
- `InventoryUI.cs` - Toggle mit "I"-Taste
- `InventorySystem.cs` - 5x5 Slot-Management
- `InventoryDisplay.cs` - Drag-and-Drop UI
- `InventorySlot.cs` - Slot-Rendering

### 3. **Item-Pickup-System**
- `PlayerInventory.cs` - Spieler-Inventar-Verwaltung
- `PickableItem.cs` - Items auf dem Spielfeld
- `ItemPickupInteraction.cs` - E-Taste zum Aufnehmen

---

## 🛠️ Setup in Unity (Schritt für Schritt)

### **Step 1: Inventar-UI Canvas erstellen**

1. Neue Scene oder im Spielfeld:
   - Erstelle Canvas (Rechtsklick Scene → UI → Canvas)
   - Name: `InventoryCanvas`

2. Canvas-Settings:
   - Canvas Scaler → Reference Resolution: 1920x1080
   - Render Mode: Screen Space - Overlay

### **Step 2: Inventar-UI Komponenten hinzufügen**

1. Neue UI-Struktur unter Canvas:
   ```
   Canvas
   └── InventoryPanel (Image, grau/dunkel)
       ├── GridContainer (Image, 320x320px)
       └── CloseButton (Button, optional)
   ```

2. InventoryUI.cs an Canvas zuweisen:
   - Public Field "Inventory Panel" → InventoryPanel (Image)
   - Public Field "Grid Container" → GridContainer (Image)

### **Step 3: InventoryDisplay & Grid Setup**

1. Slot-Prefab erstellen:
   - Neue UI → Image (nennen: SlotPrefab)
   - Größe: 64x64px
   - Farbe: Hellgrau
   - Kinder:
     ```
     SlotPrefab
     └── ItemDisplay (Image, 62x62px, weiß)
         └── QuantityText (Text/TextMeshPro)
     ```

2. Slot-Prefab speichern:
   - Drag ins Assets/Prefabs/ Folder

3. GridContainer:
   - GridLayoutGroup hinzufügen
   - Constraint: Preferred Size
   - Cell Size: 64x64px
   - Spacing: 2,2

4. InventoryDisplay an GridContainer:
   - Prefab Reference → SlotPrefab
   - Grid Columns: 5
   - Grid Rows: 5

### **Step 4: PlayerInventory Setup**

1. PlayerController Gameobject:
   - Component hinzufügen: PlayerInventory.cs
   - Public Field "Inventory System" → InventorySystem (vom GridContainer/InventoryDisplay)

2. Test Items hinzufügen (Editor oder Runtime):
   ```csharp
   PlayerInventory playerInv = GetComponent<PlayerInventory>();
   HealingObject potion = AssetDatabase.LoadAssetAtPath<HealingObject>("Assets/Items/HealthPotion.asset");
   playerInv.AddItem(potion);
   ```

### **Step 5: Item-Pickup Setup**

1. Player-Gameobject:
   - Component hinzufügen: `ItemPickupInteraction.cs`
   - Public Field "Player Inventory" → PlayerController Gameobject

2. PickableItem-Prefab erstellen:
   - Neue 2D Sprite (nennen: `ItemPickup`)
   - Components:
     - SpriteRenderer (mit Item-Sprite)
     - CircleCollider2D (Is Trigger, Radius ~0.5)
     - Rigidbody2D (Body Type: Dynamic → Gravity: 0)
   - Script: `PickableItem.cs` zuweisen
   - Public Field "Target Transform" → Player

3. Im Level Items spawnen:
   ```csharp
   // In Level-Spawn-Code:
   PickableItem item = Instantiate(itemPickupPrefab, spawnPos, Quaternion.identity);
   item.SetItemData(healthPotionAsset);
   ```

### **Step 6: Testen**

1. Play Mode starten
2. Drücke **"I"** → Inventar sollte auf/zu gehen
3. Drücke **"T"** (InventorySystemTest.cs) → Test-Items hinzufügen
4. Drag Items in Slots → sollten sich bewegen lassen
5. **"E"** bei PickableItem → sollte ins Inventar gehen

---

## 📋 Checkliste

- [ ] Canvas erstellt (InventoryCanvas)
- [ ] InventoryUI.cs an Canvas
- [ ] SlotPrefab erstellt (64x64)
- [ ] GridContainer mit GridLayoutGroup konfiguriert
- [ ] InventoryDisplay mit SlotPrefab verlinkt
- [ ] PlayerInventory an Player-Controller
- [ ] ItemPickupInteraction an Player
- [ ] PickableItem-Prefab erstellt
- [ ] 3 Items im Assets/Items/ Folder (HealthPotion, VeryBrokenSword, RustySword)

---

## 🔧 Debugging

**Inventar öffnet sich nicht:**
- Prüfe ob "I"-Input im InputManager definiert ist
- Prüfe ob InventoryUI.cs aktiv ist

**Items lassen sich nicht draggen:**
- Canvas Debug → Check ob UI-Rays von Camera getroffen werden
- GraphicRaycaster an Canvas

**Items verschwinden beim Pickup:**
- Prüfe PickableItem.cs → pickup-Animation und Destroy-Verzögerung

**Grid voll, Items können nicht platziert werden:**
- Inventar ist 5x5 = 25 Slots
- 2x1 Items nehmen 2 Plätze
- Prüfe ob richtige gridWidth/gridHeight in Items gesetzt sind

---

## 📚 Weitere Schritte

- [ ] Echte Sprites für Items ersetzen (momentan Platzhalter)
- [ ] Sound Effects für Pickup (itemPickup.OnItemPickedUp += PlaySFX)
- [ ] Item-Beschreibung/Tooltip anzeigen
- [ ] Drop-Funktion (Items aus Inventar werfen)
- [ ] Item-Verbrauch (Potion trinken = Invetar-Slot leeren)
