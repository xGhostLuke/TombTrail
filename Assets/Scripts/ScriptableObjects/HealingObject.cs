using UnityEngine;

/// <summary>
/// Scriptable Object Type to display ex.
/// Potion, Food, Poisoned Food, etc.
/// </summary>
[CreateAssetMenu(fileName = "HealingObject", menuName = "Scriptable Objects/HealingObject")]
public class HealingObject : ItemObject {

    [SerializeField] private int healingEffect;

    public int GetHealingEffect() {
        return healingEffect;
    }
}
