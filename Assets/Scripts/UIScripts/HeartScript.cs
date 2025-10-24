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
    /// The heart image is enabled when (player.GetHealth() / 10) >= numberOfHeart; otherwise it is disabled.
    /// This treats one heart as representing a block of 10 health points. Ensure 'player' and 'image' are assigned
    /// to avoid NullReferenceException. Note that if GetHealth() returns an integer, division is integer division;
    /// use a float cast if fractional heart thresholds are required.
    /// </remarks>
    void Update() {
        if (player.GetHealth() / 10 < numberOfHeart) {
            image.enabled = false;
        } else {
            image.enabled = true;
        }
    }
}
