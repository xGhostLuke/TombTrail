using System.Collections.Generic;
using System.Drawing;

using Scripts;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.Tilemaps;

public class ShadowMapController : MonoBehaviour {
    private GameController gamecontroller;
    [SerializeField] private Tilemap shadowMap;
    [SerializeField] private Tile[] shadowMapTiles;

    private Vector2Int playerPos;

    [SerializeField] private int playerViewingDistance;
    Vector2 size;

    void Awake() {
        gamecontroller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

    }

    private void Start() {
        playerPos = gamecontroller.LocatePlayer();
        size = gamecontroller.getMapSize();
        GenerateShadowMap();
        // Has to stay behind GenerateShadowMap, which paints everything black. Update only reacts to position changes.
        UpdateShadowMap();
    }

    void Update() {
        if (playerPos != gamecontroller.LocatePlayer()) {
            playerPos = gamecontroller.LocatePlayer();
            UpdateShadowMap();
        }
    }

    private void GenerateShadowMap() {
        for (int i = 0; i < size.x; i++) {
            for (int j = 0; j < size.y; j++) {
                Vector2Int pos = gamecontroller.WorldToTileCoordinate(new Vector3Int(i, j));
                shadowMap.SetTile(new Vector3Int(pos.x - ( int ) size.x + 1, pos.y, 0), shadowMapTiles[0]);
            }
        }
    }

    private void UpdateShadowMap() {
        for (int i = 0; i < size.x; i++) {
            for (int j = 0; j < size.y; j++) {
                if (gamecontroller.tilesArray[i, j].GetWasSeen()) {
                    Vector2Int Arrpos = gamecontroller.WorldToTileCoordinate(new Vector3Int(i, j));
                    shadowMap.SetTile(new Vector3Int(Arrpos.x - ( int ) size.x + 2, Arrpos.y, 0), shadowMapTiles[1]);
                }
                int dx = Mathf.Abs(i - playerPos.x);
                int dy = Mathf.Abs(j - playerPos.y);

                if (dx * dx + dy * dy <= playerViewingDistance * playerViewingDistance) {
                    if (HasLineOfSight(playerPos, new Vector2Int(i, j))) {
                        Vector3Int pos = gamecontroller.TileToWorldCoordinate(i, j);
                        shadowMap.SetTile(new Vector3Int(pos.x - 1, pos.y, 0), shadowMapTiles[2]);
                        gamecontroller.tilesArray[i - 1, j].SetWasSeen(true);

                    }
                }
            }
        }
    }

    private bool HasLineOfSight(Vector2Int start, Vector2Int end) {
        List<Vector2Int> line = GetLine(start, end);
        foreach (var tile in line) {
            if (gamecontroller.tilesArray[tile.x, tile.y].GetTileValue() == TileValue.Obstacle) {
                gamecontroller.tilesArray[tile.x - 1, tile.y].SetWasSeen(true);
                if (tile == end) return true;
                return false;
            }
        }
        return true;
    }

    private List<Vector2Int> GetLine(Vector2Int start, Vector2Int end) {
        List<Vector2Int> line = new List<Vector2Int>();

        int x0 = start.x;
        int y0 = start.y;
        int x1 = end.x;
        int y1 = end.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true) {
            line.Add(new Vector2Int(x0, y0));
            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }

        return line;
    }

}
