using System;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TileHighlighter : MonoBehaviour {
    private GameController gameController;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        SnapToGrid();
    }

    private void SnapToGrid() {
        if (gameController.GetTileValue(gameController.GetTileHoveredByMouse().x, gameController.GetTileHoveredByMouse().y) == 2) {
            spriteRenderer.color = new Color(1f, 0f, 0f, 0.27f);
        } else {
            spriteRenderer.color = new Color(0.55f, 0.55f, 0.55f, 0.27f);
        }
    }
}
