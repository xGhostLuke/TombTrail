using System;

using Scripts;

using UnityEngine;

/// <summary>
/// A request of a villager, stored on the villager itself. The state lives in the
/// scene instance, so a restart of the level resets it.
/// </summary>
[Serializable]
public class Quest {
    public QuestState state = QuestState.Open;
    public QuestItemObject requestedItem;
    public int xpReward = 100;

    // Short texts: the interaction log shows at most 26 lines.
    [TextArea] public string offerText;
    [TextArea] public string reminderText;
    [TextArea] public string completedText;
    [TextArea] public string afterText;
}
