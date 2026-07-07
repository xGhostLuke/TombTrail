# Item-Pickup und Inventar-Verwaltungs-System - Setup-Anleitung

## Überblick
Das System besteht aus 4 neuen/erweiterten Scripts:
- **ItemObject.cs** (erweitert) - ScriptableObject mit gridWidth/gridHeight
- **PickableItem.cs** - Komponente für Items auf dem Spielfeld
- **PlayerInventory.cs** - Inventar-Komponente für Player
- **ItemPickupInteraction.cs** - Input-Handling und Raycast für Pickups

## Komponenten-Beschreibung

### 1. ItemObject.cs (Erweiterung)
```csharp
public int gridWidth = 1;  // 1 für Potions, 2 für Schwerter
public int gridHeight = 1; // 1 für die meisten Items
```

**Verwendung:**
- Alle Item-ScriptableObjects (Potions, Waffen, etc.) erben von ItemObject
- Bei der Erstellung eines neuen Items die Grid-Dimensionen setzen

**Beispiele:**
- Healing Potion: 1x1
- Schwert: 2x1
- Schild: 1x2
- Rüstung: 2x2

---

### 2. PickableItem.cs
**Komponente für Items auf dem Spielfeld**

**Hauptfunktionen:**
- Float-Animation auf dem Boden (Sinus-Bewegung)
- Proximity-Sensor (Visual-Feedback: Item leuchtet gelb auf wenn nah genug)
- Pickup-Animation (schwebt zum Player, Fade-out)
- Event-System für Feedback

**Inspector-Properties:**
```
pickupRange: 1.5      // Reichweite zum Aufnehmen
floatHeight: 0.3      // Höhe der Bob-Animation
floatSpeed: 2         // Geschwindigkeit der Animation
```

**Prefab-Setup:**
```
PickableItem (GameObject)
├── SpriteRenderer (Color = White)
├── Collider2D (isTrigger = true)
├── Rigidbody2D (Body Type = Dynamic, Gravity = 0)
└── PickableItem.cs (Script)
```

---

### 3. PlayerInventory.cs
**Inventar-Komponente für den Player**

**Hauptfunktionen:**
- AddItem(ItemObject) - automatische Platzierung im Grid
- Feedback-Events (ItemAdded, InventoryFull)
- Kompatibilität mit InventorySystem vom UI-Agent
- Netzwerk-ready Struktur

**Inspector-Setup:**
- PlayerInventory.cs zur PlayerController-GameObject hinzufügen
- InventorySystem wird automatisch gesucht (als Komponente oder im GameController)

**Events subscriben:**
```csharp
playerInventory.ItemAddedCallback += HandleItemAdded;
playerInventory.InventoryFullCallback += HandleInventoryFull;

void HandleItemAdded(ItemObject item, bool success) {
    if (success) {
        Debug.Log($"Item {item.name} aufgehoben!");
        // UI aktualisieren
    } else {
        Debug.Log("Inventar voll!");
    }
}
```

---

### 4. ItemPickupInteraction.cs
**Input-Handling und Pickup-Logik**

**Hauptfunktionen:**
- E-Taste für Pickup (InputSystem)
- Raycast + OverlapCircle zur Item-Erkennung
- Ruft PlayerInventory.AddItem() auf
- Zerstört PickableItem nach erfolgreicher Aufnahme

**Inspector-Setup:**
- ItemPickupInteraction.cs zur PlayerController-GameObject hinzufügen
- pickupDistance: 2.0 (Meter)
- pickableLayer: Layer-Mask für PickableItems setzen (optional)

---

## Unity-Setup Schritt-für-Schritt

### Schritt 1: Item-Layer erstellen
1. **Project Settings → Tags and Layers**
2. Add Layer: "PickableItem"
3. Alle PickableItem-Prefabs auf diesen Layer setzen

### Schritt 2: PlayerController aktualisieren
1. **Hierarchy**: Öffne Player-GameObject
2. **Add Component**:
   - PlayerInventory.cs
   - ItemPickupInteraction.cs
3. **In beide Scripts**: InventorySystem-Referenz wird automatisch gesucht

### Schritt 3: PickableItem-Prefab erstellen
1. **Create new Prefab**: Assets/Prefabs/PickableItem.prefab
2. **Hierarchy des Prefabs**:
   ```
   PickableItem (GameObject)
   └── SpriteRenderer
   ```

3. **Komponenten hinzufügen**:
   - SpriteRenderer
   - CircleCollider2D (radius = 0.5, isTrigger = true)
   - Rigidbody2D (Body Type = Static oder Dynamic, Gravity Scale = 0)
   - PickableItem.cs

4. **PickableItem Script Inspector**:
   - ItemData: Drag-drop ein ItemObject (z.B. Potion)
   - pickupRange: 1.5
   - floatHeight: 0.3
   - floatSpeed: 2.0

5. **Speichern als Prefab**

