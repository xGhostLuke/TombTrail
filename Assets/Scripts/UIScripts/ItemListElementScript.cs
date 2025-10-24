using UnityEngine;

public class ItemListElementScript : MonoBehaviour {

    private GameController gameController;
    private ItemObject item;

    public void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public void PressedOnListElement() {
        gameController.setPlayerWeopon(( WeoponObeject ) GetItem());
    }

    public ItemObject GetItem() {
        return item;
    }

    public void SetItemObject(ItemObject _item) {
        _item = item;
    }

}
