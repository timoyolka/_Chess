using Microsoft.EntityFrameworkCore;
using Chess.Models;

namespace Chess.Data
{
    public static class boardData
    {
        public static readonly string __init__ = "a1R/b1N/c1B/d1Q/e1K/f1B/g1N/h1R/a2P/b2P/c2P/d2P/e2P/f2P/g2P/h2P/a3/b3/c3/d3/e3/f3/g3/h3/a4/b4/c4/d4/e4/f4/g4/h4/a5/b5/c5/d5/e5/f5/g5/h5/a6/b6/c6/d6/e6/f6/g6/h6/a7p/b7p/c7p/d7p/e7p/f7p/g7p/h7p/a8r/b8n/c8b/d8q/e8k/f8b/g8n/h8r/";

        public static char[,] ConvertStringToBoard(string boardString)
        {
            boardString = new string(boardString.Where(char.IsLetterOrDigit).ToArray());

            int sideLength = (int)Math.Sqrt(boardString.Length);

            char[,] board = new char[sideLength, sideLength];

            for (int i = 0; i < sideLength; i++)
            {
                for (int j = 0; j < sideLength; j++)
                {
                    int index = i * sideLength + j;
                    board[i, j] = index < boardString.Length ? boardString[index] : ' ';
                }
            }

            return board;
        }
    }

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
                
        }


        public DbSet<Models.Player> Chess_Mvc_Players { get; set; }
        public DbSet<Models.Game> Chess_Mvc_Games { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>().HasData(
                new Models.Game() { Id = 1, White = null, Board =",", Black = null, CurrentPlayer = null, IsPlaying = false },
                new Models.Game() { Id = 2, White = null, Board = ",", Black = null, CurrentPlayer = null, IsPlaying = false });

            modelBuilder.Entity<Player>().HasData(
                new Player() { Id = 1, Name = "Tim", Password = "123", Email = "example@email.com", AtGame = false, gameId = null, isWhite = null },
                new Player() { Id = 2, Name = "George", Password = "123", Email = "example@email.com", AtGame = false, gameId = null, isWhite = null });
        }
    }
}
