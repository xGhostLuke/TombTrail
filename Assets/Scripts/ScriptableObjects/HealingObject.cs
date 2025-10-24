using UnityEngine;

[CreateAssetMenu(fileName = "HealingObject", menuName = "Scriptable Objects/HealingObject")]
public class HealingObject : ItemObject {

    [SerializeField] private int healingEffect;
}
