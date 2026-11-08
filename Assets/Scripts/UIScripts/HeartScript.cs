using UnityEngine;
using UnityEngine.UI;

public class HeartScript : MonoBehaviour {
    private PlayerController player;
    private Image image;
    [SerializeField] private int numberOfHeart;
    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        image = gameObject.GetComponent<Image>();
    }

    /// <summary>
    /// Called once per frame. Toggles the heart UI image on or off based on the player's current health.
    /// </summary>
    /// <remarks>
    /// The heart image is enabled when the player's health, rounded up to blocks of 10, reaches numberOfHeart.
    /// Rounding up keeps a damaged heart visible: 39 health shows four hearts, only 0 health shows none.
    /// Ensure 'player' and 'image' are assigned to avoid NullReferenceException.
    /// </remarks>
    void Update() {
        if (Mathf.CeilToInt(player.GetHealth() / 10f) < numberOfHeart) {
            image.enabled = false;
        } else {
            image.enabled = true;
        }
    }
}
