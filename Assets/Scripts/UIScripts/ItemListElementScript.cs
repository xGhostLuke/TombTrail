using UnityEngine;

public class ItemListElementScript : MonoBehaviour {

    private GameController gameController;
    [SerializeField] private ItemObject item;

    public void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    /// <summary>
    /// Method used in the Button of the Item list Element
    /// Triggers the setPlayerWeoponMethod to set Item as Player Weopon 
    /// TODO: If the Item isnt a weopon nothing happens for now
    /// </summary>
    public void PressedOnListElement() {
        if (GetItem() is WeoponObeject) {
            gameController.setPlayerWeopon(( WeoponObeject ) GetItem());
            Debug.Log("Player Weopon set to: " + GetItem().name);
            Vector2Int playerPos = gameController.LocatePlayer();
            gameController.removeItemFromTile(playerPos.x, playerPos.y, GetItem());
        } else {
            Debug.Log("Item pickup not Implemented");
        }
    }

    public ItemObject GetItem() {
        return item;
    }

    public void SetItemObject(ItemObject _item) {
        item = _item;
    }

}
