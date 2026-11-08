using System;

using Scripts;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : EntityController {
    private void Start() {
        // REMINDER: x and y needs to be adjusted if the Grid got changed by a lot
        // (idk how to fix this) -> TODO: später über GameController/LevelConfig lösen

        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gameController.SetTile(x, y, TileValue.Player);

        UpdatePosition(x, y);
        type = TileValue.Player;
        status = EntityStatus.Neutral;
    }

    private void Update() {
        if (gameController.IsMyTurn(1)) {
            Movement();
        }
    }

    public override void Movement() {
        if (Keyboard.current.wKey.wasPressedThisFrame) Move(0, -1, type);
        if (Keyboard.current.sKey.wasPressedThisFrame) Move(0, 1, type);
        if (Keyboard.current.dKey.wasPressedThisFrame) Move(1, 0, type);
        if (Keyboard.current.aKey.wasPressedThisFrame) Move(-1, 0, type);
        if (Keyboard.current.spaceKey.wasPressedThisFrame) Move(0, 0, type);
    }

    public override void Attack() {
        throw new NotImplementedException();
        // Managed over GameController, works for now, maybe rework in the future
    }
}
