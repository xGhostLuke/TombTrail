using System;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : EntityController
{

    private bool hasMoved;
    private void Start()
    {
        //REMINDER: x and y needs to be adjusted if the Grid got changed by alot (idk how to fix this)
        gameController.SetTile(x, y, TileType.Player);
        UpdatePosition(x, y);
        // TODO: again what is this magic number?
        type = 1;
        hasMoved = false;
    }

    private void Update()
    {
        // TODO: please no magic numbers
        if (!gameController.IsMyTurn(1)) return;

        Movement();
    }

    override public void Movement()
    {
        Keyboard current = Keyboard.current;
        if (current.wKey.wasPressedThisFrame) Move(0, -1, type);
        if (current.sKey.wasPressedThisFrame) Move(0, 1, type);
        if (current.dKey.wasPressedThisFrame) Move(1, 0, type);
        if (current.aKey.wasPressedThisFrame) Move(-1, 0, type);
        if (current.spaceKey.wasPressedThisFrame) Move(0, 0, type);
    }

    override public void Attack() => throw new NotImplementedException();
}
