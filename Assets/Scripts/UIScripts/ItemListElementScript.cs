using Unity.VisualScripting;

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
    /// Prints out a Log message if the item is from an unknown type
    /// </summary>
    public void PressedOnListElement() {
        if (gameController.IsGameOver()) {
            return;
        }

        Vector2Int playerPos = gameController.LocatePlayer();
        switch (GetItem()) {
            case WeoponObeject weoponObeject:
                gameController.setPlayerWeopon(weoponObeject);
                gameController.removeItemFromTile(playerPos.x, playerPos.y, weoponObeject);
                break;
            case HealingObject healingObject:
                PlayerController player = gameController.playerController;
                if (player.GetHealth() >= player.GetMaxHealth()) {
                    break; // Nothing to heal: the potion stays on the tile until the inventory replaces this.
                }
                gameController.AdjustPlayerHealth(healingObject.GetHealingEffect());
                gameController.removeItemFromTile(playerPos.x, playerPos.y, healingObject);
                break;
            default:
                Debug.Log("Item Type pickup not Implemented");
                break;
        }
    }

    public ItemObject GetItem() {
        return item;
    }

    public void SetItemObject(ItemObject _item) {
        item = _item;
    }

}
