using System;

using Scripts;

using UnityEngine;

/// <summary>
/// A task of a villager, stored on the villager itself. The state lives in the
/// scene instance, so a restart of the level resets it.
/// </summary>
[Serializable]
public class Quest {
    // Names of the dialogue nodes a conversation can open on.
    public const string OfferNode = "offer";
    public const string NoRoomNode = "noRoom";
    public const string ReminderNode = "reminder";
    public const string CompletedNode = "completed";
    public const string DeliveredNode = "delivered";
    public const string AfterNode = "after";

    public QuestKind kind = QuestKind.Fetch;
    public QuestState state = QuestState.Open;
    [Tooltip("Fetch: what the player has to bring. Deliver: the parcel handed over on accepting.")]
    public QuestItemObject requestedItem;
    [Tooltip("Deliver only: the villager who takes the parcel and pays the reward.")]
    public NpcController recipient;
    public int xpReward = 100;
    public DialogueObject dialogue;

    /// <summary>
    /// The nodes this kind of quest opens on, every one of them has to exist in the dialogue.
    /// </summary>
    public string[] RequiredNodes() {
        return kind == QuestKind.Deliver
            ? new[] { OfferNode, NoRoomNode, ReminderNode, DeliveredNode, AfterNode }
            : new[] { OfferNode, ReminderNode, CompletedNode, AfterNode };
    }
}
