namespace Scripts {
    /// <summary>
    /// Who is currently allowed to act.
    /// Replaces the old magic numbers (1 = player, 2 = enemies) in IsMyTurn.
    /// </summary>
    public enum TurnOwner {
        Player = 0,
        Enemies = 1,
    }
}
