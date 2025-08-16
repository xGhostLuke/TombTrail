using System;
using NUnit.Framework.Constraints;
using UnityEngine;

public class EnemyController : EntityController {

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void Start() {
        x = 7;
        y = 7;
        gameController.SetTile(x, y, 2);
        UpdatePosition(x, y);
        type = 2;
    }

    private void Update() {
        if (gameController.IsMyTurn(2)) {
            switch (CalculateDistance()) {
                case 1:
                    Attack();
                    break;
                case > 1:
                    Movement();
                    break;
            }
        }
    }

    protected override void Movement() {
        if (!hasMovedThisTurn) {
            Vector2 playerPos = gameController.LocatePlayer();
            if (playerPos.y > y) Move(0, 1, type);
            else if (playerPos.y < y) Move(0, -1, type);
            else if (playerPos.x > x) Move(1, 0, type);
            else if (playerPos.x < x) Move(-1, 0, type);
        }
    }

    private int CalculateDistance() {
        Vector2 playerPos = gameController.LocatePlayer();
        int xDistance = ( int ) Math.Abs(playerPos.x - x);
        int yDistance = ( int ) Math.Abs(playerPos.y - y);
        return xDistance + yDistance;
    }

    protected override void Attack() {
        gameController.IncreaseTurn();
    }
}
