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

    void Update() {
        // TODO: consider just changing this when adjusting the number of hearts. I doubt, that it actually needs to be calculated every frame
        // TODO: Do you actually mean this condition? or do you only want to enable it, if it's exactly the number of hearts?
        // if (player.GetHealth() / 10 < numberOfHeart) {
        //     image.enabled = false;
        // } else {
        //     image.enabled = true;
        // }
        bool hasValidNumberOfHearts = player.GetHealth() / 10 >= numberOfHeart;

        image.enabled = hasValidNumberOfHearts;
    }
}
