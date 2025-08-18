using System;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EntityController : MonoBehaviour {

    protected GameController gameController;
    [SerializeField] protected PlayerController playerController;
    [SerializeField] protected int x, y;
    protected int type;
    [SerializeField] protected bool hasMovedThisTurn;
    [SerializeField] protected int health;
    protected bool isAlive;
    [SerializeField] protected int attackDamage;
    public abstract void Movement();
    public abstract void Attack();
    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        hasMovedThisTurn = false;
        isAlive = true;
    }

    protected void UpdatePosition(int x, int y) {
        transform.position = gameController.TileToWorldCoordinate(x, y);
        Debug.Log("Set Enemy to: " + x + y);
    }
    protected void Move(int dx, int dy, int type) {
        if (dx == 0 && dy == 0) {
            gameController.NextTurn();
        }

        int newX = x + dx;
        int newY = y + dy;
        //Debug.Log("Pos of Type: " + type + " is \n " + x + " " + y);
        if (gameController.IsValidTile(newX, newY) && gameController.IsTileEmpty(newX, newY)) {
            gameController.SetTile(x, y, 0);
            //Debug.Log("Set Tile " + x + y + "to 0");
            x = newX;
            y = newY;
            gameController.SetTile(x, y, type);
            UpdatePosition(x, y);
            hasMovedThisTurn = true;
            //gameController.PrintTilesArray();

            if (type == 1) {
                gameController.NextTurn();
            }
        }
    }

    public void SetHasMovedThisTurn(bool b) {
        hasMovedThisTurn = b;
    }

    public bool GetHasMovedThisTurn() {
        return hasMovedThisTurn;
    }

    public Vector2Int getXY() {
        return new Vector2Int(x, y);
    }

    public void AdjustHealth(int adjust) {
        health += adjust;
    }

    public int getHealth() {
        return health;
    }

    public bool GetIsAlive() {
        return isAlive;
    }

    public int getAttackDamage() {
        return attackDamage;
    }

}
