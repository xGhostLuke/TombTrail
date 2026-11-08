using Unity.VisualScripting;

using UnityEngine;

public class ItemListElementScript : MonoBehaviour {

    private GameController gameController;
    [SerializeField] private ItemObject item;

    public void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    /// <summary>
    /// Method used in the Button of the Item list Element.
    /// Moves the item into the pack, nothing gets used or equipped automatically.
    /// </summary>
    public void PressedOnListElement() {
        if (gameController.IsGameOver()) {
            return;
        }
        gameController.PickUpItem(GetItem());
    }

    public ItemObject GetItem() {
        return item;
    }

    public void SetItemObject(ItemObject _item) {
        item = _item;
    }

}
