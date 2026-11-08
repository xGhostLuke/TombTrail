using System;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Weighted loot table for enemies.
/// Create one via "Scriptable Objects/LootTable" and assign it to an EnemyController.
/// Each kill first rolls <see cref="dropChance"/>; on success it rolls
/// between <see cref="minDrops"/> and <see cref="maxDrops"/> items,
/// each picked by weight from <see cref="entries"/>.
/// </summary>
[CreateAssetMenu(fileName = "LootTable", menuName = "Scriptable Objects/LootTable")]
public class LootTable : ScriptableObject {

    [Serializable]
    public class LootEntry {
        public ItemObject item;
        [Min(1)] public int weight = 1;
    }

    [Tooltip("Chance (0-1) that this enemy drops anything at all.")]
    [Range(0f, 1f)] public float dropChance = 1f;

    [Tooltip("Minimum number of items rolled when the drop chance succeeds.")]
    [Min(0)] public int minDrops = 1;

    [Tooltip("Maximum number of items rolled when the drop chance succeeds.")]
    [Min(0)] public int maxDrops = 1;

    public List<LootEntry> entries = new List<LootEntry>();

    /// <summary>
    /// Rolls the table once and returns the items that dropped (may be empty).
    /// </summary>
    public List<ItemObject> RollDrops() {
        List<ItemObject> drops = new List<ItemObject>();

        if (entries == null || entries.Count == 0) {
            return drops;
        }
        if (UnityEngine.Random.value > dropChance) {
            return drops;
        }

        int min = Mathf.Max(0, minDrops);
        int max = Mathf.Max(min, maxDrops);
        int count = UnityEngine.Random.Range(min, max + 1);

        for (int i = 0; i < count; i++) {
            ItemObject item = RollSingleItem();
            if (item != null) {
                drops.Add(item);
            }
        }
        return drops;
    }

    /// <summary>
    /// Picks one entry by weight. Entries without an item are ignored.
    /// </summary>
    public ItemObject RollSingleItem() {
        int totalWeight = 0;
        foreach (LootEntry entry in entries) {
            if (entry != null && entry.item != null) {
                totalWeight += Mathf.Max(1, entry.weight);
            }
        }
        if (totalWeight == 0) {
            return null;
        }

        int roll = UnityEngine.Random.Range(0, totalWeight);
        foreach (LootEntry entry in entries) {
            if (entry == null || entry.item == null) {
                continue;
            }
            roll -= Mathf.Max(1, entry.weight);
            if (roll < 0) {
                return entry.item;
            }
        }
        return null;
    }
}
