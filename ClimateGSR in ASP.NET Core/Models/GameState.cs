using System;
using System.Collections.Generic;
using System.Linq;

namespace ClimateGSR_in_ASP.NET_Core.Models
{
    public class GameState
    {

        public Game Game { get; set; } = new();
        public int CurrentIndex { get; set; }
        public int HintsRevealed { get; set; }

        public Question Current { get { return Game.Questions[CurrentIndex]; } } 
        public bool Finished { get { return CurrentIndex >= Game.Questions.Count; } }

        public bool GameCounted { get; set; } 
        public int PointsSaved { get; set; }


    }
}