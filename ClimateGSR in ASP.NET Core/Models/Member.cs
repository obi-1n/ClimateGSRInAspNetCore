using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimateGSR_in_ASP.NET_Core.Models
{
    /// <summary>
    /// Stores a user's game statistics, including lifetime points, games played, and high score.
    /// </summary>
    public class Member
    {
        [Key] public int MemberId { get; set; }

        [Required]
        [StringLength(128)]
        [ForeignKey("ApplicationUser")]

        public string ApplicationUserId { get; set; } = string.Empty;

        public int LifetimePoints { get; set; }
        public int GamesPlayed { get; set; }
        public double PointsPerGame { get; set; }
        public int Highscore { get; set; }

        public virtual ApplicationUser? ApplicationUser { get; set; }
        //public DateTime joinTime { get; set; }
        // add this in once scoring system complete - public ICollection<Score> Scores { get; set; }
    }
}
