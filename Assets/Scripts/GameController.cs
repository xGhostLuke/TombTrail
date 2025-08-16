using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;

public class GameController : MonoBehaviour {
    public int[,] tilesArray;
    private int mapSize;
    private int currentTurn;

    [SerializeField]
    private List<GameObject> EntityList;

    private void Awake() {  // <=== Awake gets called before start()
        mapSize = 8;
        currentTurn = 1;
        tilesArray = new int[mapSize, mapSize];
        ClearTilesArray();
    }

    private void ClearTilesArray() {
        for (int i = 0; i < mapSize; i++) {
            for (int j = 0; j < mapSize; j++) {
                tilesArray[i, j] = 0;
            }
        }
    }

    public void PrintTilesArray() {
        string output = "";
        for (int i = 0; i < mapSize; i++) {
            for (int j = 0; j < mapSize; j++) {
                output += tilesArray[j, i] + " ";
            }
            output += "\n";
        }
        Debug.Log("1 = Player, 2 = Enemy, 3 = Item, 4 = Obstacle\nCurrent Tiles Array:\n" + output);
    }

    public void SetTile(int x, int y, int value) {
        // 1 = Player 
        // 2 = Enemy 
        // 3 = Item
        // 4 = Obstacle
        if (IsValidTile(x, y)) {
            tilesArray[x, y] = value;
        }
    }

    public bool IsValidTile(int x, int y) {
        return x >= 0 && x < mapSize && y >= 0 && y < mapSize;
    }

    public bool IsTileEmpty(int x, int y) {
        return tilesArray[x, y] == 0;
    }

    public Vector3 TileToWorldCoordinate(int x, int y) {
        return new Vector3(x, mapSize - 1 - y, 0);
    }

    public Vector2 LocatePlayer() {
        for (int x = 0; x < mapSize; x++) {
            for (int y = 0; y < mapSize; y++) {
                if (tilesArray[x, y] == 1) {
                    return new Vector2(x, y);
                }
            }
        }
        return new Vector2(-1, -1); //Incase Player is not on current map? 
    }

    public bool IsMyTurn(int type) {
        return type == 1 && currentTurn % 2 == 1 || type == 2 && currentTurn % 2 == 0;
    }

    public void IncreaseTurn() {
        currentTurn++;
        Debug.Log(currentTurn);
        ResetHasMovedThisTurn();
    }

    private void ResetHasMovedThisTurn() {
        foreach (GameObject entity in EntityList) {
            entity.GetComponent<EntityController>().SetHasMovedThisTurn(false);
        }
    }

}
