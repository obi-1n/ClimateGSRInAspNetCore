using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ClimateGSR_in_ASP.NET_Core.Models
{
    public class LeaderboardService
    {
        private readonly ClimateGSRDbContext _context;

        public LeaderboardService(ClimateGSRDbContext context)
        {
            _context = context;
        }

        public List<LeaderboardEntryViewModel> GetTopPlayers(string? currentUserId = null)
        {
            // Include older accounts that do not yet have a member record.
            var players = (from user in _context.Users.AsNoTracking()
                           join member in _context.Members.AsNoTracking()
                               on user.Id equals member.ApplicationUserId into membership
                           from member in membership.DefaultIfEmpty()
                           select new
                           {
                               user.Id,
                               user.FirstName,
                               user.LastName,
                               AllTimeScore = member == null ? 0 : member.LifetimePoints,
                               HighScore = member == null || user.HighScore > member.Highscore
                                   ? user.HighScore : member.Highscore,
                               GamesPlayed = member == null ? 0 : member.GamesPlayed
                           })
                .OrderByDescending(player => player.AllTimeScore)     // main ranking
                .ThenByDescending(player => player.HighScore)         // tiebreaker
                .ThenBy(player => player.FirstName)
                .ThenBy(player => player.LastName)
                .ThenBy(player => player.Id)
                .Take(10)
                .ToList();

            var entries = new List<LeaderboardEntryViewModel>();
            for (var i = 0; i < players.Count; i++)
            {
                var player = players[i];
                var name = string.IsNullOrWhiteSpace(player.FirstName) ? "Player" : player.FirstName.Trim();
                if (!string.IsNullOrWhiteSpace(player.LastName))
                    name += " " + player.LastName.Trim()[0] + ".";

                // Share a rank only when both all-time score and high score are equal
                bool tiedWithPrevious = i > 0
                    && player.AllTimeScore == players[i - 1].AllTimeScore
                    && player.HighScore == players[i - 1].HighScore;

                entries.Add(new LeaderboardEntryViewModel
                {
                    Rank = tiedWithPrevious ? entries[i - 1].Rank : i + 1,
                    PlayerName = name,
                    AllTimeScore = player.AllTimeScore,
                    HighScore = player.HighScore,
                    GamesPlayed = player.GamesPlayed,
                    IsCurrentPlayer = player.Id == currentUserId
                });
            }

            return entries;
        }
    }
}
