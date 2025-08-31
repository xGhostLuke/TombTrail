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

    //abstracts
    public abstract void Movement();
    public abstract void Attack();

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        pathfindingScript = GameObject.FindGameObjectWithTag("Pathfinding").GetComponent<PathfindingScript>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Start() {
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
            gameController.NextTurn();
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
                gameController.NextTurn();
            }
        }
    }


    //WIP
    protected List<Vector2Int> ViewableTiles() {

        for (int i = 0; i < viewingDistance; i++) {

        }

        return null;
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

    public void AdjustHealth(int adjust) {
        health += adjust;
    }

    public int GetHealth() {
        return health;
    }

    public bool GetIsAlive() {
        return isAlive;
    }

    public int GetAttackDamage() {
        return attackDamage;
    }

    public int GetMaxHealth() {
        return maxHealth;
    }

    public string GetEntityName() {
        return entityName;
    }

    public EntityStatus getStatus() {
        return status;
    }

    public int GetViewingDistance() {
        return viewingDistance;
    }

}
