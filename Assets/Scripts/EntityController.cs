using System;
using Unity.Collections;
using UnityEngine;

public abstract class EntityController : MonoBehaviour {

    protected GameController gameController;
    protected int x, y;
    protected int type;
    protected bool hasMovedThisTurn;
    protected int health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected abstract void Movement();
    protected abstract void Attack();
    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        hasMovedThisTurn = false;
    }

    protected void UpdatePosition(int x, int y) {
        transform.position = gameController.TileToWorldCoordinate(x, y);
    }
    protected void Move(int dx, int dy, int type) {
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
            gameController.IncreaseTurn();
            //gameController.PrintTilesArray();
        }
    }

    public void SetHasMovedThisTurn(bool b) {
        hasMovedThisTurn = b;
    }

}
