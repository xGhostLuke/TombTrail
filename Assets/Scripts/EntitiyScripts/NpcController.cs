using Scripts;

using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A villager standing on the grid. Deliberately not an EntityController:
/// it must not be attackable and takes no turns.
/// </summary>
public class NpcController : MonoBehaviour {
    [SerializeField] private GameController gameController;
    [SerializeField] private int x, y;
    [SerializeField] private string greeting = "Villager: Welcome, traveler.";

    private void Start() {
        gameController.SetTile(x, y, TileValue.Npc);
        transform.position = gameController.TileToWorldCoordinate(x, y);
    }

    private void Update() {
        // A key instead of a click, because clicks are not filtered by the UI yet.
        if (Keyboard.current.eKey.wasPressedThisFrame && PlayerCanTalk()) {
            Talk();
        }
    }

    private bool PlayerCanTalk() {
        Vector2Int player = gameController.LocatePlayer();
        return gameController.IsMyTurn(TurnOwner.Player)
            && gameController.GetDistanceToTile(player.x, player.y, x, y) == 1;
    }

    private void Talk() {
        gameController.infoTab.StandardLogMessage(greeting);
        gameController.infoTab.ShowInteractionlog();
    }
}
