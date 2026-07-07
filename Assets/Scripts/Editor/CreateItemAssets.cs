using UnityEngine;

using UnityEditor;

using Scripts;

/// <summary>
/// Editor-Script zur automatischen Erstellung der Item-Assets
/// Wird beim Unity-Start ausgeführt
/// </summary>
public class CreateItemAssets {

    [InitializeOnLoadMethod]
    static void CreateItems() {
        string itemsPath = "Assets/Items/";

        // 1. Health Potion (1x1)
        CreateOrUpdateHealingObject(
            itemsPath + "HealthPotion.asset",
            "HealthPotion",
            healingAmount: 20,
            gridWidth: 1,
            gridHeight: 1
        );

        // 2. Very Broken Sword (2x1)
        CreateOrUpdateWeapon(
            itemsPath + "VeryBrokenSword.asset",
            "VeryBrokenSword",
            baseDamage: 3,
            range: 1,
            gridWidth: 2,
            gridHeight: 1
        );

        // 3. Rusty Sword (2x1)
        CreateOrUpdateWeapon(
            itemsPath + "RustySword.asset",
            "RustySword",
            baseDamage: 5,
            range: 1,
            gridWidth: 2,
            gridHeight: 1
        );

        Debug.Log("✅ Item-Assets erstellt/aktualisiert!");
    }

    static void CreateOrUpdateHealingObject(string path, string displayName, int healingAmount, int gridWidth, int gridHeight) {
        HealingObject healing = AssetDatabase.LoadAssetAtPath<HealingObject>(path);

        if (healing == null) {
            healing = ScriptableObject.CreateInstance<HealingObject>();
            AssetDatabase.CreateAsset(healing, path);
            Debug.Log($"✨ Erstellt: {displayName} at {path}");
        }

        healing.name = displayName;
        healing.gridWidth = gridWidth;
        healing.gridHeight = gridHeight;

        // Healing-Effect setzen (via Reflection, da private)
        var field = typeof(HealingObject).GetField("healingEffect",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null) {
            field.SetValue(healing, healingAmount);
        }

        EditorUtility.SetDirty(healing);
        AssetDatabase.SaveAssets();
    }

    static void CreateOrUpdateWeapon(string path, string weaponName, int baseDamage, int range, int gridWidth, int gridHeight) {
        WeoponObeject weapon = AssetDatabase.LoadAssetAtPath<WeoponObeject>(path);

        if (weapon == null) {
            weapon = ScriptableObject.CreateInstance<WeoponObeject>();
            AssetDatabase.CreateAsset(weapon, path);
            Debug.Log($"✨ Erstellt: {weaponName} at {path}");
        }

        weapon.name = weaponName;
        weapon.weoponName = weaponName;
        weapon.baseDamage = baseDamage;
        weapon.range = range;
        weapon.gridWidth = gridWidth;
        weapon.gridHeight = gridHeight;
        weapon.actionCost = 1;
        weapon.effect = WeoponEffects.NONE;
        weapon.ChanceForEffekt = 0;

        EditorUtility.SetDirty(weapon);
        AssetDatabase.SaveAssets();
    }
}
