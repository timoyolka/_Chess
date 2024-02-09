using Chess.Data;
using Microsoft.AspNetCore.SignalR;
using Chess.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Chess.Hubs
{
    public class ChessHub : Hub
    {
        private readonly ApplicationDbContext db;

        private readonly IHttpContextAccessor context;

        private readonly ILogger logger;

        public ChessHub(ApplicationDbContext _db, IHttpContextAccessor _context, ILogger<ChessHub> _logger)
        {
            db = _db;
            context = _context;
            logger = _logger;
        }

        public async Task JoinGame(int gameId)
        {
            Game model = db.Chess_Mvc_Games.FirstOrDefault(x => x.Id == gameId);

            if (model != null)
            {
                if (model.White != null && model.WhiteConnectionId == null)
                {
                    model.WhiteConnectionId = Context.ConnectionId;
                    context.HttpContext.Session.SetString("wConnectionId", model.WhiteConnectionId);
                    logger.LogInformation($"\x1b[36m white connection id: {model.WhiteConnectionId} \x1b[0m");
                }
                else if (model.Black != null && model.BlackConnectionId == null)
                {
                    model.BlackConnectionId = Context.ConnectionId;
                    context.HttpContext.Session.SetString("bConnectionId", model.BlackConnectionId);
                    logger.LogInformation($"\x1b[36m black connection id: {model.BlackConnectionId} \x1b[0m");
                }


                db.Chess_Mvc_Games.Update(model);
                await db.SaveChangesAsync();
                await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());


                if (model.White != null && model.Black != null)
                {
                    model.IsPlaying = true;
                    db.Update(model);
                    await db.SaveChangesAsync();

                    logger.LogInformation($"\x1b[32m Message sent to game #{gameId} \x1b[0m");
                    await Clients.Group(gameId.ToString()).SendAsync("GameReady");
                }
            }   
        }
        public async Task LeaveGroup(int gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId.ToString());
        }


        public async Task FinishGame(int gameId)
        {
            await Clients.Group(gameId.ToString()).SendAsync("GameFinished");
        }

        public async Task SubmitMove(int gameId, string fromPosition, string toPosition)
        {
            var game = await db.Chess_Mvc_Games.FirstOrDefaultAsync(g => g.Id == gameId);
            if (game == null)
            {
                logger.LogWarning($"Game #{gameId} not found.");
                return;
            }

            // Convert the board string to a 2D array for easier manipulation
            char[,] board = ConvertStringToBoard(game.Board);

            // Convert fromPosition and toPosition to array indices
            (int fromRow, int fromCol) = PositionToIndices(fromPosition);
            (int toRow, int toCol) = PositionToIndices(toPosition);

            // Make the move on the board array
            char piece = board[fromRow, fromCol];
            board[fromRow, fromCol] = ' '; // Remove the piece from its original position
            board[toRow, toCol] = piece; // Place the piece in its new position

            // Convert the board array back to a string
            game.Board = ConvertBoardToString(board);

            // Save the updated game state to the database
            db.Update(game);
            await db.SaveChangesAsync();

            logger.LogInformation($"Move submitted for game #{gameId}: {fromPosition} -> {toPosition}");

            // Notify all clients in the group (game) about the move
            await Clients.Group(gameId.ToString()).SendAsync("MoveMade", fromPosition, toPosition);
        }

        private (int, int) PositionToIndices(string position)
        {
            int col = position[0] - 'a';
            int row = 8 - (position[1] - '0');
            return (row, col);
        }

        public char[,] ConvertStringToBoard(string boardString)
        {
            char[,] board = new char[8, 8];

            // Initialize board with empty spaces
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = ' ';
                }
            }

            // Split the input string by '/' to process each piece
            string[] pieces = boardString.TrimEnd('/').Split('/');
            foreach (var pieceInfo in pieces)
            {
                if (pieceInfo.Length < 3) continue; // Skip invalid entries

                // Extract file (column), rank (row), and piece type from the string
                char file = pieceInfo[0];
                char rank = pieceInfo[1];
                char piece = pieceInfo[2];

                // Convert file and rank to 0-based indexes for the array
                int fileIndex = file - 'a'; // Convert 'a'-'h' to 0-7
                int rankIndex = '8' - rank; // Convert '1'-'8' to 7-0 (invert the rank)

                // Place the piece on the board
                board[rankIndex, fileIndex] = piece;
            }

            return board;
        }

        public string ConvertBoardToString(char[,] board)
        {
            if (board.GetLength(0) != 8 || board.GetLength(1) != 8)
                throw new ArgumentException("Board must be 8x8.");

            StringBuilder boardString = new StringBuilder();

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    char piece = board[rank, file];
                    if (piece != ' ') 
                    {
                        char fileLetter = (char)('a' + file);
                        int rankNumber = 8 - rank;
                        boardString.Append($"{fileLetter}{rankNumber}{piece}/");
                    }
                }
            }

            // Remove the trailing slash
            if (boardString.Length > 0)
            {
                boardString.Remove(boardString.Length - 1, 1);
            }

            return boardString.ToString();
        }
    }
}
