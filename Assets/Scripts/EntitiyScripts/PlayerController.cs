using System;

using Scripts;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : EntityController {
    [SerializeField] private int xp;
    [SerializeField] private int xpToNextLevel = 100;
    [SerializeField] private int level = 1;
    [SerializeField] private int healthPerLevel = 10;
    [SerializeField] private QuestItemObject carriedItem;

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

    public QuestItemObject GetCarriedItem() {
        return carriedItem;
    }

    public void SetCarriedItem(QuestItemObject item) {
        carriedItem = item;
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
