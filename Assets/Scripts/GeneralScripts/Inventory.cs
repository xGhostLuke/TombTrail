using System;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// One item lying in the pack. X and Y are its top-left cell.
/// </summary>
public class InventoryEntry {
    public ItemObject Item { get; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public bool Rotated { get; private set; }
    public int Count { get; private set; } = 1;

    public Vector2Int Size => Inventory.SizeOf(Item, Rotated);

    public InventoryEntry(ItemObject item, int x, int y, bool rotated) {
        Item = item;
        MoveTo(x, y, rotated);
    }

    public void MoveTo(int x, int y, bool rotated) {
        X = x;
        Y = y;
        Rotated = rotated;
    }

    public void AddOne() {
        Count++;
    }

    public void RemoveOne() {
        Count--;
    }
}

/// <summary>
/// The player's pack: a grid in which items cover several cells and can be turned.
/// Knows nothing about the window, so the rules can change without touching the UI.
/// </summary>
public class Inventory {
    public event Action Changed;

    public int Width { get; }
    public int Height { get; }
    public IReadOnlyList<InventoryEntry> Entries => entries;

    private readonly List<InventoryEntry> entries = new List<InventoryEntry>();

    public Inventory(int width, int height) {
        Width = width;
        Height = height;
    }

    public static Vector2Int SizeOf(ItemObject item, bool rotated) {
        return rotated
            ? new Vector2Int(item.GridHeight, item.GridWidth)
            : new Vector2Int(item.GridWidth, item.GridHeight);
    }

    /// <summary>
    /// True if the item fits at this cell. <paramref name="ignore"/> is the entry being moved,
    /// which must not collide with its own old position.
    /// </summary>
    public bool CanPlace(ItemObject item, int x, int y, bool rotated, InventoryEntry ignore = null) {
        Vector2Int size = SizeOf(item, rotated);
        if (x < 0 || y < 0 || x + size.x > Width || y + size.y > Height) {
            return false;
        }
        foreach (InventoryEntry other in entries) {
            if (other == ignore) {
                continue;
            }
            bool apart = x + size.x <= other.X || other.X + other.Size.x <= x
                || y + size.y <= other.Y || other.Y + other.Size.y <= y;
            if (!apart) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Adds the item to a stack that still has room, otherwise puts it on the first
    /// free spot, upright if possible, otherwise turned.
    /// </summary>
    /// <returns>False if there is no room anywhere</returns>
    public bool TryAdd(ItemObject item) {
        InventoryEntry stack = entries.Find(entry => entry.Item == item && entry.Count < item.MaxStack);
        if (stack != null) {
            stack.AddOne();
        } else if (TryFindSpot(item, out InventoryEntry placed)) {
            entries.Add(placed);
        } else {
            return false;
        }
        Changed?.Invoke();
        return true;
    }

    /// <summary>
    /// Moves an entry to a new cell and rotation.
    /// </summary>
    /// <returns>False if it does not fit there; the entry then stays where it was</returns>
    public bool TryMove(InventoryEntry entry, int x, int y, bool rotated) {
        if (!CanPlace(entry.Item, x, y, rotated, entry)) {
            return false;
        }
        entry.MoveTo(x, y, rotated);
        Changed?.Invoke();
        return true;
    }

    /// <summary>
    /// Swaps one item of an entry for another item, which may have a different size.
    /// </summary>
    /// <returns>False if the other item does not fit; the entry then stays untouched</returns>
    public bool TryReplace(InventoryEntry entry, ItemObject replacement) {
        // Only the last item of a stack frees its cells.
        bool freesCells = entry.Count == 1;
        if (freesCells) {
            entries.Remove(entry);
        }
        if (!TryFindSpot(replacement, out InventoryEntry placed)) {
            if (freesCells) {
                entries.Add(entry);
            }
            return false;
        }
        if (!freesCells) {
            entry.RemoveOne();
        }
        entries.Add(placed);
        Changed?.Invoke();
        return true;
    }

    /// <summary>
    /// Takes one item out of an entry. The entry disappears with its last item.
    /// </summary>
    public bool RemoveOne(InventoryEntry entry) {
        if (!entries.Contains(entry)) {
            return false;
        }
        entry.RemoveOne();
        if (entry.Count == 0) {
            entries.Remove(entry);
        }
        Changed?.Invoke();
        return true;
    }

    /// <summary>
    /// First entry holding this item, or null. The same item can lie in the pack more than once.
    /// </summary>
    public InventoryEntry Find(ItemObject item) {
        return entries.Find(entry => entry.Item == item);
    }

    private bool TryFindSpot(ItemObject item, out InventoryEntry placed) {
        foreach (bool rotated in new[] { false, true }) {
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    if (CanPlace(item, x, y, rotated)) {
                        placed = new InventoryEntry(item, x, y, rotated);
                        return true;
                    }
                }
            }
        }
        placed = null;
        return false;
    }
}
