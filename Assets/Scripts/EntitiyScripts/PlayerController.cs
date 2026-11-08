using System;

using Scripts;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : EntityController {
    [SerializeField] private int xp;
    [SerializeField] private int xpToNextLevel = 100;
    [SerializeField] private int level = 1;
    [SerializeField] private int healthPerLevel = 10;
    [SerializeField] private int inventoryWidth = 4;
    [SerializeField] private int inventoryHeight = 3;
    private Inventory inventory;

    protected override void Awake() {
        base.Awake();
        // Built in Awake because every Awake runs before any Start, so the window can subscribe in its Start.
        inventory = new Inventory(inventoryWidth, inventoryHeight);
    }

    private void Start() {
        // REMINDER: x and y needs to be adjusted if the Grid got changed by a lot
        // (idk how to fix this) -> TODO: später über GameController/LevelConfig lösen

        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        infoTab = gameController.infoTab;
        gameController.SetTile(x, y, TileValue.Player);

        UpdatePosition(x, y);
        type = TileValue.Player;
        status = EntityStatus.Neutral;
        isAlive = true;
        if (health <= 0) {
            health = maxHealth;
        }
    }

    private void Update() {
        if (gameController.IsMyTurn(TurnOwner.Player)) {
            Movement();
        }
    }

    public override void Movement() {
        // One action per frame: IsMyTurn is checked only once, so a second key could still move a dead player.
        if (Keyboard.current.wKey.wasPressedThisFrame) Move(0, -1, type);
        else if (Keyboard.current.sKey.wasPressedThisFrame) Move(0, 1, type);
        else if (Keyboard.current.dKey.wasPressedThisFrame) Move(1, 0, type);
        else if (Keyboard.current.aKey.wasPressedThisFrame) Move(-1, 0, type);
        else if (Keyboard.current.spaceKey.wasPressedThisFrame) Move(0, 0, type);
    }

    public override void Attack() {
        throw new NotImplementedException();
        // Managed over GameController, works for now, maybe rework in the future
    }

    protected override void Die() {
        base.Die();
        gameController.GameOver();
    }

    public Inventory GetInventory() {
        return inventory;
    }

    /// <summary>
    /// Uses an item from the pack. Only a use that really happens ends the turn.
    /// </summary>
    public void UseItem(InventoryEntry entry) {
        if (!gameController.IsMyTurn(TurnOwner.Player)) {
            return;
        }
        switch (entry.Item) {
            case HealingObject potion:
                DrinkPotion(entry, potion);
                break;
            case WeoponObeject weapon:
                EquipWeapon(entry, weapon);
                break;
            default:
                infoTab.StandardLogMessage("You cannot use " + entry.Item.name + ".");
                break;
        }
    }

    private void DrinkPotion(InventoryEntry entry, HealingObject potion) {
        if (health >= maxHealth) {
            infoTab.StandardLogMessage("You are already at full health.");
            return;
        }
        inventory.RemoveOne(entry);
        AdjustHealth(potion.GetHealingEffect());
        infoTab.StandardLogMessage("You drink " + potion.name + ".");
        gameController.EndPlayerTurn();
    }

    private void EquipWeapon(InventoryEntry entry, WeoponObeject weapon) {
        WeoponObeject previous = getEntityWeopon();
        bool swapped = previous == null ? inventory.RemoveOne(entry) : inventory.TryReplace(entry, previous);
        if (!swapped) {
            infoTab.StandardLogMessage("No room for " + previous.name + " in your pack.");
            return;
        }
        gameController.setPlayerWeopon(weapon);
        infoTab.StandardLogMessage("You equip " + weapon.name + ".");
        gameController.EndPlayerTurn();
    }

    /// <summary>
    /// Puts the equipped weapon back into the pack. Bound to the weapon display in the scene.
    /// </summary>
    public void UnequipWeapon() {
        WeoponObeject weapon = getEntityWeopon();
        if (weapon == null || !gameController.IsMyTurn(TurnOwner.Player)) {
            return;
        }
        if (!inventory.TryAdd(weapon)) {
            infoTab.StandardLogMessage("No room for " + weapon.name + " in your pack.");
            return;
        }
        gameController.setPlayerWeopon(null);
        infoTab.StandardLogMessage("You put away " + weapon.name + ".");
        gameController.EndPlayerTurn();
    }

    public void AddXp(int amount) {
        xp += amount;
        infoTab.StandardLogMessage("Gained " + amount + " XP (" + xp + "/" + xpToNextLevel + ").");
        // The > 0 check stops an endless loop if the threshold is set to 0 in the Inspector.
        while (xpToNextLevel > 0 && xp >= xpToNextLevel) {
            LevelUp();
        }
    }

    private void LevelUp() {
        xp -= xpToNextLevel;
        xpToNextLevel *= 2;
        level++;
        // Raise the cap before healing, otherwise the clamp in AdjustHealth cuts the heal off.
        maxHealth += healthPerLevel;
        AdjustHealth(maxHealth);
        infoTab.StandardLogMessage("Level up! You are now level " + level + ", max health " + maxHealth + ".");
        infoTab.ShowInteractionlog();
    }
}
