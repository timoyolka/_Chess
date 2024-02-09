using Chess.Data;
using Chess.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Chess.Hubs;
using Microsoft.AspNetCore.SignalR;
using Chess.Services;

namespace Chess.Controllers
{
    public class PlayerController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly IHttpContextAccessor context;

        private readonly IHubContext<ChessHub> hubContext;

        private const int LoggedInStatusTrue = 1;
        private const int LoggedInStatusFalse = 0;
        private const string ConnectedStatusTrue = "true";
        private const string ConnectedStatusFalse = "false";


        public PlayerController(ApplicationDbContext _db, IHttpContextAccessor _context, IHubContext<ChessHub> _hubContext)
        {
            db = _db;
            context = _context;
            hubContext = _hubContext;
        }

        public IActionResult Login(Login model)
        {
            try
            {
                Player player = db.Chess_Mvc_Players.FirstOrDefault(u => u.Name == model.Name && u.Password == model.Password);

                if (player != null && context.HttpContext?.Session != null)
                {
                    context.HttpContext.Session.SetInt32(SessionKeys.LoggedInKey, LoggedInStatusTrue);
                    context.HttpContext.Session.SetString(SessionKeys.SessionUserKey, JsonConvert.SerializeObject(player));
                    context.HttpContext.Session.SetString(SessionKeys.UserIdKey, JsonConvert.SerializeObject(player.Id));
                    context.HttpContext.Session.SetString(SessionKeys.UserNameKey, JsonConvert.SerializeObject(player.Name));
                    context.HttpContext.Session.SetString(SessionKeys.UserEmailKey, JsonConvert.SerializeObject(player.Email));
                    context.HttpContext.Session.SetString(SessionKeys.UserPasswordKey, JsonConvert.SerializeObject(player.Password));
                    context.HttpContext.Session.SetString(SessionKeys.ConnectedKey, ConnectedStatusFalse);
                    //context.HttpContext.Session.SetString("UserAtGame", JsonConvert.SerializeObject(player.AtGame));
                    //context.HttpContext.Session.SetString("UserGameId", JsonConvert.SerializeObject(player.gameId));
                    //context.HttpContext.Session.SetString("UserIsWhite", JsonConvert.SerializeObject(player.isWhite));
                    context.HttpContext.Session.SetString("board", JsonConvert.SerializeObject(boardData.__init__));
                    return RedirectToAction("Home", "Pages");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
                    return View("LoginPage");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("LoginPage");
            }
            
            
        }

        public IActionResult Logout()
        {
            if(context.HttpContext != null)
            {
                context.HttpContext.Session.Clear();
            }
            return RedirectToAction("Home", "Pages");
        }



        [HttpPost]
        public IActionResult Register(RegistraionModel model)
        {
            if (ModelState.IsValid)
            {
                Player player = new Player();
                player.AtGame = false;
                player.isWhite = null;
                player.gameId = null;
                player.Name = model.Name;
                player.Email = model.Email;
                player.Password = model.Password;
                db.Chess_Mvc_Players.Add(player);
                db.SaveChanges();
                return RedirectToAction("LoginPage", "Pages");
            }

            return View("RegisterPage", model);
        }
    }
}
