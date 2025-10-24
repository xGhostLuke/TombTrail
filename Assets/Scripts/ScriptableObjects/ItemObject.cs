using UnityEngine;
using Scripts;

[CreateAssetMenu(fileName = "ItemObject", menuName = "Scriptable Objects/ItemObject")]
public class ItemObject : ScriptableObject {
    ItemType type;
    public Sprite ItemSprite;
}
