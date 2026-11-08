using Scripts;

using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A villager standing on the grid. Deliberately not an EntityController:
/// it must not be attackable and takes no turns.
/// </summary>
public class NpcController : MonoBehaviour {
    [SerializeField] private GameController gameController;
    [SerializeField] private int x, y;
    [SerializeField] private Quest quest;

    private void Start() {
        gameController.SetTile(x, y, TileValue.Npc);
        transform.position = gameController.TileToWorldCoordinate(x, y);
    }

    private void Update() {
        // A key instead of a click, because clicks are not filtered by the UI yet.
        if (Keyboard.current.eKey.wasPressedThisFrame && PlayerCanTalk()) {
            Talk();
        }
    }

    private bool PlayerCanTalk() {
        Vector2Int player = gameController.LocatePlayer();
        return gameController.IsMyTurn(TurnOwner.Player)
            && gameController.GetDistanceToTile(player.x, player.y, x, y) == 1;
    }

    private void Talk() {
        switch (quest.state) {
            case QuestState.Open:
                quest.state = QuestState.Accepted;
                Say(quest.offerText);
                break;
            case QuestState.Accepted:
                TryCompleteQuest();
                break;
            case QuestState.Done:
                Say(quest.afterText);
                break;
        }
    }

    private void TryCompleteQuest() {
        PlayerController player = gameController.playerController;
        // Find returns null for a quest without a requested item, so it cannot complete with empty hands.
        InventoryEntry entry = player.GetInventory().Find(quest.requestedItem);
        if (entry == null) {
            Say(quest.reminderText);
            return;
        }
        player.GetInventory().Remove(entry);
        quest.state = QuestState.Done;
        Say(quest.completedText);
        player.AddXp(quest.xpReward);
    }

    private void Say(string text) {
        gameController.infoTab.StandardLogMessage(text);
        gameController.infoTab.ShowInteractionlog();
    }
}
