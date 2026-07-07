using UnityEngine;

using System.Collections;
using System.Text;

/// <summary>
/// Komponente für Items auf dem Spielfeld.
/// Verwaltet Interaktion mit Player (Nähe-Sensor), Animation und Feedback beim Aufnehmen.
/// </summary>
public class PickableItem : MonoBehaviour {
    [SerializeField] private ItemObject itemData;
    [SerializeField] private float pickupRange = 1.5f;
    [SerializeField] private float floatHeight = 0.3f;
    [SerializeField] private float floatSpeed = 2f;

    private Vector3 startPosition;
    private bool isBeingPickedUp = false;

    // Events für Feedback
    public delegate void OnPickupAttempt(bool success);
    public event OnPickupAttempt PickupAttempted;

    private void Start() {
        startPosition = transform.position;
        PlayFloatAnimation();
    }

    private void Update() {
        // Prüfe ob Player in Reichweite ist
        CheckPlayerProximity();
    }

    /// <summary>
    /// Prüft ob der Player nah genug ist für Pickup-Aktion
    /// </summary>
    private void CheckPlayerProximity() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        // Visual-Feedback: Item leuchtet auf wenn nah genug
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) {
            sr.color = distance <= pickupRange ? Color.yellow : Color.white;
        }
    }

    /// <summary>
    /// Float-Animation für Item auf dem Boden
    /// </summary>
    private void PlayFloatAnimation() {
        StartCoroutine(FloatCoroutine());
    }

    private IEnumerator FloatCoroutine() {
        while (!isBeingPickedUp) {
            float sine = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            transform.position = startPosition + Vector3.up * sine;
            yield return null;
        }
    }

    /// <summary>
    /// Wird aufgerufen wenn Item aufgehoben wird - Animation und Sound-Vorbereitung
    /// </summary>
    public void PickUp() {
        if (isBeingPickedUp) return;
        isBeingPickedUp = true;

        // TODO: Sound-Feedback-Punkt (wird später mit Audio integriert)
        // AudioManager.PlaySound("pickup", transform.position);

        StartCoroutine(PickupAnimationRoutine());
    }

    /// <summary>
    /// Pickup-Animation: Item schwebt zum Player auf und verschwindet
    /// </summary>
    private IEnumerator PickupAnimationRoutine() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            Destroy(gameObject);
            yield break;
        }

        Vector3 startPos = transform.position;
        Vector3 endPos = player.transform.position;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, endPos, t);

            // Fade-out
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) {
                Color color = sr.color;
                color.a = 1f - t;
                sr.color = color;
            }

            yield return null;
        }

        // Destruction erfolgt durch Caller (ItemPickupInteraction.cs)
    }

    /// <summary>
    /// Gibt ItemObject-Referenz zurück
    /// </summary>
    public ItemObject GetItemData() {
        return itemData;
    }

    public void SetItemData(ItemObject itemData) {
        this.itemData = itemData;

        if (itemData == null) { return; }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();


        spriteRenderer.sprite = itemData.GetItemSprite();
    }

    /// <summary>
    /// Prüft ob Item in Pickup-Range des Players ist
    /// </summary>
    public bool IsInRange(Vector3 playerPosition) {
        return Vector3.Distance(transform.position, playerPosition) <= pickupRange;
    }

    /// <summary>
    /// Network-ready: Wird später für Multiplayer erweitert
    /// </summary>
    private void OnDestroy() {
        // TODO: Network-Synchronisation wenn nötig
    }
}
