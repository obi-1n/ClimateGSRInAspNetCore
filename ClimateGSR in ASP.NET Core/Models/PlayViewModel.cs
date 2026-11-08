namespace ClimateGSR_in_ASP.NET_Core.Models
{
    /// <summary>
    /// Data shown on the Play page for the current round. The answer is deliberately not included
    /// so it never reaches the browser.
    /// </summary>
    public class PlayViewModel
    {
        public int RoundNumber { get; set; }
        public int TotalRounds { get; set; }
        public int Score { get; set; }
        public string EmissionsPrompt { get; set; } = string.Empty;
        public List<Hint> RevealedHints { get; set; } = new();
        public bool CanRevealMore { get; set; }
        public int PointsAvailable { get; set; }
        public string? Feedback { get; set; }
    }
}