### Schritt 4: Test-Items spawnen
```csharp
// In irgendeinem Script (z.B. GameController)
public void SpawnPickableItem(ItemObject item, Vector3 position) {
    GameObject pickableGO = Instantiate(pickableItemPrefab, position, Quaternion.identity);
    PickableItem pickable = pickableGO.GetComponent<PickableItem>();
    // Item-Daten würden hier gesetzt (aber aktuell über Prefab-Zuordnung)
}
```

---

## Workflow: Item-Pickup implementieren

### Beispiel 1: Einfaches Schwert auf Boden
1. Erstelle ItemObject ScriptableObject:
   - Name: "IronSword"
   - gridWidth: 2, gridHeight: 1
   - ItemSprite: Schwert-Sprite

2. Erstelle Prefab mit PickableItem.cs
3. Stelle pickupRange auf 2.0
4. Platziere in Level
5. Spieler geht nah ran → Item leuchtet gelb auf
6. E-Taste drücken → Item im Inventar

### Beispiel 2: Healtrank mit Sound
```csharp
// In PlayerInventory
public override void AddItem(ItemObject item) {
    if (base.AddItem(item)) {
        // TODO: AudioManager.PlaySound("pickup", transform.position);
        ItemAddedCallback?.Invoke(item, true);
    }
}
```

---

## Event-System für Feedback

### PlayerInventory Events
```csharp
// Item erfolgreich/erfolglos aufgehoben
ItemAddedCallback(ItemObject item, bool success)

// Inventar ist voll
InventoryFullCallback()
```

### PickableItem Events
```csharp
// Wird aufgerufen wenn Item aufgehoben wird (für Animations-Sync)
PickupAttempted(bool success)
```

---

## Sound-Integration (Vorbereitet)
```csharp
// In ItemPickupInteraction.PickupItem()
// TODO: PlayPickupSound();

// In PlayerInventory.AddItem()
// TODO: AudioManager.PlaySound("itemPickup", transform.position);

// In PickableItem.PickUp()
// TODO: AudioManager.PlaySound("pickup", transform.position);
```

---

## Netzwerk-Struktur (Vorbereitet)

Alle Scripts haben Platzhalter für Multiplayer:

### PlayerInventory
```csharp
public void SyncInventoryToNetwork() {
    // TODO: photonView.RPC("RPC_SyncInventory", RpcTarget.All, GetInventoryData());
}
```

### ItemPickupInteraction
```csharp
private void NotifyNetworkPickup(ItemObject item) {
    // TODO: photonView.RPC("RPC_PickupItem", RpcTarget.All, item.name);
}
```

### PickableItem
```csharp
private void OnDestroy() {
    // TODO: Network-Synchronisation wenn nötig
}
```

---

## Debug & Testing

### Gizmos in Scene-View
- ItemPickupInteraction zeigt grüne Pickup-Range als Sphäre

### Console Debug-Ausgaben
```
PlayerInventory: Kein Platz für Item (2x1)!
ItemPickupInteraction: Inventar voll oder Item passt nicht!
```

### Häufige Fehler
1. **InventorySystem nicht gefunden**
   → Stelle sicher InventorySystem ist im GameController oder als Komponente
   
2. **Item leuchtet nicht auf**
   → Prüfe SpriteRenderer.color wird gesetzt, pickupRange ist groß genug
   
3. **Pickup funktioniert nicht**
   → Verifiziere Player-GameObject hat "Player" Tag
   → Raycast/OverlapCircle findet Item: visualisiere mit Gizmos

---

## Performance-Optimierungen

1. **Object Pooling für PickableItems** (zukünftig)
   ```csharp
   // PickableItemPool.cs
   public class PickableItemPool : MonoBehaviour {
       // Poolt häufige Item-Spawns (z.B. Loot nach Kampf)
   }
   ```

2. **Raycast-Optimierung**
   - Nutze Layer-Mask um nur PickableItems zu raycasten
   - OverlapCircle hat höhere Performance für mehrere Items

3. **Animation-Optimierung**
   - FloatCoroutine nur wenn Item sichtbar ist
   - Nutze LateUpdate statt Update für Position-Sync

---

## Kompatibilität

✅ **Funktioniert mit:**
- PlayerController (EntityController)
- InventorySystem (5x5 Grid-Management)
- ItemObject (ScriptableObject base)
- WeoponObject, HealingObject (Item-Subclasses)

⚠️ **Abhängigkeiten:**
- Spieler benötigt GameObject mit Tag "Player"
- InventorySystem muss vorhanden sein (wird automatisch gesucht)
- InputSystem muss aktiviert sein

---

## Checkliste zur Integration

- [ ] ItemObject.cs erweitert (gridWidth, gridHeight Properties)
- [ ] PickableItem.cs im GeneralScripts/ Ordner
- [ ] PlayerInventory.cs im EntitiyScripts/ Ordner
- [ ] ItemPickupInteraction.cs im UIScripts/ Ordner
- [ ] PickableItem-Prefab erstellt
- [ ] ItemPickupInteraction zu Player-GameObject hinzugefügt
- [ ] PlayerInventory zu Player-GameObject hinzugefügt
- [ ] Player-GameObject hat "Player" Tag
- [ ] InputSystem aktiviert (Project Settings)
- [ ] Test: Item auf Boden, nah gehen, E-Taste drücken
