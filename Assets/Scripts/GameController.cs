using System;
using System.Collections.Generic;

using Scripts;

using Unity.VisualScripting;

using UnityEditor.PackageManager;
using UnityEditor.ProjectWindowCallback;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour {

    //controller
    public PlayerController playerController;
    private EntityController selectedEntity;

    //map
    public TileValue[,] tilesArray;
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
    private InteractionLogTextScript interactionlog;
    private EntityInfoTabScript entityInfoTabScript;

    private void Awake() {  // <=== Awake gets called before start()
        mainCam = Camera.main;
        wallTilemap = GameObject.FindGameObjectWithTag("Walls").GetComponent<Tilemap>();
        enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        targetTilemap = GameObject.FindGameObjectWithTag("FloorTilemap").GetComponent<Tilemap>();
        tileHighlighter = GameObject.FindGameObjectWithTag("TileHighlighter");
        interactionlog = GameObject.FindGameObjectWithTag("Interactionlog").GetComponent<InteractionLogTextScript>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        entityInfoTabScript = GameObject.FindGameObjectWithTag("EnemyInfoTab").GetComponent<EntityInfoTabScript>();

    }

    private void Start() {
        bounds = wallTilemap.cellBounds;
        mapSizeX = bounds.size.x;
        mapSizeY = bounds.size.y;
        currentTurn = 1;
        tilesArray = new TileValue[mapSizeX + 1, mapSizeY];
        ClearTilesArray();
        //Debug.Log(mapSizeX + " " + mapSizeY);
        GetObstaclesFromGrid();
        PrintTilesArray();

    }

    private void Update() {
        PlayerAttack();
        if (Mouse.current.rightButton.wasPressedThisFrame) {
            GetEntityInfo();
        }
        if (Keyboard.current.lKey.wasPressedThisFrame) {
            ShowInteractionlog();
        }
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
                TileValue value = tilesArray[j, i];
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
        //Debug.Log(output);
    }

    public void SetTile(int x, int y, TileValue value) {
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

    public TileValue GetTileValue(int x, int y) {
        return tilesArray[x, y];
    }

    public Vector3 TileToWorldCoordinate(int x, int y) {
        return new Vector3(x + bounds.xMin, mapSizeY - 1 - y + bounds.yMin, 0);
    }

    public Vector2Int WorldToTileCoordinate(Vector3 worldPos) {
        int x = Mathf.RoundToInt(worldPos.x - bounds.xMin);
        int y = Mathf.RoundToInt(mapSizeY - 1 - ( worldPos.y - bounds.yMin ));
        return new Vector2Int(x, y);
    }

    public Vector2Int LocatePlayer() {
        return playerController.GetXY();
    }

    public bool IsMyTurn(int type) {
        return type == 1 && currentTurn % 2 == 1 || type == 2 && currentTurn % 2 == 0;
    }


    //This handles turn management and basic enemy logic for now
    //Enemy logic should be moved to EnemyController in the future
    //or maybe rename method to TurnLogic?
    public void NextTurn() {
        RefreshEnemyInfo();
        currentTurn++;
        //Debug.Log(currentTurn);
        ResetHasMovedThisTurn();
        if (IsMyTurn(2)) {
            Vector2Int playerPos = playerController.GetXY();
            foreach (GameObject enemy in enemyList) {
                EnemyController controller = enemy.GetComponent<EnemyController>();
                if (!controller.GetHasMovedThisTurn() && controller.GetIsAlive()) {
                    int distance = GetDistanceToTile(playerPos.x, playerPos.y, controller.GetXY().x, controller.GetXY().y);
                    if (distance == 1) {
                        controller.Attack();
                        interactionlog.AddTextToLog("Enemy attacked Player for: " + controller.GetAttackDamage());
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
        tilesArray = new TileValue[mapSizeX, mapSizeY];

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

    private void PlayerAttack() { //This is here for now, should be in the PlayerController tho
        int attackDistance = 1;
        if (GetTileValue(GetTileHoveredByMouse().x, GetTileHoveredByMouse().y) == TileValue.Enemy && IsMyTurn(1) && Mouse.current.leftButton.wasPressedThisFrame) {
            foreach (GameObject enemy in enemyList) {
                EnemyController controller = enemy.GetComponent<EnemyController>();
                if (GetDistanceToTile(LocatePlayer().x, LocatePlayer().y, controller.GetXY().x, controller.GetXY().y) == attackDistance
                    && controller.GetXY().x == GetTileHoveredByMouse().x && controller.GetXY().y == GetTileHoveredByMouse().y) {
                    controller.AdjustHealth(-playerController.GetAttackDamage());
                    interactionlog.AddTextToLog("Player attacked Enemy for: " + playerController.GetAttackDamage());
                }
            }
            NextTurn();
        }
    }

    private void GetEntityInfo() {
        if (GetTileValue(GetTileHoveredByMouse().x, GetTileHoveredByMouse().y) == TileValue.Enemy) {
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

    private void RefreshEnemyInfo() {
        if (selectedEntity != null) {
            entityInfoTabScript.LoadStats(
                selectedEntity.GetEntityName(),
                selectedEntity.GetHealth().ToString(),
                selectedEntity.GetAttackDamage().ToString(),
                selectedEntity.GetMaxHealth().ToString(),
                selectedEntity.getStatus().ToString()
            );
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

    private void ShowInteractionlog() {
        interactionlog.ShowWindow();
    }

    public Vector2Int getMapSize() {
        return new Vector2Int(mapSizeX, mapSizeY);
    }

    public void SendMessageToLog(string msg) {
        interactionlog.AddTextToLog(msg);
    }
}
