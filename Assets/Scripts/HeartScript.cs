using UnityEngine;
using UnityEngine.UI;

public class HeartScript : MonoBehaviour
{
    private PlayerController player;
    private Image image;
    [SerializeField] private int numberOfHeart;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        image = gameObject.GetComponent<Image>();
    }

    void Update()
    {
        image.enabled = player.getHealth() / 10 >= numberOfHeart;
    }
}
