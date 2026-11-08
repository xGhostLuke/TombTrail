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
    [SerializeField] private string npcName = "Villager";
    [SerializeField] private Quest quest = new Quest();

    private void Start() {
        gameController.SetTile(x, y, TileValue.Npc);
        transform.position = gameController.TileToWorldCoordinate(x, y);
    }

    private void Update() {
        if (Keyboard.current.eKey.wasPressedThisFrame && PlayerCanTalk()) {
            Talk();
        }
    }

    // Unity calls this on every change in the Inspector, so a broken setup shows up before playing.
    private void OnValidate() {
        if (quest.kind == QuestKind.Deliver && quest.recipient == null) {
            Debug.LogError(name + ": a delivery needs a recipient.", this);
        }
        if (quest.dialogue == null) {
            return;
        }
        foreach (string node in quest.RequiredNodes()) {
            if (quest.dialogue.FindNode(node) == null) {
                Debug.LogError(name + ": dialogue " + quest.dialogue.name + " has no node '" + node + "'.", this);
            }
        }
    }

    private bool PlayerCanTalk() {
        Vector2Int player = gameController.LocatePlayer();
        // IsMyTurn is false during a conversation, so a second villager nearby stays quiet.
        return gameController.IsMyTurn(TurnOwner.Player)
            && gameController.GetDistanceToTile(player.x, player.y, x, y) == 1;
    }

    private void Talk() {
        // A delivery goes first, otherwise this villager's own quest would hide it.
        Quest delivery = FindDeliveryForMe();
        if (delivery != null) {
            Complete(delivery, Quest.DeliveredNode);
            return;
        }
        switch (quest.state) {
            case QuestState.Open:
                Show(quest, Quest.OfferNode);
                break;
            case QuestState.Accepted:
                // A delivery is finished at the recipient, never at the villager who gave it.
                if (quest.kind == QuestKind.Fetch && Carried(quest) != null) {
                    Complete(quest, Quest.CompletedNode);
                } else {
                    Show(quest, Quest.ReminderNode);
                }
                break;
            case QuestState.Done:
                Show(quest, Quest.AfterNode);
                break;
        }
    }

    /// <summary>
    /// The accepted delivery for this villager whose parcel the player really carries, or null.
    /// A dropped parcel does not count, the player has to pick it up again.
    /// </summary>
    private Quest FindDeliveryForMe() {
        foreach (NpcController npc in FindObjectsByType<NpcController>(FindObjectsSortMode.None)) {
            Quest other = npc.quest;
            if (other.kind == QuestKind.Deliver && other.recipient == this
                && other.state == QuestState.Accepted && Carried(other) != null) {
                return other;
            }
        }
        return null;
    }

    private InventoryEntry Carried(Quest wanted) {
        // Find returns null for a quest without an item, so it cannot complete with empty hands.
        return gameController.playerController.GetInventory().Find(wanted.requestedItem);
    }

    private void Complete(Quest finished, string node) {
        PlayerController player = gameController.playerController;
        player.GetInventory().RemoveOne(Carried(finished));
        finished.state = QuestState.Done;
        gameController.infoTab.StandardLogMessage("Quest completed for " + npcName + ".");
        player.AddXp(finished.xpReward);
        Show(finished, node);
    }

    private void Show(Quest shown, string node) {
        if (shown.dialogue != null) {
            gameController.dialogueWindow.Open(npcName, shown.dialogue, node, answer => ResolveAnswer(shown, answer));
        }
    }

    private string ResolveAnswer(Quest answered, DialogueAnswer answer) {
        if (answer.action == DialogueAction.AcceptQuest && !TryAccept(answered)) {
            return Quest.NoRoomNode;
        }
        return answer.next;
    }

    /// <summary>
    /// Accepts a quest. A delivery hands over its parcel, and without room for it nothing changes.
    /// </summary>
    private bool TryAccept(Quest accepted) {
        if (accepted.kind == QuestKind.Deliver && !gameController.playerController.GetInventory().TryAdd(accepted.requestedItem)) {
            return false;
        }
        accepted.state = QuestState.Accepted;
        gameController.infoTab.StandardLogMessage("Quest accepted from " + npcName + ".");
        return true;
    }
}
