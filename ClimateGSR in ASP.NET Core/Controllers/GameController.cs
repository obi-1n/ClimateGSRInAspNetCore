using System.Text.Json;
using ClimateGSR_in_ASP.NET_Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClimateGSR_in_ASP.NET_Core.Controllers
{
    /// <summary>
    /// Handles game rounds, guesses, hints, results, and persistence of signed-in players' scores.
    /// The active game state is stored in the player's session.
    /// </summary>
    public class GameController : Controller
    {
        //Game settings
        private const int Rounds = 5;       //Number of questions per game
        private const int MaxPoints = 100;  //Points for a correct answer with no hints
        private const int HintCost = 25;    //Points lost for each hint revealed
        private const int FreeHints = 2;    //the first two hints cost nothing

        private const string SessionKey = "GameState";

        private readonly ClimateGSRDbContext _db;
        private readonly LeaderboardService _leaderboard;
        private readonly UserManager<ApplicationUser> _userManager;

        public GameController(
            ClimateGSRDbContext db,
            LeaderboardService leaderboard,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _leaderboard = leaderboard;
            _userManager = userManager;
        }

        //Gets the current player's game state from the session.
        //Returns null if no game has been started.
        //ASP.NET Core sessions only hold strings, so the state is stored as JSON.
        private GameState? LoadState()
        {
            var json = HttpContext.Session.GetString(SessionKey);
            return json == null ? null : JsonSerializer.Deserialize<GameState>(json);
        }

        //Writes the state back to the session. Unlike MVC 5, changing the object
        //doesn't change the session, so this must be called after every change.
        private void SaveState(GameState s)
        {
            HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(s));
        }

        //Creates a new game with a set of random questions, stores it in the
        //session, and sends the player to the first round.
        public IActionResult Start()
        {
            var game = new Game { Score = 0, Questions = new List<Question>() };
            var used = new HashSet<string>();   //Tracks answers already used in this game

            var generator = new QuestionGenerator(_db);

            //Keep generating until we have enough questions. The attempts limit
            //stops an endless loop if there aren't enough unique countries.
            for (int attempts = 0; game.Questions.Count < Rounds && attempts < 50; attempts++)
            {
                var q = generator.GenerateQuestion();
                if (used.Add(q.Answer))          //Add returns false if already present, so no repeated countries
                    game.Questions.Add(q);
            }

            //Save the game so later requests can continue it
            SaveState(new GameState
            {
                Game = game,
                PlayerId = _userManager.GetUserId(User)
            });
            return RedirectToAction("Play");
        }

        //Shows the current round: the emissions prompt, any hints revealed so far,
        //the current score and the points still available.
        public IActionResult Play()
        {
            var s = LoadState();
            if (s == null) return RedirectToAction("Start");           //No game in progress, so start one
            if (s.Finished) return RedirectToAction("Result");         //All rounds done, so show results

            var q = s.Current;

            //Answer is deliberately left out so it doesn't reach the browser.
            var model = new PlayViewModel
            {
                RoundNumber = s.CurrentIndex + 1,
                TotalRounds = s.Game.Questions.Count,
                Score = s.Game.Score,
                EmissionsPrompt = q.EmissionsPrompt,
                RevealedHints = q.Hints.Take(s.HintsRevealed).ToList(), //Only hints the player has unlocked
                CanRevealMore = s.HintsRevealed < q.Hints.Count,        //Hide the button when none are left
                PointsAvailable = PointsFor(s),
                Feedback = TempData["Feedback"] as string               //One-off message from the previous action
            };
            return View(model);
        }

        //Checks the player's guess. A correct guess scores points and moves to the
        //next round. A wrong guess reveals another hint, or ends the round if none remain.
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Guess(string? guess)
        {
            var s = LoadState();
            if (s == null || s.Finished) return RedirectToAction("Start");

            var q = s.Current;

            //Compare ignoring case and extra spaces
            if (Normalise(guess) == Normalise(q.Answer))
            {
                int points = PointsFor(s);          //Fewer points the more hints were used
                s.Game.Score += points;
                return Advance(s, "Correct! It was " + q.Answer + ". +" + points + " points.");
            }

            //Wrong: reveal another hint, or end the question if none are left
            if (s.HintsRevealed < q.Hints.Count)
            {
                s.HintsRevealed++;
                SaveState(s);
                TempData["Feedback"] = "Not quite. Here's another hint.";
                return RedirectToAction("Play");
            }

            //Wrong and no hints left: no points, move on
            return Advance(s, "Out of hints. It was " + q.Answer + ".");
        }

        //Reveals one more hint for the current question (costs points).
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult RevealHint()
        {
            var s = LoadState();
            if (s == null || s.Finished) return RedirectToAction("Start");

            //Only reveal if hints remain
            if (s.HintsRevealed < s.Current.Hints.Count)
            {
                s.HintsRevealed++;
                SaveState(s);
            }

            return RedirectToAction("Play");
        }

        //Skips current question with no points awarded, showing the answer.
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult GiveUp()
        {
            var s = LoadState();
            if (s == null || s.Finished) return RedirectToAction("Start");

            return Advance(s, "The answer was " + s.Current.Answer + ".");
        }

        //Shows final score once every round has been completed.
        public IActionResult Result()
        {
            var s = LoadState();
            if (s == null) return RedirectToAction("Start");
            if (!s.Finished) return RedirectToAction("Play");   //Don't allow skipping ahead to results

            ViewBag.ScoreSaved = SaveProgress(s);

            ViewBag.Feedback = TempData["Feedback"];            //Message about the final round
            return View(s.Game);
        }

        /// <summary>
        /// Displays the leaderboard ranked by players' highest single-game scores.
        /// </summary>
        [HttpGet]
        public IActionResult Leaderboard()
        {
            return View(_leaderboard.GetTopPlayers(_userManager.GetUserId(User)));
        }

        /// <summary>
        /// Saves the finished game's points to the logged-in player's Member row.
        /// Safe to call repeatedly: it only adds points that haven't been saved yet.
        /// </summary>
        private bool SaveProgress(GameState s)
        {
            var userId = _userManager.GetUserId(User);
            if (s.PlayerId == null || userId != s.PlayerId)
                return false; //Only save games started by the currently signed-in player.

            int newPoints = s.Game.Score - s.PointsSaved;    //points not yet saved (0 on a page refresh)
            bool countGame = !s.GameCounted;                 //only count the game once
            if (!countGame && newPoints <= 0) return true;

            var member = _db.Members.FirstOrDefault(m => m.ApplicationUserId == userId);

            if (member == null)                              //create the row if registration didn't
            {
                member = new Member { ApplicationUserId = userId };
                _db.Members.Add(member);
            }

            if (countGame) member.GamesPlayed++;

            member.LifetimePoints += newPoints;
            member.Highscore = Math.Max(member.Highscore, s.Game.Score);
            member.PointsPerGame = member.GamesPlayed > 0
                ? (double)member.LifetimePoints / member.GamesPlayed
                : 0;

            _db.SaveChanges();

            //Only mark as saved once SaveChanges has succeeded
            s.GameCounted = true;
            s.PointsSaved = s.Game.Score;
            SaveState(s);
            return true;
        }

        //HELPER FUNCTIONS
        //---------------------------------------------------------------------------------------------

        //Moves on to the next question: resets the hint counter, stores a
        //feedback message for the next page, and redirects to Play or Result.
        private IActionResult Advance(GameState s, string feedback)
        {
            s.CurrentIndex++;
            s.HintsRevealed = 0;
            SaveState(s);
            TempData["Feedback"] = feedback;   //TempData survives exactly one redirect
            return RedirectToAction(s.Finished ? "Result" : "Play");
        }

        //Points a correct answer is worth right now: 100 with no hints,
        //dropping by 25 per hint revealed, never below 0.
        private static int PointsFor(GameState s)
        {
            int chargedHints = Math.Max(0, s.HintsRevealed - FreeHints);
            return Math.Max(0, MaxPoints - HintCost * chargedHints);
        }

        //Prepares text for comparing guesses: trims extra spaces between and
        //around words, and lowercases it. A null guess is treated as empty.
        private static string Normalise(string? text)
        {
            return string.Join(" ", (text ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries))
                         .ToLowerInvariant();
        }
    }
}
