using System;
using System.Collections.Generic;

using Scripts;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    //controller
    public PlayerController playerController;
    private EntityController selectedEntity;

    //map
    public Tiles[,] tilesArray;
    private int mapSizeX;
    private int mapSizeY;

    //turn order
    private TurnOwner currentTurnOwner;
    private int roundNumber;
    private bool isGameOver;
    private GameOverWindowScript gameOverWindow;
    private Tilemap wallTilemap;
    private BoundsInt bounds;
    private Camera mainCam;
    private Tilemap targetTilemap;
    [SerializeField] private GameObject[] enemyList;
    private GameObject tileHighlighter;

    //windows
    public InfoTabsScript infoTab;

    [SerializeField] private EntityInfoTabScript entityInfoTabScript;
    [SerializeField] private GameObject itemsOnTile;
    private List<ItemObject> currentItemList = new List<ItemObject>();
    [SerializeField] private GameObject itemListElement;
    private GameObject currentPlayerWeoponDisplay;

    private void Awake() {  // <=== Awake gets called before start()
        mainCam = Camera.main;
        wallTilemap = GameObject.FindGameObjectWithTag("Walls").GetComponent<Tilemap>();
        enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        targetTilemap = GameObject.FindGameObjectWithTag("FloorTilemap").GetComponent<Tilemap>();
        tileHighlighter = GameObject.FindGameObjectWithTag("TileHighlighter");
        entityInfoTabScript = GameObject.FindGameObjectWithTag("EnemyInfoTab").GetComponent<EntityInfoTabScript>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        itemsOnTile = GameObject.FindGameObjectWithTag("ItemsOnTile");
        currentPlayerWeoponDisplay = GameObject.FindGameObjectWithTag("CurrentPlayerWeoponDisplay");

        // Optional: a window tagged "GameOverWindow". Without it the game still ends, just without UI.
        GameObject gameOverObject = GameObject.FindGameObjectWithTag("GameOverWindow");
        if (gameOverObject != null) {
            gameOverWindow = gameOverObject.GetComponent<GameOverWindowScript>();
        }

        infoTab = GetComponent<InfoTabsScript>();

    }

    private void Start() {
        bounds = wallTilemap.cellBounds;
        mapSizeX = bounds.size.x;
        mapSizeY = bounds.size.y;
        currentTurnOwner = TurnOwner.Player;
        roundNumber = 1;
        isGameOver = false;
        GetObstaclesFromGrid();
        PrintTilesArray();

    }

    private void Update() {
        PlayerAttack();
        if (Mouse.current.rightButton.wasPressedThisFrame) {
            entityInfoTabScript.SetActive(true);
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
    public string PrintTilesArray() {
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
        return output;
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

    public Tiles GetTile(int x, int y) {
        if (!IsValidTile(x, y)) {
            return null;
        }
        return tilesArray[x, y];
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
    /// True while <paramref name="owner"/> is allowed to act. Always false after game over.
    /// </summary>
    public bool IsMyTurn(TurnOwner owner) {
        return !isGameOver && currentTurnOwner == owner;
    }

    public TurnOwner GetCurrentTurnOwner() {
        return currentTurnOwner;
    }

    /// <summary>
    /// Number of the current round. One round = one player action + every enemy's reaction.
    /// </summary>
    public int GetRoundNumber() {
        return roundNumber;
    }

    public bool IsGameOver() {
        return isGameOver;
    }

    /// <summary>
    /// Ends the player's action, lets every living enemy react, then starts the next round.
    /// Called by the player's Move/Attack. Enemies never call this.
    /// </summary>
    public void EndPlayerTurn() {
        if (isGameOver || currentTurnOwner != TurnOwner.Player) {
            return;
        }

        Vector2Int playerPos = LocatePlayer();
        ReloadItemOnTileList(playerPos.x, playerPos.y);

        currentTurnOwner = TurnOwner.Enemies;
        ResetHasMovedThisTurn();
        RunEnemyTurn(playerPos);
        infoTab.RefreshEnemyInfo(selectedEntity);

        if (isGameOver) {
            return;
        }
        roundNumber++;
        currentTurnOwner = TurnOwner.Player;
    }

    /// <summary>
    /// Every living enemy either attacks (adjacent) or moves towards the player (in view).
    /// Stops early once the player dies.
    /// </summary>
    private void RunEnemyTurn(Vector2Int playerPos) {
        foreach (GameObject enemy in enemyList) {
            if (isGameOver) {
                return;
            }
            EnemyController controller = enemy.GetComponent<EnemyController>();
            if (controller == null || controller.GetHasMovedThisTurn() || !controller.GetIsAlive()) {
                continue;
            }

            int distance = GetDistanceToTile(playerPos.x, playerPos.y, controller.GetXY().x, controller.GetXY().y);
            if (distance == 1) {
                controller.Attack();
            } else if (distance <= controller.GetViewingDistance()) {
                controller.Movement();
            }
            controller.SetHasMovedThisTurn(true);
        }
    }

    private void ResetHasMovedThisTurn() {
        foreach (GameObject entity in enemyList) {
            entity.GetComponent<EntityController>().SetHasMovedThisTurn(false);
        }
    }

    /// <summary>
    /// Called by the PlayerController when the player dies. Blocks all further input
    /// and shows the GameOverWindow if the scene has one.
    /// </summary>
    public void GameOver() {
        if (isGameOver) {
            return;
        }
        isGameOver = true;
        string message = playerController.GetEntityName() + " died in round " + roundNumber + ". Game Over.";
        infoTab.StandardLogMessage(message);
        if (gameOverWindow != null) {
            gameOverWindow.ShowGameOver(message);
        } else {
            Debug.Log(message + " (no object tagged GameOverWindow in scene)");
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
        int attackDistance = 1;
        if (!IsMyTurn(TurnOwner.Player) || !Mouse.current.leftButton.wasPressedThisFrame) {
            return;
        }
        Vector2Int hovered = GetTileHoveredByMouse();
        if (GetTileValue(hovered.x, hovered.y) != TileValue.Enemy) {
            return;
        }

        Vector2Int playerPos = playerController.GetXY();
        foreach (GameObject enemy in enemyList) {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController == null || !enemyController.GetIsAlive()) {
                continue;
            }
            Vector2Int enemyPos = enemyController.GetXY();
            if (enemyPos != hovered) {
                continue;
            }
            if (GetDistanceToTile(playerPos.x, playerPos.y, enemyPos.x, enemyPos.y) > attackDistance) {
                return; // out of reach: no action, no turn spent
            }

            // log first so the message shows the damage dealt, then apply it (may kill the enemy)
            infoTab.HitLogMessage(playerController, enemyController);
            enemyController.AdjustHealth(-playerController.GetAttackDamage());
            infoTab.RefreshEnemyInfo(selectedEntity);
            EndPlayerTurn();
            return;
        }
    }

    private void GetEntityInfo() {
        if (GetTileValue(GetTileHoveredByMouse().x, GetTileHoveredByMouse().y) == TileValue.Enemy) {
            //This is prob. very consuming but works for now. Should be reworked in the future 
            foreach (GameObject enemy in enemyList) {
                EnemyController controller = enemy.GetComponent<EnemyController>();
                // A dead enemy keeps its coordinates, so without this check it could hide a living one on the same tile.
                if (controller.GetIsAlive() && controller.GetXY().x == GetTileHoveredByMouse().x && controller.GetXY().y == GetTileHoveredByMouse().y) {

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
    /// Get the Items on a certain Tile
    /// </summary>
    /// <param name="xPos">xPos of the Tile to check</param>
    /// <param name="yPos">yPos of the Tile to check</param>
    /// <returns>List with Items on Tile if the isnt empty</returns>
    private List<ItemObject> ItemOnTileInLog(int xPos, int yPos) {

        if (currentItemList.Count == 0) {
            HideWeoponList();
            Debug.Log("No Items on Tile " + xPos + "<||>" + yPos);
            return null;
        }
        Debug.Log("Items on Tile " + xPos + "<||>" + yPos + ": " + ItemListToString(currentItemList));
        ShowWeoponList();
        return currentItemList;
    }

    /// <summary>
    /// Gets the current ItemList of a Tile
    /// </summary>
    /// <param name="xPos">xPos of the Tile</param>
    /// <param name="yPos">yPos of the Tile</param>
    private void ReloadItemOnTileList(int xPos, int yPos) {
        currentItemList = tilesArray[xPos, yPos].getItemList();
        ItemOnTileInLog(xPos, yPos);
    }

    //TODO!
    private void ShowWeoponList() {
        itemsOnTile.SetActive(true);
        ReloadItemListElements();
    }

    private void HideWeoponList() {
        itemsOnTile.SetActive(false);
        ReloadItemListElements();
    }

    /// <summary>
    /// Creates a new GameOject for every Item in the current ItemList
    /// </summary>
    private void ReloadItemListElements() {
        if (currentItemList == null) {
            return;
        }
        DeleteAllChildrenOfParent(itemsOnTile);
        float xOffSet = 0;
        float spacing = 50;
        foreach (ItemObject item in currentItemList) {
            GameObject newItemListElem = Instantiate(itemListElement,
                        new Vector3(itemsOnTile.transform.position.x + xOffSet, itemsOnTile.transform.position.y),
                        itemsOnTile.transform.rotation,
                        itemsOnTile.transform
                        );
            xOffSet += spacing;
            newItemListElem.GetComponent<Image>().sprite = item.ItemSprite;
            newItemListElem.GetComponent<ItemListElementScript>().SetItemObject(item);
        }
    }


    /// <summary>
    /// Destroys all Children of a parent
    /// </summary>
    /// <param name="parent">parent with children</param>
    private void DeleteAllChildrenOfParent(GameObject parent) {
        foreach (Transform child in parent.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Returns a String of an Itemlist
    /// Items are spaced by || 
    /// </summary>
    /// <param name="items"> an Itemlist</param>
    /// <returns>String containing names off all items</returns>
    private string ItemListToString(List<ItemObject> items) {
        string output = "";
        foreach (ItemObject item in items) {
            output += item.name + " || ";
        }
        return output;
    }

    /// <summary>
    /// Sets the player Weopon and load shows the current weopon sprite in the bottom left
    /// </summary>
    /// <param name="weopon">Weopon to set as the playerWeopon</param>
    public void setPlayerWeopon(WeoponObeject weopon) {
        playerController.setPlayerWeopon(weopon);
        currentPlayerWeoponDisplay.GetComponent<Image>().color = Color.white;
        currentPlayerWeoponDisplay.GetComponent<Image>().sprite = weopon.ItemSprite;
    }

    /// <summary>
    /// Removes a certain Item from a certain Tile
    /// </summary>
    /// <param name="xPos">xPos of the tile</param>
    /// <param name="yPos">yPos of the tile</param>
    /// <param name="item">item to remove</param>
    public void removeItemFromTile(int xPos, int yPos, ItemObject item) {
        GetTile(xPos, yPos).RemoveItemFromList(item);
        ReloadItemListElements();
    }

    /// <summary>
    /// Moves an item from the player's tile into the pack. A full pack leaves it on the tile.
    /// </summary>
    /// <param name="item">item lying on the player's tile</param>
    public void PickUpItem(ItemObject item) {
        if (!playerController.GetInventory().TryAdd(item)) {
            infoTab.StandardLogMessage("No room for " + item.name + " in your pack.");
            return;
        }
        Vector2Int playerPos = LocatePlayer();
        removeItemFromTile(playerPos.x, playerPos.y, item);
        infoTab.StandardLogMessage("You pick up " + item.name + ".");
    }

    /// <summary>
    /// Moves an item from the pack onto the player's tile, from where it can be picked up again.
    /// </summary>
    /// <param name="entry">entry of the player's pack</param>
    public void DropItem(InventoryEntry entry) {
        if (isGameOver || !playerController.GetInventory().Remove(entry)) {
            return;
        }
        Vector2Int playerPos = LocatePlayer();
        GetTile(playerPos.x, playerPos.y).addItemToItemList(entry.Item);
        ReloadItemOnTileList(playerPos.x, playerPos.y);
        infoTab.StandardLogMessage("You drop " + entry.Item.name + ".");
    }
}
