using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextGameAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddGameSuggestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameSuggestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    CircleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSuggestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSuggestions_Circles_CircleId",
                        column: x => x.CircleId,
                        principalTable: "Circles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GameVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    GameSuggestionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameVotes_GameSuggestions_GameSuggestionId",
                        column: x => x.GameSuggestionId,
                        principalTable: "GameSuggestions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameSuggestions_CircleId",
                table: "GameSuggestions",
                column: "CircleId");

            migrationBuilder.CreateIndex(
                name: "IX_GameVotes_GameSuggestionId",
                table: "GameVotes",
                column: "GameSuggestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameVotes");

            migrationBuilder.DropTable(
                name: "GameSuggestions");
        }
    }
}
