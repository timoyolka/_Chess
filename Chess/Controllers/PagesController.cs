using Chess.Data;
using Chess.Hubs;
using Chess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Chess.Services;


namespace Chess.Controllers
{
    public class PagesController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly IHttpContextAccessor context;

        private readonly IHubContext<ChessHub> hubContext;

        private readonly ILogger logger;

        private const int LoggedInStatusTrue = 1;
        private const int LoggedInStatusFalse = 0;
        private const string ConnectedStatusTrue = "true";
        private const string ConnectedStatusFalse = "false";

        public PagesController(ApplicationDbContext _db, IHttpContextAccessor _context, IHubContext<ChessHub> _hubContext, ILogger<PagesController> _logger)
        {
            db = _db;
            context = _context;
            hubContext = _hubContext;
            logger = _logger;
        }

        public bool isSessionNull()
        {
            return context.HttpContext.Session == null;
        }

        public bool LoggedIn()
        {
            return context.HttpContext.Session.GetInt32(SessionKeys.LoggedInKey) != LoggedInStatusTrue;
        }


        public IActionResult FindGame()
        {
            if (isSessionNull())
            {
                return View("Home");
            }

            if (LoggedIn())
            {
                return View("LoginPage");
            }

            int? gameId = context.HttpContext.Session.GetInt32(SessionKeys.GameIdKey);
            if (gameId.HasValue)
            {
                Game existingGame = db.Chess_Mvc_Games.FirstOrDefault(u => u.Id == gameId.Value);
                if (existingGame != null)
                {
                    logger.LogInformation($"\u001b[32m Exisiting game #{existingGame.Id} joind \u001b[0mm");
                    return View("Board", existingGame);
                }
            }

            Game model = db.Chess_Mvc_Games.FirstOrDefault(u => !u.IsPlaying);

            if (model == null)
            {
                model = SingleGameInit();
                db.Chess_Mvc_Games.Add(model);
                db.SaveChanges();
                logger.LogInformation($"\u001b[32m New game #{model.Id} created \u001b[0m");
            }

            if (model.White != null && model.White == int.Parse(context.HttpContext.Session.GetString(SessionKeys.UserIdKey)))
            {
                return View("Board", model);
            }

            db.Attach(model);

            if (model.White == null)
            {
                model.White = int.Parse(context.HttpContext.Session.GetString(SessionKeys.UserIdKey));
            }
            else if (model.Black == null)
            {
                model.Black = int.Parse(context.HttpContext.Session.GetString(SessionKeys.UserIdKey));
                model.IsPlaying = true;
            }
            model.Board = boardData.__init__;
            context.HttpContext.Session.SetInt32(SessionKeys.GameIdKey, model.Id);


            db.Chess_Mvc_Games.Update(model);
            db.SaveChanges();
            logger.LogInformation("\u001b[32m The FindGame method is invoked \u001b[0m");
            logger.LogInformation("\u001b[32m White ID: {P1} \u001b[0m", model.White);
            logger.LogInformation("\u001b[32m Black ID: {P2} \u001b[0m", model.Black);

            return View("Board", model);
        }

        public async Task<IActionResult> FinishGame()
        {
            try
            {
                context.HttpContext.Session.SetString(SessionKeys.ConnectedKey, ConnectedStatusFalse);

                int? gameId = context.HttpContext.Session.GetInt32(SessionKeys.GameIdKey);
                if (!gameId.HasValue)
                {
                    return RedirectToAction("ShowDataBase");
                }

                Game model = await db.Chess_Mvc_Games.FirstOrDefaultAsync(u => u.Id == gameId);

                if (model != null)
                {
                    await hubContext.Clients.Group(model.Id.ToString()).SendAsync("GameFinished");
                    await hubContext.Groups.RemoveFromGroupAsync(model.WhiteConnectionId, model.Id.ToString());

                    db.Attach(model);
                    db.Chess_Mvc_Games.Remove(model);
                    await db.SaveChangesAsync();
                }
                logger.LogInformation($"\u001b[31mGame and group #{gameId} has been removed\u001b[0m");

                return RedirectToAction("ShowDataBase");
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred: {ex.Message}");
                return RedirectToAction("Error");
            }
        }


        public IActionResult Home()
        {
            return View();
        }

        public IActionResult Play()
        {
            return View();
        }

        public IActionResult Board(Game game)
        {
            Game model = db.Chess_Mvc_Games.FirstOrDefault(game);
            return View(model);
        }

        public IActionResult RegisterPage()
        {
            return View();
        }

        public IActionResult LoginPage()
        {
            return View();
        }

        public async  Task<IActionResult> ShowDataBase()
        {
            var games = await db.Chess_Mvc_Games.ToListAsync();
            return View(games);
        }

        public IActionResult FullyDeleteGames()
        {
            var allGames = db.Chess_Mvc_Games;
            db.Chess_Mvc_Games.RemoveRange(allGames);
            db.SaveChanges();
            return View("Home"); 
        }

        public void GameInit()
        {
            foreach (var game in db.Chess_Mvc_Games)
            {
                if (game != null)
                {
                    game.Black = null;
                    game.BlackConnectionId = null;
                    game.White = null;
                    game.WhiteConnectionId = null;
                    game.CurrentPlayer = null;
                    game.Board = ",";
                    game.IsPlaying = false;

                    db.Chess_Mvc_Games.Update(game);
                    db.SaveChanges();
                }
            }
        }

        public Game SingleGameInit()
        {
            Game model = new Game();
            model.Black = null;
            model.BlackConnectionId = null;
            model.White = null;
            model.WhiteConnectionId = null;
            model.CurrentPlayer = null;
            model.Board = ",";
            model.IsPlaying = false;
            return model;
        }
    }
}
