using System;
using System.Collections.Generic;

using Scripts;

using Unity.Collections;
using Unity.VisualScripting;

using UnityEngine;

public abstract class EntityController : MonoBehaviour {

    //Controller
    protected GameController gameController;
    protected PlayerController playerController;
    [SerializeField] protected InfoTabsScript infoTab;
    [SerializeField] protected PathfindingScript pathfindingScript;
    //Movement
    [SerializeField] protected int x, y;
    [SerializeField] protected bool hasMovedThisTurn;

    //stats
    protected TileValue type;
    [SerializeField] protected int health;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int attackDamage;
    [SerializeField] protected string entityName;
    [SerializeField] protected EntityStatus status;
    protected bool isAlive;
    [SerializeField] protected int viewingDistance;

    [SerializeField] private WeoponObeject weopon;

    //abstracts
    public abstract void Movement();
    public abstract void Attack();

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        pathfindingScript = GameObject.FindGameObjectWithTag("Pathfinding").GetComponent<PathfindingScript>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();


    }

    private void Start() {
        infoTab = gameController.infoTab;
        isAlive = true;
        hasMovedThisTurn = false;
        health = maxHealth;
        UpdatePosition(x, y);
    }

    protected void UpdatePosition(int x, int y) {
        transform.position = gameController.TileToWorldCoordinate(x, y);
        //Debug.Log("Set Enemy to: " + x + y);
    }

    protected void Move(int dx, int dy, TileValue type) {
        if (dx == 0 && dy == 0) {
            // Waiting counts as an action: the player skips, enemies just stay.
            hasMovedThisTurn = true;
            if (type == TileValue.Player) {
                gameController.EndPlayerTurn();
            }
            return;
        }

        int newX = x + dx;
        int newY = y + dy;
        //Debug.Log("Pos of Type: " + type + " is \n " + x + " " + y);
        if (gameController.IsValidTile(newX, newY) && gameController.IsTileEmpty(newX, newY)) {
            gameController.SetTile(x, y, TileValue.Empty);
            //Debug.Log("Set Tile " + x + y + "to 0");
            x = newX;
            y = newY;
            gameController.SetTile(x, y, type);
            UpdatePosition(x, y);
            hasMovedThisTurn = true;
            //gameController.PrintTilesArray();

            if (type == TileValue.Player) {
                gameController.EndPlayerTurn();
            }
        }
    }

    public void SetHasMovedThisTurn(bool b) {
        hasMovedThisTurn = b;
    }

    public bool GetHasMovedThisTurn() {
        return hasMovedThisTurn;
    }

    public Vector2Int GetXY() {
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// Changes health by <paramref name="adjust"/> (negative = damage, positive = heal).
    /// Health is clamped to [0, maxHealth]. Reaching 0 kills the entity exactly once.
    /// </summary>
    public void AdjustHealth(int adjust) {
        if (!isAlive) {
            return;
        }
        health = Mathf.Clamp(health + adjust, 0, maxHealth);
        if (health <= 0) {
            Die();
        }
    }

    /// <summary>
    /// Called once when health reaches 0. Frees the tile and marks the entity dead.
    /// Subclasses extend this (drop loot, game over, ...).
    /// </summary>
    protected virtual void Die() {
        isAlive = false;
        status = EntityStatus.Dead;
        gameController.SetTile(x, y, TileValue.Empty);
        if (infoTab != null) {
            infoTab.StandardLogMessage(entityName + " died!");
        }
    }

    public int GetHealth() {
        //Debug.Log("Leben abgerufen");
        return health;
    }

    public bool GetIsAlive() {
        return isAlive;
    }

    public int GetAttackDamage() {
        if (weopon != null) {
            return attackDamage + getEntityWeopon().baseDamage;
        }
        return attackDamage;
    }

    public int GetMaxHealth() {
        // Debug.Log("Max Health abgerufen");
        return maxHealth;
    }

    public string GetEntityName() {
        //Debug.Log("Name abgerufen");
        return entityName;
    }

    public EntityStatus getStatus() {
        //Debug.Log("Status abgerufen");
        return status;
    }

    public int GetViewingDistance() {
        return viewingDistance;
    }

    public void setPlayerWeopon(WeoponObeject weopon) {
        this.weopon = weopon;
    }

    public WeoponObeject getEntityWeopon() {
        return weopon;
    }

}
