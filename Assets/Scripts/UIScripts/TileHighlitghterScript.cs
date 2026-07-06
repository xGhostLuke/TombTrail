using System;

using Scripts;

using UnityEditor.Rendering;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TileHighlighter : MonoBehaviour {
    [SerializeField] private GameController gameController;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        SnapToGrid();
    }



    private void SnapToGrid() {
        try {
            Vector2Int hoveredTile = gameController.GetTileHoveredByMouse();
            if (gameController.GetTileValue(hoveredTile.x, hoveredTile.y) == TileValue.Enemy) {
                spriteRenderer.color = new Color(1f, 0f, 0f, 0.27f);
            } else {
                spriteRenderer.color = new Color(0.55f, 0.55f, 0.55f, 0.27f);
            }
        } catch (NullReferenceException) {
            gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            Debug.Log("Tried Setting GameController");
        }
    }
}
