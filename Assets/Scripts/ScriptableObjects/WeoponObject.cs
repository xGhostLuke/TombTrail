using System;

using Scripts;

using UnityEditor;

using UnityEngine;

[CreateAssetMenu(fileName = "WeoponObeject", menuName = "Scriptable Objects/WeoponObeject")]

public class WeoponObeject : ScriptableObject {
    public String weoponName;
    public int baseDamage;
    public int range;
    public int actionCost;
    public WeoponEffects effect;
    public double ChanceForEffekt;
    public Sprite weoponSprite;
}
