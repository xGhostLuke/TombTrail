using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour {
    public int[,] tilesArray;
    private int mapSizeX;
    private int mapSizeY;
    private int currentTurn;

    [SerializeField]
    private List<GameObject> EntityList;
    private Tilemap wallTilemap;
    private BoundsInt bounds;

    private void Awake() {  // <=== Awake gets called before start()
        wallTilemap = GameObject.FindGameObjectWithTag("Walls").GetComponent<Tilemap>();

        bounds = wallTilemap.cellBounds;
        mapSizeX = bounds.size.x;
        mapSizeY = bounds.size.y;
        currentTurn = 1;
        tilesArray = new int[mapSizeX, mapSizeY];
        ClearTilesArray();
        Debug.Log(mapSizeX + " " + mapSizeY);
        GetObstaclesFromGrid();
        PrintTilesArray();
    }

    private void ClearTilesArray() {
        for (int i = 0; i < mapSizeX; i++) {
            for (int j = 0; j < mapSizeY; j++) {
                tilesArray[i, j] = 0;
            }
        }
    }

    public void PrintTilesArray() {
        string output = "";
        for (int i = 0; i < mapSizeY; i++) {
            for (int j = 0; j < mapSizeX; j++) {
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
        return x >= 0 && x < mapSizeX && y >= 0 && y < mapSizeY;
    }

    public bool IsTileEmpty(int x, int y) {
        return tilesArray[x, y] == 0;
    }

    public Vector3 TileToWorldCoordinate(int x, int y) {
        return new Vector3(x + bounds.xMin, mapSizeY - 1 - y + bounds.yMin, 0);
    }

    public Vector2 LocatePlayer() {
        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) {
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
        //Debug.Log(currentTurn);
        ResetHasMovedThisTurn();
    }

    private void ResetHasMovedThisTurn() {
        foreach (GameObject entity in EntityList) {
            entity.GetComponent<EntityController>().SetHasMovedThisTurn(false);
        }
    }

    private void GetObstaclesFromGrid() {
        mapSizeX = bounds.size.x;
        mapSizeY = bounds.size.y;
        tilesArray = new int[mapSizeX, mapSizeY];

        foreach (var pos in bounds.allPositionsWithin) {
            if (wallTilemap.HasTile(pos)) {
                int arrayX = pos.x - bounds.xMin;
                int arrayY = pos.y - bounds.yMin;
                arrayY = mapSizeY - 1 - arrayY;
                SetTile(arrayX, arrayY, 4);
            }
        }
    }

}
