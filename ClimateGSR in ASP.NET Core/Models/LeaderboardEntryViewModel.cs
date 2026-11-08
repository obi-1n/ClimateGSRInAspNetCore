namespace ClimateGSR_in_ASP.NET_Core.Models
{
    /// <summary>
    /// A single row on the leaderboard.
    /// </summary>
    public class LeaderboardEntryViewModel
    {
        public int Rank { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public int AllTimeScore { get; set; }
        public int HighScore { get; set; }
        public int GamesPlayed { get; set; }
        public bool IsCurrentPlayer { get; set; }
    }
}
