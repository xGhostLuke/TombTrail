using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Input-Handling für Item-Pickups.
/// Erkennt E-Taste, prüft Nähe via Raycast/Overlap, ruft AddItem auf.
/// Zerstört PickableItem nach erfolgreicher Aufnahme.
/// </summary>
public class ItemPickupInteraction : MonoBehaviour {
    private PlayerInventory playerInventory;
    private PlayerController playerController;

    [SerializeField] private float pickupDistance = 2f;
    [SerializeField] private LayerMask pickableLayer = -1; // Nur PickableItems

    // Input-Events
    private InputAction pickupAction;

    private void OnEnable() {
        if (pickupAction == null) {
            pickupAction = new InputAction("Pickup", InputActionType.Button, "<Keyboard>/e");
            pickupAction.Enable();
        }
        pickupAction.performed += OnPickupInput;
    }

    private void OnDisable() {
        if (pickupAction != null) {
            pickupAction.performed -= OnPickupInput;
        }
    }

    private void Start() {
        playerInventory = GetComponent<PlayerInventory>();
        if (playerInventory == null) {
            playerInventory = GetComponentInChildren<PlayerInventory>();
        }

        playerController = GetComponent<PlayerController>();
        if (playerController == null) {
            playerController = GetComponentInParent<PlayerController>();
        }

        if (playerInventory == null) {
            Debug.LogError("ItemPickupInteraction: PlayerInventory-Komponente nicht gefunden!");
        }
    }

    /// <summary>
    /// Wird aufgerufen wenn E-Taste gedrückt wird
    /// </summary>
    private void OnPickupInput(InputAction.CallbackContext context) {
        if (!context.performed) {
            return;
        }

        AttemptPickup();
    }

    /// <summary>
    /// Versucht Items in Reichweite aufzusammeln
    /// </summary>
    private void AttemptPickup() {
        Vector3 pickupOrigin = transform.position;

        // Raycast nach PickableItem
        PickableItem nearestItem = FindNearestPickableItem(pickupOrigin);

        if (nearestItem != null && nearestItem.IsInRange(pickupOrigin)) {
            PickupItem(nearestItem);
        }
    }

    /// <summary>
    /// Sucht das nächste aufnehmbares Item mit Raycast
    /// </summary>
    private PickableItem FindNearestPickableItem(Vector3 origin) {
        // Fallback: OverlapCircle nach allen PickableItems in Range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(origin.x, origin.y), pickupDistance);
        PickableItem nearest = null;
        float nearestDistance = pickupDistance;

        foreach (Collider2D collider in colliders) {
            PickableItem pickable = collider.GetComponent<PickableItem>();
            if (pickable != null) {
                float distance = Vector3.Distance(origin, collider.transform.position);
                if (distance < nearestDistance) {
                    nearest = pickable;
                    nearestDistance = distance;
                }
            }
        }

        return nearest;
    }

    /// <summary>
    /// Verarbeitet erfolgreichen Pickup
    /// </summary>
    private void PickupItem(PickableItem pickableItem) {
        ItemObject itemData = pickableItem.GetItemData();

        if (itemData == null) {
            Debug.LogWarning("ItemPickupInteraction: ItemObject ist null!");
            return;
        }

        // Versuche Item zu Inventar hinzufügen
        if (playerInventory.AddItem(itemData)) {
            // Erfolg: Animation starten und dann zerstören
            pickableItem.PickUp();

            // TODO: Sound-Feedback wird über Events gesteuert
            // PlayPickupSound();

            Destroy(pickableItem.gameObject, 0.3f); // Nach Animation zerstören
        } else {
            // Fehler: Inventar wahrscheinlich voll
            Debug.Log("ItemPickupInteraction: Inventar voll oder Item passt nicht!");
            // TODO: UI-Feedback "Inventar voll"
        }
    }

    /// <summary>
    /// Visualisierung der Pickup-Range im Editor
    /// </summary>
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupDistance);
    }

    /// <summary>
    /// Network-ready: Wird später für Multiplayer erweitert
    /// </summary>
    private void NotifyNetworkPickup(ItemObject item) {
        // TODO: Netzwerk-RPC für Multiplayer
        // photonView.RPC("RPC_PickupItem", RpcTarget.All, item.name);
    }
}
