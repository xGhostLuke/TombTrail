using System;
using System.Collections.Generic;

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
        Vector2Int playerPos = gameController.playerController.GetXY();

        Nodes startNode = pathfindingScript.nodes[x, y];
        Nodes targetNode = pathfindingScript.nodes[playerPos.x, playerPos.y];


        List<Nodes> path = pathfindingScript.FindPath(startNode, targetNode);

        if (path != null && path.Count > 0 && path.Count < viewingDistance) {

            Nodes nextStep = path[0];


            int dx = nextStep.x - x;
            int dy = nextStep.y - y;


            Move(dx, dy, type);
        } else {

            hasMovedThisTurn = true;
        }
    }
    override public void Attack() {
        if (health > 0) {
            playerController.AdjustHealth(-attackDamage);
        }
    }
}
