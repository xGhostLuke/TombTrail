using UnityEngine;
using Scripts;

[CreateAssetMenu(fileName = "ItemObject", menuName = "Scriptable Objects/ItemObject")]
public class ItemObject : ScriptableObject {
    ItemType type;
    public Sprite ItemSprite;
    [Min(1)] public int GridWidth = 1;
    [Min(1)] public int GridHeight = 1;
    [Min(1)] public int MaxStack = 1;
}
