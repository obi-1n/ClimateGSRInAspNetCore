using System;
using System.Collections.Generic;
using System.Linq;

namespace ClimateGSR_in_ASP.NET_Core.Models
{
    public class Game
    {

        public Game()
        {
            Questions = new List<Question>();
        }

        public int Score {  get; set; }
        public List<Question> Questions { get; set; }

    }
}