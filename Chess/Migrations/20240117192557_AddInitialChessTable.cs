using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chess.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialChessTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chess_Mvc_Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Board = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    White = table.Column<int>(type: "int", nullable: true),
                    WhiteConnectionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Black = table.Column<int>(type: "int", nullable: true),
                    BlackConnectionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentPlayer = table.Column<int>(type: "int", nullable: true),
                    IsPlaying = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chess_Mvc_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Chess_Mvc_Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AtGame = table.Column<bool>(type: "bit", nullable: false),
                    isWhite = table.Column<bool>(type: "bit", nullable: true),
                    gameId = table.Column<int>(type: "int", nullable: true),
                    UserConnectionId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chess_Mvc_Players", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Chess_Mvc_Games",
                columns: new[] { "Id", "Black", "BlackConnectionId", "Board", "CurrentPlayer", "IsPlaying", "White", "WhiteConnectionId" },
                values: new object[,]
                {
                    { 1, null, null, ",", null, false, null, null },
                    { 2, null, null, ",", null, false, null, null }
                });

            migrationBuilder.InsertData(
                table: "Chess_Mvc_Players",
                columns: new[] { "Id", "AtGame", "Email", "Name", "Password", "UserConnectionId", "gameId", "isWhite" },
                values: new object[,]
                {
                    { 1, false, "example@email.com", "Tim", "123", null, null, null },
                    { 2, false, "example@email.com", "George", "123", null, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chess_Mvc_Games");

            migrationBuilder.DropTable(
                name: "Chess_Mvc_Players");
        }
    }
}
