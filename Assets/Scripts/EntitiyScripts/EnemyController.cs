using System.Collections.Generic;

using Scripts;

using UnityEngine;

public class EnemyController : EntityController {

    [Tooltip("Weighted loot table with drop chance. Takes priority over Item Drops when set.")]
    [SerializeField] private LootTable lootTable;

    [Tooltip("Legacy fallback: one random item from this list drops on death if no Loot Table is set.")]
    [SerializeField] private List<ItemObject> itemDrops = new List<ItemObject>();

    [SerializeField] private int xpReward = 5;

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
        if (health <= 0) {
            health = maxHealth;
        }
    }

    /// <summary>
    /// Runs once when health hits 0 (see EntityController.AdjustHealth).
    /// Drops loot on the tile the enemy died on, then disables the object.
    /// </summary>
    protected override void Die() {
        base.Die();
        DropLootOnTile(x, y);
        playerController.AddXp(xpReward);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Rolls the loot table (or the legacy item list) and puts the result on the tile.
    /// </summary>
    private void DropLootOnTile(int _x, int _y) {
        Tiles tile = gameController.GetTile(_x, _y);
        if (tile == null) {
            return;
        }

        foreach (ItemObject item in RollLoot()) {
            tile.addItemToItemList(item);
            infoTab.StandardLogMessage(entityName + " dropped " + item.name + ".");
        }
    }

    /// <summary>
    /// Returns the items this enemy drops right now.
    /// Uses the LootTable if assigned, otherwise one random item from itemDrops.
    /// </summary>
    public List<ItemObject> RollLoot() {
        if (lootTable != null) {
            return lootTable.RollDrops();
        }

        List<ItemObject> drops = new List<ItemObject>();
        ItemObject fallback = getRandomItemFromList();
        if (fallback != null) {
            drops.Add(fallback);
        }
        return drops;
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

    /// <summary>
    /// Selects one Random item from the legacy item list
    /// </summary>
    public ItemObject getRandomItemFromList() {
        if (itemDrops.Count != 0) {
            return itemDrops[Random.Range(0, itemDrops.Count)];
        }
        return null;
    }

    override public void Attack() {
        if (isAlive && getStatus() == EntityStatus.Hostile) {
            // Log first: this hit may kill the player, and the Game Over line has to come after it.
            infoTab.HitLogMessage(this, playerController);
            playerController.AdjustHealth(-GetAttackDamage());
        }
    }
}
