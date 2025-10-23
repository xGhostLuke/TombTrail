using System;
using System.Collections.Generic;

using Scripts;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour {

    //controller
    public PlayerController playerController;
    private EntityController selectedEntity;

    //map
    public Tiles[,] tilesArray;
    private int mapSizeX;
    private int mapSizeY;
    private int currentTurn;
    private Tilemap wallTilemap;
    private BoundsInt bounds;
    private Camera mainCam;
    private Tilemap targetTilemap;
    [SerializeField] private GameObject[] enemyList;
    private GameObject tileHighlighter;

    //windows
    public InfoTabsScript infoTab;

    [SerializeField] private EntityInfoTabScript entityInfoTabScript;

    private void Awake() {  // <=== Awake gets called before start()
        mainCam = Camera.main;
        wallTilemap = GameObject.FindGameObjectWithTag("Walls").GetComponent<Tilemap>();
        enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        targetTilemap = GameObject.FindGameObjectWithTag("FloorTilemap").GetComponent<Tilemap>();
        tileHighlighter = GameObject.FindGameObjectWithTag("TileHighlighter");
        //entityInfoTabScript = GameObject.FindGameObjectWithTag("EnemyInfoTab").GetComponent<EntityInfoTabScript>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();


        infoTab = GetComponent<InfoTabsScript>();

    }

    private void Start() {
        bounds = wallTilemap.cellBounds;
        mapSizeX = bounds.size.x;
        mapSizeY = bounds.size.y;
        currentTurn = 1;
        GetObstaclesFromGrid();
        PrintTilesArray();

    }

    private void Update() {
        PlayerAttack();
        if (Mouse.current.rightButton.wasPressedThisFrame) {
            GetEntityInfo();
        }
        if (Keyboard.current.lKey.wasPressedThisFrame) {
            infoTab.ShowInteractionlog();
        }
    }

    /// <summary>
    /// Creates an empty Tile Array
    /// </summary>
    private void InitializeTilesArray() {
        tilesArray = new Tiles[mapSizeX, mapSizeY];
        for (int i = 0; i < mapSizeX; i++) {
            for (int j = 0; j < mapSizeY; j++) {
                tilesArray[i, j] = new Tiles(TileValue.Empty);
            }
        }
    }

    /// <summary>
    /// Prints the Current Tile Array to the Unity Log
    /// </summary>
    public void PrintTilesArray() {
        string output = "";
        for (int i = 0; i < mapSizeY; i++) {
            for (int j = 0; j < mapSizeX; j++) {
                TileValue value = tilesArray[j, i].GetTileValue();
                switch (value) {
                    case TileValue.Empty:
                        output += " 0";
                        break;
                    case TileValue.Player:
                        output += " P";
                        break;
                    case TileValue.Enemy:
                        output += " E";
                        break;
                    case TileValue.Item:
                        output += " I";
                        break;
                    case TileValue.Obstacle:
                        output += " X";
                        break;
                }
            }
            output += "\n";
        }
        Debug.Log(output);
    }

    /// <summary>
    /// Gives a Tile at the given Position the given value
    /// </summary>
    /// <param name="x">x Cord of Tile</param>
    /// <param name="y">y Cord of Tile</param>
    /// <param name="value">Value the Tile should take</param>
    public void SetTile(int x, int y, TileValue value) {
        if (IsValidTile(x, y)) {
            tilesArray[x, y].SetTileValue(value);
        }
    }

    /// <summary>
    /// Checks if a Tile exists on the given Cords
    /// </summary>
    /// <param name="x">x Cord</param>
    /// <param name="y">y Cord</param>
    /// <returns>True if Tile exists, else False</returns>
    public bool IsValidTile(int x, int y) {
        return x >= 0 && x < mapSizeX && y >= 0 && y < mapSizeY;
    }

    /// <summary>
    /// Checks if Tile at current Position is Empty
    /// </summary>
    /// <param name="x">x Cord</param>
    /// <param name="y">y Cord</param>
    /// <returns>True if Tile is empty, else False</returns>
    /// <exception cref="IndexOutofBounce">
    /// if Tile Cords are not in Range of Tile Array
    /// </exception>
    public bool IsTileEmpty(int x, int y) {
        return tilesArray[x, y].GetTileValue() == TileValue.Empty;
    }

    /// <summary>
    /// Return Tile Value at given Cords
    /// </summary>
    /// <param name="x">x Cord</param>
    /// <param name="y">y Cord</param>
    /// <returns>TileValue at Cords</returns>
    /// /// <exception cref="IndexOutofBounce">
    /// if Tile Cords are not in Range of Tile Array
    /// </exception>
    public TileValue GetTileValue(int x, int y) {
        return tilesArray[x, y].GetTileValue();
    }

    public Vector3Int TileToWorldCoordinate(int x, int y) {
        return new Vector3Int(x + bounds.xMin, mapSizeY - 1 - y + bounds.yMin, 0);
    }

    public Vector2Int WorldToTileCoordinate(Vector3 worldPos) {
        int x = Mathf.RoundToInt(worldPos.x - bounds.xMin);
        int y = Mathf.RoundToInt(mapSizeY - 1 - ( worldPos.y - bounds.yMin ));
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// Gets the current position of the PlayerObject.
    /// </summary>
    /// <returns>Vector2Int PlayerPos </returns>
    public Vector2Int LocatePlayer() {
        return playerController.GetXY();
    }

    /// <summary>
    /// Returns the boolean if current turn is Types Turn
    /// </summary>
    /// <param name="type">MAGIC NUMBERS Type to Check </param>
    /// <returns>Boolean</returns>
    public bool IsMyTurn(int type) {
        return type == 1 && currentTurn % 2 == 1 || type == 2 && currentTurn % 2 == 0;
    }


    //This handles turn management and basic enemy logic for now
    //Enemy logic should be moved to EnemyController in the future
    //or maybe rename method to TurnLogic?
    public void NextTurn() {
        infoTab.RefreshEnemyInfo(selectedEntity);
        currentTurn++;
        //Debug.Log(currentTurn);
        ResetHasMovedThisTurn();

        Vector2Int playerPos = LocatePlayer();
        List<ItemObject> itemList = ItemOnCurrentPlayerTile(playerPos.x, playerPos.y);
        if (itemList != null) {

        }

        if (IsMyTurn(2)) {
            foreach (GameObject enemy in enemyList) {
                EnemyController controller = enemy.GetComponent<EnemyController>();
                if (!controller.GetHasMovedThisTurn() && controller.GetIsAlive()) {

                    int distance = GetDistanceToTile(playerPos.x, playerPos.y, controller.GetXY().x, controller.GetXY().y);

                    if (distance == 1) {
                        controller.Attack();
                    } else {
                        if (distance <= controller.GetViewingDistance()) {
                            controller.Movement();
                        }
                    }
                }
            }
            NextTurn();
        }
    }

    private void ResetHasMovedThisTurn() {
        foreach (GameObject entity in enemyList) {
            entity.GetComponent<EntityController>().SetHasMovedThisTurn(false);
        }
    }

    private void GetObstaclesFromGrid() {
        mapSizeX = bounds.size.x;
        mapSizeY = bounds.size.y;
        tilesArray = new Tiles[mapSizeX + 1, mapSizeY + 1];
        InitializeTilesArray();
        foreach (var pos in bounds.allPositionsWithin) {
            if (wallTilemap.HasTile(pos)) {
                int arrayX = pos.x - bounds.xMin;
                int arrayY = pos.y - bounds.yMin;
                arrayY = mapSizeY - 1 - arrayY;
                SetTile(arrayX + 1, arrayY, TileValue.Obstacle);
            }
        }
    }

    public int GetDistanceToTile(int startX, int startY, int endX, int endY) {
        return Math.Abs(startX - endX) + Math.Abs(startY - endY);
    }

    private void PlayerAttack() { //This is here for now, should be in the PlayerController tho. Update idk maybe this should stay here
        infoTab.RefreshEnemyInfo(selectedEntity);
        int attackDistance = 1;
        if (GetTileValue(GetTileHoveredByMouse().x, GetTileHoveredByMouse().y) == TileValue.Enemy && IsMyTurn(1) && Mouse.current.leftButton.wasPressedThisFrame) {
            foreach (GameObject enemy in enemyList) {
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                Vector2Int playerPos = playerController.GetXY();
                Vector2Int enemyPos = enemyController.GetXY();
                if (!( GetDistanceToTile(playerPos.x, playerPos.y, enemyPos.x, enemyPos.y) <= attackDistance )) {
                    break;
                }
                if (enemyController.GetXY().x == GetTileHoveredByMouse().x && enemyController.GetXY().y == GetTileHoveredByMouse().y) {
                    enemyController.AdjustHealth(-playerController.GetAttackDamage());
                    infoTab.HitLogMessage(playerController, enemyController);
                    infoTab.RefreshEnemyInfo(selectedEntity);
                }
            }
            NextTurn();
        }
    }

    private void GetEntityInfo() {
        if (GetTileValue(GetTileHoveredByMouse().x, GetTileHoveredByMouse().y) == TileValue.Enemy) {
            //This is prob. very consuming but works for now. Should be reworked in the future 
            foreach (GameObject enemy in enemyList) {
                EnemyController controller = enemy.GetComponent<EnemyController>();
                if (controller.GetXY().x == GetTileHoveredByMouse().x && controller.GetXY().y == GetTileHoveredByMouse().y) {
                    entityInfoTabScript.ShowWindow(
                        controller.GetEntityName(),
                        controller.GetHealth().ToString(),
                         controller.GetAttackDamage().ToString(),
                         controller.GetMaxHealth().ToString(),
                         controller.getStatus().ToString()
                    );
                    selectedEntity = controller;
                }
            }
        } else if (GetTileValue(GetTileHoveredByMouse().x, GetTileHoveredByMouse().y) == TileValue.Player) {
            entityInfoTabScript.ShowWindow(
                        playerController.GetEntityName(),
                        playerController.GetHealth().ToString(),
                         playerController.GetAttackDamage().ToString(),
                         playerController.GetMaxHealth().ToString(),
                         playerController.getStatus().ToString()
            );
            selectedEntity = playerController;
        }
    }



    public Vector2Int GetTileHoveredByMouse() {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, mainCam.nearClipPlane));
        Vector3Int cellPos = targetTilemap.WorldToCell(mouseWorldPos);

        Vector3 cellCenterPos = targetTilemap.GetCellCenterWorld(cellPos);
        tileHighlighter.transform.position = cellCenterPos;

        Vector2Int arrayPos = WorldToTileCoordinate(tileHighlighter.transform.position);

        arrayPos.x = Mathf.Clamp(arrayPos.x, 0, mapSizeX - 1);
        arrayPos.y = Mathf.Clamp(arrayPos.y, 0, mapSizeY - 1);
        return new Vector2Int(arrayPos.x, arrayPos.y);
    }


    public Vector2Int getMapSize() {
        return new Vector2Int(mapSizeX, mapSizeY);
    }

    /// <summary>
    /// Checks if the Player is standing on an Tile with Items 
    /// </summary>
    /// <param name="xPos"></param>
    /// <param name="yPos"></param>
    /// <returns>Boolean if yes or no</returns>
    private List<ItemObject> ItemOnCurrentPlayerTile(int xPos, int yPos) {
        List<ItemObject> items = tilesArray[xPos, yPos].getItemList();

        if (items.Count == 0) {
            HideWeoponList();
            return null;
        }
        Debug.Log(items.ToString());
        ShowWeoponList();
        return items;
    }

    //TODO!

    private void ShowWeoponList() {

    }

    private void HideWeoponList() {

    }
}
