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

    // Update is called once per frame
    void Update() {
        if (player.getHealth() / 10 < numberOfHeart) {
            image.enabled = false;
        } else {
            image.enabled = true;
        }
    }
}
