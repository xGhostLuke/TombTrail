using System;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : EntityController {
    private void Start() {
        x = 0;
        y = 0;
        gameController.SetTile(x, y, 1);
        UpdatePosition(x, y);
        type = 1;
    }

    private void Update() {
        if (gameController.IsMyTurn(1)) {
            Movement();
        }
    }

    override protected void Movement() {
        if (Keyboard.current.wKey.wasPressedThisFrame) Move(0, -1, type);
        if (Keyboard.current.sKey.wasPressedThisFrame) Move(0, 1, type);
        if (Keyboard.current.dKey.wasPressedThisFrame) Move(1, 0, type);
        if (Keyboard.current.aKey.wasPressedThisFrame) Move(-1, 0, type);
    }

    protected override void Attack() => throw new NotImplementedException();


}
