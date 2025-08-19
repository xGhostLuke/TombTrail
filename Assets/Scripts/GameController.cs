using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    public TileType[,] tilesArray;
    private int mapSizeX;
    private int mapSizeY;
    private int currentTurn;
    [SerializeField] private GameObject[] enemyList;
    private Tilemap wallTilemap;
    private BoundsInt bounds;
    private Camera mainCam;
    private Tilemap targetTilemap;
    [SerializeField] private int playerDamage;
    private GameObject tileHighlighter;
    private InteractionLogTextScript interactionLogText;
    //private PlayerController playerController;

    private void Awake()
    {
        mainCam = Camera.main;
        wallTilemap = GameObject.FindGameObjectWithTag("Walls").GetComponent<Tilemap>();
        enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        targetTilemap = GameObject.FindGameObjectWithTag("FloorTilemap").GetComponent<Tilemap>();
        tileHighlighter = GameObject.FindGameObjectWithTag("TileHighlighter");
        interactionLogText = GameObject.FindGameObjectWithTag("InteractionLogText").GetComponent<InteractionLogTextScript>();
        //playerController = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();

        bounds = wallTilemap.cellBounds;
        mapSizeX = bounds.size.x;
        mapSizeY = bounds.size.y;
        currentTurn = 1;
        tilesArray = new int[mapSizeX + 1, mapSizeY];
        ClearTilesArray();
        Debug.Log(mapSizeX + " " + mapSizeY);
        GetObstaclesFromGrid();
        PrintTilesArray();

    }

    private void Start() { }

    private void Update()
    {
        PlayerAttack();
    }

    private void ClearTilesArray()
    {
        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                tilesArray[i, j] = TileType.Empty;
            }
        }
    }

    public void PrintTilesArray()
    {
        string output = "";
        for (int i = 0; i < mapSizeY; i++)
        {
            for (int j = 0; j < mapSizeX; j++)
            {
                output += tilesArray[j, i] + " ";
            }
            output += "\n";
        }
        Debug.Log("1 = Player, 2 = Enemy, 3 = Item, 4 = Obstacle\nCurrent Tiles Array:\n" + output);
    }

    public void SetTile(int x, int y, TileType tileType)
    {
        if (!IsValidTile(x, y)) return;

        tilesArray[x, y] = tileType;
    }

    public bool IsValidTile(int x, int y)
    {
        return x >= 0 && x < mapSizeX && y >= 0 && y < mapSizeY;
    }

    public bool IsTileEmpty(int x, int y)
    {
        return tilesArray[x, y] == TileType.Empty;
    }

    public int GetTileValue(int x, int y)
    {
        return tilesArray[x, y];
    }

    public Vector3 TileToWorldCoordinate(int x, int y)
    {
        return new Vector3(x + bounds.xMin, mapSizeY - 1 - y + bounds.yMin, 0);
    }

    public Vector2Int WorldToTileCoordinate(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x - bounds.xMin);
        int y = Mathf.RoundToInt(mapSizeY - 1 - (worldPos.y - bounds.yMin));
        return new Vector2Int(x, y);
    }

    public Vector2Int LocatePlayer()
    {
        // TODO: please don't do it like this.
        // Instead just creae a player object that knows its position
        // This is just a unnecessary computational task
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                if (tilesArray[x, y] != TileType.Player) continue;

                return new Vector2Int(x, y);
            }
        }

        return new Vector2Int(-1, -1);
    }

    public bool IsMyTurn(TileType tileType)
    {
        // TODO: don't use int type ... = magic numbers
        // I don't know even know what that's supposed to mean here
        // Use another enum. I used TileType here just as a reminder
        return (
            tileType == TileType.Player && currentTurn % 2 == 1
            || tileType == TileType.Enemy && currentTurn % 2 == 0
        );
    }

    private void EnemyActionCheck(GameObject enemy)
    {
        EnemyController controller = enemy.GetComponent<EnemyController>();
        bool entityMayMove = !controller.GetHasMovedThisTurn() && controller.GetIsAlive();

        if (!entityMayMove) continue;

        bool entityIsInAttackRange = GetDistanceToTile(playerPos.x, playerPos.y, controller.GetXY()) == 1;

        if (entityIsInAttackRange)
        {
            controller.Attack();
            interactionLogText.AddTextToLog("Enemy attacked Player for: " + controller.getAttackDamage());
        }
        else
        {
            controller.Movement();
        }
    }

    public void NextTurn()
    {
        currentTurn++;
        Debug.Log(currentTurn);
        ResetHasMovedThisTurn();

        // TODO: please don't use magic numbers
        if (!IsMyTurn(2)) return;

        Vector2Int playerPos = LocatePlayer();

        foreach (GameObject enemy in enemyList)
        {
            EnemyActionCheck(enemy);
        }
        NextTurn();
    }

    private void ResetHasMovedThisTurn()
    {
        foreach (GameObject entity in enemyList)
        {
            entity.GetComponent<EntityController>().SetHasMovedThisTurn(false);
        }
    }

    private void GetObstaclesFromGrid()
    {
        mapSizeX = bounds.size.x;
        mapSizeY = bounds.size.y;
        tilesArray = new int[mapSizeX, mapSizeY];

        foreach (var pos in bounds.allPositionsWithin)
        {
            if (!wallTilemap.HasTile(pos)) continue;

            int arrayX = pos.x - bounds.xMin;
            int arrayY = pos.y - bounds.yMin;
            arrayY = mapSizeY - 1 - arrayY;
            SetTile(arrayX + 1, arrayY, TileType.Obstacle);
        }
    }

    private int GetDistanceToTile(Vector2Int startPosition, Vector2Int endPosition)
    {
        return Math.Abs(startPosition.x - endPosition.x) + Math.Abs(startPosition.y - endPosition.y);
    }

    private void PlayerAttack()
    {
        int attackDistance = 1;
        Vector2Int tileHoveredByMouse = GetTileHoveredByMouse();

        TileType tileType = GetTileValue(tileHoveredByMouse.x, tileHoveredByMouse.y);
        bool mousePressed = Mouse.current.leftButton.wasPressedThisFrame;

        // TODO: please fix the magic number here. I don't know what it means
        bool isValidAttackAction = (
            tileType == TileType.Enemy
            && IsMyTurn(1)
            && mousePressed
            );

        if (!isValidAttackAction) return;

        foreach (GameObject enemy in enemyList)
        {
            EnemyController controller = enemy.GetComponent<EnemyController>();
            Vector2Int enemyPosition = controller.GetXY();
            Vector2Int playerPosition = LocatePlayer();
            Vector2Int tileHoveredByMousePosition = GetTileHoveredByMouse();

            int distanceToTile = GetDistanceToTile(playerPosition, enemyPosition);

            bool isValidAttack = (
                distanceToTile == attackDistance
                && enemyPosition == tileHoveredByMousePosition
                );

            if (!isValidAttack) continue;

            controller.AdjustHealth(-playerDamage);
            interactionLogText.AddTextToLog("Player attacked Enemy for: " + playerDamage);
        }
    }

    public Vector2Int GetTileHoveredByMouse()
    {
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
}
