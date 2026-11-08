using UnityEngine;

/// <summary>
/// An item a villager asks for. It needs a type of its own, because weapons
/// and potions are equipped or used the moment they are picked up.
/// </summary>
[CreateAssetMenu(fileName = "QuestItemObject", menuName = "Scriptable Objects/QuestItemObject")]
public class QuestItemObject : ItemObject {
}
