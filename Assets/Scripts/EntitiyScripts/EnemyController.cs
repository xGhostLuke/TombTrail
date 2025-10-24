using System.Collections.Generic;

using Scripts;

using UnityEngine;

public class EnemyController : EntityController {

    [SerializeField] private List<ItemObject> itemDrops = new List<ItemObject>();

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        infoTab = gameController.infoTab;
    }

    private void Start() {
        type = TileValue.Enemy;
        gameController.SetTile(x, y, type);
        UpdatePosition(x, y);
        isAlive = true;
    }

    private void Update() {
        if (health <= 0) {
            DropItemOnTileWhenDead(x, y);

            gameController.SetTile(x, y, 0);
            isAlive = false;
            status = EntityStatus.Dead;
            gameObject.SetActive(false);
        }
    }

    private void DropItemOnTileWhenDead(int _x, int _y) {
        //for now only drops the first item in list
        gameController.GetTile(_x, _y).addItemToItemList(dropItem());
        gameController.GetTile(_x, _y).addItemToItemList(dropItem());
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

    public ItemObject dropItem() {
        if (itemDrops.Count != 0) {
            return itemDrops[Random.Range(0, itemDrops.Count)];
        }
        return null;
    }

    override public void Attack() {
        if (health > 0 && getStatus() == EntityStatus.Hostile) {
            playerController.AdjustHealth(-attackDamage);
            infoTab.HitLogMessage(this, playerController);
        }
    }
}
