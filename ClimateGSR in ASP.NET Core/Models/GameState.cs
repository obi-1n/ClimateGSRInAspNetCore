using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ClimateGSR_in_ASP.NET_Core.Models
{
    public class GameState
    {

        public Game Game { get; set; } = new();
        public int CurrentIndex { get; set; }
        public int HintsRevealed { get; set; }

        // Calculated, so not stored in the session JSON (Current would throw once the game is finished)
        [JsonIgnore] public Question Current { get { return Game.Questions[CurrentIndex]; } } 
        [JsonIgnore] public bool Finished { get { return CurrentIndex >= Game.Questions.Count; } }

        public bool GameCounted { get; set; } 
        public int PointsSaved { get; set; }


    }
}