using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple "Game Over" window.
/// Put it on a CanvasGroup object tagged "GameOverWindow"; the GameController
/// finds it by tag and shows it when the player dies. If no such object exists
/// the game still ends, only the window is skipped.
/// </summary>
public class GameOverWindowScript : WindowController {

    [SerializeField] private TextMeshProUGUI messageText;

    private void Awake() {
        HideWindow();
    }

    public void ShowGameOver(string message) {
        if (messageText != null) {
            messageText.text = message;
        }
        SetActive(true);
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    /// <summary>
    /// Hook this up to a "Restart" button.
    /// </summary>
    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
