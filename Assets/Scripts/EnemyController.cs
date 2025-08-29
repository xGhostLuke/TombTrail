using System;

using NUnit.Framework.Constraints;

using Scripts;

using UnityEngine;

public class EnemyController : EntityController {

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Start() {
        type = TileValue.Enemy;
        gameController.SetTile(x, y, type);
        UpdatePosition(x, y);
        isAlive = true;
        status = EntityStatus.Hostile;
    }

    private void Update() {
        if (health <= 0) {
            gameController.SetTile(x, y, 0);
            isAlive = false;
            status = EntityStatus.Dead;
            gameObject.SetActive(false);
        }
    }

    override public void Movement() {
        if (!hasMovedThisTurn) {
            Vector2 playerPos = gameController.LocatePlayer();
            if (playerPos.y > y) Move(0, 1, type);
            else if (playerPos.y < y) Move(0, -1, type);
            else if (playerPos.x > x) Move(1, 0, type);
            else if (playerPos.x < x) Move(-1, 0, type);
        }
    }
    override public void Attack() {
        if (health > 0) {
            playerController.AdjustHealth(-attackDamage);
        }
    }
}
