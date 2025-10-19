using System;
using System.Linq.Expressions;

using Scripts;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : EntityController {

    [SerializeField] private WeoponObeject playerWeopon;

    private void Start() {
        //REMINDER: x and y needs to be adjusted if the Grid got changed by alot (idk how to fix this)
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

    override public void Movement() {
        if (Keyboard.current.wKey.wasPressedThisFrame) Move(0, -1, type);
        if (Keyboard.current.sKey.wasPressedThisFrame) Move(0, 1, type);
        if (Keyboard.current.dKey.wasPressedThisFrame) Move(1, 0, type);
        if (Keyboard.current.aKey.wasPressedThisFrame) Move(-1, 0, type);
        if (Keyboard.current.spaceKey.wasPressedThisFrame) Move(0, 0, type);
    }

    override public void Attack() => throw new NotImplementedException(); //Managed over Gamecontroller, works for now, maybe rework in the future

    public WeoponObeject getPlayerWeopon() {
        return playerWeopon;
    }

    public void setPlayerWeopon(WeoponObeject weopon) {
        playerWeopon = weopon;
    }
}
