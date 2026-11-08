using Scripts;

using UnityEngine;

/// <summary>
/// A villager standing on the grid. Deliberately not an EntityController:
/// it must not be attackable and takes no turns.
/// </summary>
public class NpcController : MonoBehaviour {
    [SerializeField] private GameController gameController;
    [SerializeField] private int x, y;

    private void Start() {
        gameController.SetTile(x, y, TileValue.Npc);
        transform.position = gameController.TileToWorldCoordinate(x, y);
    }
}
